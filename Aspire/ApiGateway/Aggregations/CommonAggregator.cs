using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Fx.Data.SQL.Helpers;

namespace ApiGateway.Aggregations;
public class CommonAggregator : IDefinedAggregator
{
    /// <summary>    
    /// Aggregates the 2 or more API's
    /// </summary>
    /// <remarks>
    /// Ref: 
    /// https://arbems.com/en/building-api-gateway-on-net-with-ocelot/
    /// https://ocelot.readthedocs.io/en/latest/features/requestaggregation.html
    /// </remarks>
    /// <param name="responses"></param>
    /// <returns></returns>
    public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
    {             
        ResponseModel result = new ResponseModel()
        {
            Message = "Ok",
            Success = true,
            Data = new List<dynamic>()
        };
        HttpStatusCode lastStatusCode = HttpStatusCode.OK;
        foreach (var response in responses)
        {           
            var jsonItem = await response.Items.DownstreamResponse().Content.ReadFromJsonAsync<ResponseModel>();
            if (jsonItem is not null)
            {
                if (jsonItem.Data is not null)
                    result.Data.Add(jsonItem.Data);
            }
            else
            {
                result.Success = false;
                result.Message = "Execution Failed";
                result.Data = string.Empty;
            }
            lastStatusCode = response.Items.DownstreamResponse().StatusCode;
        }
        
        JsonContent jsonContent = JsonContent.Create(result);

        return new DownstreamResponse( jsonContent, lastStatusCode, new List<KeyValuePair<string, IEnumerable<string>>>(), result.Message);
    }
}

