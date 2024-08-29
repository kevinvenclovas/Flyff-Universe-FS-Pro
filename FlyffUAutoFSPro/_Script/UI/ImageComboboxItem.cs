using MvvmHelpers;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace FlyffUAutoFSPro._Script.UI
{
    public class ImageComboboxItem : ObservableObject
    {
        public int ID { get; set; }
        public BitmapImage ImageSource { get; set; }
    }
}
