using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_LICHSU")]
    [ApiController]
    public class HT_LICHSUController : BaseController
    {
        public HT_LICHSUController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_LICHSU;
        }

        public static List<QUYEN> Permission()
        {
            HT_LICHSUController mn = new HT_LICHSUController(null);
            return mn.QuyenCoBan(Quyen.Xem);
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.Xem),
                    Title = CurrentMenu?.NAME + "",
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.HT_LICHSUCollection.GetByDVQL_ID(DVQL_ID, GetDsDM_DONVI_IDS()));
            }
            return new BadRequestResult();
        }

    }
}
