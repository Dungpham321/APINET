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
    [Route("api/HeThong/HT_CAUHINH")]
    [ApiController]
    public class HT_CAUHINHController : BaseController
    {
        public HT_CAUHINHController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_CAUHINH;
        }
        public static List<QUYEN> Permission()
        {
            HT_CAUHINHController mn = new HT_CAUHINHController(null);
            return mn.QuyenCoBan(Quyen.CauHinh, Quyen.QuanTriDonVi, Quyen.QuanTriHeThong);
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
                    Data = _db.HT_VARIABLECollection.VariableGet<JObject>("SystemConfig", "HT_CAUHINH_" + DVQL_ID, new JObject()),
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
                _db.HT_VARIABLECollection.VariableSet("SystemConfig", "HT_CAUHINH_" + DVQL_ID, item);
                HtLog(Quyen.CauHinh, DVQL_ID);
            }
            return new NoContentResult();
        }
    }
}
