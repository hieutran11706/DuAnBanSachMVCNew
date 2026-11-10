using System;
using System.Collections.Generic;

namespace DuAnBanSachMVC.Utilities
{
    public static class EnumerableExtensions
    {
        /// <summary>Chia 1 danh sách thành nhiều danh sách con kích thước "size" (dùng cho slider trang chủ).</summary>
        public static List<List<T>> ChunkBy<T>(this List<T> source, int size)
        {
            var result = new List<List<T>>();
            if (source == null || source.Count == 0) return result;

            for (int i = 0; i < source.Count; i += size)
            {
                int length = Math.Min(size, source.Count - i);
                result.Add(source.GetRange(i, length));
            }
            return result;
        }

        /// <summary>Định dạng tiền VNĐ dạng "190.000đ".</summary>
        public static string DinhDangTien(this decimal soTien)
        {
            return string.Format("{0:N0}đ", soTien);
        }
    }
}