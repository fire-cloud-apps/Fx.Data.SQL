using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sun.Delivery.API.Helpers
{
    public static class Utilities
    {
        /// <summary>
        /// Initializes Dynamic Connection String
        /// </summary>
        /// <param name="configuration">Configuration object</param>
        /// <param name="dbName">runtime Database</param>
        /// <returns></returns>
        public static string InitializeConnectionString(IConfiguration configuration, string dbName)
        {
            string connectionString = configuration.GetValue<string>("DBSettings:DynamicConnectionString");
            connectionString = string.Format(connectionString, dbName);
            return connectionString;
        }
    }
}
