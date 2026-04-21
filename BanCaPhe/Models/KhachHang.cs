using System;
using System.ComponentModel.DataAnnotations;

namespace BanCaPhe.Models
{
    public class KhachHang
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Họ tên không được để trống")]
        public string HoTen { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [RegularExpression(@"^(0[3|5|7|8|9])[0-9]{8}$", ErrorMessage = "Số điện thoại không đúng định dạng (VD: 0987654321)")]
        public string SoDienThoai { get; set; }

        public int DiemTichLuy { get; set; }
        public string CapBac { get; set; }
        public int PhanTramGiamGia { get; set; }
        public DateTime? NgayTao { get; set; }
        
        // Legacy properties (có thể không dùng)
        public DateTime? NgayDangKy { get; set; }
        public DateTime? NgayHetHanVoucher { get; set; }
        public bool DaSuDungVoucher { get; set; }
        public bool? ConDung { get; set; }
    }
}
