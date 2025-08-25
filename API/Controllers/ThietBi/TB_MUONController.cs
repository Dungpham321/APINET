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
    [Route("api/ThietBi/TB_MUON")]
    [ApiController]
    public class TB_MUONController : BaseController
    {
        public TB_MUONController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_MUON;
        }

        public static List<QUYEN> Permission()
        {
            TB_MUONController mn = new TB_MUONController(null, null, null);
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
                if (IsQuanTriHeThong || IsQuanTriDonVi)
                {
                    return ListAll(_db.TB_MUONCollection.Get(DVQL_ID, DVSD_ID));
                }
                else
                {
                    return ListAll(_db.TB_MUONCollection.Get(DVQL_ID, DVSD_ID).Where(c => c.CANBO_ID == CANBO_ID));
                }
            }
            else if(op == "thietbi")
            {
                long MUON_ID = ConvertClass.ToLong(Request.Query["MUON_ID"] + "", 0);
                var lstMUONCT = _db.TB_MUONCTCollection.GetByMUON_ID(MUON_ID);
                if (lstMUONCT != null) { 
                    var lstTHIETBI_IDS = lstMUONCT.Select(x => x.THIETBI_ID.Value).ToList();
                    var dsThietBi = _db.TB_THIETBICollection.GetByIDS(lstTHIETBI_IDS);
                    return ObjectResult(dsThietBi.Select(e => new { e.ID, e.MA, e.TEN, e.TT_SUDUNG, e.GHI_CHU }));
                }
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
                var itemNew = item.ToObject<TB_MUON>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                _db.TB_MUONCollection.Add(itemNew);

                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_MUONCT> lstTB_MUON = new List<TB_MUONCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                      
                        var insertCT = objCT.ToObject<TB_MUONCT>();
                        insertCT.ID = 0;
                        insertCT.MUON_ID = itemNew.ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_MUONCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Them, itemNew.ID, "Đăng ký mượn thiết bị "+ itemNew.ID);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    var obgTB_MUON= _db.TB_MUONCollection.GetByID(id.Value<long>());
                    _db.TB_MUONCTCollection.RemoveByMUON_ID(obgTB_MUON.ID);
                    _db.TB_MUONCollection.Remove(obgTB_MUON);
                    HtLog(Quyen.Xoa, obgTB_MUON.ID);
                }
            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {

            var objTB_MUON = _db.TB_MUONCollection.GetByID(ID);

            if (objTB_MUON != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_MUON.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_MUON, item, new string[] { "id canbo_id" });
                objTB_MUON.NGAY_SUA = DateTime.Now;
                _db.TB_MUONCollection.Update(objTB_MUON);
                var lstTB_MUONCT = _db.TB_MUONCTCollection.GetByMUON_ID(ID);
                if(lstTB_MUONCT.Count() > 0) _db.TB_MUONCTCollection.Remove(lstTB_MUONCT);
                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_MUONCT> lstTB_MUON = new List<TB_MUONCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                        
                        var insertCT = objCT.ToObject<TB_MUONCT>();
                        insertCT.ID = 0;
                        insertCT.MUON_ID = ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_MUONCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Sua, objTB_MUON.ID);
            }
            return new NoContentResult();
        }
    }
}
