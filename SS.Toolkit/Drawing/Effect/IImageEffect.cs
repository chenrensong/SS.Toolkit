using System.Collections.Generic;
using System.Drawing;

namespace SS.Toolkit.Drawing.Effect
{

    /// <summary>
    /// 效果
    /// </summary>
    public interface IImageEffect<T>
    {
        T Execute(Bitmap bitmap);
    }

    public interface IImageFilterEffect : IImageEffect<Bitmap>
    {

    }

    /// <summary>
    /// 图像拆分效果
    /// </summary>
    public interface IImageSplitEffect : IImageEffect<IList<Bitmap>>
    {

    }
}
