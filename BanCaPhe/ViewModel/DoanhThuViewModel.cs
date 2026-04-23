using BanCaPhe.Models;
using BanCaPhe.Services;
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
    internal class DoanhThuViewModel : BaseViewModel
    {
        private readonly DoanhThuService _service;

        private decimal _tongDoanhThu;
        public decimal TongDoanhThu
        {
            get => _tongDoanhThu;
            set
            {
                _tongDoanhThu = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TongDoanhThuDisplay));
            }
        }

        private int _tongDonHang;
        public int TongDonHang
        {
            get => _tongDonHang;
            set
            {
                _tongDonHang = value;
                OnPropertyChanged();
            }
        }

        private string _thangNamHienTai;
        public string ThangNamHienTai
        {
            get => _thangNamHienTai;
            set
            {
                _thangNamHienTai = value;
                OnPropertyChanged();
            }
        }

        public string TongDoanhThuDisplay => $"{TongDoanhThu:N0} ₫";

        // thống kê Đơn hàng 
        private decimal _doanhThuTrungBinh;
        public decimal DoanhThuTrungBinh
        {
            get => _doanhThuTrungBinh;
            set
            {
                _doanhThuTrungBinh = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DoanhThuTrungBinhDisplay));
            }
        }

        private decimal _donHangLonNhat;
        public decimal DonHangLonNhat
        {
            get => _donHangLonNhat;
            set
            {
                _donHangLonNhat = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DonHangLonNhatDisplay));
            }
        }


        private decimal _donHangNhoNhat;
        public decimal DonHangNhoNhat
        {
            get => _donHangNhoNhat;
            set
            {
                _donHangNhoNhat = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DonHangNhoNhatDisplay));
            }
        }

        public string DoanhThuTrungBinhDisplay => $"{DoanhThuTrungBinh:N0} ₫";
        public string DonHangLonNhatDisplay => $"{DonHangLonNhat:N0} ₫";
        public string DonHangNhoNhatDisplay => $"{DonHangNhoNhat:N0} ₫";


        // sản phẩm bán chạy

        private ObservableCollection<SanPhamBanChayModel> _sanPhamBanChay;
        public ObservableCollection<SanPhamBanChayModel> SanPhamBanChay
        {
            get => _sanPhamBanChay;
            set
            {
                _sanPhamBanChay = value;
                OnPropertyChanged();
            }
        }

        private SanPhamBanChayModel _sanPhamBanChayNhat;
        public SanPhamBanChayModel SanPhamBanChayNhat
        {
            get => _sanPhamBanChayNhat;
            set
            {
                _sanPhamBanChayNhat = value;
                OnPropertyChanged();
            }
        }

        // topping phor biến 
        
        private ObservableCollection<DoanhThuTheoNgayModel> _chiTietDoanhThu;
        public ObservableCollection<DoanhThuTheoNgayModel> ChiTietDoanhThu
        {
            get => _chiTietDoanhThu;
            set
            {
                _chiTietDoanhThu = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<ToppingPhoBienModel> _toppingPhoBien;
        public ObservableCollection<ToppingPhoBienModel> ToppingPhoBien
        {
            get => _toppingPhoBien;
            set
            {
                _toppingPhoBien = value;
                OnPropertyChanged();
            }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }


        public ICommand LoadDataCommand { get; }
        public ICommand RefreshCommand { get; }


        public DoanhThuViewModel()
        {
            _service = new DoanhThuService();

            SanPhamBanChay = new ObservableCollection<SanPhamBanChayModel>();
            ToppingPhoBien = new ObservableCollection<ToppingPhoBienModel>();
            ChiTietDoanhThu = new ObservableCollection<DoanhThuTheoNgayModel>();

            LoadDataCommand = new RelayCommand(_ => LoadData());
            RefreshCommand = new RelayCommand(_ => RefreshData());

            // Load dữ liệu khi khởi tạo
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                IsLoading = true;
                StatusMessage = "Đang tải dữ liệu...";

                // 1. Load doanh thu tháng này
                var doanhThuThang = _service.GetDoanhThuThangNay();
                if (doanhThuThang != null)
                {
                    TongDoanhThu = doanhThuThang.TongDoanhThu;
                    TongDonHang = doanhThuThang.TongDonHang;
                    ThangNamHienTai = doanhThuThang.ThangNamDisplay;
                }

                // 2. Load thống kê đơn hàng
                var thongKeDonHang = _service.GetTongDonHangThangNay();
                if (thongKeDonHang != null)
                {
                    DoanhThuTrungBinh = thongKeDonHang.DoanhThuTrungBinh;
                    DonHangLonNhat = thongKeDonHang.DonHangLonNhat;
                    DonHangNhoNhat = thongKeDonHang.DonHangNhoNhat;
                }

                // 3. Load sản phẩm bán chạy (top 10)
                var sanPhamList = _service.GetSanPhamBanChayNhat(10);
                SanPhamBanChay.Clear();
                int maxSoLuong = sanPhamList.Any() ? sanPhamList.Max(x => x.TongSoLuongBan) : 1;
                if (maxSoLuong == 0) maxSoLuong = 1;
                
                double totalSoLuong = sanPhamList.Sum(x => x.TongSoLuongBan);
                if (totalSoLuong == 0) totalSoLuong = 1;
                double currentAngle = 0;
                double radius = 100;
                double cx = 100;
                double cy = 100;
                string[] pieColors = { "#EF4444", "#3B82F6", "#10B981", "#F59E0B", "#8B5CF6", "#EC4899", "#14B8A6", "#F97316", "#6366F1", "#84CC16" };
                int colorIndex = 0;

                foreach (var sp in sanPhamList)
                {
                    sp.ValueRatio = (double)sp.TongSoLuongBan / maxSoLuong;
                    
                    // --- Pie Chart Calculation ---
                    double sweepAngle = (sp.TongSoLuongBan / totalSoLuong) * 360;
                    if (sweepAngle >= 360) sweepAngle = 359.99; // Ngăn lỗi vẽ nguyên vòng tròn trong WPF Path
                    
                    double startAngleRad = (currentAngle - 90) * Math.PI / 180.0; // -90 để bắt đầu từ hướng 12h
                    double endAngleRad = (currentAngle + sweepAngle - 90) * Math.PI / 180.0;
                    
                    double x1 = cx + radius * Math.Cos(startAngleRad);
                    double y1 = cy + radius * Math.Sin(startAngleRad);
                    
                    double x2 = cx + radius * Math.Cos(endAngleRad);
                    double y2 = cy + radius * Math.Sin(endAngleRad);
                    
                    int isLargeArc = sweepAngle > 180 ? 1 : 0;
                    
                    sp.PiePathData = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                        "M {0},{1} L {2},{3} A {4},{4} 0 {5},1 {6},{7} Z",
                        cx, cy, x1, y1, radius, isLargeArc, x2, y2);
                        
                    sp.PieColor = pieColors[colorIndex % pieColors.Length];
                    
                    currentAngle += sweepAngle;
                    colorIndex++;

                    SanPhamBanChay.Add(sp);
                }

                // Lấy sản phẩm bán chạy nhất (top 1)
                if (sanPhamList.Count > 0)
                {
                    SanPhamBanChayNhat = sanPhamList[0];
                }

                // 4. Load topping phổ biến (top 5)
                var toppingList = _service.GetToppingPhoBienNhat(5);
                ToppingPhoBien.Clear();
                foreach (var tp in toppingList)
                {
                    ToppingPhoBien.Add(tp);
                }

                // 5. Load chi tiết doanh thu theo ngày
                var chiTietList = _service.GetChiTietDoanhThuTheoNgay(DateTime.Now.Month, DateTime.Now.Year);
                ChiTietDoanhThu.Clear();
                decimal maxDoanhThu = chiTietList.Any() ? chiTietList.Max(x => x.TongDoanhThu) : 1;
                if (maxDoanhThu == 0) maxDoanhThu = 1;
                foreach (var ct in chiTietList)
                {
                    ct.ValueRatio = (double)(ct.TongDoanhThu / maxDoanhThu);
                    ChiTietDoanhThu.Add(ct);
                }

                StatusMessage = "Tải dữ liệu thành công!";
            }
            catch (Exception ex)
            {
                StatusMessage = $"Lỗi: {ex.Message}";
                DialogService.ShowError($"Không thể tải dữ liệu: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void RefreshData()
        {
            LoadData();
        }
    }
}
