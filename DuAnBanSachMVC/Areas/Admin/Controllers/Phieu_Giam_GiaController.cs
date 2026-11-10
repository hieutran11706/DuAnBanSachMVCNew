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
    public class Phieu_Giam_GiaController : Controller
    {
        private DuAnBanSachMVCEntities db = new DuAnBanSachMVCEntities();

        // GET: Admin/Phieu_Giam_Gia
        public ActionResult Index()
        {
            return View(db.Phieu_Giam_Gia.ToList());
        }

        // GET: Admin/Phieu_Giam_Gia/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phieu_Giam_Gia phieu_Giam_Gia = db.Phieu_Giam_Gia.Find(id);
            if (phieu_Giam_Gia == null)
            {
                return HttpNotFound();
            }
            return View(phieu_Giam_Gia);
        }

        // GET: Admin/Phieu_Giam_Gia/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Phieu_Giam_Gia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Ma_Phieu_Giam_Gia,Code_Giam_Gia,Loai_Giam_Gia,Gia_Tri_Giam,Gia_Tri_Don_Hang_Toi_Thieu,Gia_Tri_Giam_Toi_Da,Ngay_Bat_Dau,Ngay_Ket_Thuc,So_Luong_Con_Lai,Trang_Thai")] Phieu_Giam_Gia phieu_Giam_Gia)
        {
            if (ModelState.IsValid)
            {
                db.Phieu_Giam_Gia.Add(phieu_Giam_Gia);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(phieu_Giam_Gia);
        }

        // GET: Admin/Phieu_Giam_Gia/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phieu_Giam_Gia phieu_Giam_Gia = db.Phieu_Giam_Gia.Find(id);
            if (phieu_Giam_Gia == null)
            {
                return HttpNotFound();
            }
            return View(phieu_Giam_Gia);
        }

        // POST: Admin/Phieu_Giam_Gia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Ma_Phieu_Giam_Gia,Code_Giam_Gia,Loai_Giam_Gia,Gia_Tri_Giam,Gia_Tri_Don_Hang_Toi_Thieu,Gia_Tri_Giam_Toi_Da,Ngay_Bat_Dau,Ngay_Ket_Thuc,So_Luong_Con_Lai,Trang_Thai")] Phieu_Giam_Gia phieu_Giam_Gia)
        {
            if (ModelState.IsValid)
            {
                db.Entry(phieu_Giam_Gia).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(phieu_Giam_Gia);
        }

        // GET: Admin/Phieu_Giam_Gia/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Phieu_Giam_Gia phieu_Giam_Gia = db.Phieu_Giam_Gia.Find(id);
            if (phieu_Giam_Gia == null)
            {
                return HttpNotFound();
            }
            return View(phieu_Giam_Gia);
        }

        // POST: Admin/Phieu_Giam_Gia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Phieu_Giam_Gia phieu_Giam_Gia = db.Phieu_Giam_Gia.Find(id);
            db.Phieu_Giam_Gia.Remove(phieu_Giam_Gia);
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
