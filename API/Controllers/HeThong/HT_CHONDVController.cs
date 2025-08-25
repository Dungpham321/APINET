using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_CHONDV")]
    [ApiController]
    public class HT_CHONDVController : BaseController
    {
        public HT_CHONDVController(GDBContext db) : base(db) {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_CHONDV;
        }

        public static List<QUYEN> Permission()
        {
            HT_CHONDVController mn = new HT_CHONDVController(null);
            return mn.QuyenCoBan(Quyen.ChuyenDoi);
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.ChuyenDoi),
                    DVQL_ID,
                    DVSD_ID
                });
            }
            else if (op == "List")
            {
                return ListAll(GetLcDM_DONVI());

            }
            return new BadRequestResult();
        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] JObject item)
        {
            if (!UserAccess(Quyen.ChuyenDoi)) return UserAccessDenied();
            DM_DANHMUC_ITEM objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(item["DVSD_ID"].Value<long>());
            if (objDM_DONVI != null)
            {
                DateTime requestAt = DateTime.Now;
                DateTime expiresIn = requestAt + TokenAuthOption.ExpiresSpan;
                CurrentUser.DVSD_ID = objDM_DONVI.ID;
                CurrentUser.DVQL_ID = objDM_DONVI.DVQL_ID.Value;
               
                var u = _db.HT_NGUOIDUNGCollection.GetByID(CurrentUser.ID);
                u.DVSD_ID = CurrentUser.DVSD_ID;
                _db.HT_NGUOIDUNGCollection.Update(u);
                string token = GenerateToken(CurrentUser, expiresIn);
                HtLog(Quyen.ChuyenDoi, CurrentUser.DVSD_ID);
                return ObjectResult(new
                {
                    requertAt = requestAt,
                    expiresIn = TokenAuthOption.ExpiresSpan.TotalSeconds,
                    tokeyType = TokenAuthOption.TokenType,
                    accessToken = token,
                    CurrentUser.ID,
                    CurrentUser.EMAIL,
                    CurrentUser.TEN_DANG_NHAP,
                    CurrentUser.TEN_DAY_DU,
                    CurrentUser.DVQL_ID,
                    CurrentUser.DVSD_ID,
                    TEN_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(CurrentUser.DVSD_ID)?.TEN + "",
                    TEN_DONVI_CHA = _db.DM_DANHMUC_ITEMCollection.GetByID(CurrentUser.DVQL_ID)?.TEN + "",
                    GD_MAU_SAC = HT_CAUHINH_Get<string>("GD_MAU_SAC", "", CurrentUser.DVQL_ID),
                });
            }

            return ObjectResult(null, Resource.strLuaChonSai, RequestState.Failed);
        }
    }
}
