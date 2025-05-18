using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineMuhasebeServer.Presentation.Abstraction;

namespace OnlineMuhasebeServer.Presentation.Controller
{
    public sealed class DemoController : ApiContoller
    {
        [HttpGet]
        public IActionResult GetDemo()
        {
            return Ok("Demo endpoint is working!");
        }

    }
}
