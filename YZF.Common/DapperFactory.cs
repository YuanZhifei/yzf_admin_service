using Microsoft.AspNetCore.Builder;
using System;
using System.Data.SqlClient;

namespace YZF.Common
{
    public static class DapperUtilMiddleWare
    {
        public static IApplicationBuilder UserDapper(this IApplicationBuilder builder, string conString)
        {
            if (DapperFactory.ConString == null) DapperFactory.ConString = conString;
            return builder;
        }
    }
    public class DapperFactory
    {
        internal static string ConString { get; set; }

        //public static OracleConnection DatabaseInstance
        //{
        //    get
        //    {
        //        return new OracleConnection(ConString);
        //    }
        //}
        public static SqlConnection DatabaseInstance
        {
            get
            {
                return new SqlConnection(ConString);
            }
        }
    }
}
