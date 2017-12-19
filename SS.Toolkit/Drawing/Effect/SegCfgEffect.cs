using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;

namespace SS.Toolkit.Drawing.Effect
{
    public class SegCfgEffect : IImageEffect<ImageResult>
    {
        /**
	 * cfg 分割出来的结果
	 */
        private List<Bitmap> cfgList;

        public SegCfgEffect()
        {
            init();
        }


        private void init()
        {
            cfgList = new List<Bitmap>();
        }


        /**
         * cfs进行分割,返回分割后的数组
         * @param sourceImage
         * @return
         */

        public ImageResult Execute(Bitmap sourceImage)
        {
            this.cfgList.Clear();
            int width = sourceImage.Width;
            int height = sourceImage.Height;

            List<ImageCanvas> subImgList = new List<ImageCanvas>(); //保存子图像
            Dictionary<String, Boolean> trackMap = new Dictionary<String, Boolean>(); //已经访问过的点
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var rgb = sourceImage.GetPixel(x, y);
                    String key = x + "-" + y;
                    //如果不是黑色，或者已经被访问过，则跳过cfg
                    if (!isBlack(rgb) || trackMap.ContainsKey(key))
                    {
                        continue;
                    }

                    //如果黑色，且没有访问，则以此点开始进行连通域探索
                    ImageCanvas subImage = new ImageCanvas();//保存当前字符块的坐标点
                    Queue<Point> queue = new Queue<Point>();//保存当前字符块的访问队列
                    queue.Enqueue(new Point(x, y));
                    trackMap.Add(key, true);
                    subImage.pixelList.Add(new Point(x, y));
                    subImage.left = x;
                    subImage.top = y;
                    subImage.right = x;
                    subImage.bottom = y;

                    while (queue.Count != 0)
                    {
                        Point tmp = queue.Dequeue();

                        //搜寻目标的八个方向
                        int startX = (tmp.X - 1 < 0) ? 0 : tmp.X - 1;
                        int startY = (tmp.Y - 1 < 0) ? 0 : tmp.Y - 1;
                        int endX = (tmp.X + 1 > width - 1) ? width - 1 : tmp.X + 1;
                        int endY = (tmp.Y + 1 > height - 1) ? height - 1 : tmp.Y + 1;

                        for (int tx = startX; tx <= endX; tx++)
                        {
                            for (int ty = startY; ty <= endY; ty++)
                            {
                                if (tx == tmp.X && ty == tmp.Y)
                                {
                                    continue;
                                }

                                key = tx + "-" + ty;
                                //System.out.println(key);
                                if (isBlack(sourceImage.GetPixel(tx, ty)) && !trackMap.ContainsKey(key))
                                {
                                    queue.Enqueue(new Point(tx, ty));
                                    trackMap.Add(key, true);
                                    subImage.pixelList.Add(new Point(tx, ty)); //加入到路径中

                                    //更新边界区域
                                    subImage.left = Math.Min(subImage.left, tx);
                                    subImage.top = Math.Min(subImage.top, ty);
                                    subImage.right = Math.Max(subImage.right, tx);
                                    subImage.bottom = Math.Max(subImage.bottom, ty);
                                }
                            }
                        }
                    }//end of while

                    subImage.width = subImage.right - subImage.left + 1;
                    subImage.height = subImage.bottom - subImage.top + 1;
                    subImgList.Add(subImage);
                }
            }

            //System.out.println();
            cfsToImage(subImgList);
            return new ImageResult(this.cfgList);
        }


        private void cfsToImage(List<ImageCanvas> subImgList)
        {
            for (int i = 0; i < subImgList.Count; i++)
            {
                ImageCanvas subImage = subImgList[i];
                var image = new Bitmap(subImage.width, subImage.height);
                for (int x = 0; x < subImage.width; x++)
                {
                    for (int y = 0; y < subImage.height; y++)
                    {
                        image.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
                var pixeList = subImage.pixelList;
                foreach (Point point in pixeList)
                {
                    //				System.out.println("(" + (point.X - subImage.left) + "," + (point.Y - subImage.top) + ")");
                    image.SetPixel(point.X - subImage.left, point.Y - subImage.top, Color.FromArgb(0, 0, 0));
                }

                //将切割的中间图片加入到cfgList中
                this.cfgList.Add(image);
            }
        }

        private bool isBlack(Color color)
        {
            if (color.R + color.G + color.B <= 300)
            {
                return true;
            }
            return false;
        }


    }
}
