using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YZFAdmin.Service;

namespace YZFAdmin.Controllers
{
    public class LoginController : Controller
    {
        private LoginService _loginService;
        
        public LoginController(LoginService loginService)
        {
            
            _loginService = loginService;
        }
        [HttpPost("api/Login/Login")]
        public IActionResult Login(string username,string pwd)
        {
            
            var user = _loginService.GetUser(username, pwd);
            return Ok();
        }
    }
}