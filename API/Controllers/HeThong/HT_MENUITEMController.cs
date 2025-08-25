using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_MENUITEM")]
    [ApiController]
    public class HT_MENUITEMController: BaseController
    {
        public HT_MENUITEMController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_MENUITEM;
        }
     
        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                string mid = Request.Query["MID"].ToString();
                Menus mn = _db.HT_VARIABLECollection.MenusGet(mid);
                bool access = UserAccess(Quyen.Them, Quyen.Sua, Quyen.Xoa);
                return ObjectResult(new
                {
                    View = access,
                    New = access,
                    Edit = access,
                    Delete = access,
                    Title = mn?.NAME
                });
            }
            else if (op == "List")
            {
                string mid = Request.Query["MID"];
                if (mid == null || mid == "") return new BadRequestResult();
                return ListAll(_db.HT_VARIABLECollection.MenuItemGetAll(mid));
            }
            else if (op == "Perm")
            {
                return ListAll(GetSystemPermissionTree().AsQueryable());
            }
            return new BadRequestResult();
        }

        //add new/ delete multi
        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            string mid = item["MID"].ToString();
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Create")
            {
                if (!UserAccess(Quyen.Them, Quyen.Sua, Quyen.Xoa)) return UserAccessDenied();
                var itemNew = item.ToObject<MenuItem>();
                _db.HT_VARIABLECollection.MenuItemAdd(itemNew, mid);
                HtLog(Quyen.Them, itemNew.ID + "");
            }
            return new NoContentResult();
        }
        //add new/ delete multi
        [HttpPost("{op}/{mid}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, string mid, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Delete")
            {
                if (!UserAccess(Quyen.Them, Quyen.Sua, Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item["items"].Value<string>());
                foreach (var id in jItem)
                {
                    _db.HT_VARIABLECollection.MenuItemDelete(id.Value<int>(), mid);
                    HtLog(Quyen.Xoa, id.Value<string>() + "");
                }
            }
            return new NoContentResult();
        }

        //edit
        [HttpPut("{MID}/{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(string MID, int ID, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Them, Quyen.Sua, Quyen.Xem)) return UserAccessDenied();
            var oldItem = _db.HT_VARIABLECollection.MenuItemGet(ID, MID);
            if (oldItem != null)
            {
                var updateItem = (MenuItem)UpdateObject(oldItem, item, new string[] { "id" });
                _db.HT_VARIABLECollection.MenuItemUpdate(updateItem, MID);
                HtLog(Quyen.Sua, updateItem.ID + "");
            }
            return new NoContentResult();
        }
    }
}
