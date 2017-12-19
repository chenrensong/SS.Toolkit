using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Drawing2D;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    /// <summary>
    /// 缩放图片
    /// </summary>
    public class ScaleEffect : IImageEffect<ImageResult>
    {
        private int _scale = 1;

        public ScaleEffect(int scale)
        {
            _scale = scale;
        }

        public ImageResult Execute(Bitmap bitmap)
        {
            int w = bitmap.Width * _scale;
            int h = bitmap.Height * _scale;
            Bitmap cloneBitmap = new Bitmap(w, h);
            using (Graphics g = Graphics.FromImage(cloneBitmap))
            {
                //设置高质量插值法   
                g.InterpolationMode = InterpolationMode.High;
                //设置高质量,低速度呈现平滑程度   
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                //消除锯齿 
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawImage(bitmap, new Rectangle(0, 0, w, h), new Rectangle(0, 0, bitmap.Width, bitmap.Height), GraphicsUnit.Pixel);
                g.Flush();
            }
            return new ImageResult(cloneBitmap);
        }
    }
}
