using API.Common;
using GDB;
using GCommon;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers.ThietBi
{
    [Consumes("application/json")]
    [Route("api/ThietBi/TB_CHUONGTRINH")]
    [ApiController]
    public class TB_CHUONGTRINHController : BaseController
    {
        public TB_CHUONGTRINHController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_CHUONGTRINH;
        }

        public static List<QUYEN> Permission()
        {
            TB_CHUONGTRINHController mn = new TB_CHUONGTRINHController(null, null, null);
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
                return ListAll(_db.TB_CHUONGTRINHCollection.Get(DVQL_ID, DVSD_ID));
            }else if(op == "chonchuongtrinh")
            {
                var lstChuongTrinh = _db.TB_CHUONGTRINHCollection.Get(DVQL_ID, DVSD_ID);
                return ListAll(lstChuongTrinh.Where(e => e.TRANG_THAI == 2));
            }
            else if (op == "thietbi")
            {
                long CHUONGTRINH_ID = ConvertClass.ToLong(Request.Query["CHUONGTRINH_ID"] + "", 0);
                var lstMUONTRA_CT = _db.TB_CHUONGTRINHCTCollection.GetInfo(CHUONGTRINH_ID);
                return ObjectResult(lstMUONTRA_CT);
            }else if(op == "thietbichon")
            {
                long CHUONGTRINH_ID = ConvertClass.ToLong(Request.Query["CHUONGTRINH_ID"] + "", 0);
                var lstCHUONGTRINH_CT = _db.TB_CHUONGTRINHCTCollection.GetByCHUONGTRINH_ID(CHUONGTRINH_ID);
                var THIETBI_IDS = lstCHUONGTRINH_CT.Select(e => e.THIETBI_ID.Value).ToList();
                var data = _db.TB_THIETBICollection.GetByIDS(THIETBI_IDS);
                return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.GHI_CHU, CHON = true }));
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
                var itemNew = item.ToObject<TB_CHUONGTRINH>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                _db.TB_CHUONGTRINHCollection.Add(itemNew);

                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_CHUONGTRINHCT> lstTB_MUON = new List<TB_CHUONGTRINHCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                      
                        var insertCT = objCT.ToObject<TB_CHUONGTRINHCT>();
                        insertCT.ID = 0;
                        insertCT.CHUONGTRINH_ID = itemNew.ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_CHUONGTRINHCTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Them, itemNew.ID, "Thêm biên bản kiêm kê thiết bị " + itemNew.ID);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    var objMUONTRA = _db.TB_CHUONGTRINHCollection.GetByID(id.Value<long>());
                    _db.TB_CHUONGTRINHCTCollection.RemoveByCHUONGTRINH_ID(objMUONTRA.ID);
                    _db.TB_CHUONGTRINHCollection.Remove(objMUONTRA);
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

            var objTB_CHUONGTRINH = _db.TB_CHUONGTRINHCollection.GetByID(ID);

            if (objTB_CHUONGTRINH != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_CHUONGTRINH.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_CHUONGTRINH, item, new string[] { "id" });
                objTB_CHUONGTRINH.NGAY_SUA = DateTime.Now;
                _db.TB_CHUONGTRINHCollection.Update(objTB_CHUONGTRINH);

                var lstTB_MUONCT = _db.TB_CHUONGTRINHCTCollection.GetByCHUONGTRINH_ID(ID);
                if (lstTB_MUONCT.Count() > 0) _db.TB_CHUONGTRINHCTCollection.Remove(lstTB_MUONCT);
                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_CHUONGTRINHCT> lstTB_MUON = new List<TB_CHUONGTRINHCT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                        
                        var insertCT = objCT.ToObject<TB_CHUONGTRINHCT>();
                        insertCT.ID = 0;
                        insertCT.CHUONGTRINH_ID = ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_CHUONGTRINHCTCollection.Add(lstTB_MUON);
                   
                }

                HtLog(Quyen.Sua, objTB_CHUONGTRINH.ID);
            }
            return new NoContentResult();
        }
    }
}
