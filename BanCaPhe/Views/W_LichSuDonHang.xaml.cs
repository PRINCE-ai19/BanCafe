using BanCaPhe.ViewModel;
using System.Windows;

namespace BanCaPhe.Views
{
    public partial class W_LichSuDonHang : Window
    {
        public W_LichSuDonHang()
        {
            InitializeComponent();
            DataContext = new LichSuDonHangViewModel();
        }
    }
}
