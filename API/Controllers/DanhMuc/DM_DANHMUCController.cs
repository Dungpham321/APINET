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
    [Route("api/DanhMuc/DM_DANHMUC")]
    [ApiController]
    public class DM_DANHMUCController : BaseController
    {
        public DM_DANHMUCController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_DANHMUC;
        }

        public static List<QUYEN> Permission()
        {
            DM_DANHMUCController mn = new DM_DANHMUCController(null);
            return mn.QuyenCoBanThem(Quyen.CauHinhCot);
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
                return ListAll(_db.DM_DANHMUCCollection.Get());
            }
            else if (op == "Check")
            {
                string MA = Request.Query["ID"] + "";
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (MA == "") return new BadRequestResult();
                DM_DANHMUC objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(MA);
                return ObjectResult(new { Check = objDM_DANHMUC != null && objDM_DANHMUC.ID != OldID });
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
                var itemNew = item.ToObject<DM_DANHMUC>();
                itemNew.HETHONG = false;
                _db.DM_DANHMUCCollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID, itemNew.MA);
                ResetPermission();
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                foreach (var id in jItem)
                {
                    var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(id.Value<long>());
                    if (objDM_DANHMUC != null && !objDM_DANHMUC.HETHONG)
                    {
                        var lstDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID).ToList();
                        if (lstDM_DANHMUC_ITEM.Count > 0 && !IsAdmin)
                        {
                            mess.Add("Không thể xóa danh mục " + objDM_DANHMUC.MA);
                        }
                        else
                        {
                             
                            _db.DM_DANHMUC_COLCollection.RemoveByDANHMUC_ID(objDM_DANHMUC.ID);
                            _db.DM_DANHMUC_ITEMCollection.RemoveByDANHMUC_ID(objDM_DANHMUC.ID);
                            _db.DM_DANHMUCCollection.Remove(objDM_DANHMUC);
                            //todo: xoa lien quan
                            HtLog(Quyen.Xoa, objDM_DANHMUC.ID, objDM_DANHMUC.MA);
                        }
                    }
                    else
                    {
                        mess.Add("Không thể xóa danh mục " + objDM_DANHMUC.MA);
                    }
                }
                ResetPermission();
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
            var oldItem = _db.DM_DANHMUCCollection.GetByID(ID);
            if (oldItem != null)
            {
                var updateItem = (DM_DANHMUC)UpdateObject(oldItem, item, new string[] { "id", "ma" });
                _db.DM_DANHMUCCollection.Update(updateItem);
                HtLog(Quyen.Sua, updateItem.ID, updateItem.MA);
                ResetPermission();
            }
            return new NoContentResult();
        }
    }
}
