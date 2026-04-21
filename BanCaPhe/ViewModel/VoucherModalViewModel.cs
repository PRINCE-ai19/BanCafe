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
            }
        }

        public bool HasSelectedCustomer => SelectedCustomer != null;

        // Commands
        public ICommand SelectCustomerCommand { get; }
        public ICommand ApplyCustomerCommand { get; }

        // Events
        public event EventHandler<CustomerSelectedEventArgs> CustomerSelected;

        public VoucherModalViewModel()
        {
            _khachHangService = new KhachHangService();
            CustomerSearchResults = new ObservableCollection<KhachHang>();

            SelectCustomerCommand = new RelayCommand<KhachHang>(SelectCustomer);
            ApplyCustomerCommand = new RelayCommand(_ => ApplyCustomer());
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

        private void ApplyCustomer()
        {
            if (SelectedCustomer == null)
            {
                DialogService.ShowError("Vui lòng chọn khách hàng!");
                return;
            }

            // Raise event to notify parent window
            CustomerSelected?.Invoke(this, new CustomerSelectedEventArgs
            {
                Customer = SelectedCustomer
            });
        }
    }

    // Event args for customer selected
    public class CustomerSelectedEventArgs : EventArgs
    {
        public KhachHang Customer { get; set; }
    }
}
