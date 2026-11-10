using System.ComponentModel.DataAnnotations;

namespace DuAnBanSachMVC.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string Ho_Ten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [Display(Name = "Tên đăng nhập")]
        [StringLength(50, MinimumLength = 4, ErrorMessage = "Tên đăng nhập phải từ 4-50 ký tự")]
        public string Ten_Dang_Nhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Display(Name = "Số điện thoại")]
        [RegularExpression(@"^0\d{9,10}$", ErrorMessage = "Số điện thoại không hợp lệ")]
        public string So_Dien_Thoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ {2} đến {1} ký tự")]
        [RegularExpression(
     @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[@$!%*#?&^()\-_=+\[\]{};:,.<>/\\|~`""']).+$",
     ErrorMessage = "Mật khẩu phải gồm chữ, số và ký tự đặc biệt")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Mat_Khau { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu")]
        [DataType(DataType.Password)]
        [Compare("Mat_Khau", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        [Display(Name = "Xác nhận mật khẩu")]
        public string Xac_Nhan_Mat_Khau { get; set; }
    }
}   