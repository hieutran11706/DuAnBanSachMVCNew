using DuAnBanSachMVC.Models;
using System.Collections.Generic;

namespace DuAnBanSachMVC.ViewModels
{
    /// <summary>
    /// Gộp dữ liệu cho toàn bộ trang "Tài khoản của tôi" (thông tin cá nhân,
    /// đổi mật khẩu, lịch sử đơn hàng) để 1 view duy nhất có thể hiển thị cả 3 tab.
    /// </summary>
    public class TaiKhoanTongHopViewModel
    {
        public TaiKhoanViewModel ThongTin { get; set; }
        public DoiMatKhauViewModel DoiMatKhau { get; set; }
        public List<Don_Hang> DonHangs { get; set; }

        // Tab đang được chọn: "thongtin" | "doimatkhau" | "donhang"
        public string TabHienTai { get; set; }

        public TaiKhoanTongHopViewModel()
        {
            ThongTin = new TaiKhoanViewModel();
            DoiMatKhau = new DoiMatKhauViewModel();
            DonHangs = new List<Don_Hang>();
            TabHienTai = "thongtin";
        }
    }
}
