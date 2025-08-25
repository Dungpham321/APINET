using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/DanhMuc/DM_HETHONGKEY")]
    [ApiController]
    public class DM_HETHONGKEYController : BaseController
    {
        public DM_HETHONGKEYController(GDBContext db) : base(db) { }
        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "HT_NGUOIDUNGByKey")
            {
                return ObjectResult(_db.HT_NGUOIDUNGCollection.GetByID(ConvertClass.ToLong(Request.Query["key"] + "", 0))?.TEN_DANG_NHAP);
            }
            //else if (op == "DM_DONVIByKey")
            //{
            //    return ObjectResult(_db.DM_DONVICollection.GetByID(ConvertClass.ToLong(Request.Query["key"] + "", 0))?.TEN_DON_VI);
            //}
            return new BadRequestResult();
        }
    }
}
