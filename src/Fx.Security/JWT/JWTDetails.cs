using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace Fx.Security.JWT;

/// <summary>
/// To generate token the below property will be used as a meta data.
/// </summary>
public class JWTDetails
{
    /// <summary>
    /// Token Issuer
    /// </summary>
    public string Issuer { get; set; }
    /// <summary>
    /// End user <see cref="Audiance"/>
    /// </summary>
    public string Audiance { get; set; }
    /// <summary>
    /// Subject for the Generated Token
    /// </summary>
    public string Subject { get; set; }
    /// <summary>
    /// Expiration in date time to be used as eg. 'DateTime.UtcNow.AddHours(24)'
    /// </summary>
    public DateTime Expiry { get; set; }

    /// <summary>
    /// Key used to Decrypt the token
    /// </summary>
    public string PublicKey { get; set; }
    /// <summary>
    /// Key used to Generate the Toke and Encrypt it.
    /// </summary>
    public string PrivateKey { get; set; }
    
}

