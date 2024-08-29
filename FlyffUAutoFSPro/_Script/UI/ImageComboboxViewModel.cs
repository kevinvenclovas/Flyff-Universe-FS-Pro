using MvvmHelpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FlyffUAutoFSPro._Script.UI
{
    public class ImageComboboxViewModel : BaseViewModel
    {
		private ObservableCollection<ImageComboboxItem> _ImageList = new ObservableCollection<ImageComboboxItem>();

        public ObservableCollection<ImageComboboxItem> ImageList
        {
			get { return _ImageList; }
			set { _ImageList = value; }
		}

		public ImageComboboxViewModel(List<ImageComboboxItem> _items)
        {
            _items.ForEach(x => ImageList.Add(x));
        }

    }
}
