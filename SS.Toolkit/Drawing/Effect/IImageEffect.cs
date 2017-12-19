using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    /// <summary>
    /// 效果
    /// </summary>
    public interface IImageEffect<T> where T : ImageResult
    {
        T Execute(Bitmap bitmap);
    }

    public interface IImageEffect : IImageEffect<ImageResult>
    {

    }
}
