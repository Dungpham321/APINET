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
    [Route("api/ThietBi/TB_MUONTRA")]
    [ApiController]
    public class TB_MUONTRAController : BaseController
    {
        public TB_MUONTRAController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_MUONTRA;
        }

        public static List<QUYEN> Permission()
        {
            TB_MUONTRAController mn = new TB_MUONTRAController(null, null, null);
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
                return ListAll(_db.TB_MUONTRACollection.Get(DVQL_ID, DVSD_ID));
            }else if(op == "thietbi")
            {
                long MUON_ID = ConvertClass.ToLong(Request.Query["MUON_ID"] + "", 0);
                var lstMUONTRA_CT = _db.TB_MUONTRACTCollection.GetInfo(MUON_ID);
                return ObjectResult(lstMUONTRA_CT);
                //}
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
            }else if(op == "Getdangky")
            {
                List<string> mess = new List<string>();
                long DANGKY_ID = ConvertClass.ToLong(Request.Query["DANGKY_ID"] + "", 0);
                var lstDANGKY_CT = _db.TB_MUONCTCollection.GetByMUON_ID(DANGKY_ID);
                var THIETBI_IDS = lstDANGKY_CT.Select(e => e.THIETBI_ID.Value).ToList();
                var checkThietBi = _db.TB_THIETBICollection.GetByIDS(THIETBI_IDS);
                foreach (var check in checkThietBi)
                {
                    if(check.TINH_TRANG == (int)TinhTrangLuuTru.DANG_CHO_MUON) mess.Add("Thiết bị "+ check.MA+" đang được mượn");
                }
                var data = _db.TB_THIETBICollection.GetByIDS(THIETBI_IDS).Where(e=>e.TINH_TRANG == (int)TinhTrangLuuTru.TRONG_KHO && e.PHAN_LOAI == (int)PhanLoai.ThueMuon);
                return ObjectResult( new
                {
                    Mess = mess,
                    Data = data.Select(e => new { e.ID, e.MA, e.TEN, TINH_TRANG = "", e.GHI_CHU, DA_TRA = false })
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
                var itemNew = item.ToObject<TB_MUONTRA>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                _db.TB_MUONTRACollection.Add(itemNew);

                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_MUONTRACT> lstTB_MUON = new List<TB_MUONTRACT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                    
                        var insertCT = objCT.ToObject<TB_MUONTRACT>();
                        insertCT.ID = 0;
                        insertCT.MUONTRA_ID = itemNew.ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        insertCT.TINH_TRANG = objCT["TINH_TRANG"] + "";
                        insertCT.GHI_CHU = objCT["GHI_CHU"] + "";
                        insertCT.DA_TRA = ConvertClass.ToBoolean(objCT["DA_TRA"] + "");
                        if (insertCT.DA_TRA.Value && itemNew.TRANG_THAI == (int)TrangThaiMuonTra.DA_TRA) {
                            var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                            if(objTB_THIETBI != null)
                            {
                                objTB_THIETBI.TINH_TRANG = (int)TinhTrangLuuTru.TRONG_KHO;
                                _db.TB_THIETBICollection.Update(objTB_THIETBI);
                            }
                        }
                        if(!insertCT.DA_TRA.Value && itemNew.TRANG_THAI == (int)TrangThaiMuonTra.DANG_MUON)
                        {
                            var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                            if (objTB_THIETBI != null)
                            {
                                objTB_THIETBI.TINH_TRANG = (int)TinhTrangLuuTru.DANG_CHO_MUON;
                                _db.TB_THIETBICollection.Update(objTB_THIETBI);
                            }
                        }
                        
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_MUONTRACTCollection.Add(lstTB_MUON);
                }

                HtLog(Quyen.Them, itemNew.ID, "Đăng ký mượn thiết bị " + itemNew.ID);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    var objMUONTRA = _db.TB_MUONTRACollection.GetByID(id.Value<long>());
                    _db.TB_MUONTRACTCollection.RemoveGetByMUONTRA(objMUONTRA.ID);
                    _db.TB_MUONTRACollection.Remove(objMUONTRA);
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

            var objTB_MUONTRA = _db.TB_MUONTRACollection.GetByID(ID);

            if (objTB_MUONTRA != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (objTB_MUONTRA.DVSD_ID != DVSD_ID) return UserAccessDenied();

                UpdateObject(objTB_MUONTRA, item, new string[] { "id" });
                objTB_MUONTRA.NGAY_SUA = DateTime.Now;
               
                var lstTB_MUONCT = _db.TB_MUONTRACTCollection.GetByMUONTRA_ID(ID);
                if (lstTB_MUONCT.Count() > 0) _db.TB_MUONTRACTCollection.Remove(lstTB_MUONCT);
                if (item["TB_MUONCT"].Count() != 0)
                {
                    var TB_MUONCT = JsonConvert.DeserializeObject<JArray>(item["TB_MUONCT"] + "");
                    List<TB_MUONTRACT> lstTB_MUON = new List<TB_MUONTRACT>();
                    foreach (var objCT in TB_MUONCT)
                    {
                       
                        var insertCT = objCT.ToObject<TB_MUONTRACT>();
                        insertCT.ID = 0;
                        insertCT.MUONTRA_ID = ID;
                        insertCT.THIETBI_ID = ConvertClass.ToLong(objCT["ID"] + "", 0);
                        insertCT.TINH_TRANG = objCT["TINH_TRANG"] + "";
                        insertCT.GHI_CHU = objCT["GHI_CHU"] + "";
                        insertCT.DA_TRA = ConvertClass.ToBoolean(objCT["DA_TRA"] + "");
                        if (insertCT.DA_TRA.Value)
                        {
                            var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                            if (objTB_THIETBI != null)
                            {
                                objTB_THIETBI.TINH_TRANG = (int)TinhTrangLuuTru.TRONG_KHO;
                                _db.TB_THIETBICollection.Update(objTB_THIETBI);
                            }
                        }
                        if (!insertCT.DA_TRA.Value)
                        {
                            var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(insertCT.THIETBI_ID.Value);
                            if (objTB_THIETBI != null)
                            {
                                objTB_THIETBI.TINH_TRANG = (int)TinhTrangLuuTru.DANG_CHO_MUON;
                                _db.TB_THIETBICollection.Update(objTB_THIETBI);
                            }
                        }
                        lstTB_MUON.Add(insertCT);
                    }
                    _db.TB_MUONTRACTCollection.Add(lstTB_MUON);
                    // check nếu trả hết thì đổi trạng thái
                    var tongsoTB = _db.TB_MUONTRACTCollection.GetByMUONTRA_ID(ID).Count();
                    var tongsoTB_DATRA = _db.TB_MUONTRACTCollection.GetByMUONTRA_ID(ID).Where(e=> e.DA_TRA.Value && e.NGAY_TRA != null).Count();
                    if (tongsoTB == tongsoTB_DATRA) objTB_MUONTRA.TRANG_THAI = (int)TrangThaiMuonTra.DA_TRA;
                    else objTB_MUONTRA.TRANG_THAI = (int)TrangThaiMuonTra.DANG_MUON;
                    _db.TB_MUONTRACollection.Update(objTB_MUONTRA);
                    //
                }

                HtLog(Quyen.Sua, objTB_MUONTRA.ID);
            }
            return new NoContentResult();
        }
    }
}
