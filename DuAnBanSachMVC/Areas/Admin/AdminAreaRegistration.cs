using System.Web.Mvc;

namespace DuAnBanSachMVC.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get { return "Admin"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { controller = "Home", action = "Dashboard", id = UrlParameter.Optional },
                // Ép hệ thống quét chính xác thư mục Controller của Admin để không bị lẫn với bên ngoài
                new[] { "DuAnBanSachMVC.Areas.Admin.Controllers" }
            );
        }
    }
}