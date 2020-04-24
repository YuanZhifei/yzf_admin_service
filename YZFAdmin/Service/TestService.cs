using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YZF.Common;

namespace YZFAdmin.Service
{
    public class TestService
    {
        public IEnumerable<dynamic> GetAllInfo()
        {
            using (var con = DapperFactory.DatabaseInstance)
            {
                var list = con.Query(@"select * from UserInfo");
                return list;
            }
        }
    }
}
