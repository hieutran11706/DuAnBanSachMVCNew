using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DuAnBanSachMVC.ViewModels
{
    public class YeuThichViewModel
    {
        public int Ma_Yeu_Thich { get; set; }
        public int Ma_Sach { get; set; }
        public string Ten_Sach { get; set; }
        public string Hinh_Anh { get; set; }
        public decimal Gia { get; set; }
        public DateTime Ngay_Them { get; set; }
    }
}