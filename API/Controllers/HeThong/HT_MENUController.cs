using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_MENU")]
    [ApiController]
    public class HT_MENUController : BaseController
    {
        public HT_MENUController(GDBContext db) : base(db) {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_MENU;
        }
        public static List<QUYEN> Permission()
        {
            HT_MENUController mn = new HT_MENUController(null);
            return mn.QuyenCoBan();
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
                    New = UserAccess(Quyen.Them),
                    Edit = UserAccess(Quyen.Sua),
                    Delete = UserAccess(Quyen.Xoa),
                    Title = CurrentMenu?.NAME + "",
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.HT_VARIABLECollection.MenusGet());
            }
            else if (op == "Check")
            {
                string mid = Request.Query["ID"];
                if (mid + "" == "") return new BadRequestResult();
                return ObjectResult(new { Check = _db.HT_VARIABLECollection.MenusGet(mid) != null });
            }
            return new BadRequestResult();
        }

        //add new/ delete multi
        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if(op == "Create")
            {
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                var itemNew = item.ToObject<Menus>();
                _db.HT_VARIABLECollection.MenusAdd(itemNew);
                HtLog(Quyen.Them, itemNew.MID);
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var mid in jItem)
                {
                    _db.HT_VARIABLECollection.MenusDelete(mid.Value<string>());
                    HtLog(Quyen.Xoa, mid.Values<string>() + "");
                }
            }
            return new NoContentResult();
        }

        //edit
        [HttpPut("{MID}")]
        [Authorize("Bearer")]
        public IActionResult Put(string MID, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
            var oldItem = _db.HT_VARIABLECollection.MenusGet(MID);
            if (oldItem != null)
            {
                var updateItem = (Menus)UpdateObject(oldItem, item, new string[] { "mid" });
                _db.HT_VARIABLECollection.MenusUpdate(MID, updateItem);
                HtLog(Quyen.Sua, updateItem.MID);
            }
            return new NoContentResult();
        }
    }
}
