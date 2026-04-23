using BanCaPhe.Models;
using BanCaPhe.Pdf;
using Microsoft.Win32;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BanCaPhe.ViewModel
{
    internal class HoaDonTamViewModel : BaseViewModel
    {
        public ObservableCollection<OrderItem> Items { get; }
        public decimal TongTien { get; }
        public decimal GiamGia { get; }
        public decimal TongPhaiThanhToan { get; }

        private System.Windows.Media.Imaging.BitmapImage _qrCodeImage;
        public System.Windows.Media.Imaging.BitmapImage QrCodeImage
        {
            get => _qrCodeImage;
            set { _qrCodeImage = value; OnPropertyChanged(); }
        }

        public string QrCodePayload { get; private set; }

        public ICommand XuatPdfCommand { get; }

        public HoaDonTamViewModel(
            ObservableCollection<OrderItem> items,
            decimal tongTien,
            decimal giamGia,
            decimal tongPhaiThanhToan)
        {
            Items = new ObservableCollection<OrderItem>(items);
            TongTien = tongTien;
            GiamGia = giamGia;
            TongPhaiThanhToan = tongPhaiThanhToan;

            XuatPdfCommand = new RelayCommand(_ => XuatPdf());

            LoadQrCode();
        }

        private async void LoadQrCode()
        {
            try
            {
                var payOSService = new BanCaPhe.Services.PayOSService();
                var result = await payOSService.CreatePaymentLink((long)TongPhaiThanhToan);

                if (result != null && !string.IsNullOrEmpty(result.QrCode))
                {
                    QrCodePayload = result.QrCode;
                    QrCodeImage = BanCaPhe.Helpers.QrHelper.GenerateQrCodeImage(result.QrCode);
                }
            }
            catch (Exception ex)
            {
                // Log error or show message if needed
                System.Diagnostics.Debug.WriteLine("Error loading QR code: " + ex.Message);
            }
        }

        private void XuatPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF file (*.pdf)|*.pdf",
                FileName = $"HoaDonTam_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                var document = new HoaDonTamPdf(Items, TongTien, GiamGia, TongPhaiThanhToan, QrCodePayload);
                document.GeneratePdf(saveDialog.FileName);
            }
        }
    }
}
