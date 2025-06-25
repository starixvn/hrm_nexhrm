namespace Fanex.NexHR.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Core;
    using Interfaces;

    public class CryptoService(AppSetting appSetting) : ICryptoService
    {
        private const byte FeedbackSize = 128;

        public string Encrypt(string plainText, string rijndaelKey, string rijndaelIV)
        {
            if (!string.IsNullOrEmpty(plainText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
                    rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);

                    var encryptor = rijAlg.CreateEncryptor();
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (var srEncrypt = new StreamWriter(csEncrypt))
                            {
                                srEncrypt.Write(plainText);
                            }

                            return Convert.ToBase64String(msEncrypt.ToArray());
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV)
        {
            if (!string.IsNullOrEmpty(cipherText))
            {
                using (var rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.FeedbackSize = FeedbackSize;
                    rijAlg.Key = Encoding.UTF8.GetBytes(rijndaelKey);
                    rijAlg.IV = Encoding.UTF8.GetBytes(rijndaelIV);
                    var decryptor = rijAlg.CreateDecryptor();
                    using (var msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                return srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public string RsaEncrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                var rijndaelKey = appSetting.RijndaelKey;
                var rijndaelIV = appSetting.RijndaelIV;
                cipherText = Encrypt(plainText, rijndaelKey, rijndaelIV);
            }

            return cipherText;
        }

        public string RsaDecrypt(string cipherText)
        {
            var plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                var rijndaelKey = appSetting.RijndaelKey;
                var rijndaelIV = appSetting.RijndaelIV;
                plainText = Decrypt(cipherText, rijndaelKey, rijndaelIV);
            }

            return plainText;
        }
    }
}
