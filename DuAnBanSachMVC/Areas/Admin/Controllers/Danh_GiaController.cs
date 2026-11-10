using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnBanSachMVC.Models;

namespace DuAnBanSachMVC.Areas.Admin.Controllers
{
    public class Danh_GiaController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // ==========================================
        // 1. DANH SÁCH ĐÁNH GIÁ (INDEX)
        // ==========================================
        // GET: Admin/Danh_Gia
        public ActionResult Index(int? starFilter, string searchString)
        {
            // Eager Loading để kéo hết các bảng liên quan, tránh lỗi Lazy Loading khi ra View
            var danhGias = db.Danh_Gia
                             .Include(d => d.Chi_Tiet_Don_Hang)
                             .Include(d => d.Chi_Tiet_Don_Hang.Don_Hang.Khach_Hang)
                             .Include(d => d.Chi_Tiet_Don_Hang.Sach) // Sử dụng Saches theo cấu trúc của bạn
                             .AsQueryable();

            // 1. Lọc theo số sao (nếu Admin chọn)
            if (starFilter.HasValue && starFilter >= 1 && starFilter <= 5)
            {
                danhGias = danhGias.Where(d => d.So_Sao == starFilter.Value);
            }

            // 2. Tìm kiếm theo Tên Sách hoặc Tên Khách Hàng hoặc Nội dung bình luận
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                danhGias = danhGias.Where(d => d.Binh_luan.Contains(searchString)
                                            || d.Chi_Tiet_Don_Hang.Sach.Ten_Sach.Contains(searchString)
                                            || d.Chi_Tiet_Don_Hang.Don_Hang.Khach_Hang.Ho_Ten.Contains(searchString));
            }

            // Sắp xếp các đánh giá mới nhất lên trên đầu
            var result = danhGias.OrderByDescending(d => d.Ngay_Danh_Gia).ToList();

            // Lưu lại bộ lọc để hiển thị trạng thái đã chọn trên giao diện Giao diện (Dropdown/Input)
            ViewBag.CurrentStarFilter = starFilter;
            ViewBag.CurrentSearch = searchString;

            return View(result);
        }

        // ==========================================
        // 2. XÓA ĐÁNH GIÁ (DELETE) - Dùng POST để bảo mật
        // ==========================================
        // POST: Admin/Danh_Gia/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Danh_Gia danhGia = db.Danh_Gia.Find(id);
            if (danhGia == null)
            {
                TempData["Error"] = "Không tìm thấy đánh giá cần xóa.";
                return RedirectToAction("Index");
            }

            try
            {
                db.Danh_Gia.Remove(danhGia);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa đánh giá thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra khi xóa đánh giá: " + ex.Message;
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}