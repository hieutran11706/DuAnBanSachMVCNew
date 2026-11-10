using System;

namespace DuAnBanSachMVC.ViewModels
{
    [Serializable]
    public class CartItemViewModel
    {
        public int Ma_Sach { get; set; }
        public string Ten_Sach { get; set; }
        public string Anh_Bia_Url { get; set; }
        public decimal Don_Gia { get; set; }
        public int So_Luong { get; set; }
        public int Ton_Kho { get; set; }

        public decimal Thanh_Tien
        {
            get { return Don_Gia * So_Luong; }
        }
    }
}