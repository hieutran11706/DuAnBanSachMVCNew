using DuAnBanSachMVC.Models;
using DuAnBanSachMVC.Utilities;
using DuAnBanSachMVC.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DuAnBanSachMVC.Controllers
{
    public class HomeController : BaseController
    {
        #region Helpers dùng chung (đọc thêm ở BaseController: GioHang, MaKhachHangHienTai)

        // Tìm thể loại gần đúng theo tên (không phân biệt hoa thường, chấp nhận nhiều cách viết)
        private The_Loai TimTheLoai(params string[] tenGanDung)
        {
            var tatCaTheLoai = db.The_Loai.ToList();
            return tatCaTheLoai.FirstOrDefault(t =>
                !string.IsNullOrEmpty(t.Ten_The_Loai) &&
                tenGanDung.Any(ten => t.Ten_The_Loai.IndexOf(ten, StringComparison.OrdinalIgnoreCase) >= 0));
        }

        private List<Sach> LaySachTheoTheLoai(int soLuong, params string[] tenGanDung)
        {
            var theLoai = TimTheLoai(tenGanDung);
            if (theLoai == null) return new List<Sach>();

            return db.Saches
                .Include(s => s.Tac_Gia)
                .Where(s => s.Ma_The_Loai == theLoai.Ma_The_Loai)
                .OrderByDescending(s => s.Ma_Sach)
                .Take(soLuong)
                .ToList();
        }

        private List<Sach> XemDanhMuc(string tieuDe, params string[] tenGanDung)
        {
            var theLoai = TimTheLoai(tenGanDung);
            List<Sach> danhSach = theLoai != null
                ? db.Saches.Include(s => s.Tac_Gia)
                           .Where(s => s.Ma_The_Loai == theLoai.Ma_The_Loai)
                           .OrderByDescending(s => s.Ma_Sach)
                           .ToList()
                : new List<Sach>();

            ViewBag.TieuDeDanhMuc = tieuDe;
            return danhSach;
        }

        #endregion

        #region Trang chủ & danh mục

        // GET: / (route mặc định) -> chuyển sang trang chủ thật
        public ActionResult Index()
        {
            return RedirectToAction("HomePage");
        }

        public ActionResult HomePage()
        {
            var maSachBanChay = db.Chi_Tiet_Don_Hang
                .GroupBy(c => c.Ma_Sach)
                .OrderByDescending(g => g.Sum(x => x.So_Luong))
                .Select(g => g.Key)
                .Take(9)
                .ToList();

            List<Sach> sachBanChay = maSachBanChay.Any()
                ? db.Saches.Include(s => s.Tac_Gia).Where(s => maSachBanChay.Contains(s.Ma_Sach)).ToList()
                : db.Saches.Include(s => s.Tac_Gia).OrderByDescending(s => s.Ma_Sach).Take(9).ToList();

            ViewBag.SachBanChay = sachBanChay;
            ViewBag.SachKhoaHoc = LaySachTheoTheLoai(9, "Khoa Học", "Khoa Hoc");
            ViewBag.SachTamLy = LaySachTheoTheLoai(9, "Tâm Lý", "Tam Ly");
            ViewBag.SachVanHoc = LaySachTheoTheLoai(9, "Văn Học", "Van Hoc");
            ViewBag.SachKinhTe = LaySachTheoTheLoai(9, "Kinh Tế", "Kinh Te");
            ViewBag.SachSelfHelp = LaySachTheoTheLoai(9, "Self-help", "Self Help", "Kỹ năng sống", "Ky nang song");

            return View();
        }

        public ActionResult KhoaHoc()
        {
            var ds = XemDanhMuc("Sách Khoa Học", "Khoa Học", "Khoa Hoc");
            return View(ds);
        }

        public ActionResult TamLyHoc()
        {
            var ds = XemDanhMuc("Sách Tâm Lý Học", "Tâm Lý", "Tam Ly");
            return View(ds);
        }

        public ActionResult VanHoc()
        {
            var ds = XemDanhMuc("Sách Văn Học", "Văn Học", "Van Hoc");
            return View(ds);
        }

        public ActionResult KinhTe()
        {
            var ds = XemDanhMuc("Sách Kinh Tế", "Kinh Tế", "Kinh Te");
            return View(ds);
        }

        public ActionResult SelfHelp()
        {
            var ds = XemDanhMuc("Sách Self-help", "Self-help", "Self Help", "Kỹ năng sống", "Ky nang song");
            return View(ds);
        }

        // Tìm kiếm theo tên sách / tác giả / nhà xuất bản (ô search trên header)
        public ActionResult Search(string q)
        {
            q = (q ?? "").Trim();
            List<Sach> ketQua = new List<Sach>();
            if (!string.IsNullOrEmpty(q))
            {
                ketQua = db.Saches
                    .Include(s => s.Tac_Gia)
                    .Where(s => s.Ten_Sach.Contains(q)
                             || s.Nha_Xuat_Ban.Contains(q)
                             || s.Tac_Gia.Ten_Tac_Gia.Contains(q))
                    .OrderByDescending(s => s.Ma_Sach)
                    .ToList();
            }
            ViewBag.TieuDeDanhMuc = "Kết quả tìm kiếm cho \"" + q + "\"";
            ViewBag.TuKhoa = q;
            return View(ketQua);
        }

        #endregion

        #region Chi tiết sách

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("HomePage");
            }

            var sach = db.Saches
                .Include(s => s.Tac_Gia)
                .Include(s => s.The_Loai)
                .FirstOrDefault(s => s.Ma_Sach == id);

            if (sach == null)
            {
                return HttpNotFound("Không tìm thấy sách.");
            }

            // Đánh giá của sách này (thông qua các đơn hàng đã mua sách)
            var danhSachDanhGia = db.Danh_Gia
                .Include(d => d.Chi_Tiet_Don_Hang.Don_Hang.Khach_Hang)
                .Where(d => d.Chi_Tiet_Don_Hang.Ma_Sach == sach.Ma_Sach)
                .OrderByDescending(d => d.Ngay_Danh_Gia)
                .ToList();

            ViewBag.SoDanhGia = danhSachDanhGia.Count;
            ViewBag.DiemTrungBinh = danhSachDanhGia.Any() ? Math.Round(danhSachDanhGia.Average(d => d.So_Sao), 1) : 0;
            ViewBag.DanhSachDanhGia = danhSachDanhGia.Take(5).ToList();

            // Sách cùng thể loại
            ViewBag.SachLienQuan = db.Saches
                .Where(s => s.Ma_The_Loai == sach.Ma_The_Loai && s.Ma_Sach != sach.Ma_Sach)
                .OrderByDescending(s => s.Ma_Sach)
                .Take(4)
                .ToList();

            return View(sach);
        }

        #endregion

        #region Giỏ hàng

        public ActionResult ShoppingCart()
        {
            return View(GioHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThemVaoGioHang(int maSach, int soLuong = 1)
        {
            var sach = db.Saches.Find(maSach);
            if (sach == null)
            {
                TempData["ThongBao"] = "Không tìm thấy sách này.";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("HomePage");
            }

            if (soLuong < 1) soLuong = 1;

            var gioHang = GioHang;
            var dongDaCo = gioHang.FirstOrDefault(x => x.Ma_Sach == maSach);
            int soLuongMongMuon = (dongDaCo != null ? dongDaCo.So_Luong : 0) + soLuong;

            if (sach.So_Luong_Ton < soLuongMongMuon)
            {
                TempData["ThongBao"] = "Sách \"" + sach.Ten_Sach + "\" chỉ còn " + sach.So_Luong_Ton + " cuốn trong kho.";
                TempData["LoaiThongBao"] = "danger";
                soLuongMongMuon = sach.So_Luong_Ton;
                if (soLuongMongMuon <= 0)
                {
                    return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("HomePage"));
                }
            }
            else
            {
                TempData["ThongBao"] = "Đã thêm \"" + sach.Ten_Sach + "\" vào giỏ hàng.";
                TempData["LoaiThongBao"] = "success";
            }

            if (dongDaCo != null)
            {
                dongDaCo.So_Luong = soLuongMongMuon;
                dongDaCo.Ton_Kho = sach.So_Luong_Ton;
            }
            else
            {
                gioHang.Add(new CartItemViewModel
                {
                    Ma_Sach = sach.Ma_Sach,
                    Ten_Sach = sach.Ten_Sach,
                    Anh_Bia_Url = sach.Anh_Bia_Url,
                    Don_Gia = sach.Gia_Ban,
                    So_Luong = soLuongMongMuon,
                    Ton_Kho = sach.So_Luong_Ton
                });
            }
            GioHang = gioHang;

            return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Action("ShoppingCart"));
        }

        // Mua ngay: thêm vào giỏ rồi chuyển thẳng tới trang giỏ hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MuaNgay(int maSach, int soLuong = 1)
        {
            var sach = db.Saches.Find(maSach);
            if (sach != null)
            {
                if (soLuong < 1) soLuong = 1;
                var gioHang = GioHang;
                var dongDaCo = gioHang.FirstOrDefault(x => x.Ma_Sach == maSach);
                int soLuongMongMuon = Math.Min((dongDaCo != null ? dongDaCo.So_Luong : 0) + soLuong, sach.So_Luong_Ton);

                if (dongDaCo != null)
                {
                    dongDaCo.So_Luong = soLuongMongMuon;
                    dongDaCo.Ton_Kho = sach.So_Luong_Ton;
                }
                else if (soLuongMongMuon > 0)
                {
                    gioHang.Add(new CartItemViewModel
                    {
                        Ma_Sach = sach.Ma_Sach,
                        Ten_Sach = sach.Ten_Sach,
                        Anh_Bia_Url = sach.Anh_Bia_Url,
                        Don_Gia = sach.Gia_Ban,
                        So_Luong = soLuongMongMuon,
                        Ton_Kho = sach.So_Luong_Ton
                    });
                }
                GioHang = gioHang;
            }
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CapNhatSoLuong(int maSach, int soLuong)
        {
            var gioHang = GioHang;
            var dong = gioHang.FirstOrDefault(x => x.Ma_Sach == maSach);
            if (dong != null)
            {
                if (soLuong <= 0)
                {
                    gioHang.Remove(dong);
                }
                else
                {
                    var sach = db.Saches.Find(maSach);
                    int tonKho = sach != null ? sach.So_Luong_Ton : dong.Ton_Kho;
                    dong.So_Luong = Math.Min(soLuong, tonKho);
                    dong.Ton_Kho = tonKho;
                }
            }
            GioHang = gioHang;
            return RedirectToAction("ShoppingCart");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XoaKhoiGioHang(int maSach)
        {
            var gioHang = GioHang;
            var dong = gioHang.FirstOrDefault(x => x.Ma_Sach == maSach);
            if (dong != null) gioHang.Remove(dong);
            GioHang = gioHang;
            return RedirectToAction("ShoppingCart");
        }

        // Bật/tắt yêu thích cho 1 cuốn sách (gọi qua AJAX từ book-card)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ToggleYeuThich(int maSach)
        {
            if (!MaKhachHangHienTai.HasValue)
            {
                return Json(new { success = false, requireLogin = true, message = "Vui lòng đăng nhập để thêm sách vào danh sách yêu thích." });
            }

            var sach = db.Saches.Find(maSach);
            if (sach == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sách này." });
            }

            int maKh = MaKhachHangHienTai.Value;
            var daYeuThich = db.Yeu_Thich.FirstOrDefault(x => x.Ma_Khach_Hang == maKh && x.Ma_Sach == maSach);
            bool isFavorited;

            if (daYeuThich != null)
            {
                db.Yeu_Thich.Remove(daYeuThich);
                isFavorited = false;
            }
            else
            {
                db.Yeu_Thich.Add(new Yeu_Thich
                {
                    Ma_Khach_Hang = maKh,
                    Ma_Sach = maSach,
                    Ngay_Them = DateTime.Now
                });
                isFavorited = true;
            }
            db.SaveChanges();

            return Json(new
            {
                success = true,
                isFavorited = isFavorited,
                message = isFavorited ? "Đã thêm \"" + sach.Ten_Sach + "\" vào yêu thích." : "Đã bỏ \"" + sach.Ten_Sach + "\" khỏi yêu thích."
            });
        }

        #endregion

        #region Thanh toán / Đặt hàng

        public ActionResult CheckOutDetails()
        {
            var gioHang = GioHang;
            if (!gioHang.Any())
            {
                TempData["ThongBao"] = "Giỏ hàng của bạn đang trống.";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("ShoppingCart");
            }

            var model = new CheckoutViewModel
            {
                GioHang = gioHang,
                Tong_Tien = gioHang.Sum(x => x.Thanh_Tien)
            };

            if (MaKhachHangHienTai.HasValue)
            {
                var kh = db.Khach_Hang.Find(MaKhachHangHienTai.Value);
                if (kh != null)
                {
                    model.Ho_Ten = kh.Ho_Ten;
                    model.Email = kh.Email;
                    model.So_Dien_Thoai = kh.So_Dien_Thoai;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CheckOutDetails(CheckoutViewModel model)
        {
            var gioHang = GioHang;
            if (!gioHang.Any())
            {
                TempData["ThongBao"] = "Giỏ hàng của bạn đang trống.";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("ShoppingCart");
            }

            if (!MaKhachHangHienTai.HasValue)
            {
                TempData["ThongBao"] = "Vui lòng đăng nhập trước khi đặt hàng.";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("Login", new { returnUrl = "/Home/CheckOutDetails" });
            }

            model.GioHang = gioHang;
            model.Tong_Tien = gioHang.Sum(x => x.Thanh_Tien);

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Kiểm tra tồn kho lần cuối trước khi đặt hàng
            foreach (var dong in gioHang)
            {
                var sach = db.Saches.Find(dong.Ma_Sach);
                if (sach == null || sach.So_Luong_Ton < dong.So_Luong)
                {
                    ModelState.AddModelError("", "Sách \"" + dong.Ten_Sach + "\" không đủ số lượng trong kho.");
                    return View(model);
                }
            }

            // Áp dụng mã giảm giá (nếu có)
            decimal tienGiamGia = 0;
            Phieu_Giam_Gia coupon = null;
            if (!string.IsNullOrWhiteSpace(model.Ma_Coupon))
            {
                var ma = model.Ma_Coupon.Trim();
                coupon = db.Phieu_Giam_Gia.FirstOrDefault(c => c.Code_Giam_Gia == ma);

                if (coupon == null)
                {
                    ModelState.AddModelError("Ma_Coupon", "Mã giảm giá không tồn tại.");
                    return View(model);
                }
                if (coupon.Trang_Thai != "Hoat_Dong" || coupon.Ngay_Bat_Dau > DateTime.Now || coupon.Ngay_Ket_Thuc < DateTime.Now || coupon.So_Luong_Con_Lai <= 0)
                {
                    ModelState.AddModelError("Ma_Coupon", "Mã giảm giá đã hết hạn hoặc không còn khả dụng.");
                    return View(model);
                }
                if (model.Tong_Tien < coupon.Gia_Tri_Don_Hang_Toi_Thieu)
                {
                    ModelState.AddModelError("Ma_Coupon", "Đơn hàng chưa đạt giá trị tối thiểu " + coupon.Gia_Tri_Don_Hang_Toi_Thieu.DinhDangTien() + " để dùng mã này.");
                    return View(model);
                }

                if (coupon.Loai_Giam_Gia == "Phan_Tram")
                {
                    tienGiamGia = model.Tong_Tien * coupon.Gia_Tri_Giam / 100;
                    if (coupon.Gia_Tri_Giam_Toi_Da.HasValue && tienGiamGia > coupon.Gia_Tri_Giam_Toi_Da.Value)
                    {
                        tienGiamGia = coupon.Gia_Tri_Giam_Toi_Da.Value;
                    }
                }
                else
                {
                    tienGiamGia = coupon.Gia_Tri_Giam;
                }
                if (tienGiamGia > model.Tong_Tien) tienGiamGia = model.Tong_Tien;
            }

            var donHang = new Don_Hang
            {
                Ma_Khach_Hang = MaKhachHangHienTai.Value,
                Ma_Phieu_Giam_Gia = coupon != null ? (int?)coupon.Ma_Phieu_Giam_Gia : null,
                Ngay_Dat = DateTime.Now,
                Trang_Thai = "Chờ xử lý",
                Dia_Chi_Giao_Hang = model.Dia_Chi_Giao_Hang + " | " + model.Ho_Ten + " - " + model.So_Dien_Thoai +
                                     (string.IsNullOrWhiteSpace(model.Ghi_Chu) ? "" : " | Ghi chú: " + model.Ghi_Chu),
                Tong_Tien = model.Tong_Tien,
                Tien_Giam_Gia = tienGiamGia,
                Thanh_Tien = model.Tong_Tien - tienGiamGia
            };

            foreach (var dong in gioHang)
            {
                donHang.Chi_Tiet_Don_Hang.Add(new Chi_Tiet_Don_Hang
                {
                    Ma_Sach = dong.Ma_Sach,
                    So_Luong = dong.So_Luong,
                    Gia_Luc_Mua = dong.Don_Gia
                });
            }

            db.Don_Hang.Add(donHang);

            if (coupon != null)
            {
                coupon.So_Luong_Con_Lai -= 1;
            }

            try
            {
                db.SaveChanges();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo đơn hàng, vui lòng thử lại.");
                return View(model);
            }
            // Đặt hàng thành công
            donHang.Trang_Thai = "Chờ thanh toán";
            db.SaveChanges();

            GioHang = new List<CartItemViewModel>();

            // Chuyển sang VNPay
            string paymentUrl = TaoUrlThanhToanVnPay(donHang);
            return Redirect(paymentUrl);
        }

        public ActionResult OrderComplete(int? id)
        {
            if (id == null) return RedirectToAction("HomePage");

            var donHang = db.Don_Hang
                .Include(d => d.Chi_Tiet_Don_Hang.Select(c => c.Sach))
                .FirstOrDefault(d => d.Ma_Don_Hang == id);

            if (donHang == null || (MaKhachHangHienTai.HasValue && donHang.Ma_Khach_Hang != MaKhachHangHienTai.Value))
            {
                return RedirectToAction("HomePage");
            }

            return View(donHang);
        }
        private string TaoUrlThanhToanVnPay(Don_Hang donHang)
        {
            string vnp_TmnCode = System.Configuration.ConfigurationManager.AppSettings["vnp_TmnCode"];
            string vnp_HashSecret = System.Configuration.ConfigurationManager.AppSettings["vnp_HashSecret"];
            string vnp_Url = System.Configuration.ConfigurationManager.AppSettings["vnp_Url"];
            string vnp_ReturnUrl = System.Configuration.ConfigurationManager.AppSettings["vnp_ReturnUrl"];

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((long)(donHang.Thanh_Tien * 100)).ToString()); // VNPay yêu cầu *100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang #" + donHang.Ma_Don_Hang);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
            vnpay.AddRequestData("vnp_TxnRef", donHang.Ma_Don_Hang.ToString());

            return vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        }
        public ActionResult VnPayReturn()
        {
            if (Request.QueryString.Count == 0)
                return RedirectToAction("HomePage");

            string vnp_HashSecret = System.Configuration.ConfigurationManager.AppSettings["vnp_HashSecret"];
            var vnpay = new VnPayLibrary();

            foreach (string key in Request.QueryString.AllKeys)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, Request.QueryString[key]);
                }
            }

            long orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];
            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);

            var donHang = db.Don_Hang.Find((int)orderId);
            if (donHang == null)
                return RedirectToAction("HomePage");

            if (checkSignature)
            {
                if (vnp_ResponseCode == "00") // Thanh toán thành công
                {
                    donHang.Trang_Thai = "Đã thanh toán";
                    // Có thể trừ tồn kho ở đây nếu muốn
                    db.SaveChanges();
                    return RedirectToAction("OrderComplete", new { id = donHang.Ma_Don_Hang });
                }
                else
                {
                    donHang.Trang_Thai = "Thanh toán thất bại";
                    db.SaveChanges();
                    TempData["ThongBao"] = "Thanh toán không thành công. Mã lỗi: " + vnp_ResponseCode;
                    TempData["LoaiThongBao"] = "danger";
                    return RedirectToAction("ShoppingCart");
                }
            }
            else
            {
                TempData["ThongBao"] = "Chữ ký không hợp lệ từ VNPay.";
                TempData["LoaiThongBao"] = "danger";
                return RedirectToAction("HomePage");
            }
        }
        #endregion

        #region Đăng nhập / Đăng ký / Đăng xuất

        public ActionResult Login(string returnUrl)
        {
            if (MaKhachHangHienTai.HasValue) return RedirectToAction("HomePage");
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var tenDangNhap = model.Ten_Dang_Nhap.Trim();
            var khachHang = db.Khach_Hang.FirstOrDefault(k =>
                k.Ten_Dang_Nhap == tenDangNhap || k.Email == tenDangNhap);

            if (khachHang == null || !PasswordHasher.VerifyPassword(model.Mat_Khau, khachHang.Mat_Khau))
            {
                ModelState.AddModelError("", "Tên đăng nhập/email hoặc mật khẩu không đúng.");
                return View(model);
            }

            Session[CUSTOMER_ID_KEY] = khachHang.Ma_Khach_Hang;
            Session[CUSTOMER_NAME_KEY] = khachHang.Ho_Ten;

            if (model.RememberMe)
            {
                Session.Timeout = 60 * 24 * 7; // 7 ngày
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("HomePage");
        }

        public ActionResult Register()
        {
            if (MaKhachHangHienTai.HasValue) return RedirectToAction("HomePage");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var tenDangNhap = model.Ten_Dang_Nhap.Trim();
            var email = model.Email.Trim();

            if (db.Khach_Hang.Any(k => k.Ten_Dang_Nhap == tenDangNhap))
            {
                ModelState.AddModelError("Ten_Dang_Nhap", "Tên đăng nhập đã tồn tại.");
                return View(model);
            }
            if (db.Khach_Hang.Any(k => k.Email == email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký.");
                return View(model);
            }

            // Gán vai trò mặc định cho khách hàng mới (ưu tiên vai trò có tên chứa "khách")
            var vaiTroKhachHang = db.Vai_Tro.ToList()
                .FirstOrDefault(v => !string.IsNullOrEmpty(v.Ten_Vai_Tro) &&
                                      v.Ten_Vai_Tro.IndexOf("khách", StringComparison.OrdinalIgnoreCase) >= 0);
            if (vaiTroKhachHang == null)
            {
                vaiTroKhachHang = db.Vai_Tro.OrderBy(v => v.Ma_Vai_Tro).FirstOrDefault();
            }

            if (vaiTroKhachHang == null)
            {
                ModelState.AddModelError("", "Hệ thống chưa có vai trò cho khách hàng, vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            var khachHangMoi = new Khach_Hang
            {
                Ma_Vai_Tro = vaiTroKhachHang.Ma_Vai_Tro,
                Ten_Dang_Nhap = tenDangNhap,
                Email = email,
                So_Dien_Thoai = model.So_Dien_Thoai,
                Ho_Ten = model.Ho_Ten,
                Ngay_Tao = DateTime.Now,
                Mat_Khau = PasswordHasher.HashPassword(model.Mat_Khau)
            };

            db.Khach_Hang.Add(khachHangMoi);
            db.SaveChanges();

            Session[CUSTOMER_ID_KEY] = khachHangMoi.Ma_Khach_Hang;
            Session[CUSTOMER_NAME_KEY] = khachHangMoi.Ho_Ten;

            TempData["ThongBao"] = "Đăng ký tài khoản thành công. Chào mừng bạn đến với Bookora!";
            TempData["LoaiThongBao"] = "success";
            return RedirectToAction("HomePage");
        }

        public ActionResult Logout()
        {
            Session.Remove(CUSTOMER_ID_KEY);
            Session.Remove(CUSTOMER_NAME_KEY);
            return RedirectToAction("HomePage");
        }

        #endregion

        #region Trang tĩnh có sẵn

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        #endregion
    }
}