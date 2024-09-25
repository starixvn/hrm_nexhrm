namespace Fanex.NexHR.Services.Interfaces
{
    public interface ICryptoService
    {
        string Encrypt(string plainText, string rijndaelKey, string rijndaelIV);

        string Decrypt(string cipherText, string rijndaelKey, string rijndaelIV);

        string RsaEncrypt(string plainText);

        string RsaDecrypt(string cipherText);
    }
}
