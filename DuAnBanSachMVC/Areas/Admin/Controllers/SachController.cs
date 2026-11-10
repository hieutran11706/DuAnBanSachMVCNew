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
    public class SachController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // GET: Admin/Sach
        public ActionResult Index()
        {
            var saches = db.Saches.Include(s => s.Tac_Gia).Include(s => s.The_Loai);
            return View(saches.ToList());
        }

        // GET: Admin/Sach/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = db.Saches.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
        }

        // GET: Admin/Sach/Create
        public ActionResult Create()
        {
            ViewBag.Ma_Tac_Gia = new SelectList(db.Tac_Gia, "Ma_Tac_Gia", "Ten_Tac_Gia");
            ViewBag.Ma_The_Loai = new SelectList(db.The_Loai, "Ma_The_Loai", "Ten_The_Loai");
            return View();
        }

        // POST: Admin/Sach/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Sach sach)
        {
            var tenSach = Request.Form["Ten_Sach"];

            System.Diagnostics.Debug.WriteLine("Ten_Sach = " + tenSach);

            foreach (var key in Request.Form.AllKeys)
            {
                System.Diagnostics.Debug.WriteLine($"{key} = {Request.Form[key]}");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Saches.Add(sach);
                    db.SaveChanges();

                    TempData["AlertMessage"] = "Thêm sách mới thành công!";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    // 1. Bắt các lỗi validation trực tiếp từ các trường Entity (ví dụ: thiếu trường bắt buộc, vượt quá độ dài ký tự)
                    var errorMessages = dbEx.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                    TempData["AlertMessage"] = "Lỗi dữ liệu: " + string.Join(" | ", errorMessages);
                    TempData["AlertType"] = "danger";
                }
                catch (Exception ex)
                {
                    // 2. Bắt các lỗi hệ thống hoặc ràng buộc Database từ SQL (ví dụ: sai khóa ngoại, sai ràng buộc CHECK)
                    string innerLog = ex.InnerException != null ? ex.InnerException.InnerException?.Message ?? ex.InnerException.Message : ex.Message;
                    TempData["AlertMessage"] = "Lỗi hệ thống: " + innerLog;
                    TempData["AlertType"] = "danger";
                }
            }
            else
            {
                // 3. Bắt lỗi nếu form nhập sai định dạng (ví dụ: nhập chữ vào ô số, sai định dạng ngày tháng)
                var errors = string.Join(" | ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                TempData["AlertMessage"] = "Dữ liệu form không hợp lệ: " + errors;
                TempData["AlertType"] = "danger";
            }

            ViewBag.Ma_Tac_Gia = new SelectList(db.Tac_Gia, "Ma_Tac_Gia", "Ten_Tac_Gia", sach.Ma_Tac_Gia);
            ViewBag.Ma_The_Loai = new SelectList(db.The_Loai, "Ma_The_Loai", "Ten_The_Loai", sach.Ma_The_Loai);
            return View(sach);
        }

        // GET: Admin/Sach/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = db.Saches.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            ViewBag.Ma_Tac_Gia = new SelectList(db.Tac_Gia, "Ma_Tac_Gia", "Ten_Tac_Gia", sach.Ma_Tac_Gia);
            ViewBag.Ma_The_Loai = new SelectList(db.The_Loai, "Ma_The_Loai", "Ten_The_Loai", sach.Ma_The_Loai);
            return View(sach);
        }

        // POST: Admin/Sach/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma_Sach,Ma_The_Loai,Ma_Tac_Gia,Ten_Sach,Mo_Ta,Gia_Ban,So_Luong_Ton,Nha_Xuat_Ban,Ngay_Xuat_Ban,Anh_Bia_Url")] Sach sach)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(sach).State = EntityState.Modified;
                    db.SaveChanges();

                    // Lưu thông báo thành công khi cập nhật sách
                    TempData["AlertMessage"] = "Cập nhật thông tin sách thành công!";
                    TempData["AlertType"] = "success";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    TempData["AlertMessage"] = "Không thể cập nhật thông tin sách!";
                    TempData["AlertType"] = "danger";
                }
            }
            ViewBag.Ma_Tac_Gia = new SelectList(db.Tac_Gia, "Ma_Tac_Gia", "Ten_Tac_Gia", sach.Ma_Tac_Gia);
            ViewBag.Ma_The_Loai = new SelectList(db.The_Loai, "Ma_The_Loai", "Ten_The_Loai", sach.Ma_The_Loai);
            return View(sach);
        }

        // GET: Admin/Sach/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Sach sach = db.Saches.Find(id);
            if (sach == null)
            {
                return HttpNotFound();
            }
            return View(sach);
        }

        // POST: Admin/Sach/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                Sach sach = db.Saches.Find(id);
                db.Saches.Remove(sach);
                db.SaveChanges();

                // Lưu thông báo thành công khi xóa sách
                TempData["AlertMessage"] = "Đã xóa sách khỏi hệ thống thành công!";
                TempData["AlertType"] = "success";
            }
            catch (Exception)
            {
                // Trường hợp dính khóa ngoại (sách đã nằm trong Chi_Tiet_Don_Hang không xóa thẳng được)
                TempData["AlertMessage"] = "Không thể xóa sách này vì dữ liệu liên quan đến đơn hàng của khách!";
                TempData["AlertType"] = "danger";
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