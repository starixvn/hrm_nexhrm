namespace Fanex.NexHR.Services
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    using Fanex.NexHR.Core;

    public class CryptoService : ICryptoService
    {
        private const bool OptimalAsymmetricEncryptionPadding = false;
        private readonly string publicKey;
        private readonly string privateKey;
        private const short KeySize = 2048;
        private readonly AppSetting appSetting;

        public CryptoService(AppSetting appSetting)
        {
            this.appSetting = appSetting;
            publicKey = ReadTextFile(appSetting.RSAPublicKeyPath);
            privateKey = ReadTextFile(appSetting.RSAPrivateKeyPath);
        }

        public string RsaEncrypt(string plainText)
        {
            var cipherText = string.Empty;

            if (!string.IsNullOrWhiteSpace(plainText))
            {
                if (appSetting.EnableEncrypt)
                {
                    byte[] bytesToEncrypt = ASCIIEncoding.ASCII.GetBytes(plainText);

                    using (var rsa = new RSACryptoServiceProvider(KeySize))
                    {
                        rsa.FromXmlString(publicKey);
                        byte[] encryptedBytes = rsa.Encrypt(bytesToEncrypt, OptimalAsymmetricEncryptionPadding);
                        cipherText = Convert.ToBase64String(encryptedBytes);
                    }
                }
                else
                {
                    cipherText = plainText;
                }
            } 

            return cipherText;
        }

        public string RsaDecrypt(string cipherText)
        {
            var plainText = string.Empty;

            if (!string.IsNullOrWhiteSpace(cipherText))
            {
                if (appSetting.EnableEncrypt)
                {
                    byte[] bytesToDecrypt = Convert.FromBase64String(cipherText);
                    using (var rsa = new RSACryptoServiceProvider(KeySize))
                    {
                        rsa.FromXmlString(privateKey);
                        byte[] plainbytes = rsa.Decrypt(bytesToDecrypt, OptimalAsymmetricEncryptionPadding);
                        var enc = new ASCIIEncoding();
                        plainText = enc.GetString(plainbytes);
                    }
                }
                else
                {
                    plainText = cipherText;
                }
            }

            return plainText;
        }

        private static string ReadTextFile(string filePath)
        {
            var text = string.Empty;
            if(File.Exists(filePath))
            {
                text = File.ReadAllText(filePath);
            }

            return text;
        }
    }
}
