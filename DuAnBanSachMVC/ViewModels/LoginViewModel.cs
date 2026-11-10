using System.ComponentModel.DataAnnotations;

namespace DuAnBanSachMVC.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập hoặc email")]
        [Display(Name = "Tên đăng nhập hoặc email")]
        public string Ten_Dang_Nhap { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Mat_Khau { get; set; }

        public bool RememberMe { get; set; }
    }
}