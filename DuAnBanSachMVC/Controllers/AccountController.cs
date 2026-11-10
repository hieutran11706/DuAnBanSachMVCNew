using DuAnBanSachMVC.Models;
using DuAnBanSachMVC.Utilities;
using DuAnBanSachMVC.ViewModels;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DuAnBanSachMVC.Controllers
{
    // Toàn bộ action trong controller này chỉ dành cho khách hàng đã đăng nhập.
    public class AccountController : BaseController
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Chưa có kết quả (chưa bị redirect bởi filter khác) và chưa đăng nhập -> đá về trang Login
            if (filterContext.Result == null && !MaKhachHangHienTai.HasValue)
            {
                var returnUrl = filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = RedirectToAction("Login", "Home", new { returnUrl = returnUrl });
            }
        }

        // Dựng lại toàn bộ dữ liệu cho trang (dùng chung cho GET và khi POST bị lỗi validate)
        private TaiKhoanTongHopViewModel XayDungModel(string tab)
        {
            var kh = db.Khach_Hang.Find(MaKhachHangHienTai.Value);

            var model = new TaiKhoanTongHopViewModel
            {
                TabHienTai = string.IsNullOrEmpty(tab) ? "thongtin" : tab,
                ThongTin = new TaiKhoanViewModel
                {
                    Ma_Khach_Hang = kh.Ma_Khach_Hang,
                    Ten_Dang_Nhap = kh.Ten_Dang_Nhap,
                    Ngay_Tao = kh.Ngay_Tao,
                    Ho_Ten = kh.Ho_Ten,
                    Email = kh.Email,
                    So_Dien_Thoai = kh.So_Dien_Thoai
                },
                DonHangs = db.Don_Hang
                    .Include(d => d.Chi_Tiet_Don_Hang.Select(c => c.Sach))
                    .Where(d => d.Ma_Khach_Hang == MaKhachHangHienTai.Value)
                    .OrderByDescending(d => d.Ngay_Dat)
                    .ToList()
            };

            return model;
        }

        // GET: /Account/ThongTinTaiKhoan?tab=thongtin|doimatkhau|donhang
        public ActionResult ThongTinTaiKhoan(string tab)
        {
            var model = XayDungModel(tab);
            return View(model);
        }

        // POST: /Account/CapNhatThongTin — cập nhật họ tên, email, số điện thoại
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatThongTin(TaiKhoanViewModel ThongTin)
        {
            int maKh = MaKhachHangHienTai.Value;
            var email = (ThongTin.Email ?? "").Trim();

            if (db.Khach_Hang.Any(k => k.Ma_Khach_Hang != maKh && k.Email == email))
            {
                ModelState.AddModelError("ThongTin.Email", "Email này đã được sử dụng bởi tài khoản khác.");
            }

            if (!ModelState.IsValid)
            {
                var fullModel = XayDungModel("thongtin");
                fullModel.ThongTin.Ho_Ten = ThongTin.Ho_Ten;
                fullModel.ThongTin.Email = ThongTin.Email;
                fullModel.ThongTin.So_Dien_Thoai = ThongTin.So_Dien_Thoai;
                return View("ThongTinTaiKhoan", fullModel);
            }

            var kh = db.Khach_Hang.Find(maKh);
            kh.Ho_Ten = ThongTin.Ho_Ten.Trim();
            kh.Email = email;
            kh.So_Dien_Thoai = ThongTin.So_Dien_Thoai;
            db.SaveChanges();

            // Đồng bộ lại tên hiển thị trên header
            Session["TenKhachHang"] = kh.Ho_Ten;

            TempData["ThongBao"] = "Cập nhật thông tin cá nhân thành công.";
            TempData["LoaiThongBao"] = "success";
            return RedirectToAction("ThongTinTaiKhoan", new { tab = "thongtin" });
        }

        // POST: /Account/DoiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiMatKhau(DoiMatKhauViewModel DoiMatKhau)
        {
            int maKh = MaKhachHangHienTai.Value;
            var kh = db.Khach_Hang.Find(maKh);

            if (!PasswordHasher.VerifyPassword(DoiMatKhau.Mat_Khau_Hien_Tai, kh.Mat_Khau))
            {
                ModelState.AddModelError("DoiMatKhau.Mat_Khau_Hien_Tai", "Mật khẩu hiện tại không đúng.");
            }

            if (!ModelState.IsValid)
            {
                var fullModel = XayDungModel("doimatkhau");
                return View("ThongTinTaiKhoan", fullModel);
            }

            kh.Mat_Khau = PasswordHasher.HashPassword(DoiMatKhau.Mat_Khau_Moi);
            db.SaveChanges();

            TempData["ThongBao"] = "Đổi mật khẩu thành công.";
            TempData["LoaiThongBao"] = "success";
            return RedirectToAction("ThongTinTaiKhoan", new { tab = "doimatkhau" });
        }
    }
}
