using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Controllers.ThietBi
{
    [Consumes("application/json")]
    [Route("api/ThietBi/TB_KIEMKE")]
    [ApiController]
    public class TB_KIEMKEController : BaseController
    {
        public TB_KIEMKEController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_KIEMKE;
        }

        public static List<QUYEN> Permission()
        {
            TB_KIEMKEController mn = new TB_KIEMKEController(null, null, null);
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
                return ListAll(_db.TB_KIEMKECollection.Get(DVQL_ID, DVSD_ID));
            }
           
            else if (op == "ThongTinForm")
            {
                long BBKK_ID = ConvertClass.ToLong(Request.Query["ID"] + "", 0);
                TB_KIEMKE objTB_KIEMKE = _db.TB_KIEMKECollection.GetByID(BBKK_ID);
                List<TB_KIEMKECT> CTBBKK = new List<TB_KIEMKECT>();
                if (objTB_KIEMKE == null)
                {
                    var lstTB_THIETBI = _db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID).Where(c => c.TINH_TRANG == (int)TinhTrangLuuTru.TRONG_KHO && (!c.DATMAT.HasValue || (c.DATMAT.HasValue && !c.DATMAT.Value)) && (!c.DATTL.HasValue || (c.DATTL.HasValue && !c.DATTL.Value)) && c.NGAY_TAO <= DateTime.Now);
                    CTBBKK = lstTB_THIETBI.Select(s => new TB_KIEMKECT { THIETBI_ID = s.ID, TRUOC_KK_HONG = (!s.DATHONG.HasValue || (s.DATHONG.HasValue && !s.DATHONG.Value)) ? null : 1, SAU_KK_HONG = (!s.DATHONG.HasValue || (s.DATHONG.HasValue && !s.DATHONG.Value) ? null : 1) }).ToList();
                    foreach (var objCTBBKK in CTBBKK)
                    {
                        objCTBBKK.SO_LUONG = 1;
                        objCTBBKK.CON_DUNG = objCTBBKK.SAU_KK_HONG.HasValue ? objCTBBKK.SO_LUONG - objCTBBKK.SAU_KK_HONG : objCTBBKK.SO_LUONG;
                    }
                }
                else
                {
                    CTBBKK = _db.TB_KIEMKECTCollection.GetByKIEMKE_ID(BBKK_ID).ToList();
                }
                return ObjectResult(new { CTBBKK });
            }
            else if (op == "Check")
            {
                string SO_BIEN_BAN = Request.Query["ID"];
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (SO_BIEN_BAN + "" == "") return new BadRequestResult();
                TB_KIEMKE objTB_KIEMKE = _db.TB_KIEMKECollection.GetByMA_BB(SO_BIEN_BAN, DVSD_ID);
                return ObjectResult(new { Check = objTB_KIEMKE != null && objTB_KIEMKE.ID != OldID });
            }
            else if (op == "ListTB")
            {
                return ListAll(_db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID));
            }
            else if (op == "ListCTBBKK")
            {
                DateTime ngaykk = ConvertClass.ToDateTime(Request.Query["NGAY_KK"] + "", DateTime.Now);
                
                List<TB_KIEMKECT> CTBBKK = new List<TB_KIEMKECT>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID).Where(c => (!c.DATMAT.HasValue || (c.DATMAT.HasValue && !c.DATMAT.Value)) && c.NGAY_TAO <= ngaykk);
                if(Request.Query["KHOKK"] + "" != "")
                {
                    var lstKHOKK = (Request.Query["KHOKK"] + "").Split(";").Select(Int64.Parse).ToList();
                    lstTB_THIETBI = lstTB_THIETBI.Where(c => c.KHO_ID.HasValue && lstKHOKK.Contains(c.KHO_ID.Value));
                }
                CTBBKK = lstTB_THIETBI.Select(s => new TB_KIEMKECT { THIETBI_ID = s.ID }).ToList();
                foreach (var objCTBBKK in CTBBKK)
                {
                    objCTBBKK.CON_DUNG = 1;
                    objCTBBKK.SO_LUONG = 1;
                }
                return ObjectResult(CTBBKK);
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
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        var itemNew = item.ToObject<TB_KIEMKE>();
                        itemNew.NGAY_TAO = DateTime.Now;
                        itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                        itemNew.DVQL_ID = DVQL_ID;
                        itemNew.DVSD_ID = DVSD_ID;
                        _db.TB_KIEMKECollection.Add(itemNew);

                        if (item["CTBBKK"].Count() != 0)
                        {
                            var CTBBKK = JsonConvert.DeserializeObject<JArray>(item["CTBBKK"] + "");
                            List<TB_KIEMKECT> lstTB_KIEMKECT = new List<TB_KIEMKECT>();
                            foreach (var objCT in CTBBKK)
                            {
                                var insertCT = objCT.ToObject<TB_KIEMKECT>();
                                insertCT.ID = 0;
                                insertCT.KIEMKE_ID = itemNew.ID;
                                lstTB_KIEMKECT.Add(insertCT);
                            }
                            _db.TB_KIEMKECTCollection.Add(lstTB_KIEMKECT);
                        }

                        HtLog(Quyen.Them, itemNew.ID, "Thêm biên bản kiêm kê thiết bị " + itemNew.ID);
                        //---
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in jItem)
                        {
                            var objMUONTRA = _db.TB_KIEMKECollection.GetByID(id.Value<long>());
                            _db.TB_KIEMKECTCollection.RemoveByKIEMKE_ID(objMUONTRA.ID);
                            _db.TB_KIEMKECollection.Remove(objMUONTRA);
                            HtLog(Quyen.Xoa, objMUONTRA.ID);

                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        dbContextTransaction.Rollback();
                    }
                }
            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {

            var objTB_KIEMKE = _db.TB_KIEMKECollection.GetByID(ID);

            if (objTB_KIEMKE != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_KIEMKE.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_KIEMKE, item, new string[] { "id" });
                objTB_KIEMKE.NGAY_SUA = DateTime.Now;
                _db.TB_KIEMKECollection.Update(objTB_KIEMKE);

                _db.TB_KIEMKECTCollection.RemoveByKIEMKE_ID(objTB_KIEMKE.ID);
                if (item["CTBBKK"].Count() != 0)
                {
                    var CTBBKK = JsonConvert.DeserializeObject<JArray>(item["CTBBKK"] + "");
                    List<TB_KIEMKECT> lstTB_KIEMKECT = new List<TB_KIEMKECT>();
                    foreach (var objCT in CTBBKK)
                    {
                        var insertCT = objCT.ToObject<TB_KIEMKECT>();
                        insertCT.ID = 0;
                        insertCT.KIEMKE_ID = ID;
                        lstTB_KIEMKECT.Add(insertCT);
                    }
                    _db.TB_KIEMKECTCollection.Add(lstTB_KIEMKECT);

                }

                HtLog(Quyen.Sua, objTB_KIEMKE.ID);
            }
            return new NoContentResult();
        }
    }
}
