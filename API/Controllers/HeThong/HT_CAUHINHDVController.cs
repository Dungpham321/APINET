using API.Common;

using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_CAUHINHDV")]
    [ApiController]
    public class HT_CAUHINHDVController : BaseController
    {
        public HT_CAUHINHDVController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_CAUHINHDV;
        }
        public static List<QUYEN> Permission()
        {
            HT_CAUHINHDVController mn = new HT_CAUHINHDVController(null);
            return mn.QuyenCoBan(Quyen.CauHinh);
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.CauHinh),
                    Data = _db.HT_VARIABLECollection.VariableGet<JObject>("DvConfig", "HT_CAUHINHDV_" + DVSD_ID, new JObject()),
                    Title = CurrentMenu?.NAME + "",
                });
            }
            return new BadRequestResult();
        }

        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Save")
            {
                if (!UserAccess(Quyen.CauHinh)) return UserAccessDenied();
                _db.HT_VARIABLECollection.VariableSet("DvConfig", "HT_CAUHINHDV_" + DVSD_ID, item);
                HtLog(Quyen.CauHinh, DVQL_ID);
            }
            return new NoContentResult();
        }
    }
}
