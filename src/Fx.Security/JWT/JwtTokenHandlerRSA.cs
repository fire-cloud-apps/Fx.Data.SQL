using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Fx.Security.JWT;

/// <summary>
/// Generates and Verifies JWT Token using RSA256
/// </summary>
public class JwtTokenHandlerRSA
{
    private readonly RSA _rsa;

    /// <summary>
    /// JWT Token Initializer
    /// </summary>
    /// <param name="publicKey"></param>
    /// <param name="privateKey"></param>
    public JwtTokenHandlerRSA(string publicKey, string privateKey)
    {
        _rsa = RSA.Create();
        _rsa.ImportRSAPublicKey(Convert.FromBase64String(publicKey), out _);
        _rsa.ImportRSAPrivateKey(Convert.FromBase64String(privateKey), out _);
    }

    /// <summary>
    /// Generate Token based on the details provided.
    /// </summary>
    /// <param name="details">JWT Metadata to be assigned.</param>
    /// <param name="claims">Claim details to be added in the token</param>
    /// <returns>JWT Token as string</returns>
    public string GenerateToken(JWTDetails details, Claim[] claims)
    {
        return GenerateToken(
            issuer: details.Issuer, 
            audience: details.Audiance, 
            subject: details.Subject, 
            expiry: details.Expiry, 
            claims: claims);
    }

    public string GenerateToken(string issuer,
        string audience, string subject,
        DateTime expiry, Claim[] claims)
    {
        try
        {
            var key = new RsaSecurityKey(_rsa);
            var credentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: expiry,
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }
        catch
        {
            throw;
        }
    }

    public bool VerifyToken(string token, string issuer, string audience, out ClaimsPrincipal principal)
    {
        principal = null;

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = issuer,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new RsaSecurityKey(_rsa)
            };

            principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return true;
        }
        catch
        {            
            return false;
            throw;
        }
    }


}



