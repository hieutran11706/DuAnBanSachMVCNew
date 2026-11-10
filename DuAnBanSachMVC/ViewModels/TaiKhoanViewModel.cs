using System;
using System.ComponentModel.DataAnnotations;

namespace DuAnBanSachMVC.ViewModels
{
    /// <summary>Dùng cho form "Thông tin cá nhân" ở trang Tài khoản của tôi.</summary>
    public class TaiKhoanViewModel
    {
        public int Ma_Khach_Hang { get; set; }

        // Chỉ hiển thị, không cho sửa
        [Display(Name = "Tên đăng nhập")]
        public string Ten_Dang_Nhap { get; set; }

        [Display(Name = "Ngày tạo tài khoản")]
        public DateTime Ngay_Tao { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
        [Display(Name = "Họ tên")]
        public string Ho_Ten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string So_Dien_Thoai { get; set; }
    }
}
