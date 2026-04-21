using BanCaPhe.Helpers;
using BanCaPhe.ViewModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace BanCaPhe
{
    public partial class W_AuthWindow : Window
    {
        private DangNhapViewModel _loginViewModel;
        private DangKyViewModel _registerViewModel;

        public W_AuthWindow()
        {
            InitializeComponent();
            _loginViewModel = new DangNhapViewModel();
            _registerViewModel = new DangKyViewModel();
        }

        private void ShowRegister_Click(object sender, MouseButtonEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("SlideToRegister");
            storyboard.Begin();
            Title = "Modtra - Đăng ký";
        }

        private void ShowLogin_Click(object sender, MouseButtonEventArgs e)
        {
            var storyboard = (Storyboard)FindResource("SlideToLogin");
            storyboard.Begin();
            Title = "Modtra - Đăng nhập";
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            _loginViewModel.Email = txtLoginEmail.Text;
            
            if (_loginViewModel.DangNhapCommand.CanExecute(txtLoginPassword))
            {
                _loginViewModel.DangNhapCommand.Execute(txtLoginPassword);
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            _registerViewModel.HoTen = txtRegisterName.Text;
            _registerViewModel.Email = txtRegisterEmail.Text;
            _registerViewModel.SoDienThoai = txtRegisterPhone.Text;

            if (txtRegisterPassword.Password != txtRegisterConfirm.Password)
            {
                DialogService.ShowError("Mật khẩu xác nhận không khớp!");
                return;
            }

            if (_registerViewModel.DangKyCommand.CanExecute(txtRegisterPassword))
            {
                _registerViewModel.DangKyCommand.Execute(txtRegisterPassword);
                
                // Nếu đăng ký thành công, chuyển về login
                var storyboard = (Storyboard)FindResource("SlideToLogin");
                storyboard.Begin();
                Title = "Modtra - Đăng nhập";
            }
        }
    }
}
