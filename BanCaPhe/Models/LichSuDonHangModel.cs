using System;

namespace BanCaPhe.Models
{
    public class LichSuDonHangModel
    {
        public int DonHangID { get; set; }
        public DateTime NgayLap { get; set; }
        public decimal TongTienDonHang { get; set; }
        public string HinhThucThanhToan { get; set; }
        public string TrangThaiThanhToan { get; set; }
        public string TenNhanVien { get; set; }
        public string TenDoUong { get; set; }
        public string HinhAnhDoUong { get; set; }
        public string TenKichThuoc { get; set; }
        public int SoLuong { get; set; }
        public decimal GiaBan { get; set; }
        public string DanhSachTopping { get; set; }
    }
}
