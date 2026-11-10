using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DuAnBanSachMVC.Models;

namespace DuAnBanSachMVC.Areas.Admin.Controllers
{
    public class Khach_HangController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // GET: Admin/Khach_Hang
        public ActionResult Index()
        {
            // Lấy danh sách khách hàng và kèm theo thông tin Vai trò để hiển thị
            var khach_Hang = db.Khach_Hang.Include(k => k.Vai_Tro);
            return View(khach_Hang.ToList());
        }

        // GET: Admin/Khach_Hang/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khach_Hang khach_Hang = db.Khach_Hang.Find(id);
            if (khach_Hang == null)
            {
                return HttpNotFound();
            }
            return View(khach_Hang);
        }

        // GET: Admin/Khach_Hang/Create
        public ActionResult Create()
        {
            // Vẫn gửi danh sách vai trò qua View đề phòng bạn muốn hiển thị ô chọn.
            // Nếu bạn tự động gán vai trò bằng code ở POST, bạn có thể ẩn ô chọn này ở giao diện View.
            ViewBag.Ma_Vai_Tro = new SelectList(db.Vai_Tro, "Ma_Vai_Tro", "Ten_Vai_Tro");
            return View();
        }

        // POST: Admin/Khach_Hang/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ma_Khach_Hang,Ma_Vai_Tro,Ten_Dang_Nhap,Email,So_Dien_Thoai,Ho_Ten,Mat_Khau")] Khach_Hang khach_Hang)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Kiểm tra trùng tên đăng nhập trong hệ thống
                    var isExist = db.Khach_Hang.Any(k => k.Ten_Dang_Nhap.ToLower() == khach_Hang.Ten_Dang_Nhap.Trim().ToLower());
                    if (isExist)
                    {
                        ModelState.AddModelError("Ten_Dang_Nhap", "Tên đăng nhập này đã tồn tại!");
                        ViewBag.Ma_Vai_Tro = new SelectList(db.Vai_Tro, "Ma_Vai_Tro", "Ten_Vai_Tro", khach_Hang.Ma_Vai_Tro);
                        return View(khach_Hang);
                    }

                    // 2. TỰ ĐỘNG GÁN VAI TRÒ KHÁCH HÀNG
                    // Thay số 2 bằng ID thực tế của vai trò "Khách hàng" trong bảng Vai_Tro của bạn
                    khach_Hang.Ma_Vai_Tro = 2;

                    // 3. TỰ ĐỘNG GÁN NGÀY TẠO LÀ THỜI GIAN HIỆN TẠI
                    khach_Hang.Ngay_Tao = DateTime.Now;

                    db.Khach_Hang.Add(khach_Hang);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi khi tạo tài khoản: " + ex.Message);
                }
            }

            ViewBag.Ma_Vai_Tro = new SelectList(db.Vai_Tro, "Ma_Vai_Tro", "Ten_Vai_Tro", khach_Hang.Ma_Vai_Tro);
            return View(khach_Hang);
        }

        // GET: Admin/Khach_Hang/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khach_Hang khach_Hang = db.Khach_Hang.Find(id);
            if (khach_Hang == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ma_Vai_Tro = new SelectList(db.Vai_Tro, "Ma_Vai_Tro", "Ten_Vai_Tro", khach_Hang.Ma_Vai_Tro);
            return View(khach_Hang);
        }

        // POST: Admin/Khach_Hang/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma_Khach_Hang,Ma_Vai_Tro,Ten_Dang_Nhap,Email,So_Dien_Thoai,Ho_Ten,Ngay_Tao,Mat_Khau")] Khach_Hang khach_Hang)
        {
            if (ModelState.IsValid)
            {
                // Khi chỉnh sửa thông tin, ta giữ nguyên ngày tạo và vai trò cũ (đã được lưu ngầm trong thẻ Hidden ở View)
                db.Entry(khach_Hang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Ma_Vai_Tro = new SelectList(db.Vai_Tro, "Ma_Vai_Tro", "Ten_Vai_Tro", khach_Hang.Ma_Vai_Tro);
            return View(khach_Hang);
        }

        // GET: Admin/Khach_Hang/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Khach_Hang khach_Hang = db.Khach_Hang.Find(id);
            if (khach_Hang == null)
            {
                return HttpNotFound();
            }
            return View(khach_Hang);
        }

        // POST: Admin/Khach_Hang/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Khach_Hang khach_Hang = db.Khach_Hang.Find(id);
            db.Khach_Hang.Remove(khach_Hang);
            db.SaveChanges();
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