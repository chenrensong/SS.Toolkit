using SS.Toolkit.Helpers;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Diagnostics.Debug;

namespace SS.Toolkit.Extensions
{
    //https://www.codemag.com/Article/1807051/Introducing-.NET-Core-2.1-Flagship-Types-Span-T-and-Memory-T
    public static class StreamExtension
    {
        private static readonly Func<Stream, Memory<byte>, CancellationToken, ValueTask<int>> _readAsyncShim;
        private static readonly Func<Stream, ReadOnlyMemory<byte>, CancellationToken, ValueTask> _writeAsyncShim;

        private static readonly ReadShim _readShim;
        private static readonly WriteShim _writeShim;

        private delegate int ReadShim(Stream stream, Span<byte> buffer);
        private delegate void WriteShim(Stream stream, ReadOnlySpan<byte> buffer);

        static StreamExtension()
        {
            // Mono seems to define the methods but throws a NotImplementedException when called.
            // https://github.com/mono/mono/blob/c5b88ec4f323f2bdb7c7d0a595ece28dae66579c/mcs/class/corlib/corert/Stream.cs
            if (!RuntimeHelper.IsRunningOnMono())
            {
                var streamType = typeof(Stream);
                var readAsyncMethod = streamType.GetMethod(nameof(Stream.ReadAsync), new[] { typeof(Memory<byte>), typeof(CancellationToken) });

                if (readAsyncMethod != null)
                {
                    Assert(readAsyncMethod.ReturnType == typeof(ValueTask<int>));

                    var streamParameter = Expression.Parameter(typeof(Stream), "stream");
                    var bufferParameter = Expression.Parameter(typeof(Memory<byte>), "buffer");
                    var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var methodCall = Expression.Call(streamParameter, readAsyncMethod, bufferParameter, cancellationTokenParameter);
                    _readAsyncShim = Expression.Lambda<Func<Stream, Memory<byte>, CancellationToken, ValueTask<int>>>(
                        methodCall,
                        streamParameter,
                        bufferParameter,
                        cancellationTokenParameter).Compile();
                }

                var writeAsyncMethod = streamType.GetMethod(nameof(Stream.WriteAsync), new[] { typeof(ReadOnlyMemory<byte>), typeof(CancellationToken) });

                if (writeAsyncMethod != null)
                {
                    Assert(writeAsyncMethod.ReturnType == typeof(ValueTask));

                    var streamParameter = Expression.Parameter(typeof(Stream), "stream");
                    var bufferParameter = Expression.Parameter(typeof(ReadOnlyMemory<byte>), "buffer");
                    var cancellationTokenParameter = Expression.Parameter(typeof(CancellationToken), "cancellationToken");
                    var methodCall = Expression.Call(streamParameter, writeAsyncMethod, bufferParameter, cancellationTokenParameter);
                    _writeAsyncShim = Expression.Lambda<Func<Stream, ReadOnlyMemory<byte>, CancellationToken, ValueTask>>(
                        methodCall,
                        streamParameter,
                        bufferParameter,
                        cancellationTokenParameter).Compile();
                }

                var readMethod = streamType.GetMethod(nameof(Stream.Read), new[] { typeof(Span<byte>) });

                if (readMethod != null)
                {
                    Assert(readMethod.ReturnType == typeof(int));
                    var streamParameter = Expression.Parameter(typeof(Stream), "stream");
                    var bufferParameter = Expression.Parameter(typeof(Span<byte>), "buffer");
                    var methodCall = Expression.Call(streamParameter, readMethod, bufferParameter);
                    _readShim = Expression.Lambda<ReadShim>(
                        methodCall,
                        streamParameter,
                        bufferParameter).Compile();
                }

                var writeMethod = streamType.GetMethod(nameof(Stream.Write), new[] { typeof(ReadOnlySpan<byte>) });

                if (writeMethod != null)
                {
                    Assert(writeMethod.ReturnType == typeof(void));
                    var streamParameter = Expression.Parameter(typeof(Stream), "stream");
                    var bufferParameter = Expression.Parameter(typeof(ReadOnlySpan<byte>), "buffer");
                    var methodCall = Expression.Call(streamParameter, writeMethod, bufferParameter);
                    _writeShim = Expression.Lambda<WriteShim>(
                       methodCall,
                       streamParameter,
                       bufferParameter).Compile();
                }
            }
        }

        public static ValueTask<int> ReadAsync(this Stream stream, Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (_readAsyncShim != null)
            {
                return _readAsyncShim(stream, buffer, cancellationToken);
            }

            if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var memoryStreamBuffer))
            {
                var position = checked((int)stream.Position);
                var length = checked((int)stream.Length);
                var result = Math.Min(length - position, buffer.Length);

                memoryStreamBuffer.AsMemory().Slice(start: position, length: result).CopyTo(buffer);

                return new ValueTask<int>(result);
            }

            if (MemoryMarshal.TryGetArray((ReadOnlyMemory<byte>)buffer, out var arraySegment))
            {
                return new ValueTask<int>(stream.ReadAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count));
            }

            return ReadCoreAsync(stream, buffer, cancellationToken);
        }

        private static async ValueTask<int> ReadCoreAsync(Stream stream, Memory<byte> buffer, CancellationToken cancellationToken)
        {
            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                var result = await stream.ReadAsync(array, offset: 0, buffer.Length, cancellationToken);
                if (result > 0)
                {
                    array.AsMemory().Slice(start: 0, length: result).CopyTo(buffer);
                }
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static ValueTask WriteAsync(this Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (_writeAsyncShim != null)
            {
                return _writeAsyncShim(stream, buffer, cancellationToken);
            }

            if (stream is MemoryStream memoryStream && memoryStream.CanWrite && memoryStream.TryGetBuffer(out var memoryStreamBuffer))
            {
                var position = checked((int)stream.Position);
                var length = checked((int)stream.Length);

                // Check if there is enough space in the stream.
                if (length - position >= buffer.Length)
                {
                    buffer.CopyTo(memoryStreamBuffer.AsMemory().Slice(start: position));
                    return new ValueTask(Task.CompletedTask); // TODO: How can we return an already completed ValueTask??
                }
            }

            if (MemoryMarshal.TryGetArray(buffer, out var arraySegment))
            {
                return new ValueTask(stream.WriteAsync(arraySegment.Array, arraySegment.Offset, arraySegment.Count));
            }

            return WriteCoreAsync(stream, buffer, cancellationToken);
        }

        private static async ValueTask WriteCoreAsync(Stream stream, ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken)
        {
            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                buffer.CopyTo(array);

                await stream.WriteAsync(array, offset: 0, buffer.Length, cancellationToken);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static int Read(this Stream stream, Span<byte> buffer)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (_readShim != null)
            {
                return _readShim(stream, buffer);
            }

            if (stream is MemoryStream memoryStream && memoryStream.TryGetBuffer(out var memoryStreamBuffer))
            {
                var position = checked((int)stream.Position);
                var length = checked((int)stream.Length);
                var result = Math.Min(length - position, buffer.Length);

                memoryStreamBuffer.AsSpan().Slice(start: position, length: result).CopyTo(buffer);

                return result;
            }

            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);

            try
            {
                var result = stream.Read(array, offset: 0, buffer.Length);
                if (result > 0)
                {
                    array.AsSpan().Slice(start: 0, length: result).CopyTo(buffer);
                }
                return result;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }

        public static void Write(this Stream stream, ReadOnlySpan<byte> buffer)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (_writeShim != null)
            {
                _writeShim(stream, buffer);
                return;
            }

            if (stream is MemoryStream memoryStream && memoryStream.CanWrite && memoryStream.TryGetBuffer(out var memoryStreamBuffer))
            {
                var position = checked((int)stream.Position);
                var length = checked((int)stream.Length);

                // Check if there is enough space in the stream.
                if (length - position >= buffer.Length)
                {
                    buffer.CopyTo(memoryStreamBuffer.AsSpan().Slice(start: position));
                    return;
                }
            }

            var array = ArrayPool<byte>.Shared.Rent(buffer.Length);
            try
            {
                buffer.CopyTo(array);
                stream.Write(array, offset: 0, buffer.Length);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(array);
            }
        }
    }
}
