using System.ComponentModel.DataAnnotations;

namespace DuAnBanSachMVC.ViewModels
{
    /// <summary>Dùng cho form "Đổi mật khẩu" ở trang Tài khoản của tôi.</summary>
    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string Mat_Khau_Hien_Tai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string Mat_Khau_Moi { get; set; }

        [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [Compare("Mat_Khau_Moi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string Xac_Nhan_Mat_Khau_Moi { get; set; }
    }
}
