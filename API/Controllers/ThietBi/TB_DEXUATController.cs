using API.Common;
using GDB;
using GCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace API.Controllers.ThietBi
{
    [Consumes("application/json")]
    [Route("api/ThietBi/TB_DEXUAT")]
    [ApiController]
    public class TB_DEXUATController : BaseController
    {
        public TB_DEXUATController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_DEXUAT;
        }

        public static List<QUYEN> Permission()
        {
            TB_DEXUATController mn = new TB_DEXUATController(null, null, null);
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
                return ListAll(_db.TB_DEXUATCollection.Get(DVQL_ID, DVSD_ID));

            }else if(op == "thietbi")
            {
                long PHIEU_ID = ConvertClass.ToLong(Request.Query["PHIEU_ID"] + "", 0);
                var lstDEXUAT_CT = _db.TB_DEXUATCTCollection.GetByDEXUAT_ID(PHIEU_ID);
                return ObjectResult(lstDEXUAT_CT);
            }
            else if(op == "CanBo")
            {
                long CANBO_ID = ConvertClass.ToLong(Request.Query["CANBO_ID"] + "", 0);
                var objDM_CANBO = _db.DM_DANHMUC_ITEMCollection.GetByID(CANBO_ID);
                if(objDM_CANBO != null)
                {
                    return ObjectResult(objDM_CANBO);
                }
                return ObjectResult(null);
            }
           
            return new BadRequestResult();
        }
        //post
        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Create")
            {
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                var itemNew = item.ToObject<TB_DEXUAT>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                _db.TB_DEXUATCollection.Add(itemNew);

                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_DEXUATCT> lstTB_MUON = new List<TB_DEXUATCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                      
                        var insertCT = objCT.ToObject<TB_DEXUATCT>();
                        insertCT.ID = 0;
                        insertCT.PHIEU_ID = itemNew.ID;
                       
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_DEXUATCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Them, itemNew.ID, "Phiếu đề xuất mua thiết bị " + itemNew.ID);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    var objMUONTRA = _db.TB_DEXUATCollection.GetByID(id.Value<long>());
                    _db.TB_DEXUATCTCollection.RemoveByDEXUAT_ID(objMUONTRA.ID);
                    _db.TB_DEXUATCollection.Remove(objMUONTRA);
                    HtLog(Quyen.Xoa, objMUONTRA.ID);
                }
            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {

            var objTB_DEXUAT = _db.TB_DEXUATCollection.GetByID(ID);

            if (objTB_DEXUAT != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_DEXUAT.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_DEXUAT, item, new string[] { "id" });
                objTB_DEXUAT.NGAY_SUA = DateTime.Now;
                _db.TB_DEXUATCollection.Update(objTB_DEXUAT);

                var lstTB_MUONCT = _db.TB_DEXUATCTCollection.GetByDEXUAT_ID(ID);
                if (lstTB_MUONCT.Count() > 0) _db.TB_DEXUATCTCollection.Remove(lstTB_MUONCT);
                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_DEXUATCT> lstTB_MUON = new List<TB_DEXUATCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                        
                        var insertCT = objCT.ToObject<TB_DEXUATCT>();
                        insertCT.ID = 0;
                        insertCT.PHIEU_ID = ID;
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_DEXUATCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Sua, objTB_DEXUAT.ID);
            }
            return new NoContentResult();
        }
    }
}
