using BanCaPhe.ViewModel;
using System.Windows;

namespace BanCaPhe
{
    public partial class VoucherModal : Window
    {
        private VoucherModalViewModel _viewModel;

        public KhachHang SelectedCustomer { get; private set; }

        public VoucherModal()
        {
            InitializeComponent();
            _viewModel = new VoucherModalViewModel();
            _viewModel.CustomerSelected += OnCustomerSelected;
            DataContext = _viewModel;
        }

        private void OnCustomerSelected(object sender, CustomerSelectedEventArgs e)
        {
            SelectedCustomer = e.Customer;
            this.DialogResult = true;
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
