using System;
using System.Security.Cryptography;

namespace DuAnBanSachMVC.Utilities
{
    /// <summary>
    /// Mã hoá và kiểm tra mật khẩu bằng PBKDF2 (RFC 2898), không cần thư viện ngoài.
    /// Chuỗi lưu trong DB có định dạng: {so_vong_lap}.{salt_base64}.{hash_base64}
    /// </summary>
    public static class PasswordHasher
    {
        private const int SaltSize = 16;       // 128 bit
        private const int HashSize = 32;       // 256 bit
        private const int Iterations = 100000; // độ khó

        public static string HashPassword(string password)
        {
            if (password == null) throw new ArgumentNullException(nameof(password));

            using (var rfc = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256))
            {
                byte[] salt = rfc.Salt;
                byte[] hash = rfc.GetBytes(HashSize);

                return string.Format("{0}.{1}.{2}",
                    Iterations,
                    Convert.ToBase64String(salt),
                    Convert.ToBase64String(hash));
            }
        }

        public static bool VerifyPassword(string password, string hashedValue)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(hashedValue))
                return false;

            // Hỗ trợ dữ liệu cũ chưa mã hoá (so sánh thường) để không khoá tài khoản có sẵn trong DB seed/demo
            if (!hashedValue.Contains("."))
            {
                return string.Equals(password, hashedValue, StringComparison.Ordinal);
            }

            var parts = hashedValue.Split('.');
            if (parts.Length != 3) return false;

            int iterations;
            if (!int.TryParse(parts[0], out iterations)) return false;

            byte[] salt;
            byte[] storedHash;
            try
            {
                salt = Convert.FromBase64String(parts[1]);
                storedHash = Convert.FromBase64String(parts[2]);
            }
            catch (FormatException)
            {
                return false;
            }

            using (var rfc = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256))
            {
                byte[] computedHash = rfc.GetBytes(storedHash.Length);
                return SlowEquals(computedHash, storedHash);
            }
        }

        // So sánh 2 mảng byte theo thời gian không đổi để chống timing attack
        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint)a.Length ^ (uint)b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
                diff |= (uint)(a[i] ^ b[i]);
            return diff == 0;
        }
    }
}