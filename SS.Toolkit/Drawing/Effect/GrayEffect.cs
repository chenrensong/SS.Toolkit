using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    public class GrayEffect : IImageFilterEffect
    {
        private GrayEffectType _grayEffectType;

        public GrayEffect() : this(GrayEffectType.WeightAvg)
        {
        }

        public GrayEffect(GrayEffectType grayEffectType)
        {
            _grayEffectType = grayEffectType;
        }


        public Bitmap Execute(Bitmap bitmap)
        {
            Func<int, int, int, int> getGrayValue;
            switch (_grayEffectType)
            {
                case GrayEffectType.Max:
                    getGrayValue = GetGrayValueByMax;
                    break;
                case GrayEffectType.Avg:
                    getGrayValue = GetGrayValueByAvg;
                    break;
                default:
                    getGrayValue = GetGrayValueByWeightAvg;
                    break;
            }
            int height = bitmap.Height;
            int width = bitmap.Width;
            BitmapData bdata = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppRgb);
            unsafe
            {
                byte* ptr = (byte*)bdata.Scan0.ToPointer();
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int v = getGrayValue(ptr[0], ptr[1], ptr[2]);
                        ptr[0] = ptr[1] = ptr[2] = (byte)v;
                        ptr += 4;
                    }
                    ptr += bdata.Stride - width * 4;
                }
            }
            bitmap.UnlockBits(bdata);
            return bitmap;
        }

        /// <summary>
        /// 最大值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int GetGrayValueByMax(int r, int g, int b)
        {
            int max = r;
            max = max > g ? max : g;
            max = max > b ? max : b;
            return max;
        }

        /// <summary>
        /// 平均值
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        private static int GetGrayValueByAvg(int r, int g, int b)
        {
            return (r + g + b) / 3;
        }

        /// <summary>
        /// 加权平均
        /// </summary>
        /// <param name="b"></param>
        /// <param name="g"></param>
        /// <param name="r"></param>
        /// <returns></returns>
        private static int GetGrayValueByWeightAvg(int b, int g, int r)
        {
            return (int)(r * 0.299 + g * 0.587 + b * 0.114);
        }


    }

    public enum GrayEffectType
    {
        WeightAvg, Avg, Max
    }
}
