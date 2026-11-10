using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using DuAnBanSachMVC.Models;

namespace DuAnBanSachMVC.Areas.Admin.Controllers
{
    public class Don_HangController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // ==========================================
        // 1. DANH SÁCH ĐƠN HÀNG (INDEX)
        // ==========================================
        // GET: Admin/Don_Hang
        public ActionResult Index(string searchString, string statusFilter)
        {
            // Khởi tạo truy vấn lấy toàn bộ đơn hàng
            var donHangs = db.Don_Hang.Include(d => d.Khach_Hang).AsQueryable();

            // Nếu Admin nhập chuỗi tìm kiếm (Mã đơn hoặc Tên khách)
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
                donHangs = donHangs.Where(d => d.Khach_Hang.Ho_Ten.Contains(searchString)
                                            || d.Ma_Don_Hang.ToString().Equals(searchString));
            }

            // Nếu Admin lọc theo Trạng thái
            if (!string.IsNullOrEmpty(statusFilter))
            {
                donHangs = donHangs.Where(d => d.Trang_Thai == statusFilter);
            }

            // Sắp xếp đơn hàng mới nhất lên trên đầu
            var result = donHangs.OrderByDescending(d => d.Ngay_Dat).ToList();
            return View(result);
        }

        // ==========================================
        // 2. CHI TIẾT ĐƠN HÀNG (DETAILS)
        // ==========================================
        // GET: Admin/Don_Hang/Details/5
        // GET: Admin/Don_Hang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // SỬA THÀNH .Select(ct => ct.Sach) vì tên navigation property bên trong Chi_Tiet_Don_Hang là Sach
            var donHang = db.Don_Hang
                            .Include(d => d.Khach_Hang)
                            .Include(d => d.Chi_Tiet_Don_Hang.Select(ct => ct.Sach))
                            .FirstOrDefault(d => d.Ma_Don_Hang == id);

            if (donHang == null)
            {
                return HttpNotFound();
            }
            return View(donHang);
        }

        // POST: Admin/Don_Hang/CapNhatTrangThai
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatTrangThai(int id, string trangThaiMoi)
        {
            var donHang = db.Don_Hang.Include(d => d.Chi_Tiet_Don_Hang).FirstOrDefault(d => d.Ma_Don_Hang == id);

            if (donHang != null)
            {
                string trangThaiCu = donHang.Trang_Thai;

                if (trangThaiCu != trangThaiMoi)
                {
                    // TRƯỜNG HỢP 1: Trừ kho khi chuyển từ Chờ xử lý -> Đang giao/Đã giao
                    if (trangThaiCu == "Chờ xử lý" && (trangThaiMoi == "Đang giao" || trangThaiMoi == "Đã giao"))
                    {
                        foreach (var item in donHang.Chi_Tiet_Don_Hang)
                        {
                            // ĐÚNG CẤU TRÚC: Gọi bảng db.Saches để tìm sách
                            var sachInDb = db.Saches.Find(item.Ma_Sach);
                            if (sachInDb != null)
                            {
                                if (sachInDb.So_Luong_Ton < item.So_Luong)
                                {
                                    TempData["Error"] = $"Sách '{sachInDb.Ten_Sach}' không đủ số lượng trong kho! (Tồn: {sachInDb.So_Luong_Ton})";
                                    return RedirectToAction("Details", new { id = id });
                                }
                                sachInDb.So_Luong_Ton -= item.So_Luong; // Trừ tồn kho
                            }
                        }
                    }
                    // TRƯỜNG HỢP 2: Hoàn kho khi đơn hàng bị Hủy
                    else if ((trangThaiCu == "Đang giao" || trangThaiCu == "Đã giao") && trangThaiMoi == "Đã hủy")
                    {
                        foreach (var item in donHang.Chi_Tiet_Don_Hang)
                        {
                            var sachInDb = db.Saches.Find(item.Ma_Sach);
                            if (sachInDb != null)
                            {
                                sachInDb.So_Luong_Ton += item.So_Luong; // Cộng trả lại kho
                            }
                        }
                    }

                    donHang.Trang_Thai = trangThaiMoi;
                    db.Entry(donHang).State = EntityState.Modified;
                    db.SaveChanges();

                    TempData["Success"] = $"Cập nhật trạng thái đơn hàng sang '{trangThaiMoi}' thành công!";
                }
            }
            return RedirectToAction("Details", new { id = id });
        }
    }
}