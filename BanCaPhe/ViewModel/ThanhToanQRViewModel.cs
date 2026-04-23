using BanCaPhe.Models;
using BanCaPhe.Services;
using QRCoder;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace BanCaPhe.ViewModel
{
    public class ThanhToanQRViewModel : BaseViewModel
    {
        private BitmapImage _qrCodeImage;
        public BitmapImage QrCodeImage
        {
            get => _qrCodeImage;
            set { _qrCodeImage = value; OnPropertyChanged(); }
        }

        private string _bankName;
        public string BankName
        {
            get => _bankName;
            set { _bankName = value; OnPropertyChanged(); }
        }

        private string _accountName;
        public string AccountName
        {
            get => _accountName;
            set { _accountName = value; OnPropertyChanged(); }
        }

        private string _accountNumber;
        public string AccountNumber
        {
            get => _accountNumber;
            set { _accountNumber = value; OnPropertyChanged(); }
        }

        private string _shopName;
        public string ShopName
        {
            get => _shopName;
            set { _shopName = value; OnPropertyChanged(); }
        }

        private string _shopAddress;
        public string ShopAddress
        {
            get => _shopAddress;
            set { _shopAddress = value; OnPropertyChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); }
        }

        private string _description;
        public string Description
        {
            get => _description;
            set { _description = value; OnPropertyChanged(); }
        }

        public long OrderCode { get; set; }
        public event EventHandler PaymentSuccess;

        private DispatcherTimer _timer;
        private readonly PayOSService _payOSService;

        public ThanhToanQRViewModel(string qrCodeData, decimal amount, long orderCode = 0)
        {
            Amount = amount;
            Description = "Thanh toán đơn hàng";
            OrderCode = orderCode;
            _payOSService = new PayOSService();

            LoadShopInfo();
            GenerateQRCode(qrCodeData);

            if (OrderCode > 0)
            {
                StartPolling();
            }
        }

        private void StartPolling()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3); // Kiểm tra mỗi 3 giây
            _timer.Tick += async (s, e) =>
            {
                string status = await _payOSService.CheckPaymentStatus(OrderCode);
                if (status == "PAID")
                {
                    _timer.Stop();
                    PaymentSuccess?.Invoke(this, EventArgs.Empty);
                }
            };
            _timer.Start();
        }

        public void StopPolling()
        {
            _timer?.Stop();
        }

        private void LoadShopInfo()
        {
            try
            {
                var shopService = new CuaHangService();
                var shopInfo = shopService.GetThongTin();
                if (shopInfo != null)
                {
                    ShopName = shopInfo.TenCuaHang;
                    ShopAddress = shopInfo.DiaChi;
                    BankName = shopInfo.TenNganHang;
                    AccountName = shopInfo.TenChuTaiKhoan?.ToUpper();
                    AccountNumber = shopInfo.SoTaiKhoan;
                }
            }
            catch { }
        }

        private void GenerateQRCode(string payload)
        {
            try
            {
                if (string.IsNullOrEmpty(payload)) return;

                using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
                using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q))
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    using (Bitmap qrCodeImage = qrCode.GetGraphic(20))
                    {
                        QrCodeImage = BitmapToImageSource(qrCodeImage);
                    }
                }
            }
            catch { }
        }

        private BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze(); // Quan trọng để dùng được đa luồng
                return bitmapimage;
            }
        }
    }
}
