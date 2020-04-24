using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZFAdmin.Service;

namespace YZFAdmin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly DapperClient _SqlDB;
        private readonly DapperClient _OracleDB;
        public TestController(IDapperFactory dapperFactory)
        {
            //_OracleDB = dapperFactory.CreateClient("OracleConnection");
            _SqlDB = dapperFactory.CreateClient("YZFConnection");
        }

        [HttpGet]
        public object Get()
        {
        //    var testQuery = _OracleDB.Query<dynamic>(@"SELECT * FROM BASE_DEPT where ROWNUM<=5");

            var result = _SqlDB.Query<dynamic>(@"select * from UserInfo");

            return Ok();
        }

    }
}