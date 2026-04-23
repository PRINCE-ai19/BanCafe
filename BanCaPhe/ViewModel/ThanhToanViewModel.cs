using BanCaPhe.Models;
using BanCaPhe.Services;
using BanCaPhe.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace BanCaPhe.ViewModel
{
    internal class ThanhToanViewModel : BaseViewModel
    {
        public decimal TongTienGoc { get; }
        public decimal GiamGia { get; }
        public decimal TongPhaiThanhToan { get; }
        public ObservableCollection<OrderItem> Items { get; }

        public ICommand TienMatCommand { get; }
        public ICommand ChuyenKhoanCommand { get; }
        public ICommand VnpayCommand { get; }
        public ICommand DongCommand { get; }

        public ThanhToanViewModel(decimal tongTienGoc, decimal giamGia, decimal tongPhaiThanhToan, ObservableCollection<OrderItem> items)
        {
            TongTienGoc = tongTienGoc;
            GiamGia = giamGia;
            TongPhaiThanhToan = tongPhaiThanhToan;
            Items = items;

            TienMatCommand = new RelayCommand(_ => ThanhToanTienMat());
            ChuyenKhoanCommand = new RelayCommand(_ => ThanhToanChuyenKhoan());
            VnpayCommand = new RelayCommand(_ => ThanhToanVnpay());
            DongCommand = new RelayCommand(_ => Dong());
        }

     
        private void ThanhToanTienMat()
        {
      
            var vm = new ThanhToanTienMatViewModel(TongTienGoc);

            var view = new ThanhToanTienMatView
            {
                DataContext = vm,
                Owner = Application.Current.Windows
                    .OfType<Window>()
                    .FirstOrDefault(w => w.IsActive)
            };

            view.ShowDialog();
            Dong();
        }

   
        private void ThanhToanChuyenKhoan()
        {
            DialogService.ShowMessage("Thanh toán chuyển khoản (sẽ làm sau)");
        }


        private async void ThanhToanVnpay()
        {
            var payOSService = new BanCaPhe.Services.PayOSService();
            var donHangService = new BanCaPhe.Services.DonHangService();

            try
            {
           
                string maGiaoDich = "PAYOS" + DateTime.Now.Ticks.ToString().Substring(10);

              
                var currentUser = UserSession.CurrentUser;
                var donHang = new DonHang
                {
                    NgayLap = DateTime.Now,
                    NhanVienID = currentUser?.ID ?? 0,
                    TongTien = TongPhaiThanhToan,
                    HinhThucThanhToan = "Chuyen khoan",
                    KhachHangID = CartService.Instance.CurrentKhachHang?.ID,
                    MaGiaoDich = maGiaoDich,
                    TrangThaiThanhToan = "Chưa thanh toán"
                };

                var items = Items.ToList();
                donHangService.ThanhToan(donHang, items);

     
                var result = await payOSService.CreatePaymentLink((long)TongPhaiThanhToan);

                if (result != null && !string.IsNullOrEmpty(result.QrCode))
                {
                    // 4. Mở Form hiển thị mã QR (MVVM)
                    var qrVM = new ThanhToanQRViewModel(result.QrCode, TongPhaiThanhToan, result.OrderCode);
                    var qrWindow = new W_ThanhToanQR
                    {
                        DataContext = qrVM,
                        Owner = Application.Current.Windows
                            .OfType<Window>()
                            .FirstOrDefault(w => w.IsActive)
                    };
                    
                    bool isPaid = false;
                    qrVM.PaymentSuccess += (s, e) => {
                        isPaid = true;
                        qrWindow.Close(); // Tự động đóng form QR khi đã nhận được tiền
                    };

                    qrWindow.ShowDialog();
                    qrVM.StopPolling(); // Dừng polling nếu đóng bằng tay

                    if (isPaid)
                    {
                        // Cập nhật trạng thái thành công trong Database
                        donHangService.UpdateTrangThaiThanhToan(maGiaoDich, "Thanh toán thành công");

                        // 5. Hiện form thành công
                        var successVM = new ThanhToanThanhCongViewModel(maGiaoDich, TongPhaiThanhToan, () => {
                            // Sẽ gán sau khi tạo window
                        });

                        var successWindow = new W_ThanhToanThanhCong
                        {
                            Owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                        };

                        successVM = new ThanhToanThanhCongViewModel(maGiaoDich, TongPhaiThanhToan, () => {
                            successWindow.Close();
                        });
                        successWindow.DataContext = successVM;

                        successWindow.ShowDialog();

                        CartService.Instance.Clear();
                        CartService.Instance.NotifyTongTienChanged();
                        Dong();
                    }
                }
                else
                {
                    MessageBox.Show("Không thể lấy được thông tin thanh toán từ API.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu đơn/tạo QR: " + ex.Message);
            }
        }


        private void Dong()
        {
            Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w.IsActive)
                ?.Close();
        }
    }
}
