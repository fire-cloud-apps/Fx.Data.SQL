using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Security.Helper;

/// <summary>
/// Generates Key for Password
/// </summary>
public static class KeyGenerator
{
    /// <summary>
    /// Key Size in Bits ( 1024, 2048, 4096, 8192 etc.). Default is 2048
    /// </summary>
    public static int Bits { get; set; } = 2048;
    /// <summary>
    /// Returns Public and Private key used or RSA 
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="privateKey"></param>
    /// <param name="isClean">true indicates the generated will not have any header.</param>
    public static void GenerateKey(out string publicKey, out string privateKey, bool isClean = true)
    {
        using (var rsa = new RSACryptoServiceProvider(Bits))
        {
            publicKey = rsa.ExportRSAPublicKeyPem();
            privateKey = rsa.ExportRSAPrivateKeyPem(); 
        }
        if (isClean)
        {
            publicKey = CleanKey(publicKey);
            privateKey = CleanKey(privateKey);
        }
    }

    private static string CleanKey(string key)
    {
        string filtered = key.Replace("\n", "")
                     .Replace("\r", "").Replace(" ", "")
                     .Replace("-----", "")
                     .Replace("END", "").Replace("RSA", "")
                     .Replace("BEGIN", "").Replace("KEY", "")
                     .Replace("PUBLIC", "").Replace("PRIVATE", "");
        return filtered;
    }


}

