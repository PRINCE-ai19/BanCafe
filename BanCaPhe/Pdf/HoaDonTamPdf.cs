using BanCaPhe.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BanCaPhe.Pdf
{
    internal class HoaDonTamPdf : IDocument
    {
        private readonly IEnumerable<OrderItem> _items;
        private readonly decimal _tongTien;
        private readonly decimal _giamGia;
        private readonly decimal _tongPhaiThanhToan;
        private readonly string _qrCodePayload;

        public HoaDonTamPdf(IEnumerable<OrderItem> items, decimal tongTien, decimal giamGia, decimal tongPhaiThanhToan, string qrCodePayload = null)
        {
            _items = items;
            _tongTien = tongTien;
            _giamGia = giamGia;
            _tongPhaiThanhToan = tongPhaiThanhToan;
            _qrCodePayload = qrCodePayload;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A5);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // HEADER 
                    col.Item().AlignCenter().Text("HÓA ĐƠN TẠM")
                        .FontSize(18)
                        .Bold();

                    col.Item().AlignCenter().Text($"Ngày: {DateTime.Now:dd/MM/yyyy HH:mm}");

                    col.Item().LineHorizontal(1);

                    // DANH SÁCH MÓN 
                    foreach (var item in _items)
                    {
                        col.Item().Column(itemCol =>
                        {
                            itemCol.Spacing(4);

                            itemCol.Item().Text($"{item.Ten} ({item.Size})")
                                .Bold();

                            // Hiển thị topping nếu có
                            if (item.Toppings != null && item.Toppings.Any())
                            {
                                foreach (var topping in item.Toppings)
                                {
                                    itemCol.Item().PaddingLeft(10).Text($"+ {topping.Ten} x{topping.SoLuong} ({topping.Gia:N0} đ)")
                                        .FontSize(10)
                                        .FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                }
                            }

                            if (!string.IsNullOrWhiteSpace(item.GhiChu))
                            {
                                itemCol.Item().Text($"Ghi chú: {item.GhiChu}")
                                    .Italic()
                                    .FontSize(10)
                                  .FontColor(QuestPDF.Helpers.Colors.Grey.Darken1);

                            }

                            // Số lượng và thành tiền
                            itemCol.Item().Row(row =>
                            {
                                row.RelativeItem().Text($"SL: {item.SoLuong}");
                                row.ConstantItem(80)
                                   .AlignRight()
                                   .Text($"{item.ThanhTien:N0} đ")
                                   .Bold();
                            });
                        });

                        col.Item().LineHorizontal(0.5f);
                    }

                    // TỔNG 
                    col.Item().PaddingTop(10).Column(totalCol =>
                    {
                        totalCol.Spacing(5);

                        totalCol.Item().Row(row =>
                        {
                            row.RelativeItem().Text("TỔNG CỘNG").Bold();
                            row.ConstantItem(100).AlignRight().Text($"{_tongTien:N0} đ").Bold();
                        });

                        if (_giamGia > 0)
                        {
                            totalCol.Item().Row(row =>
                            {
                                row.RelativeItem().Text("GIẢM GIÁ").FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                                row.ConstantItem(100).AlignRight().Text($"- {_giamGia:N0} đ").FontColor(QuestPDF.Helpers.Colors.Grey.Darken2);
                            });
                        }

                        totalCol.Item().Row(row =>
                        {
                            row.RelativeItem().Text("THANH TOÁN").Bold().FontSize(14);
                            row.ConstantItem(100).AlignRight().Text($"{_tongPhaiThanhToan:N0} đ")
                                .Bold()
                                .FontSize(14)
                                .FontColor(QuestPDF.Helpers.Colors.Red.Darken1);
                        });
                    });

                    // QR CODE
                    if (!string.IsNullOrEmpty(_qrCodePayload))
                    {
                        var qrBytes = BanCaPhe.Helpers.QrHelper.GenerateQrCodeBytes(_qrCodePayload);
                        if (qrBytes != null)
                        {
                            col.Item().PaddingTop(20).Column(qrCol =>
                            {
                                qrCol.Item().AlignCenter().Text("MÃ QUÉT THANH TOÁN").FontSize(10).Bold();
                                qrCol.Item().AlignCenter().Width(120).Height(120).Image(qrBytes);
                            });
                        }
                    }
                });
            });
        }
    }
}
