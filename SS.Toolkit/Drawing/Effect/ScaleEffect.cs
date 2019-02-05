using System.Drawing;
using System.Drawing.Drawing2D;

namespace SS.Toolkit.Drawing.Effect
{
    /// <summary>
    /// 缩放图片
    /// </summary>
    public class ScaleEffect : IImageFilterEffect
    {
        private double _scale = 1;
        private int _width = 0;
        private int _height = 0;

        public ScaleEffect(double scale)
        {
            _scale = scale;
        }

        public ScaleEffect(int height, int width)
        {
            _height = height;
            _width = width;
        }

        public Bitmap Execute(Bitmap bitmap)
        {
            int w = (int)(bitmap.Width * _scale);
            int h = (int)(bitmap.Height * _scale);
            if (_width != 0 && _height != 0)
            {
                w = _width;
                h = _height;
            }
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
            return cloneBitmap;
        }
    }
}
