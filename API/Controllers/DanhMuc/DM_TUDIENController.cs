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
    [Route("api/DanhMuc/DM_TUDIEN")]
    [ApiController]
    public class DM_TUDIENController : BaseController
    {
        public DM_TUDIENController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_TUDIEN;
        }

        public static List<QUYEN> Permission()
        {
            DM_TUDIENController mn = new DM_TUDIENController(null);
            return mn.QuyenCoBanThem();
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
                return ListAll(_db.DM_TUDIENCollection.Get());
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
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                var itemNew = item.ToObject<DM_TUDIEN>();
                _db.DM_TUDIENCollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID, itemNew.TEN);
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                foreach (var id in jItem)
                {
                    var objDM_TUDIEN = _db.DM_TUDIENCollection.GetByID(id.Value<long>());
                    if (objDM_TUDIEN != null)
                    {
                        var lstDM_TUDIEN_ITEM = _db.DM_TUDIEN_ITEMCollection.GetByTUDIEN_ID(objDM_TUDIEN.ID).ToList();
                        if (lstDM_TUDIEN_ITEM.Count > 0 && !IsAdmin)
                        {
                            mess.Add("Không thể xóa danh mục " + objDM_TUDIEN.TEN);
                        }
                        else
                        {
                            _db.DM_TUDIEN_ITEMCollection.RemoveByTUDIEN_ID(objDM_TUDIEN.ID);
                            _db.DM_TUDIENCollection.Remove(objDM_TUDIEN);
                            HtLog(Quyen.Xoa, objDM_TUDIEN.ID, objDM_TUDIEN.TEN);
                        }
                    }
                    else
                    {
                        mess.Add("Không thể xóa danh mục " + id);
                    }
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
            var oldItem = _db.DM_TUDIENCollection.GetByID(ID);
            if (oldItem != null)
            {
                var updateItem = (DM_TUDIEN)UpdateObject(oldItem, item, new string[] { "id" });
                _db.DM_TUDIENCollection.Update(updateItem);
                HtLog(Quyen.Sua, updateItem.ID, updateItem.TEN);
                ResetPermission();
            }
            return new NoContentResult();
        }
    }
}
