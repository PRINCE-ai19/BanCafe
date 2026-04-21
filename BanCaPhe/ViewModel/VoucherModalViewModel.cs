using BanCaPhe.Helpers;
using BanCaPhe.Models;
using BanCaPhe.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BanCaPhe.ViewModel
{
    public class VoucherModalViewModel : BaseViewModel
    {
        private readonly KhachHangService _khachHangService;

        // Customer Search
        private string _customerSearchText;
        public string CustomerSearchText
        {
            get => _customerSearchText;
            set
            {
                _customerSearchText = value;
                OnPropertyChanged();
                SearchCustomers();
            }
        }

        private ObservableCollection<KhachHang> _customerSearchResults;
        public ObservableCollection<KhachHang> CustomerSearchResults
        {
            get => _customerSearchResults;
            set
            {
                _customerSearchResults = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasCustomerResults));
                OnPropertyChanged(nameof(ShowNoCustomerMessage));
            }
        }

        public bool HasCustomerResults => CustomerSearchResults != null && CustomerSearchResults.Count > 0;
        public bool ShowNoCustomerMessage => !string.IsNullOrWhiteSpace(CustomerSearchText) && 
                                             CustomerSearchResults != null && 
                                             CustomerSearchResults.Count == 0;

        // Selected Customer
        private KhachHang _selectedCustomer;
        public KhachHang SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                _selectedCustomer = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedCustomer));
                LoadVouchersForCustomer();
            }
        }

        public bool HasSelectedCustomer => SelectedCustomer != null;

        // Vouchers
        private ObservableCollection<VoucherItem> _availableVouchers;
        public ObservableCollection<VoucherItem> AvailableVouchers
        {
            get => _availableVouchers;
            set
            {
                _availableVouchers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasVouchers));
                OnPropertyChanged(nameof(ShowNoVouchersMessage));
            }
        }

        public bool HasVouchers => AvailableVouchers != null && AvailableVouchers.Count > 0;
        public bool ShowNoVouchersMessage => HasSelectedCustomer && !IsLoadingVouchers && !HasVouchers;

        private bool _isLoadingVouchers;
        public bool IsLoadingVouchers
        {
            get => _isLoadingVouchers;
            set
            {
                _isLoadingVouchers = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ShowNoVouchersMessage));
            }
        }

        // Selected Voucher
        private VoucherItem _selectedVoucher;
        public VoucherItem SelectedVoucher
        {
            get => _selectedVoucher;
            set
            {
                if (_selectedVoucher != null)
                    _selectedVoucher.IsSelected = false;

                _selectedVoucher = value;
                
                if (_selectedVoucher != null)
                    _selectedVoucher.IsSelected = true;

                OnPropertyChanged();
                OnPropertyChanged(nameof(HasSelectedVoucher));
            }
        }

        public bool HasSelectedVoucher => SelectedVoucher != null;

        // Commands
        public ICommand SelectCustomerCommand { get; }
        public ICommand SelectVoucherCommand { get; }
        public ICommand ApplyVoucherCommand { get; }

        // Events
        public event EventHandler<VoucherAppliedEventArgs> VoucherApplied;

        public VoucherModalViewModel()
        {
            _khachHangService = new KhachHangService();
            CustomerSearchResults = new ObservableCollection<KhachHang>();
            AvailableVouchers = new ObservableCollection<VoucherItem>();

            SelectCustomerCommand = new RelayCommand<KhachHang>(SelectCustomer);
            SelectVoucherCommand = new RelayCommand<VoucherItem>(SelectVoucher);
            ApplyVoucherCommand = new RelayCommand(_ => ApplyVoucher());
        }

        private void SearchCustomers()
        {
            if (string.IsNullOrWhiteSpace(CustomerSearchText))
            {
                CustomerSearchResults.Clear();
                return;
            }

            try
            {
                var results = _khachHangService.SearchByPhone(CustomerSearchText);
                CustomerSearchResults = new ObservableCollection<KhachHang>(results);
            }
            catch (Exception ex)
            {
                DialogService.ShowError($"Lỗi tìm kiếm: {ex.Message}");
                CustomerSearchResults.Clear();
            }
        }

        private void SelectCustomer(KhachHang customer)
        {
            if (customer == null) return;

            SelectedCustomer = customer;
            CustomerSearchText = string.Empty; // Clear search box
            CustomerSearchResults.Clear(); // Hide search results
        }

        private void LoadVouchersForCustomer()
        {
            if (SelectedCustomer == null)
            {
                AvailableVouchers.Clear();
                return;
            }

            IsLoadingVouchers = true;

            try
            {
                // TODO: Thay thế bằng service thực tế để lấy voucher từ database
                // Hiện tại dùng dữ liệu mẫu
                var vouchers = GetSampleVouchers();
                AvailableVouchers = new ObservableCollection<VoucherItem>(vouchers);
            }
            catch (Exception ex)
            {
                DialogService.ShowError($"Lỗi tải voucher: {ex.Message}");
                AvailableVouchers.Clear();
            }
            finally
            {
                IsLoadingVouchers = false;
            }
        }

        private void SelectVoucher(VoucherItem voucher)
        {
            if (voucher == null) return;
            SelectedVoucher = voucher;
        }

        private void ApplyVoucher()
        {
            if (SelectedVoucher == null || SelectedCustomer == null)
            {
                DialogService.ShowError("Vui lòng chọn khách hàng và voucher!");
                return;
            }

            // Raise event to notify parent window
            VoucherApplied?.Invoke(this, new VoucherAppliedEventArgs
            {
                VoucherCode = SelectedVoucher.MaVoucher,
                DiscountAmount = SelectedVoucher.GiaTriGiamDecimal,
                Customer = SelectedCustomer
            });
        }

        // TODO: Thay thế bằng service thực tế
        private VoucherItem[] GetSampleVouchers()
        {
            return new[]
            {
                new VoucherItem
                {
                    MaVoucher = "SUMMER2024",
                    MoTa = "Giảm giá mùa hè - Áp dụng cho đơn hàng từ 100.000đ",
                    GiaTriGiam = "20.000đ",
                    GiaTriGiamDecimal = 20000,
                    NgayHetHan = DateTime.Now.AddDays(30)
                },
                new VoucherItem
                {
                    MaVoucher = "NEWMEMBER",
                    MoTa = "Ưu đãi thành viên mới - Giảm 15% tối đa 50.000đ",
                    GiaTriGiam = "15%",
                    GiaTriGiamDecimal = 50000,
                    NgayHetHan = DateTime.Now.AddDays(60)
                },
                new VoucherItem
                {
                    MaVoucher = "FREESHIP",
                    MoTa = "Miễn phí giao hàng cho đơn từ 50.000đ",
                    GiaTriGiam = "10.000đ",
                    GiaTriGiamDecimal = 10000,
                    NgayHetHan = DateTime.Now.AddDays(15)
                }
            };
        }
    }

    // Model for Voucher Item
    public class VoucherItem : BaseViewModel
    {
        public string MaVoucher { get; set; }
        public string MoTa { get; set; }
        public string GiaTriGiam { get; set; }
        public decimal GiaTriGiamDecimal { get; set; }
        public DateTime NgayHetHan { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    // Event args for voucher applied
    public class VoucherAppliedEventArgs : EventArgs
    {
        public string VoucherCode { get; set; }
        public decimal DiscountAmount { get; set; }
        public KhachHang Customer { get; set; }
    }
}
