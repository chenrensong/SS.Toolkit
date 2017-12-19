using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;
using System.Linq;

namespace SS.Toolkit.Drawing.Effect
{
    public class ImageProcessor
    {

        /// <summary>
        /// 结果数据
        /// </summary>
        private ImageResult _imageResult;

        /// <summary>
        /// 返回结果
        /// </summary>
        public ImageResult Result
        {
            get
            {
                return _imageResult;
            }
        }

        private ImageProcessor(Bitmap bitmap)
        {
            _imageResult = new ImageResult(bitmap);
        }

        public static ImageProcessor Create(Bitmap bitmap)
        {
            //ImageProcessor.Create(bitmap).Effect<Bitmap>(new ClearBackgroundEffect()).Result.SaveAdd);
            return new ImageProcessor(bitmap);
        }

        public ImageProcessor Effect(IImageEffect<ImageResult> effect)
        {
            var newResult = new ImageResult();
            foreach (var item in _imageResult)
            {
                var result = effect.Execute(item);
                newResult.AddResult(result);
            }
            _imageResult = newResult;
            return this;
        }

    }

}
