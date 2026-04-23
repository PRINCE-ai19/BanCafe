using System;
using System.Windows;
using System.Windows.Threading;

namespace BanCaPhe.ViewModel
{
    public class ThanhToanThanhCongViewModel : BaseViewModel
    {
        private int _countdown = 5;
        public int Countdown
        {
            get => _countdown;
            set { _countdown = value; OnPropertyChanged(); }
        }

        private string _maGiaoDich;
        public string MaGiaoDich
        {
            get => _maGiaoDich;
            set { _maGiaoDich = value; OnPropertyChanged(); }
        }

        private decimal _tongTien;
        public decimal TongTien
        {
            get => _tongTien;
            set { _tongTien = value; OnPropertyChanged(); }
        }

        private DispatcherTimer _timer;
        private Action _onClose;

        public ThanhToanThanhCongViewModel(string maGiaoDich, decimal tongTien, Action onClose)
        {
            MaGiaoDich = maGiaoDich;
            TongTien = tongTien;
            _onClose = onClose;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            Countdown--;
            if (Countdown <= 0)
            {
                _timer.Stop();
                _onClose?.Invoke();
            }
        }
    }
}
