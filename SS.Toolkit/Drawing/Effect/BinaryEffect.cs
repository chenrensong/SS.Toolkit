using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    /// <summary>
    /// 二值化处理
    /// </summary>
    public class BinaryEffect : IImageFilterEffect
    {
        public Bitmap Execute(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* start = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (start[0] != 255)
                        {
                            start[0] = start[1] = start[2] = 0;
                        }
                        start += 4;
                    }
                    start += bdata.Stride - width * 4;
                }
            }
            bitmap.UnlockBits(bdata);
            return bitmap;
        }
    }
}
