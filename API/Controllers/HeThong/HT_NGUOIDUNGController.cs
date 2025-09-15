using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using GoogleAuthenticatorService.Core;
using GCommon;
using System.Linq;
using System.Security.Cryptography;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_NGUOIDUNG")]
    [ApiController]
    public class HT_NGUOIDUNGController : BaseController
    {
        public HT_NGUOIDUNGController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_NGUOIDUNG;
        }

        public static List<QUYEN> Permission()
        {
            HT_NGUOIDUNGController mn = new HT_NGUOIDUNGController(null);
            return mn.QuyenCoBan(Quyen.Xem, Quyen.Sua, Quyen.Them, Quyen.Xoa, Quyen.DatLaiMatKhau, Quyen.MoKhoa, Quyen.Khoa, Quyen.GanDonVi, Quyen.SuaThongTin);
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                _db.HT_NGUOIDUNG_SDCollection.RemoveByNguoiDung(0, CurrentUser.ID);
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.Xem),
                    New = UserAccess(Quyen.Them),
                    Edit = UserAccess(Quyen.Sua),
                    Delete = UserAccess(Quyen.Xoa),
                    Reset = UserAccess(Quyen.DatLaiMatKhau),
                    Grant = UserAccess(Quyen.GanDonVi),
                    MoKhoa = UserAccess(Quyen.MoKhoa),
                    Khoa = UserAccess(Quyen.Khoa),
                    Title = CurrentMenu?.NAME + "",
                    VAI_TRO = IntVaiTro
                });
            }
            else if (op == "AccessEdit")
            {
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                var setupInfo = tfa.GenerateSetupCode(HT_CAUHINH_Get<string>("TS_TEN_APP", "QLLT"), CurrentUser.EMAIL + "", CurrentUser.TEN_DANG_NHAP + "QLHTNV2BUOC", 150, 150);
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.SuaThongTin),
                    Data = CurrentUser,
                    TFA_IMAGE = setupInfo.QrCodeSetupImageUrl,
                    TFA_CODE = setupInfo.ManualEntryKey,
                });
            }
            else if (op == "List")
            {
                if (IsQuanTriHeThong)
                {
                    return ListAll(_db.HT_NGUOIDUNGCollection.Get(DVQL_ID));
                }
                return ListAll(GetDsHT_NGUOIDUNG());
            }
            else if (op == "Check")
            {
                string TEN_DANG_NHAP = Request.Query["ID"] + "";
                long NGUOIDUNG_ID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (TEN_DANG_NHAP + "" == "") return new BadRequestResult();
                HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByTEN_DANG_NHAP(TEN_DANG_NHAP, DVQL_ID);
                bool Check = false;
                if (u != null && u.ID != NGUOIDUNG_ID) Check = true;
                return ObjectResult(new { Check });
            }
            else if (op == "CheckOldPass")
            {
                string password = Request.Query["MAT_KHAU"] + "";
                if (password + "" == "") return new BadRequestResult();
                HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByID(CurrentUser.ID);
                bool check = u.MAT_KHAU == password.EncodePassword();
                return ObjectResult(new { check });
            }
            else if (op == "CheckEmail")
            {
                string EMAIL = Request.Query["ID"] + "";
                long NGUOIDUNG_ID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (EMAIL + "" == "") return new BadRequestResult();
                HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByEMAIL(EMAIL, DVQL_ID);
                bool Check = false;
                if (u != null && u.ID != NGUOIDUNG_ID) Check = true;
                return ObjectResult(new { Check });
            }
            else if (op == "CheckDienThoai")
            {
                string DIEN_THOAI = Request.Query["ID"] + "";
                long NGUOIDUNG_ID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (DIEN_THOAI + "" == "") return new BadRequestResult();
                HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByDIEN_THOAI(DIEN_THOAI, DVQL_ID);
                bool Check = false;
                if (u != null && u.ID != NGUOIDUNG_ID) Check = true;
                return ObjectResult(new { Check });
            }
            return new BadRequestResult();
        }

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
                        var objHT_NGUOIDUNG = item.ToObject<HT_NGUOIDUNG>();
                        objHT_NGUOIDUNG.MAT_KHAU = objHT_NGUOIDUNG.MAT_KHAU.EncodePassword();
                        objHT_NGUOIDUNG.NGAY_TAO = DateTime.Now;
                        //objHT_NGUOIDUNG.TRANG_THAI = (int)TrangThai.CHO_DUYET;
                        objHT_NGUOIDUNG.SAI_MAT_KHAU = 0;
                        if (!objHT_NGUOIDUNG.TFA.HasValue) objHT_NGUOIDUNG.TFA = false;
                        objHT_NGUOIDUNG.NGAY_MAT_KHAU = DateTime.Now;
                        objHT_NGUOIDUNG.DVQL_ID = DVQL_ID;
                        objHT_NGUOIDUNG.DVSD_ID = DVSD_ID;
                        objHT_NGUOIDUNG.NGUOIDUNG_ID = CurrentUser.ID;
                        _db.HT_NGUOIDUNGCollection.Add(objHT_NGUOIDUNG);


                        _db.HT_NGUOIDUNG_SDCollection.UpdateByNguoiDung(objHT_NGUOIDUNG.ID, DsDoiTuong.DM_DANHMUC, CurrentUser.ID);
                        if (!UserAccess(Quyen.GanDonVi))
                        {
                            _db.HT_NGUOIDUNG_SDCollection.Add(new HT_NGUOIDUNG_SD { NGUOIDUNG_ID = objHT_NGUOIDUNG.ID, DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC, CHUCNANG = DsChucNang.DonVi.ToString(), DOITUONG_ID = DVSD_ID, ND_ID = CurrentUser.ID });
                        }
                        //else
                        //{
                        //    _db.HT_NGUOIDUNG_SDCollection.UpdateByNguoiDung(objHT_NGUOIDUNG.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString(), CurrentUser.ID);
                        //}

                        //check dong vi
                        var checkDV = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(objHT_NGUOIDUNG.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString()).ToList();
                        if (checkDV.Count == 0)
                        {
                            _db.HT_NGUOIDUNG_SDCollection.Add(new HT_NGUOIDUNG_SD { NGUOIDUNG_ID = objHT_NGUOIDUNG.ID, DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC, CHUCNANG = DsChucNang.DonVi.ToString(), DOITUONG_ID = DVSD_ID, ND_ID = CurrentUser.ID });
                        }
                        else
                        {
                            if (!checkDV.Select(s => s.DOITUONG_ID).Contains(objHT_NGUOIDUNG.DVSD_ID))
                            {
                                objHT_NGUOIDUNG.DVSD_ID = checkDV.FirstOrDefault().DOITUONG_ID;
                                _db.HT_NGUOIDUNGCollection.Update(objHT_NGUOIDUNG);
                            }
                        }
                        //nhom quyen
                        if (item["RID"] + "" != "")
                        {
                            if (item["RID"].Type == JTokenType.Array)
                            {
                                foreach (var rid in item["RID"])
                                {
                                    HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = new HT_NGUOIDUNG_SD();
                                    objHT_NGUOIDUNG_SD.NGUOIDUNG_ID = objHT_NGUOIDUNG.ID;
                                    objHT_NGUOIDUNG_SD.DOITUONG_ID = rid.Value<long>();
                                    objHT_NGUOIDUNG_SD.DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC;
                                    objHT_NGUOIDUNG_SD.CHUCNANG = DsChucNang.NhomQuyen.ToString();
                                    objHT_NGUOIDUNG_SD.ND_ID = NGUOIDUNG_ID;
                                    _db.HT_NGUOIDUNG_SDCollection.Add(objHT_NGUOIDUNG_SD);
                                }
                            }
                            else
                            {
                                HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = new HT_NGUOIDUNG_SD();
                                objHT_NGUOIDUNG_SD.NGUOIDUNG_ID = objHT_NGUOIDUNG.ID;
                                objHT_NGUOIDUNG_SD.DOITUONG_ID = item["RID"].Value<long>();
                                objHT_NGUOIDUNG_SD.DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC;
                                objHT_NGUOIDUNG_SD.CHUCNANG = DsChucNang.NhomQuyen.ToString();
                                objHT_NGUOIDUNG_SD.ND_ID = NGUOIDUNG_ID;
                                _db.HT_NGUOIDUNG_SDCollection.Add(objHT_NGUOIDUNG_SD);
                            }
                        };
                        //log
                        HtLog(Quyen.Them, objHT_NGUOIDUNG.TEN_DANG_NHAP);
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
                foreach (var uid in jItem)
                {
                    HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByID(uid.Value<long>());
                    if (!IsAdmin && u.NGUOIDUNG_ID.HasValue && u.NGUOIDUNG_ID.Value == 1) continue;
                    if (u.TEN_DANG_NHAP.ToLower() == CurrentUser.TEN_DANG_NHAP.ToLower() || u.TEN_DANG_NHAP.ToLower() == "khach" || u.TEN_DANG_NHAP.ToLower() == "superadmin" || u.TEN_DANG_NHAP.ToLower() == "admin") continue;
                    _db.HT_NGUOIDUNGCollection.Remove(u);
                    HtLog(Quyen.Xoa, u.TEN_DANG_NHAP);
                }
            }
            else if (op == "TrangThai")
            {
                if (!UserAccess(Quyen.Khoa) && !UserAccess(Quyen.MoKhoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var uid in jItem)
                {
                    HT_NGUOIDUNG u = _db.HT_NGUOIDUNGCollection.GetByID(uid.Value<long>());
                    if (!IsAdmin && u.NGUOIDUNG_ID.HasValue && u.NGUOIDUNG_ID.Value == 1) continue;
                    if (u.TEN_DANG_NHAP.ToLower() == CurrentUser.TEN_DANG_NHAP.ToLower() || u.TEN_DANG_NHAP.ToLower() == "khach" || u.TEN_DANG_NHAP.ToLower() == "superadmin" || u.TEN_DANG_NHAP.ToLower() == "admin") continue;
                    u.TRANG_THAI = item.GetValue("trangthai").Value<int>();
                    _db.HT_NGUOIDUNGCollection.Update(u);
                    if (u.TRANG_THAI == (int)TrangThai.DA_DUYET)
                    {
                        HtLog(Quyen.MoKhoa, u.TEN_DANG_NHAP);
                    }
                    else
                    {
                        HtLog(Quyen.Khoa, u.TEN_DANG_NHAP);
                    }

                }
            }
            else if (op == "Reset")
            {
                if (!UserAccess(Quyen.DatLaiMatKhau)) return UserAccessDenied();
                long uid = item.GetValue("ID").Value<long>();
                string defPass = HT_CAUHINH_Get<string>("DefaultPassword", "111111");
                var oldItem = _db.HT_NGUOIDUNGCollection.GetByID(uid);
                if (oldItem != null)
                {
                    oldItem.MAT_KHAU = defPass.EncodePassword();
                    _db.HT_NGUOIDUNGCollection.Update(oldItem);
                    HtLog(Quyen.DatLaiMatKhau, oldItem.TEN_DANG_NHAP);
                }
            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{NGUOIDUNG_ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long NGUOIDUNG_ID, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
            var oldHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.GetByID(NGUOIDUNG_ID);
            if (oldHT_NGUOIDUNG != null)
            {
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        UpdateObject(oldHT_NGUOIDUNG, item, new string[] { "id", "ten_dang_nhap" });//, 
                        oldHT_NGUOIDUNG.NGAY_SUA = DateTime.Now;
                        _db.HT_NGUOIDUNGCollection.Update(oldHT_NGUOIDUNG);

                        var checkDV = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(oldHT_NGUOIDUNG.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString()).ToList();
                        if (checkDV.Count == 0)
                        {
                            _db.HT_NGUOIDUNG_SDCollection.Add(new HT_NGUOIDUNG_SD { NGUOIDUNG_ID = oldHT_NGUOIDUNG.ID, DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC, CHUCNANG = DsChucNang.DonVi.ToString(), DOITUONG_ID = DVSD_ID, ND_ID = CurrentUser.ID });
                        }
                        else
                        {
                            if (!checkDV.Select(s => s.DOITUONG_ID).Contains(oldHT_NGUOIDUNG.DVSD_ID))
                            {
                                oldHT_NGUOIDUNG.DVSD_ID = checkDV.FirstOrDefault().DOITUONG_ID;
                                _db.HT_NGUOIDUNGCollection.Update(oldHT_NGUOIDUNG);
                            }
                        }
                        //nhom quyen
                        _db.HT_NGUOIDUNG_SDCollection.RemoveByNguoiDung(oldHT_NGUOIDUNG.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString());
                        if (item["RID"] + "" != "")
                        {
                            if(item["RID"].Type == JTokenType.Array)
                            {
                                foreach (var rid in item["RID"])
                                {
                                    HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = new HT_NGUOIDUNG_SD();
                                    objHT_NGUOIDUNG_SD.NGUOIDUNG_ID = oldHT_NGUOIDUNG.ID;
                                    objHT_NGUOIDUNG_SD.DOITUONG_ID = rid.Value<long>();
                                    objHT_NGUOIDUNG_SD.DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC;
                                    objHT_NGUOIDUNG_SD.CHUCNANG = DsChucNang.NhomQuyen.ToString();
                                    objHT_NGUOIDUNG_SD.ND_ID = NGUOIDUNG_ID;
                                    _db.HT_NGUOIDUNG_SDCollection.Add(objHT_NGUOIDUNG_SD);
                                }
                            }
                            else
                            {
                                HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = new HT_NGUOIDUNG_SD();
                                objHT_NGUOIDUNG_SD.NGUOIDUNG_ID = oldHT_NGUOIDUNG.ID;
                                objHT_NGUOIDUNG_SD.DOITUONG_ID = item["RID"].Value<long>();
                                objHT_NGUOIDUNG_SD.DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC;
                                objHT_NGUOIDUNG_SD.CHUCNANG = DsChucNang.NhomQuyen.ToString();
                                objHT_NGUOIDUNG_SD.ND_ID = NGUOIDUNG_ID;
                                _db.HT_NGUOIDUNG_SDCollection.Add(objHT_NGUOIDUNG_SD);
                            }
                            
                        };
                        HtLog(Quyen.Sua, oldHT_NGUOIDUNG.TEN_DANG_NHAP);
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

        //Pass
        [HttpPut("Pass")]
        [Authorize("Bearer")]
        public IActionResult PutPass([FromBody] JObject item)
        {
            if (!UserAccess(Quyen.SuaThongTin)) return UserAccessDenied();
            HT_NGUOIDUNG objHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.GetByID(CurrentUser.ID);
            foreach (var p in item)
            {
                if (p.Key.ToLower() == "matkhaucu")
                {
                    if (objHT_NGUOIDUNG.MAT_KHAU != p.Value.Value<string>().EncodePassword()) break;
                    continue;
                }
                if (p.Key.ToLower() == "matkhaumoi")
                {
                    var po = objHT_NGUOIDUNG.GetType().GetProperty("MAT_KHAU");
                    po.SetValue(objHT_NGUOIDUNG, p.Value.Value<string>().EncodePassword());
                }
                
            }
            objHT_NGUOIDUNG.NGAY_MAT_KHAU = DateTime.Now;
            _db.HT_NGUOIDUNGCollection.Update(objHT_NGUOIDUNG);
            HtLog(Quyen.DoiMatKhau, objHT_NGUOIDUNG.TEN_DANG_NHAP);
            return new NoContentResult();
        }

        //edit
        [HttpPut("Edit")]
        [Authorize("Bearer")]
        public IActionResult PutEdit([FromBody] JObject item)
        {
            if (!UserAccess(Quyen.SuaThongTin)) return UserAccessDenied();
            HT_NGUOIDUNG objHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.GetByID(CurrentUser.ID);
            UpdateObject(objHT_NGUOIDUNG, item, new string[] { "id", "ten_dang_nhap" });
            objHT_NGUOIDUNG.NGAY_SUA = DateTime.Now;
            _db.HT_NGUOIDUNGCollection.Update(objHT_NGUOIDUNG);
            HtLog(Quyen.SuaThongTin, objHT_NGUOIDUNG.TEN_DANG_NHAP);
            return new NoContentResult();
        }
    }
}
