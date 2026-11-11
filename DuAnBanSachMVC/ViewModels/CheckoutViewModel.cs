using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DuAnBanSachMVC.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Display(Name = "Họ tên")]
        public string Ho_Ten { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        public string So_Dien_Thoai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng")]
        [Display(Name = "Địa chỉ giao hàng")]
        public string Dia_Chi_Giao_Hang { get; set; }

        [Display(Name = "Ghi chú đơn hàng")]
        public string Ghi_Chu { get; set; }

        [Display(Name = "Mã giảm giá")]
        public string Ma_Coupon { get; set; }

        // Không gửi từ form, dùng để hiển thị lại tóm tắt đơn hàng khi có lỗi
        public List<CartItemViewModel> GioHang { get; set; }
        public decimal Tong_Tien { get; set; }
        public decimal Tien_Giam_Gia { get; set; }
        public decimal Thanh_Tien { get; set; }
        public string Phuong_Thuc_Thanh_Toan { get; set; }
        public CheckoutViewModel() { Phuong_Thuc_Thanh_Toan = "COD"; }
    }
}