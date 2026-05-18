using System.Security.Cryptography;
using System.Text;

namespace PowerTraderExam.Infrastructure.Security;

public static class RsaSignHelper
{
    public static string CreateSign(string plainText, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem.AsSpan());

        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipherBytes = rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(cipherBytes);
    }
}
