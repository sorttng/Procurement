using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Signet.Common
{
    internal class PasswordStorage
    {
        // 盐的长度（字节）
        private const int SaltSize = 16;

        // 哈希长度（字节）
        private const int HashSize = 32;

        // PBKDF2迭代次数（越高越安全，但计算越慢）
        private const int Iterations = 10000;

        /// <summary>
        /// 恒定时间比较，防止时序攻击
        /// 手动实现替代 CryptographicOperations.FixedTimeEquals
        /// </summary>
        private static bool FixedTimeEquals(byte[] a, byte[] b)
        {
            if (a == null || b == null || a.Length != b.Length)
                return false;

            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                result |= a[i] ^ b[i];
            }
            return result == 0;
        }

        /// <summary>
        /// 生成随机盐
        /// </summary>
        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[SaltSize];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        /// <summary>
        /// 生成密码哈希
        /// </summary>
        /// <param name="password">明文密码</param>
        /// <returns>哈希结果（格式：迭代次数:盐:哈希值）</returns>
        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("密码不能为空");

            // 生成随机盐
            byte[] salt = GenerateSalt();

            // 使用 Rfc2898DeriveBytes 替代 KeyDerivation.Pbkdf2
            byte[] hash;
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations))
            {
                hash = pbkdf2.GetBytes(HashSize);
            }

            // 返回格式：迭代次数:盐(Base64):哈希值(Base64)
            return $"{Iterations}:{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
        }

        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="password">用户输入的密码</param>
        /// <param name="hashedPassword">数据库存储的哈希密码</param>
        /// <returns>是否匹配</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                // 解析存储的哈希字符串
                var parts = hashedPassword.Split(':');
                if (parts.Length != 3) return false;

                int iterations = int.Parse(parts[0]);
                byte[] salt = Convert.FromBase64String(parts[1]);
                byte[] storedHash = Convert.FromBase64String(parts[2]);

                // 用相同参数计算用户输入的密码哈希
                byte[] computedHash;
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations))
                {
                    computedHash = pbkdf2.GetBytes(storedHash.Length);
                }

                // 使用恒定时间比较，防止时序攻击
                return FixedTimeEquals(computedHash, storedHash);
            }
            catch
            {
                return false;
            }
        }
    }
}
