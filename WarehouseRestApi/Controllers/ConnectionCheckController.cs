using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WarehouseRestApi.Controllers
{
    [Produces("application/json")]
    [Route("api/ConnectionCheck")]
    public class ConnectionCheckController : Controller
    {
        [HttpGet]
        public string Get()
        {
            return "Server is connected";
        }
    }
}