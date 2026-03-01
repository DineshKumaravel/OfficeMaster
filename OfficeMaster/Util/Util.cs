using System;
using System.IO;
using System.Security.Cryptography;

namespace OfficeHelper
{
	public class Util
	{
		public static string GetSheetName(string name)
		{
			try
			{
				return Environment.UserName + name;
			}
			catch (Exception e) { ErrorHandler.RecordError(e.Message + "error at util"); return ""; }
		}
        public static string Encrypt(string plainText, string secretKey)
        {
            using (Aes aes = Aes.Create())
            {
                // Generate random salt
                byte[] salt = RandomNumberGenerator.GetBytes(16);

                // Derive key from secret
                var key = new Rfc2898DeriveBytes(secretKey, salt, 100000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(32); // 256-bit key
                aes.GenerateIV();

                using var ms = new MemoryStream();
                ms.Write(salt, 0, salt.Length);      // prepend salt
                ms.Write(aes.IV, 0, aes.IV.Length);  // prepend IV

                using (var cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var sw = new StreamWriter(cs))
                {
                    sw.Write(plainText);
                }

                return Convert.ToBase64String(ms.ToArray());
            }
        }
        public static string Decrypt(string cipherText, string secretKey)
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                byte[] salt = new byte[16];
                byte[] iv = new byte[16];

                Array.Copy(fullCipher, 0, salt, 0, 16);
                Array.Copy(fullCipher, 16, iv, 0, 16);

                var key = new Rfc2898DeriveBytes(secretKey, salt, 100000, HashAlgorithmName.SHA256);
                aes.Key = key.GetBytes(32);
                aes.IV = iv;

                using var ms = new MemoryStream(fullCipher, 32, fullCipher.Length - 32);
                using var cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
        }
    }
}
