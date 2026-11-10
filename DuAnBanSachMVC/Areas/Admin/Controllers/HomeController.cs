using DuAnBanSachMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace DuAnBanSachMVC.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        public ActionResult Dashboard()
        {
            try
            {
                // 1. TÍNH DOANH THU THÁNG HIỆN TẠI
                var ngayDauThang = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var ngayCuoiThang = ngayDauThang.AddMonths(1).AddDays(-1);

                decimal doanhThu = db.Don_Hang
                    .Where(d => d.Ngay_Dat >= ngayDauThang && d.Ngay_Dat <= ngayCuoiThang && d.Trang_Thai == "Đã giao")
                    .Select(d => (decimal?)d.Tong_Tien)
                    .Sum() ?? 0;

                ViewBag.DoAccessValue = doanhThu.ToString("N0") + "đ"; // Gán chuỗi doanh thu sạch

                // 2. THỐNG KÊ SÁCH CẦN NHẬP KHO
                int mucCanhBao = 10;
                var sachSapHet = db.Saches
                    .Include(s => s.Tac_Gia)
                    .Where(s => s.So_Luong_Ton < mucCanhBao)
                    .OrderBy(s => s.So_Luong_Ton)
                    .ToList();

                ViewBag.SoDauSachCanNhap = sachSapHet.Count;
                ViewBag.DanhSachSachCanNhap = sachSapHet.Take(5).ToList();

                // 3. THỐNG KÊ ĐƠN HÀNG CHỜ XỬ LÝ
                ViewBag.DonHangChoXuLy = db.Don_Hang.Count(d => d.Trang_Thai == "Chờ xử lý");

                // 4. LẤY KHÁCH HÀNG MỚI THAM GIA THẬT (Tách biệt hoàn toàn)
                ViewBag.ThanhVienMoi = db.Khach_Hang
                    .OrderByDescending(k => k.Ngay_Tao)
                    .Take(4)
                    .ToList();

                // 5. LẤY PHẢN HỒI & ĐÁNH GIÁ THỰC TẾ TRÊN HỆ THỐNG
                ViewBag.DanhGiaGanDay = db.Danh_Gia
                    .Include(d => d.Chi_Tiet_Don_Hang.Don_Hang.Khach_Hang)
                    .OrderByDescending(d => d.Ngay_Danh_Gia)
                    .Take(4)
                    .ToList();
            }
            catch (Exception ex)
            {
                ViewBag.DoanhThuThang = "0đ";
                ViewBag.SoDauSachCanNhap = 0;
                ViewBag.DonHangChoXuLy = 0;
                ViewBag.DanhSachSachCanNhap = new List<DuAnBanSachMVC.Models.Sach>();
                ViewBag.ThanhVienMoi = new List<DuAnBanSachMVC.Models.Khach_Hang>();
                ViewBag.DanhGiaGanDay = new List<DuAnBanSachMVC.Models.Danh_Gia>();
                ViewBag.ErrorMessage = ex.Message;
            }

            return View("Dashboard");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose(); // Đóng kết nối DB khi dùng xong để nhẹ server
            }
            base.Dispose(disposing);
        }

       
    }
}