namespace Fanex.NexHR.Services
{
    public interface ICryptoService
    {
        string RsaEncrypt(string plainText);

        string RsaDecrypt(string cipherText);
    }
}