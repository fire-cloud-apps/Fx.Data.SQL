using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Fx.Security.JWT;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Fx.Security.Helper;
public class SecretManager : IDisposable
{
    private readonly string _accessKey = string.Empty;
    private readonly string _secretKey = string.Empty;
    private readonly string _region = string.Empty;
    private readonly AmazonSecretsManagerClient _client = null;

    public SecretManager(string accKey, string secKey, string region = "ap-southeast-1")
    {
        _accessKey = accKey;
        _secretKey = secKey;
        _region = region;
        var regionPoint = RegionEndpoint.GetBySystemName(_region);
        _client = new AmazonSecretsManagerClient(_accessKey, _secretKey, regionPoint);
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        GetSecretValueResponse response = await _client.GetSecretValueAsync(request);

        // Decrypt the secret value if necessary
        if (response.SecretString != null)
        {
            return response.SecretString;
        }

        return string.Empty;

    }

    public async Task<bool> UpdateSecretAsync(string secretName, JWTDetails jwtMeta)
    {
        var request = new PutSecretValueRequest // Use PutSecretValueRequest, not UpdateSecretValueRequest
        {
            SecretId = secretName,
            SecretString = JsonConvert.SerializeObject(jwtMeta)
        };

        var response =  await _client.PutSecretValueAsync(request);

        if(response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }
        return false; 
    }

    public async Task<bool> CreateSecretAsync(string secretName, JWTDetails jwtMeta)
    {   
        var request = new CreateSecretRequest
        {
           Name = secretName,
           SecretString = JsonConvert.SerializeObject(jwtMeta)            
        };
        var response = await _client.CreateSecretAsync(request);

        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
        {
            return true;
        }

        return false;
    }

    #region Implements Dispose Methods
    private bool _disposed = false;
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            if (_client != null)
            {
                _client.Dispose();
            }
        }        
        _disposed = true;
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
    #endregion
}

