namespace Fanex.NexHR.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Fanex.NexHR.Core;
    using Fanex.NexHR.Services.Interfaces;

    public class CryptoService(AppSetting appSetting) : ICryptoService
    {
        private const byte FeedbackSize = 128;
        private const byte KeyLength = 16;
        private string rsaPublicKey;

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
            if (string.IsNullOrWhiteSpace(rsaPublicKey))
            {
                rsaPublicKey = ReadTextFile(appSetting.RSAPublicKeyPath);
            }

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                var rijndaelKey = rsaPublicKey.Substring(0, KeyLength);
                var rijndaelIV = rsaPublicKey.Substring(rsaPublicKey.Length - KeyLength);
                cipherText = Encrypt(plainText, rijndaelKey, rijndaelIV);
            }

            return cipherText;
        }

        public string RsaDecrypt(string cipherText)
        {
            var plainText = string.Empty;
            if (string.IsNullOrWhiteSpace(rsaPublicKey))
            {
                rsaPublicKey = ReadTextFile(appSetting.RSAPublicKeyPath);
            }

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                var rijndaelKey = rsaPublicKey.Substring(0, KeyLength);
                var rijndaelIV = rsaPublicKey.Substring(rsaPublicKey.Length - KeyLength);
                plainText = Decrypt(cipherText, rijndaelKey, rijndaelIV);
            }

            return plainText;
        }

        private static string ReadTextFile(string filePath)
        {
            var text = string.Empty;
            if (File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
            }

            return text;
        }
    }
}
