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
    public class Vai_TroController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // GET: Admin/Vai_Tro
        public ActionResult Index()
        {
            return View(db.Vai_Tro.ToList());
        }

        // GET: Admin/Vai_Tro/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vai_Tro vai_Tro = db.Vai_Tro.Find(id);
            if (vai_Tro == null)
            {
                return HttpNotFound();
            }
            return View(vai_Tro);
        }

        // GET: Admin/Vai_Tro/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Vai_Tro/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Chỉ nhận Ten_Vai_Tro từ View gửi lên, Ma_Vai_Tro kiểu int sẽ tự sinh trong SQL
        public ActionResult Create([Bind(Include = "Ten_Vai_Tro")] Vai_Tro vaiTro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Kiểm tra xem Tên vai trò này đã tồn tại chưa để tránh trùng lặp ý nghĩa
                    var checkExist = db.Vai_Tro.Any(r => r.Ten_Vai_Tro.ToLower() == vaiTro.Ten_Vai_Tro.Trim().ToLower());
                    if (checkExist)
                    {
                        ModelState.AddModelError("Ten_Vai_Tro", "Tên vai trò này đã tồn tại trên hệ thống!");
                        return View(vaiTro);
                    }

                    // Lưu vào database
                    db.Vai_Tro.Add(vaiTro);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Đã xảy ra lỗi hệ thống: " + ex.Message);
                }
            }

            return View(vaiTro);
        }

        // GET: Admin/Vai_Tro/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vai_Tro vai_Tro = db.Vai_Tro.Find(id);
            if (vai_Tro == null)
            {
                return HttpNotFound();
            }
            return View(vai_Tro);
        }

        // POST: Admin/Vai_Tro/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma_Vai_Tro,Ten_Vai_Tro")] Vai_Tro vai_Tro)
        {
            if (ModelState.IsValid)
            {
                db.Entry(vai_Tro).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(vai_Tro);
        }

        // GET: Admin/Vai_Tro/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Vai_Tro vai_Tro = db.Vai_Tro.Find(id);
            if (vai_Tro == null)
            {
                return HttpNotFound();
            }
            return View(vai_Tro);
        }

        // POST: Admin/Vai_Tro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Vai_Tro vai_Tro = db.Vai_Tro.Find(id);
            db.Vai_Tro.Remove(vai_Tro);
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
