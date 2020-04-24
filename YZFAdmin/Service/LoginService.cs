using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YZF.Common;

namespace YZFAdmin.Service
{
    public class LoginService
    {
        public dynamic GetUser(string username,string pwd)
        {
            using (var con = DapperFactory.DatabaseInstance)
            {
                var user = con.Query<dynamic>($@"select * from [user] where login='{username}' and pwd='{pwd}' ").FirstOrDefault();
                return user;
            }
        }
    }
}
