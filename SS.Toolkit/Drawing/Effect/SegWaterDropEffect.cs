using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{

    /// <summary>
    /// 大水滴法
    /// </summary>
    public class SegWaterDropEffect : IImageSplitEffect
    {

        private int minD = 12;//最小字符宽度
        private int maxD = 26;
        private int meanD = 17;//平均字符宽度

        private int b = 1;//大水滴的宽度 2*B+1,取0或者1效果最好

        private Bitmap sourceImage;


        public SegWaterDropEffect()
        {

        }


        /**
         * 滴水法入口
         * @param sourceImage
         * @return 切割完图片的数组
         */
        public IList<Bitmap> Execute(Bitmap sourceImage)
        {
            var imageList = new List<Bitmap>();
            this.sourceImage = sourceImage;

            int width = sourceImage.Width;
            int height = sourceImage.Height;

            Console.WriteLine("width:" + width + " height:" + height);

            //		if (width <= maxD) {
            //			//如果是单个字符，则直接返回
            //			this.imageList.add(sourceImage);
            //			return this.imageList;
            //		}

            //在x轴的投影
            int[] histData = new int[width];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var color = sourceImage.GetPixel(x, y);

                    if (isBlack(color))
                    {
                        histData[x]++;
                    }
                }
            }

            List<int> extrems = Extremum.GetMinExtrem(histData);

            Point[] startRoute = new Point[height];
            Point[] endRoute = null;

            for (int y = 0; y < height; y++)
            {
                startRoute[y] = new Point(0, y);
            }

            int num = (int)Math.Round((double)(width * 1.0 / meanD * 1.0));//字符的个数
            int lastP = 0; //上一次分割的位置
            int curSplit = 1;//分割点的个数，小于等于 num - 1;
            for (int i = 0; i < extrems.Count; i++)
            {
                if (curSplit > (num - 1))
                {
                    break;
                }

                //判断两个分割点之间的距离是否合法
                int curP = extrems[(i)];
                int dBetween = curP - lastP + 1;
                if (dBetween < minD || dBetween > maxD)
                {
                    continue;
                }

                //			//判断当前分割点与末尾结束点的位置是否合法
                //			int dAll = width - curP + 1;
                //			if (dAll < minD*(num - curSplit) || dAll > maxD*(num - curSplit)) {
                //				continue;
                //			}
                endRoute = getEndRoute(new Point(curP, 0), height, curSplit);
                doSplit(imageList, startRoute, endRoute);
                startRoute = endRoute;
                lastP = curP;
                curSplit++;
                Console.WriteLine(curP);
            }

            endRoute = new Point[height];
            for (int y = 0; y < height; y++)
            {
                endRoute[y] = new Point(width - 1, y);
            }
            doSplit(imageList, startRoute, endRoute);

            Console.WriteLine("=================");
            Console.WriteLine(width + "," + height);

            return imageList;
        }

        /**
         * 获得滴水的路径
         * @param startP
         * @param height
         * @return
         */
        private Point[] getEndRoute(Point startP, int height, int curSplit)
        {

            //获得分割的路径
            Point[] endRoute = new Point[height];
            Point curP = new Point(startP.X, startP.Y);
            Point lastP = curP;
            endRoute[0] = curP;
            while (curP.Y < height - 1)
            {
                int maxW = 0;
                int sum = 0;
                int nextX = curP.X;
                int nextY = curP.Y;

                for (int j = 1; j <= 5; j++)
                {
                    try
                    {
                        int curW = getPixelValue(curP.X, curP.Y, j) * (6 - j);
                        sum += curW;
                        if (curW > maxW)
                        {
                            maxW = curW;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }

                //如果全黑，需要看惯性
                if (sum == 0)
                {
                    maxW = 4;
                }
                //如果周围全白，则默认垂直下落
                if (sum == 15)
                {
                    maxW = 6;
                }

                switch (maxW)
                {
                    case 1:
                        nextX = curP.X - 1;
                        nextY = curP.Y;
                        break;
                    case 2:
                        nextX = curP.X + 1;
                        nextY = curP.Y;
                        break;
                    case 3:
                        nextX = curP.X + 1;
                        nextY = curP.Y + 1;
                        break;
                    case 5:
                        nextX = curP.X - 1;
                        nextY = curP.Y + 1;
                        break;
                    case 6:
                        nextX = curP.X;
                        nextY = curP.Y + 1;
                        break;
                    case 4:
                        if (nextX > curP.X)
                        {//具有向右的惯性
                            nextX = curP.X + 1;
                            nextY = curP.Y + 1;
                        }

                        if (nextX < curP.X)
                        {//向左的惯性或者sum = 0
                            nextX = curP.X;
                            nextY = curP.Y + 1;
                        }

                        if (sum == 0)
                        {
                            nextX = curP.X;
                            nextY = curP.Y + 1;
                        }
                        break;

                    default:

                        break;
                }

                //如果出现重复运动
                if (lastP.X == nextX && lastP.Y == nextY)
                {
                    if (nextX < curP.X)
                    {//向左重复
                        maxW = 5;
                        nextX = curP.X + 1;
                        nextY = curP.Y + 1;
                    }
                    else
                    {//向右重复
                        maxW = 3;
                        nextX = curP.X - 1;
                        nextY = curP.Y + 1;
                    }
                }

                lastP = curP;
                int rightLimit = meanD * curSplit + 1;
                if (nextX > rightLimit)
                {
                    nextX = rightLimit;
                    nextY = curP.Y + 1;
                }

                int leftLimit = meanD * (curSplit - 1) + meanD / 2;
                if (nextX < leftLimit)
                {
                    nextX = leftLimit;
                    nextY = curP.Y + 1;
                }
                curP = new Point(nextX, nextY);

                endRoute[curP.Y] = curP;
            }

            return endRoute;
        }

        /**
         * 具体实行切割
         * @param starts
         * @param ends
         */
        private void doSplit(IList<Bitmap> imageList, Point[] starts, Point[] ends)
        {
            int left = starts[0].X;
            int top = starts[0].Y;
            int right = ends[0].X;
            int bottom = ends[0].Y;

            for (int i = 0; i < starts.Length; i++)
            {
                left = Math.Min(starts[i].X, left);
                top = Math.Min(starts[i].Y, top);
                right = Math.Max(ends[i].X, right);
                bottom = Math.Max(ends[i].Y, bottom);
            }

            int width = right - left + 1;
            int height = bottom - top + 1;
            var image = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    image.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                }
            }
            for (int i = 0; i < ends.Length; i++)
            {
                Point start = starts[i];
                Point end = ends[i];
                for (int x = start.X; x < end.X; x++)
                {
                    if (isBlack(sourceImage.GetPixel(x, i)))
                    {
                        Console.WriteLine((x - left) + ", " + (start.Y - top));
                        image.SetPixel(x - left, start.Y - top, Color.FromArgb(0, 0, 0));
                    }
                }

            }
            imageList.Add(image);

            Console.WriteLine("-----------------------");

        }


        /**
         * 判断是否位黑色像素
         * @param rgb
         * @return
         */
        private bool isBlack(Color color)
        {
            if (color.R + color.G + color.B <= 300)
            {
                return true;
            }
            return false;
        }

        /**
         * 获得大水滴中心点周围的像素值
         * @param cx
         * @param cy
         * @param j 中心点周围的编号
         * @return
         */
        private int getPixelValue(int cx, int cy, int j)
        {
            Color rgb = default(Color);

            if (j == 4)
            {
                int right = cx + b + 1;
                right = right >= sourceImage.Width - 1 ? sourceImage.Width - 1 : right;
                rgb = sourceImage.GetPixel(right, cy);
                return isBlack(rgb) ? 0 : 1;
            }

            if (j == 5)
            {
                int left = cx - b - 1;
                left = left <= 0 ? 0 : left;
                rgb = sourceImage.GetPixel(left, cy);
                return isBlack(rgb) ? 0 : 1;
            }

            //如果 1<= j <= 3, 则判断下方的区域， 只要有一个黑点，则当做黑点，
            int start = cx - b + j - 2;
            int end = cx + b + j - 2;

            start = start <= 0 ? 0 : start;
            end = end >= sourceImage.Width - 1 ? sourceImage.Width - 1 : end;
            int blackNum = 0;
            int whiteNum = 0;
            for (int i = start; i <= end; i++)
            {
                rgb = sourceImage.GetPixel(i, cy + 1);
                if (isBlack(rgb))
                {
                    blackNum++;
                }
                else
                {
                    whiteNum++;
                }
            }

            return (blackNum >= whiteNum) ? 0 : 1;
        }


    }
}
