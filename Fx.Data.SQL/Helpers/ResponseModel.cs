using Microsoft.AspNetCore.Http;

namespace Fx.Data.SQL.Helpers;
public class ResponseModel
{
    /// <summary>
    /// Indicates that the execution is success or failure
    /// </summary>
    public bool Success { get; set; }
    /// <summary>
    /// Message about the execution, if success, simply it says "Ok", if error, it will display high level error message. 
    /// </summary>
    public string? Message { get; set; }
    ///// <summary>
    ///// Brings HTTP Status code.
    ///// Should come from 'StatusCodes'
    ///// </summary>
    //public int? StatusCode { get; set; }
    /// <summary>
    /// The actual returned data. If no content or no data it would be empty.
    /// </summary>
    public dynamic? Data { get; set; }
}

