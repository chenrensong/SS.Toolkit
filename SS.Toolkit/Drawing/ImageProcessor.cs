using System;
using System.Collections.Generic;
using System.Drawing;

namespace SS.Toolkit.Drawing.Effect
{
    public class ImageProcessor
    {
        private ImageProcessor(Bitmap bitmap)
        {

        }

        public static Bitmap FilterEffect<T>(Bitmap bitmap) where T : IImageFilterEffect
        {
            var filterEffect = (IImageFilterEffect)Activator.CreateInstance(typeof(T));
            return filterEffect.Execute(bitmap);
        }

        public static IList<Bitmap> SplitEffect<T>(Bitmap bitmap) where T : IImageSplitEffect
        {
            var splitEffect = (IImageSplitEffect)Activator.CreateInstance(typeof(T));
            return splitEffect.Execute(bitmap);
        }

    }

    public class ImageResult<T>
    {
        T Result { get; set; }
    }

}
