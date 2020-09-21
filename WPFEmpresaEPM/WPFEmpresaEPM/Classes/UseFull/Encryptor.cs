using System;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace WPFEmpresaEPM.Classes
{
    public static class Encryptor
    {
        public static string Encrypt(string plainText, string key = null)
        {
            try
            {
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                var encryptBity = GetRijndaelManaged(key).CreateEncryptor().TransformFinalBlock(plainBytes, 0, plainBytes.Length);
                return Convert.ToBase64String(encryptBity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Decrypt(string encryptedText, string key = null)
        {
            try
            {
                var encryptedBytes = Convert.FromBase64String(encryptedText);
                var decryptByte = GetRijndaelManaged(key).CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                return Encoding.UTF8.GetString(decryptByte);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private static RijndaelManaged GetRijndaelManaged(string secretKey)
        {
            try
            {
                if (secretKey == null)
                {
                    secretKey = Assembly.GetExecutingAssembly().EntryPoint.DeclaringType.Namespace;
                }

                var keyBytes = new byte[16];
                var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
                Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
                return new RijndaelManaged
                {
                    Mode = CipherMode.CBC,
                    Padding = PaddingMode.PKCS7,
                    KeySize = 128,
                    BlockSize = 128,
                    Key = keyBytes,
                    IV = keyBytes
                };
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
