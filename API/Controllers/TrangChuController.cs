using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;

namespace API.Controllers.TrangChu
{
    [Consumes("application/json")]
    [Route("api/TrangChu")]
    [ApiController]
    public class TrangChuController : BaseController
    {
        public TrangChuController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : 
            base(db, hostingEnvironment, configuration)
        {
        }

        [HttpGet("{op}")]
        public IActionResult Get(string op)
        {
            
            return new BadRequestResult();
        }

    }
}
