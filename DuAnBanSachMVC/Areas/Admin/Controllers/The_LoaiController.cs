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
    public class The_LoaiController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // GET: Admin/The_Loai
        public ActionResult Index()
        {
            return View(db.The_Loai.ToList());
        }

        // GET: Admin/The_Loai/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            The_Loai the_Loai = db.The_Loai.Find(id);
            if (the_Loai == null)
            {
                return HttpNotFound();
            }
            return View(the_Loai);
        }

        // GET: Admin/The_Loai/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/The_Loai/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ma_The_Loai,Ten_The_Loai,Mo_Ta")] The_Loai the_Loai)
        {
            if (ModelState.IsValid)
            {
                db.The_Loai.Add(the_Loai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(the_Loai);
        }

        // GET: Admin/The_Loai/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            The_Loai the_Loai = db.The_Loai.Find(id);
            if (the_Loai == null)
            {
                return HttpNotFound();
            }
            return View(the_Loai);
        }

        // POST: Admin/The_Loai/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma_The_Loai,Ten_The_Loai,Mo_Ta")] The_Loai the_Loai)
        {
            if (ModelState.IsValid)
            {
                db.Entry(the_Loai).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(the_Loai);
        }

        // GET: Admin/The_Loai/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            The_Loai the_Loai = db.The_Loai.Find(id);
            if (the_Loai == null)
            {
                return HttpNotFound();
            }
            return View(the_Loai);
        }

        // POST: Admin/The_Loai/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                The_Loai the_Loai = db.The_Loai.Find(id);
                db.The_Loai.Remove(the_Loai);
                db.SaveChanges();

                // Nếu xóa thành công, gán thông báo thành công
                TempData["AlertMessage"] = "Đã xóa thể loại sách thành công!";
                TempData["AlertType"] = "success";
            }
            catch (Exception)
            {
                // Nếu dính lỗi (ví dụ: thể loại này đang có Sách ràng buộc, không xóa được)
                TempData["AlertMessage"] = "Không thể xóa thể loại này vì đang có sách thuộc danh mục này!";
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
