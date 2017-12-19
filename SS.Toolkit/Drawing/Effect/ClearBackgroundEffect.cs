using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.DrawingCore.Imaging;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    /// <summary>
    /// 清除配置
    /// </summary>
    public class ClearBackgroundEffect : IImageEffect<ImageResult>
    {
        public ImageResult Execute(Bitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;
            var dgGrayValue = ComputeThresholdValue(bitmap);
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb); //红绿蓝个八位，其余8位没使用
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        if (ptr[1] > dgGrayValue)//背景点
                        {
                            ptr[0] = ptr[1] = ptr[2] = 255;
                        }
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            #region 内存法
            //获取位图中第一个像素数据的地址

            //int byteNum = width * height * 4;
            ////byte[] four = new byte[width *height ];
            //byte[] rgbValue = new byte[byteNum];
            ////把内存中的图像copy到数组
            //Marshal.Copy(ptr, rgbValue, 0, byteNum);
            //for (int i = 0; i < rgbValue.Length; i += 4)
            //{
            //    if (rgbValue[i] >= dgGrayValue) //是背景点
            //    {
            //        rgbValue[i] = rgbValue[i + 1] = rgbValue[i + 2] = 255;
            //        //  four[i/4] = rgbValue[4];
            //    }
            //    else
            //    {
            //        //不是背景点的做标记，下一阶段处理噪点用**第四个字节默认值都是255**我们标记为 111
            //        rgbValue[i + 3] = 111;
            //    }
            //}
            ////将修改好的数据复制到内存
            //Marshal.Copy(rgbValue, 0, ptr, byteNum); 
            #endregion
            //从内存中解锁
            bitmap.UnlockBits(bdata);
            return new ImageResult(bitmap);
        }

        /// <summary>
        /// 计算阈值
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        private static int ComputeThresholdValue(Bitmap img)
        {
            int i;
            int k;
            double csum;
            int thresholdValue = 1;
            int[] ihist = new int[0x100];
            for (i = 0; i < 0x100; i++)
            {
                ihist[i] = 0;
            }
            int gmin = 0xff;
            int gmax = 0;
            for (i = 1; i < (img.Width - 1); i++)
            {
                for (int j = 1; j < (img.Height - 1); j++)
                {
                    int cn = img.GetPixel(i, j).R; //生成直方图
                    ihist[cn]++;
                    if (cn > gmax)
                    {
                        gmax = cn; //找到最大像素点R
                    }
                    if (cn < gmin)
                    {
                        gmin = cn; //找到最小像素点R

                    }
                }
            }
            double sum = csum = 0.0;
            int n = 0;
            for (k = 0; k <= 0xff; k++)
            {
                sum += k * ihist[k];
                n += ihist[k];
            }
            if (n == 0)
            {
                return 60;
            }
            double fmax = -1.0;
            int n1 = 0;
            for (k = 0; k < 0xff; k++)
            {
                n1 += ihist[k];
                if (n1 != 0)
                {
                    int n2 = n - n1;
                    if (n2 == 0)
                    {
                        return thresholdValue;
                    }
                    csum += k * ihist[k];
                    double m1 = csum / ((double)n1);
                    double m2 = (sum - csum) / ((double)n2);
                    double sb = ((n1 * n2) * (m1 - m2)) * (m1 - m2);
                    if (sb > fmax)
                    {
                        fmax = sb;
                        thresholdValue = k;
                    }
                }
            }
            return thresholdValue;
        }
    }
}
