using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Security.Helper;
/// <summary>
/// Used to Hash the Key value
/// </summary>
public static class KeyHash
{
    /// <summary>
    /// Hash the password using any of the alogrithm SHA256 or MD5. 
    /// </summary>
    /// <param name="hashing">Default SHA256</param>
    /// <param name="key">Key to Hash</param>
    /// <returns>Password as Hash</returns>
    public static string HashPassword(Hashing hashing, string key)
    {
        string hashKey = string.Empty;
        switch(hashing)
        {
            case Hashing.MD5:
                hashKey = MD5_HashPassword(key);
                break;
            case Hashing.SHA256:
            default:
                hashKey = SHA_HashPassword(key);
                break;
        }
        return hashKey;
    }
    private static string SHA_HashPassword(string password)
    {
        StringBuilder builder;
        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder = builder.Append(b.ToString("x2"));
            }            
        }
        return builder.ToString();
    }

    private static string MD5_HashPassword( string password)
    {
        StringBuilder builder;
        using (MD5 md5Hash = MD5.Create())
        {
            byte[] bytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

            builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                builder = builder.Append(b.ToString("x2"));
            }            
        }
        return builder.ToString();

    }

}

public enum Hashing
{
    MD5,SHA256

}

