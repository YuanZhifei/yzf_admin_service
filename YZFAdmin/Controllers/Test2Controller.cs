using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YZFAdmin.Service;

namespace YZFAdmin.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class Test2Controller : ControllerBase
    {
        private TestService  _test2Service;
        public Test2Controller(TestService testService)
        {
            _test2Service = testService;
        }


        [HttpGet("api/test2/GetZYRY")]
        public object GetZYRY()
        {
            //    var testQuery = _OracleDB.Query<dynamic>(@"SELECT * FROM BASE_DEPT where ROWNUM<=5");

            var result = _test2Service.GetAllInfo();

            return Ok();
        }

        [HttpGet("api/test2/Login")]
        public object Login()
        {
            //    var testQuery = _OracleDB.Query<dynamic>(@"SELECT * FROM BASE_DEPT where ROWNUM<=5");

            var result = _test2Service.GetAllInfo();

            return Ok();
        }

    }
}