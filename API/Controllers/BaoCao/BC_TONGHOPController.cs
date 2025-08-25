using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;
namespace API.Controllers.BaoCao
{
    [Consumes("application/json")]
    [Route("api/BaoCao/BC_TONGHOP")]
    [ApiController]
    public class BC_TONGHOPController : BaseController
    {
        public BC_TONGHOPController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.BaoCao;
            NhomQuyen = Resource.QuyenBC_TONGHOP;
        }

        public static List<QUYEN> Permission()
        {
            BC_TONGHOPController mn = new BC_TONGHOPController(null);
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
                    DVSD_ID
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.BC_BAOCAOCollection.Get().Where(c => c.TRANG_THAI == 2));
            }
            return new BadRequestResult();
        }
    }
}
