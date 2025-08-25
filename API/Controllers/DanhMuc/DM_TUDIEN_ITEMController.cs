using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/DanhMuc/DM_TUDIEN_ITEM")]
    [ApiController]
    public class DM_TUDIEN_ITEMController : BaseController
    {
        public DM_TUDIEN_ITEMController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_TUDIEN_ITEM;
        }


        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                var objDM_TUDIEN = _db.DM_TUDIENCollection.GetByID(ConvertClass.ToLong(Request.Query["TUDIEN_ID"], 0));
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.Xem),
                    New = UserAccess(Quyen.Them),
                    Edit = UserAccess(Quyen.Sua),
                    Delete = UserAccess(Quyen.Xoa),
                    Title = objDM_TUDIEN?.TEN + "",
                });
            }
            else if (op == "List")
            {
                long TUDIEN_ID = ConvertClass.ToLong(Request.Query["TUDIEN_ID"] + "", 0);
                return ListAll(_db.DM_TUDIEN_ITEMCollection.GetByTUDIEN_ID(TUDIEN_ID));
            }
            else if (op == "Check")
            {
                string MA = Request.Query["ID"] + "";
                long TUDIEN_ID = ConvertClass.ToLong(Request.Query["TUDIEN_ID"] + "", 0);
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (MA == "") return new BadRequestResult();
                DM_TUDIEN_ITEM objDM_TUDIEN_ITEM = _db.DM_TUDIEN_ITEMCollection.GetByMA(MA, TUDIEN_ID);
                return ObjectResult(new { Check = objDM_TUDIEN_ITEM != null && objDM_TUDIEN_ITEM.ID != OldID });
            }
            return new BadRequestResult();
        }

        //add new/ delete multi
        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Create")
            {
                var itemNew = item.ToObject<DM_TUDIEN_ITEM>();
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                _db.DM_TUDIEN_ITEMCollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID, itemNew.MA);
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                foreach (var id in jItem)
                {
                    var objDM_TUDIEN_ITEM = _db.DM_TUDIEN_ITEMCollection.GetByID(id.Value<long>());
                    _db.DM_TUDIEN_ITEMCollection.Remove(objDM_TUDIEN_ITEM);
                    HtLog(Quyen.Xoa, objDM_TUDIEN_ITEM.ID, (objDM_TUDIEN_ITEM.MA + "" == "" ? objDM_TUDIEN_ITEM.TEN : objDM_TUDIEN_ITEM.MA));
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            return new NoContentResult();
        }

        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
            var oldItem = _db.DM_TUDIEN_ITEMCollection.GetByID(ID);
            if (oldItem != null)
            {
                var updateItem = (DM_TUDIEN_ITEM)UpdateObject(oldItem, item, new string[] { "id" });
                _db.DM_TUDIEN_ITEMCollection.Update(updateItem);
                HtLog(Quyen.Sua, updateItem.ID, updateItem.MA);
            }
            return new NoContentResult();
        }
    }
}
