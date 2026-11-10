using DuAnBanSachMVC.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DuAnBanSachMVC.Controllers
{
    /// <summary>
    /// Controller cha chứa các thuộc tính/khoá Session dùng chung cho toàn site
    /// (giỏ hàng, thông tin đăng nhập) để mọi controller kế thừa đều dùng chung một
    /// nguồn duy nhất, tránh lệch dữ liệu giữa các trang.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected DuAnBanSachMVC.Models.DuAnBanSachMVCEntities db = new DuAnBanSachMVC.Models.DuAnBanSachMVCEntities();

        protected const string CART_SESSION_KEY = "GioHang";
        protected const string CUSTOMER_ID_KEY = "MaKhachHang";
        protected const string CUSTOMER_NAME_KEY = "TenKhachHang";

        protected List<CartItemViewModel> GioHang
        {
            get
            {
                var cart = Session[CART_SESSION_KEY] as List<CartItemViewModel>;
                if (cart == null)
                {
                    cart = new List<CartItemViewModel>();
                    Session[CART_SESSION_KEY] = cart;
                }
                return cart;
            }
            set { Session[CART_SESSION_KEY] = value; }
        }

        protected int? MaKhachHangHienTai
        {
            get { return Session[CUSTOMER_ID_KEY] as int?; }
        }

        // Truyền thông tin giỏ hàng/đăng nhập cho _Layout ở MỌI action của MỌI controller kế thừa lớp này
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.SoLuongGioHang = GioHang.Sum(x => x.So_Luong);
            ViewBag.DaDangNhap = MaKhachHangHienTai.HasValue;
            ViewBag.TenKhachHangHienTai = Session[CUSTOMER_NAME_KEY] as string;
            ViewBag.DanhSachYeuThichIds = MaKhachHangHienTai.HasValue
                ? db.Yeu_Thich.Where(x => x.Ma_Khach_Hang == MaKhachHangHienTai.Value).Select(x => x.Ma_Sach).ToList()
                : new List<int>();
            base.OnActionExecuting(filterContext);
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
