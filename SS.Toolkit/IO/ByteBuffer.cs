using System;

namespace SS.Toolkit.IO
{
    public class ByteBuffer
    {
        //字节缓存区
        private byte[] _buf;
        //读取索引
        private int _readIndex = 0;
        //写入索引
        private int _writeIndex = 0;
        //读取索引标记
        private int _markReadIndex = 0;
        //写入索引标记
        private int _markWirteIndex = 0;
        //缓存区字节数组的长度
        private int _capacity;

        /**
         * 构造方法
         */
        private ByteBuffer(int capacity)
        {
            _buf = new byte[capacity];
            this._capacity = capacity;
        }

        /**
         * 构造方法
         */
        private ByteBuffer(byte[] bytes)
        {
            _buf = bytes;
            this._capacity = bytes.Length;
        }

        /**
         * 构建一个capacity长度的字节缓存区ByteBuffer对象
         */
        public static ByteBuffer Allocate(int capacity)
        {
            return new ByteBuffer(capacity);
        }

        /**
         * 构建一个以bytes为字节缓存区的ByteBuffer对象，一般不推荐使用
         */
        public static ByteBuffer Allocate(byte[] bytes)
        {
            return new ByteBuffer(bytes);
        }

        public static void Reverse(Array array)
        {
            Array.Reverse(array);
        }

        /**
         * 根据length长度，确定大于此leng的最近的2次方数，如length=7，则返回值为8
         */
        private int FixLength(int length)
        {
            int n = 2;
            int b = 2;
            while (b < length)
            {
                b = 2 << n;
                n++;
            }
            return b;
        }


        /// <summary>
        ///  翻转字节数组，如果本地字节序列为低字节序列，则进行翻转以转换为高字节序列（大小端）
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="isLittleEndian"></param>
        /// <returns></returns>
        private byte[] flip(byte[] bytes, bool isLittleEndian = false)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (!isLittleEndian)//系统为小端，这时候需要的是大端则转换
                {
                    Array.Reverse(bytes);
                }
            }
            else
            {
                if (isLittleEndian)//系统为大端，这时候需要的是小端则转换
                {
                    Array.Reverse(bytes);
                }
            }
            return bytes;
        }

        /**
         * 确定内部字节缓存数组的大小
         */
        private int FixSizeAndReset(int currLen, int futureLen)
        {
            if (futureLen > currLen)
            {
                //以原大小的2次方数的两倍确定内部字节缓存区大小
                int size = FixLength(currLen) * 2;
                if (futureLen > size)
                {
                    //以将来的大小的2次方的两倍确定内部字节缓存区大小
                    size = FixLength(futureLen) * 2;
                }
                byte[] newbuf = new byte[size];

                Array.Copy(_buf, 0, newbuf, 0, currLen);
                _buf = newbuf;
                _capacity = newbuf.Length;
            }
            return futureLen;
        }

        /**
         * 将bytes字节数组从startIndex开始的length字节写入到此缓存区
         */
        public void WriteBytes(byte[] bytes, int startIndex, int length)
        {
            lock (this)
            {
                int offset = length - startIndex;
                if (offset <= 0) return;
                int total = offset + _writeIndex;
                int len = _buf.Length;
                FixSizeAndReset(len, total);
                for (int i = _writeIndex, j = startIndex; i < total; i++, j++)
                {
                    _buf[i] = bytes[j];
                }
                _writeIndex = total;
            }
        }

        /**
         * 将字节数组中从0到length的元素写入缓存区
         */
        public void WriteBytes(byte[] bytes, int length)
        {
            WriteBytes(bytes, 0, length);
        }

        /**
         * 将字节数组全部写入缓存区
         */
        public void WriteBytes(byte[] bytes)
        {
            WriteBytes(bytes, bytes.Length);
        }

        /**
         * 将一个ByteBuffer的有效字节区写入此缓存区中
         */
        public void Write(ByteBuffer buffer, bool isLittleEndian = false)
        {
            if (buffer == null) { return; }
            if (buffer.ReadableBytes() <= 0) { return; }
            var bytes = buffer.ToArray();
            if (isLittleEndian)
            {
                Array.Reverse(bytes);
            }
            WriteBytes(bytes);
        }

        /**
         * 写入一个int16数据
         */
        public void WriteShort(short value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个uint16数据
         */
        public void WriteUshort(ushort value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个int32数据
         */
        public void WriteInt(int value, bool isLittleEndian = false)
        {
            //byte[] array = new byte[4];
            //for (int i = 3; i >= 0; i--)
            //{
            //    array[i] = (byte)(value & 0xff);
            //    value = value >> 8;
            //}
            //Array.Reverse(array);
            //Write(array);
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个uint32数据
         */
        public void WriteUint(uint value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个int64数据
         */
        public void WriteLong(long value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个uint64数据
         */
        public void WriteUlong(ulong value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个float数据
         */
        public void WriteFloat(float value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 写入一个byte数据
         */
        public void WriteByte(byte value)
        {
            lock (this)
            {
                int afterLen = _writeIndex + 1;
                int len = _buf.Length;
                FixSizeAndReset(len, afterLen);
                _buf[_writeIndex] = value;
                _writeIndex = afterLen;
            }
        }

        /**
         * 写入一个double类型数据
         */
        public void WriteDouble(double value, bool isLittleEndian = false)
        {
            WriteBytes(flip(BitConverter.GetBytes(value), isLittleEndian));
        }

        /**
         * 读取一个字节
         */
        public byte ReadByte()
        {
            byte b = _buf[_readIndex];
            _readIndex++;
            return b;
        }

        /**
         * 从读取索引位置开始读取len长度的字节数组
         */
        private byte[] Read(int len, bool isLittleEndian = false)
        {
            byte[] bytes = new byte[len];
            Array.Copy(_buf, _readIndex, bytes, 0, len);
            //if (BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(bytes);
            //}
            bytes = flip(bytes, isLittleEndian);
            _readIndex += len;
            return bytes;
        }

        /**
         * 读取一个uint16数据
         */
        public ushort ReadUshort(bool isLittleEndian = false)
        {
            return BitConverter.ToUInt16(Read(2, isLittleEndian), 0);
        }

        /**
         * 读取一个int16数据
         */
        public short ReadShort(bool isLittleEndian = false)
        {
            return BitConverter.ToInt16(Read(2, isLittleEndian), 0);
        }

        /**
         * 读取一个uint32数据
         */
        public uint ReadUint(bool isLittleEndian = false)
        {
            return BitConverter.ToUInt32(Read(4, isLittleEndian), 0);
        }

        /**
         * 读取一个int32数据
         */
        public int ReadInt(bool isLittleEndian = false)
        {
            return BitConverter.ToInt32(Read(4, isLittleEndian), 0);
        }

        /**
         * 读取一个uint64数据
         */
        public ulong ReadUlong(bool isLittleEndian = false)
        {
            return BitConverter.ToUInt64(Read(8, isLittleEndian), 0);
        }

        /**
         * 读取一个long数据
         */
        public long ReadLong(bool isLittleEndian = false)
        {
            return BitConverter.ToInt64(Read(8, isLittleEndian), 0);
        }

        /**
         * 读取一个float数据
         */
        public float ReadFloat(bool isLittleEndian = false)
        {
            return BitConverter.ToSingle(Read(4, isLittleEndian), 0);
        }

        /**
         * 读取一个double数据
         */
        public double ReadDouble(bool isLittleEndian = false)
        {
            return BitConverter.ToDouble(Read(8, isLittleEndian), 0);
        }

        /// <summary>
        /// 读出来都默认当成是高端的
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public byte[] ReadBytes(int len, bool isLittleEndian = false)
        {
            var bytes = Read(len, isLittleEndian);
            return bytes;
        }

        /// <summary>
        /// 读取剩下所有的字节
        /// </summary>
        /// <returns></returns>
        public byte[] ReadToEnd(bool isLittleEndian = false)
        {
            var len = this._capacity - this._readIndex;
            var bytes = Read(len, isLittleEndian);
            return bytes;
        }

        /**
         * 从读取索引位置开始读取len长度的字节到disbytes目标字节数组中
         * @params disstart 目标字节数组的写入索引 （性能不高）
         */
        public void ReadBytes(byte[] disbytes, int disstart, int len)
        {
            int size = disstart + len;
            for (int i = disstart; i < size; i++)
            {
                disbytes[i] = this.ReadByte();
            }
        }


        /**
         * 清除已读字节并重建缓存区
         */
        public void DiscardReadBytes()
        {
            if (_readIndex <= 0)
            {
                return;
            }
            int len = _buf.Length - _readIndex;
            byte[] newbuf = new byte[len];
            Array.Copy(_buf, _readIndex, newbuf, 0, len);
            _buf = newbuf;
            _writeIndex -= _readIndex;
            _markReadIndex -= _readIndex;
            if (_markReadIndex < 0)
            {
                _markReadIndex = _readIndex;
            }
            _markWirteIndex -= _readIndex;
            if (_markWirteIndex < 0 || _markWirteIndex < _readIndex || _markWirteIndex < _markReadIndex)
            {
                _markWirteIndex = _writeIndex;
            }
            _readIndex = 0;
        }

        /**
         * 清空此对象
         */
        public void Clear()
        {
            _buf = new byte[_buf.Length];
            _readIndex = 0;
            _writeIndex = 0;
            _markReadIndex = 0;
            _markWirteIndex = 0;
        }

        /**
         * 设置开始读取的索引
         */
        public void SetReaderIndex(int index)
        {
            if (index < 0) return;
            _readIndex = index;
        }

        /**
         * 标记读取的索引位置
         */
        public void MarkReaderIndex()
        {
            _markReadIndex = _readIndex;
        }

        /**
         * 标记写入的索引位置
         */
        public void MarkWriterIndex()
        {
            _markWirteIndex = _writeIndex;
        }

        /**
         * 将读取的索引位置重置为标记的读取索引位置
         */
        public void ResetReaderIndex()
        {
            _readIndex = _markReadIndex;
        }

        /**
         * 将写入的索引位置重置为标记的写入索引位置
         */
        public void ResetWriterIndex()
        {
            _writeIndex = _markWirteIndex;
        }

        /**
         * 可读的有效字节数
         */
        public int ReadableBytes()
        {
            return _writeIndex - _readIndex;
        }

        /**
         * 获取可读的字节数组
         */
        public byte[] ToArray()
        {
            byte[] bytes = new byte[_buf.Length - _readIndex];
            Array.Copy(_buf, _readIndex, bytes, 0, bytes.Length);
            return bytes;
        }

        /**
         * 获取缓存区大小
         */
        public int GetCapacity()
        {
            return this._capacity;
        }



    }
}
