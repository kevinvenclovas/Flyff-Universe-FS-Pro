using CefSharp;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace FlyffUAutoFSPro.AppViews
{
    /// <summary>
    /// Interaction logic for Snackbar.xaml
    /// </summary>
    public partial class Snackbar : UserControl
    {
        public Snackbar()
        {
            InitializeComponent();
        }

        public void Initialize(Panel _parent, int time, string text)
        {
            InfoText.Content = text;
            _parent.Children.Add(this);
            StartTimer(_parent, time);
        }


        private async void StartTimer(Panel _parent, int time)
        {
           await Task.Delay(time * 1000);
            _parent.Children.Remove(this);
        }
    }
}
