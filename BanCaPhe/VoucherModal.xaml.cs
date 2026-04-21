using BanCaPhe.ViewModel;
using System.Windows;

namespace BanCaPhe
{
    public partial class VoucherModal : Window
    {
        private VoucherModalViewModel _viewModel;

        public string AppliedVoucherCode { get; private set; }
        public decimal VoucherDiscount { get; private set; }

        public VoucherModal()
        {
            InitializeComponent();
            _viewModel = new VoucherModalViewModel();
            _viewModel.VoucherApplied += OnVoucherApplied;
            DataContext = _viewModel;
        }

        private void OnVoucherApplied(object sender, VoucherAppliedEventArgs e)
        {
            AppliedVoucherCode = e.VoucherCode;
            VoucherDiscount = e.DiscountAmount;
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
