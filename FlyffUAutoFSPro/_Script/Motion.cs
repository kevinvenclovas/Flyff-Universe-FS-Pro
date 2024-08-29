using System.Drawing;

namespace FlyffUAutoFSPro._Script
{
    public class Motion
    {
        public int Id { get; set; }
        public Bitmap Image { get; set; }
        public Bitmap SearchImage { get; set; }

        public Motion(int _id, Bitmap _image, Bitmap searchImage)
        {
            Id = _id;
            Image = _image;
            SearchImage = searchImage;
        }
    }
}
