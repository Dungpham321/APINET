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
    [Route("api/ThietBi/TB_BANGIAO")]
    [ApiController]
    public class TB_BANGIAOController : BaseController
    {
        public TB_BANGIAOController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_BANGIAO;
        }

        public static List<QUYEN> Permission()
        {
            TB_BANGIAOController mn = new TB_BANGIAOController(null, null, null);
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
                int LoaiBB = ConvertClass.ToInt(Request.Query["LOAI_BB"] + "", 0);

                var lstTB_BANGIAO = _db.TB_BANGIAOCollection.Get(DVQL_ID, DVSD_ID);
                return ListAll(lstTB_BANGIAO.Where(e => e.LOAIBG == LoaiBB));
            }
            else if (op == "thietbi")
            {
                long BANGIAO_ID = ConvertClass.ToLong(Request.Query["BANGIAO_ID"] + "", 0);
                var lstMUONTRA_CT = _db.TB_BANGIAOCTCollection.GetInfo(BANGIAO_ID);
                return ObjectResult(lstMUONTRA_CT);
                //}
            }
            else if (op == "ThongTinForm")
            {
                long BANGIAO_ID = ConvertClass.ToLong(Request.Query["ID"] + "", 0);
                TB_BANGIAO objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(BANGIAO_ID);
                return ObjectResult(new
                {
                    HOIDONG = _db.TB_BANGIAOHDCollection.GetByBANGIAO_ID(BANGIAO_ID),
                });
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
                        var itemNew = item.ToObject<TB_BANGIAO>();
                        itemNew.NGAY_TAO = DateTime.Now;
                        itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                        itemNew.DVQL_ID = DVQL_ID;
                        itemNew.DVSD_ID = DVSD_ID;
                        _db.TB_BANGIAOCollection.Add(itemNew);

                        if (item["TB_MUONCT"].Count() != 0)
                        {
                            var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                            List<TB_BANGIAOCT> lstTB_MUON = new List<TB_BANGIAOCT>();
                            foreach (var objCT in TB_MUONCT)
                            {

                                var insertCT = objCT.ToObject<TB_BANGIAOCT>();
                                insertCT.ID = 0;
                                insertCT.BANGIAO_ID = itemNew.ID;
                                insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                                insertCT.TINH_TRANG = objCT["TINH_TRANG"] + "";
                                insertCT.GHI_CHU = objCT["GHI_CHU"] + "";
                                lstTB_MUON.Add(insertCT);

                                //check trang thai thiet bi
                                if(itemNew.LOAIBG == (int)LoaiBanGiao.ThanhLy)
                                {
                                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                                    if (!objTB_THIETBI.DATTL.HasValue || (objTB_THIETBI.DATTL.HasValue && !objTB_THIETBI.DATTL.Value))
                                    {
                                        objTB_THIETBI.DATTL = true;
                                        objTB_THIETBI.NGAYTL = itemNew.NGAY_BG;
                                        objTB_THIETBI.GHICHUTL = insertCT.GHI_CHU;
                                        _db.TB_THIETBICollection.Update(objTB_THIETBI);
                                    }
                                }
                            }
                            _db.TB_BANGIAOCTCollection.Add(lstTB_MUON);

                            var lstTB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(itemNew.ID);
                            var objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(itemNew.ID);
                            objTB_BANGIAO.SO_TIEN = lstTB_BANGIAOCT.Sum(e => e.THANH_TIEN);
                            _db.TB_BANGIAOCollection.Update(objTB_BANGIAO);

                        }

                        if (item["HOIDONG"] + "" != "" && item["HOIDONG"].Count() != 0)
                        {
                            var HOIDONG = JsonConvert.DeserializeObject<JArray>(item["HOIDONG"] + "");
                            List<TB_BANGIAOHD> lstTB_BANGIAOHD = new List<TB_BANGIAOHD>();

                            foreach (var objHOI_DONG in HOIDONG)
                            {
                                var insertHOIDONG = objHOI_DONG.ToObject<TB_BANGIAOHD>();
                                insertHOIDONG.ID = 0;
                                insertHOIDONG.BANGIAO_ID = itemNew.ID;
                                lstTB_BANGIAOHD.Add(insertHOIDONG);
                            }
                            _db.TB_BANGIAOHDCollection.Add(lstTB_BANGIAOHD);

                        }

                        HtLog(Quyen.Them, itemNew.ID, "Biên bản bàn giao thiết bị " + itemNew.ID);

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
                foreach (var id in jItem)
                {
                    var objMUONTRA = _db.TB_BANGIAOCollection.GetByID(id.Value<long>());
                    _db.TB_BANGIAOCTCollection.RemoveByBANGIAO_ID(objMUONTRA.ID);
                    _db.TB_BANGIAOHDCollection.RemoveByBANGIAO_ID(objMUONTRA.ID);
                    _db.TB_BANGIAOCollection.Remove(objMUONTRA);
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

            var objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(ID);

            if (objTB_BANGIAO != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_BANGIAO.DVSD_ID != DVSD_ID) return UserAccessDenied();

                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {

                        UpdateObject(objTB_BANGIAO, item, new string[] { "id" });
                        objTB_BANGIAO.NGAY_SUA = DateTime.Now;

                        var lstTB_MUONCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(ID).ToList();
                        if (lstTB_MUONCT.Count() > 0)
                        {

                            if (objTB_BANGIAO.LOAIBG == (int)LoaiBanGiao.ThanhLy)
                            {
                                foreach (var objTB_MUONCT in lstTB_MUONCT)
                                {
                                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_MUONCT.THIETBI_ID.Value);
                                    if (objTB_THIETBI.DATTL.HasValue && objTB_THIETBI.DATTL.Value)
                                    {
                                        objTB_THIETBI.DATTL = false;
                                        objTB_THIETBI.NGAYTL = null;
                                        objTB_THIETBI.GHICHUTL = null;
                                        _db.TB_THIETBICollection.Update(objTB_THIETBI);
                                    }
                                }
                            }
                            _db.TB_BANGIAOCTCollection.Remove(lstTB_MUONCT);
                        }
                        if (item["TB_MUONCT"].Count() != 0)
                        {
                            var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                            List<TB_BANGIAOCT> lstTB_MUON = new List<TB_BANGIAOCT>();
                            foreach (var objCT in TB_MUONCT)
                            {
                                if (objTB_BANGIAO.LOAIBG == 1) objCT["ID"] = 0;
                                var insertCT = objCT.ToObject<TB_BANGIAOCT>();
                                insertCT.ID = 0;
                                insertCT.BANGIAO_ID = ID;
                                insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                                insertCT.TINH_TRANG = objCT["TINH_TRANG"] + "";
                                insertCT.GHI_CHU = objCT["GHI_CHU"] + "";
                                lstTB_MUON.Add(insertCT);
                                //check trang thai thiet bi
                                if (objTB_BANGIAO.LOAIBG == (int)LoaiBanGiao.ThanhLy)
                                {
                                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                                    if (!objTB_THIETBI.DATTL.HasValue || (objTB_THIETBI.DATTL.HasValue && !objTB_THIETBI.DATTL.Value))
                                    {
                                        objTB_THIETBI.DATTL = true;
                                        objTB_THIETBI.NGAYTL = objTB_BANGIAO.NGAY_BG;
                                        objTB_THIETBI.GHICHUTL = insertCT.GHI_CHU;
                                        _db.TB_THIETBICollection.Update(objTB_THIETBI);
                                    }
                                }
                            }
                            _db.TB_BANGIAOCTCollection.Add(lstTB_MUON);
                            var lstTB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(ID);

                            objTB_BANGIAO.SO_TIEN = lstTB_BANGIAOCT.Sum(e => e.THANH_TIEN);
                            _db.TB_BANGIAOCollection.Update(objTB_BANGIAO);
                        }

                        var lstTB_MUONHD = _db.TB_BANGIAOHDCollection.GetByBANGIAO_ID(ID);
                        if (lstTB_MUONHD.Count() > 0) _db.TB_BANGIAOHDCollection.Remove(lstTB_MUONHD);
                        if (item["HOIDONG"] + "" != "" && item["HOIDONG"].Count() != 0)
                        {
                            var HOIDONG = JsonConvert.DeserializeObject<JArray>(item["HOIDONG"] + "");
                            foreach (var objHOI_DONG in HOIDONG)
                            {
                                var insertHOIDONG = objHOI_DONG.ToObject<TB_BANGIAOHD>();
                                insertHOIDONG.ID = 0;
                                insertHOIDONG.BANGIAO_ID = ID;
                                _db.TB_BANGIAOHDCollection.Add(insertHOIDONG);
                            }
                        }

                        HtLog(Quyen.Sua, objTB_BANGIAO.ID);

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
    }
}
