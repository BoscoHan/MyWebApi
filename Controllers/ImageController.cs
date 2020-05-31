using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MyWebApi.Models;

namespace MyWebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IConfiguration _config;
        public ImageController(IConfiguration config)
        {
            _config = config;
        }

        //[HttpGet]
        //public IEnumerable<Image> GetImages()
        //{
        //    throw new NotImplementedException();
        //}

        //get values from appsettings.json
        [HttpGet]
        public string Checkconfig()
        {
            //var config = _config.GetValue<int>(
            //    "ConnectionStrings:MyWebApiConnection:Port");
            //return config.ToString();

            var baseUrl = _config.GetValue<string>("ConnectionStrings:MyWebApiConnection:Server");
            return baseUrl;
        }

    }
}
