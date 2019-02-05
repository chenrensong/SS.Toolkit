using System.Collections.Generic;
<<<<<<< .mine
using System.Drawing;
using System.Text;
=======
using System.Drawing;

>>>>>>> .theirs

namespace SS.Toolkit.Drawing
{
    /// <summary>
    /// 图片画板
    /// </summary>
    public class ImageCanvas
    {
        public List<Point> pixelList;
        public int left;
        public int top;
        public int right;
        public int bottom;
        public int width;
        public int height;

        public ImageCanvas()
        {
            this.pixelList = new List<Point>();
            this.left = 0;
            this.top = 0;
            this.right = 0;
            this.bottom = 0;
            this.width = 0;
            this.height = 0;
        }
    }
}
