using BanCaPhe.Models;
using BanCaPhe.Services;
using System.Collections.ObjectModel;

namespace BanCaPhe.ViewModel
{
    public class LichSuDonHangViewModel : BaseViewModel
    {
        private ObservableCollection<LichSuDonHangModel> _lichSuDonHang;
        public ObservableCollection<LichSuDonHangModel> LichSuDonHang
        {
            get => _lichSuDonHang;
            set { _lichSuDonHang = value; OnPropertyChanged(); }
        }

        private readonly DonHangService _donHangService;

        public LichSuDonHangViewModel()
        {
            _donHangService = new DonHangService();
            LoadLichSu();
        }

        private void LoadLichSu()
        {
            var data = _donHangService.GetLichSuDonHang();
            LichSuDonHang = new ObservableCollection<LichSuDonHangModel>(data);
        }
    }
}
