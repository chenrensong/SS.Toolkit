using System;
using System.Collections.Generic;
using System.DrawingCore;
using System.Text;
using System.Linq;
namespace SS.Toolkit.Drawing
{
    public class ImageResult : List<Bitmap>
    {

        public ImageResult()
        {
        }

        public ImageResult(Bitmap bitmap)
        {
            this.Add(bitmap);
        }

        public ImageResult(IList<Bitmap> bitmaps)
        {
            this.AddRange(bitmaps);
        }

        public void AddResult(ImageResult imageResult)
        {
            foreach (var item in imageResult)
            {
                this.Add(item);
            }
        }

        public bool IsSingle
        {
            get
            {
                return this.Count == 1;
            }
        }

    }
}
