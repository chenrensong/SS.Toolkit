using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{

    /// <summary>
    /// 清除噪点
    /// </summary>
    public class ClearNoiseEffect : IImageFilterEffect
    {
        private int _maxAroundPoints = 1;

        public ClearNoiseEffect(int maxAroundPoints = 1)
        {
            _maxAroundPoints = maxAroundPoints;
        }

        public Bitmap Execute(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);

            #region 指针法

            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (i == 0 || i == height - 1 || j == 0 || j == width - 1) //边界点，直接当作噪点去除掉
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        else
                        {
                            int aroundPoint = 0;
                            if (ptr[0] != 255) //看标记，不是背景点
                            {
                                //判断其周围8个方向与自己相连接的有几个点
                                if ((ptr - 4)[0] != 255) aroundPoint++; //左边
                                if ((ptr + 4)[0] != 255) aroundPoint++; //右边
                                if ((ptr - width * 4)[0] != 255) aroundPoint++; //正上方
                                if ((ptr - width * 4 + 4)[0] != 255) aroundPoint++; //右上角
                                if ((ptr - width * 4 - 4)[0] != 255) aroundPoint++; //左上角
                                if ((ptr + width * 4)[0] != 255) aroundPoint++; //正下方
                                if ((ptr + width * 4 + 4)[0] != 255) aroundPoint++; //右下方
                                if ((ptr + width * 4 - 4)[0] != 255) aroundPoint++; //左下方
                            }
                            if (aroundPoint < _maxAroundPoints)//目标点是噪点
                            {
                                ptr[0] = ptr[1] = ptr[2] = 255; //去噪点
                            }
                        }
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            bitmap.UnlockBits(bdata);

            #endregion

            return bitmap;
        }
    }
}
