using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace Fx.Security.JWT;

public static class JwtAuthExtension
{
    public static void AddCustomJwtAuthentication(this IServiceCollection service, JWTDetails jWTDetails )
    {
        RSA rsa;
        rsa = RSA.Create();
        rsa.ImportRSAPublicKey(Convert.FromBase64String(jWTDetails.PublicKey), out _);
        rsa.ImportRSAPrivateKey(Convert.FromBase64String(jWTDetails.PrivateKey), out _);
        var jwtToken = new JwtTokenHandlerRSA(jWTDetails.PublicKey, jWTDetails.PrivateKey);

        service.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = jWTDetails.Issuer,
                ValidAudience = jWTDetails.Audiance,
                //IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtMeta.PublicKey)),
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new RsaSecurityKey(rsa),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true
            };
        });
    }
}

