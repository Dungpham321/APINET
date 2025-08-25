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
    [Route("api/DanhMuc/DM_DANHMUC_COL")]
    [ApiController]
    public class DM_DANHMUC_COLController : BaseController
    {
        public DM_DANHMUC_COLController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_DANHMUC;
        }
        

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(Request.Query["MA_DANHMUC"]);
                bool access = UserAccess(Quyen.CauHinhCot);
                return ObjectResult(new
                {
                    View = access,
                    New = access,
                    Edit = access,
                    Delete = access,
                    Title = objDM_DANHMUC?.TEN + "",
                    danhmuc = objDM_DANHMUC
                });
            }
            else if (op == "List")
            {
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                return ListAll(_db.DM_DANHMUC_COLCollection.GetByDANHMUC_ID(DANHMUC_ID));
            }
            else if (op == "Check")
            {
                string MA = Request.Query["ID"] + "";                
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (MA == "") return new BadRequestResult();
                DM_DANHMUC_COL objDM_DANHMUC_COL = _db.DM_DANHMUC_COLCollection.GetByMA(MA, DANHMUC_ID);
                return ObjectResult(new { Check = objDM_DANHMUC_COL != null && objDM_DANHMUC_COL.ID != OldID });
            }
            else if (op == "ListDanhMuc")
            {
                return ListAll(_db.DM_DANHMUCCollection.Get());
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
                var itemNew = item.ToObject<DM_DANHMUC_COL>();
                if (!UserAccess(Quyen.CauHinhCot)) return UserAccessDenied();
                _db.DM_DANHMUC_COLCollection.Add(itemNew);
                HtLog(Quyen.CauHinhCot, itemNew.ID, itemNew.MA);
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                foreach (var id in jItem)
                {
                    var objDM_DANHMUC_COL = _db.DM_DANHMUC_COLCollection.GetByID(id.Value<long>());
                    _db.DM_DANHMUC_COLCollection.Remove(objDM_DANHMUC_COL);
                    HtLog(Quyen.CauHinhCot, objDM_DANHMUC_COL.ID, (objDM_DANHMUC_COL.MA + "" == "" ? objDM_DANHMUC_COL.TEN : objDM_DANHMUC_COL.MA));
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
            var oldItem = _db.DM_DANHMUC_COLCollection.GetByID(ID);
            if (oldItem != null)
            {
                var updateItem = (DM_DANHMUC_COL)UpdateObject(oldItem, item, new string[] { "id" });
                _db.DM_DANHMUC_COLCollection.Update(updateItem);
                HtLog(Quyen.CauHinhCot, updateItem.ID, updateItem.MA);
            }
            return new NoContentResult();
        }
    }
}
