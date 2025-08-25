using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace API.Controllers.ThietBi
{
    [Consumes("application/json")]
    [Route("api/ThietBi/TB_IMPORT")]
    [ApiController]
    public class TB_IMPORTController : BaseController
    {
        public TB_IMPORTController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_IMPORT;
        }

        public static List<QUYEN> Permission()
        {
            TB_IMPORTController mn = new TB_IMPORTController(null);
            return mn.QuyenCoBan(Quyen.Xem);
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
                });
            }

            return new BadRequestResult();
        }

        [HttpPost("{op}")]
        [Authorize("Bearer")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "MAU_TAILIEU")
            {
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    Dictionary<string, int> lstTinhTrang = new Dictionary<string, int>();
                    lstTinhTrang.Add("Trong kho", 0);
                    lstTinhTrang.Add("Đang cho mượn", 1);
                    lstTinhTrang.Add("Đã trang cấp", 2);
                    lstTinhTrang.Add("Kho mượn", 3);
                    int stt = 0;
                    int phanloai = item["PHAN_LOAI"].Value<int>();
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            if (joData.Count() > 0)
                            {
                                stt++;
                                //ma thiết bị
                                if (joData["MA"] + "" != "")
                                {
                                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByMa(DVQL_ID, DVSD_ID, joData["MA"] + "");
                                    if (objTB_THIETBI != null)
                                    {
                                        mess.Add("Vi trí " + stt + ": Mã tài liệu đã tồn tại");
                                        continue;
                                    }
                                }
                                else
                                {
                                    mess.Add("Vi trí " + stt + ": Mã tài liệu là thông tin cần nhập");
                                    continue;
                                }
                                //ten thiết bị
                                if (joData["TEN"] + "" == "")
                                {
                                    mess.Add("Vi trí " + stt + ": Tên tài liệu là thông tin cần nhập");
                                    continue;
                                }

                                //if (joData["GIA_MUA"] + "" == "")
                                //{
                                //    mess.Add("Vi trí " + stt + ": Giá mua là thông tin cần nhập");
                                //    continue;
                                //}
                                //if (joData["VAT"] + "" == "")
                                //{
                                //    mess.Add("Vi trí " + stt + ": VAT là thông tin cần nhập");
                                //    continue;
                                //}
                                //if (joData["THANH_TIEN"] + "" == "")
                                //{
                                //    mess.Add("Vi trí " + stt + ": Thành tiền là thông tin cần nhập");
                                //    continue;
                                //}
                                if (joData["NGAY_MUA"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["NGAY_MUA"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["NGAY_SD"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["NGAY_SD"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["HAN_BH"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["HAN_BH"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["HAN_KH"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["HAN_KH"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["NGAYMAT"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["NGAYMAT"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["NGAYHONG"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["NGAYHONG"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                if (joData["NGAYTL"] + "" != "")
                                {
                                    DateTime MinValue = DateTime.MinValue;
                                    DateTime dataValuel;
                                    if (DateTime.TryParse(joData["NGAYTL"] + "", out dataValuel))
                                    {
                                        if (MinValue == dataValuel)
                                        {
                                            mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                            continue;

                                        }

                                    }
                                    else
                                    {
                                        mess.Add("Vi trí " + stt + ": Sai định dạng ngày tháng");
                                        continue;

                                    }
                                }
                                //Tình trạng
                                if (joData["TINH_TRANG"] + "" == "")
                                {
                                    mess.Add("Vi trí " + stt + ": Tình trạng là thông tin cần nhập");
                                    continue;
                                }
                                else
                                {
                                    joData["TINH_TRANG"] = lstTinhTrang[joData["TINH_TRANG"] + ""];
                                }
                                //Kho lưu trữ
                                if (joData["KHO_ID"] + "" != "")
                                {
                                    var objDM_KHOLUUTRU = _db.DM_DANHMUC_ITEMCollection.GetByTEN(joData["KHO_ID"] + "", DsChucNang.KhoLuuTru);
                                    if (objDM_KHOLUUTRU == null)
                                    {
                                        var ItemKhoNew = new DM_DANHMUC_ITEM();
                                        ItemKhoNew.DANHMUC_ID = DsChucNang.KhoLuuTru;
                                        ItemKhoNew.MA = "KHO-" + stt;
                                        ItemKhoNew.TEN = joData["KHO_ID"] + "";
                                        ItemKhoNew.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                        ItemKhoNew.DVQL_ID = DVQL_ID;
                                        ItemKhoNew.DVSD_ID = DVSD_ID;
                                        _db.DM_DANHMUC_ITEMCollection.Add(ItemKhoNew);
                                        joData["KHO_ID"] = ItemKhoNew.ID;
                                    }
                                    else
                                    {
                                        joData["KHO_ID"] = objDM_KHOLUUTRU.ID;
                                    }
                                }

                                //nha xuat ban
                                if (joData["NHACC_ID"] + "" != "")
                                {
                                    var objDM_NHACC = _db.DM_DANHMUC_ITEMCollection.GetByTEN(joData["NHACC_ID"] + "", DsChucNang.NhaCungCap);
                                    if (objDM_NHACC == null)
                                    {
                                        var ItemNew = new DM_DANHMUC_ITEM();
                                        ItemNew.DANHMUC_ID = DsChucNang.NhaCungCap;
                                        ItemNew.TEN = joData["NHACC_ID"] + "";
                                        ItemNew.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                        ItemNew.DVQL_ID = DVQL_ID;
                                        ItemNew.DVSD_ID = DVSD_ID;
                                        _db.DM_DANHMUC_ITEMCollection.Add(ItemNew);
                                        joData["NHACC_ID"] = ItemNew.ID;

                                    }
                                    else
                                    {
                                        joData["NHACC_ID"] = objDM_NHACC.ID;
                                    }
                                }
                                //------------
                                var itemNewTB = joData.ToObject<TB_THIETBI>();
                                itemNewTB.PHAN_LOAI = phanloai;
                                itemNewTB.DVSD_ID = DVSD_ID;
                                itemNewTB.DVQL_ID = DVQL_ID;
                                itemNewTB.NGUOIDUNG_ID = CurrentUser.ID;
                                itemNewTB.NGAY_TAO = DateTime.Now;
                                if (itemNewTB.NGAYMAT != null) itemNewTB.DATMAT = true;
                                else itemNewTB.DATMAT = false;
                                if (itemNewTB.NGAYHONG != null) itemNewTB.DATHONG = true;
                                else itemNewTB.DATHONG = false;
                                if (itemNewTB.NGAYTL != null) itemNewTB.DATTL = true;
                                else itemNewTB.DATTL = false;
                                _db.TB_THIETBICollection.Add(itemNewTB);
                            }

                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message);
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }

                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "donvi")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            stt++;

                            DM_DANHMUC_ITEM donvi = new DM_DANHMUC_ITEM();
                            donvi.DANHMUC_ID = DsChucNang.DonVi;
                            donvi.MA = joData["ma_don_vi"].Value<string>();
                            donvi.TEN = joData["ten_don_vi"].Value<string>();
                            donvi.HETHONG = false;
                            donvi.TRANG_THAI = (int)TrangThai.DA_DUYET;
                            donvi.SAP_XEP = 0;
                            donvi.HASCHILD = false;
                            donvi.PID = 72872;
                            donvi.DVSD_ID = DVSD_ID;
                            donvi.DVQL_ID = DVQL_ID;
                            donvi.DATA = "{}";
                            _db.DM_DANHMUC_ITEMCollection.Add(donvi);

                            var checkDV = _db.TB_IMPORTCollection.Get(op, joData["id_don_vi"].Value<decimal>());
                            if (checkDV != null)
                            {
                                _db.DM_DANHMUC_ITEMCollection.RemoveByID((long)checkDV.ID_MOI.Value);
                                _db.TB_IMPORTCollection.Remove(checkDV);
                            }
                            TB_IMPORT objTB_IMPORT = new TB_IMPORT() { ID_MOI = donvi.ID, LOAI = op, ID_CU = joData["id_don_vi"].Value<decimal>() };
                            //_db.TB_IMPORTCollection.Remove(objTB_IMPORT.LOAI, objTB_IMPORT.ID_CU.Value);
                            _db.TB_IMPORTCollection.Add(objTB_IMPORT);
                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message);
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "nguoidung")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            stt++;
                            HT_NGUOIDUNG objHT_NGUOIDUNG = new HT_NGUOIDUNG();
                            objHT_NGUOIDUNG.TEN_DANG_NHAP = joData["UserName"].Value<string>();
                            objHT_NGUOIDUNG.TEN_DAY_DU = objHT_NGUOIDUNG.TEN_DANG_NHAP;
                            objHT_NGUOIDUNG.EMAIL = joData["Email"].Value<string>();
                            objHT_NGUOIDUNG.MAT_KHAU = ("123456").EncodePassword();
                            objHT_NGUOIDUNG.NGAY_TAO = DateTime.Now;
                            objHT_NGUOIDUNG.TRANG_THAI = (int)TrangThai.DA_DUYET;
                            objHT_NGUOIDUNG.SAI_MAT_KHAU = 0;
                            objHT_NGUOIDUNG.TFA = false;
                            objHT_NGUOIDUNG.NGAY_MAT_KHAU = DateTime.Now;
                            objHT_NGUOIDUNG.DVQL_ID = DVQL_ID;
                            objHT_NGUOIDUNG.DVSD_ID = DVSD_ID;
                            objHT_NGUOIDUNG.NGUOIDUNG_ID = CurrentUser.ID;
                            _db.HT_NGUOIDUNGCollection.Add(objHT_NGUOIDUNG);

                            //nhom quyen
                            HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = new HT_NGUOIDUNG_SD();
                            objHT_NGUOIDUNG_SD.NGUOIDUNG_ID = objHT_NGUOIDUNG.ID;
                            objHT_NGUOIDUNG_SD.DOITUONG_ID = 72876;
                            objHT_NGUOIDUNG_SD.DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC;
                            objHT_NGUOIDUNG_SD.CHUCNANG = DsChucNang.NhomQuyen.ToString();
                            objHT_NGUOIDUNG_SD.ND_ID = CurrentUser.ID;
                            _db.HT_NGUOIDUNG_SDCollection.Add(objHT_NGUOIDUNG_SD);

                            var checkND = _db.TB_IMPORTCollection.Get(op, joData["Id"].Value<string>());
                            if (checkND != null)
                            {
                                _db.HT_NGUOIDUNGCollection.RemoveByID((long)checkND.ID_MOI);
                                _db.TB_IMPORTCollection.Remove(checkND);
                            }
                            //_db.TB_IMPORTCollection.Remove(objTB_IMPORT.LOAI + "", objTB_IMPORT.GID_CU + "");
                            TB_IMPORT objTB_IMPORT = new TB_IMPORT() { ID_MOI = objHT_NGUOIDUNG.ID, LOAI = op, GID_CU = joData["Id"].Value<string>() };
                            _db.TB_IMPORTCollection.Add(objTB_IMPORT);
                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message);
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "canbo")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            stt++;
                            if (joData["id_don_vi"] + "" == "" || joData["id_don_vi"] + "" == "NULL") continue;
                            DM_DANHMUC_ITEM canbo = new DM_DANHMUC_ITEM();
                            canbo.DANHMUC_ID = DsChucNang.CanBo;
                            canbo.TEN = joData["ho_ten"].Value<string>();
                            //check chuc vu
                            var tencv = (joData["chuc_vu"] + "").Trim();
                            if (tencv != "" && tencv != "NULL" && tencv != "Admin" && tencv != "Sông Mã" && tencv != "ththcsbosinh")
                            {
                                if (tencv == "GV" || tencv == "GV" || tencv == "Giao vien" || tencv == "Giáo ciên" || tencv == "Giaoa viên") tencv = "Giáo viên";
                                if (tencv == "PHT" || tencv == "P. Hiệu Trưởng" || tencv == "Hiệu phó" || tencv == "Phó HT") tencv = "Phó Hiệu trưởng";
                                if (tencv == "HT") tencv = "Hiệu trưởng";
                                if (tencv == "Thư viện viên" || tencv == "nhân viên thư viện") tencv = "Thư viện";
                                if (tencv == "NV Văn Thư" || tencv == "VT") tencv = "Văn thư";
                                if (tencv == "KT") tencv = "Kế toán";
                                var chucvu = _db.DM_DANHMUC_ITEMCollection.GetByTEN(tencv, DsChucNang.ChucVu);
                                if (chucvu == null)
                                {
                                    chucvu = new DM_DANHMUC_ITEM();
                                    chucvu.TEN = tencv;
                                    chucvu.DANHMUC_ID = DsChucNang.ChucVu;
                                    chucvu.HETHONG = false;
                                    chucvu.DVQL_ID = DVQL_ID;
                                    chucvu.DVSD_ID = DVSD_ID;
                                    chucvu.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                    chucvu.SAP_XEP = 0;
                                    _db.DM_DANHMUC_ITEMCollection.Add(chucvu);
                                }
                                canbo.CC1 = chucvu.ID;
                            }
                            if (joData["ngay_sinh"] + "" != "" && joData["ngay_sinh"] + "" != "NULL") canbo.CD1 = ConvertClass.ExcelToDateTime(joData["ngay_sinh"] + "");
                            canbo.CC2 = joData["gioi_tinh"] + "" == "Nữ" ? 2 : 1;
                            if (joData["dien_thoai"] + "" != "" && joData["dien_thoai"] + "" != "NULL") canbo.MO_TA = joData["dien_thoai"].Value<string>();
                            if (joData["email"] + "" != "" && joData["email"] + "" != "NULL") canbo.CT1 = joData["email"].Value<string>();
                            if (joData["cmnd_cccd"] + "" != "" && joData["cmnd_cccd"] + "" != "NULL") canbo.CT2 = joData["cmnd_cccd"].Value<string>();
                            if (joData["ngay_cap"] + "" != "" && joData["ngay_cap"] + "" != "NULL") canbo.CD2 = ConvertClass.ExcelToDateTime(joData["ngay_cap"] + "");
                            if (joData["noi_cap"] + "" != "" && joData["noi_cap"] + "" != "NULL") canbo.CT3 = joData["noi_cap"].Value<string>();
                            if (joData["ho_khau_tt"] + "" != "" && joData["ho_khau_tt"] + "" != "NULL") canbo.CT4 = joData["ho_khau_tt"].Value<string>();
                            if (joData["cho_o_hien_nay"] + "" != "" && joData["dien_cho_o_hien_naythoai"] + "" != "NULL") canbo.CT5 = joData["cho_o_hien_nay"].Value<string>();
                            canbo.HETHONG = false;
                            canbo.TRANG_THAI = joData["is_nghi_viec"].Value<string>() != "1" ? (int)TrangThai.DA_DUYET : (int)TrangThai.TU_CHOI;
                            canbo.SAP_XEP = 0;
                            canbo.HASCHILD = false;
                            canbo.DVSD_ID = DVSD_ID;
                            //check dvsd
                            var checkID = _db.TB_IMPORTCollection.Get("donvi", joData["id_don_vi"].Value<decimal>());
                            if (checkID != null) canbo.DVSD_ID = (long)checkID.ID_MOI;
                            else continue;
                            var checkDV = _db.DM_DANHMUC_ITEMCollection.GetByID(canbo.DVSD_ID.Value);
                            if (checkDV == null) continue;
                            //--------
                            canbo.DVQL_ID = DVQL_ID;
                            canbo.DATA = "{}";
                            _db.DM_DANHMUC_ITEMCollection.Add(canbo);
                            //check nguoidung
                            if (joData["UserId"] + "" != "" && joData["UserId"] + "" != "NULL")
                            {
                                var checkUID = _db.TB_IMPORTCollection.Get("nguoidung", joData["UserId"].Value<string>());
                                if (checkUID != null)
                                {
                                    var nguoidung = _db.HT_NGUOIDUNGCollection.GetByID((long)checkUID.ID_MOI);
                                    if (nguoidung != null)
                                    {
                                        nguoidung.CANBO_ID = canbo.ID;
                                        _db.HT_NGUOIDUNGCollection.Update(nguoidung);
                                        //xu ly don vi
                                        _db.HT_NGUOIDUNG_SDCollection.RemoveByDoiTuong(canbo.DVSD_ID.Value, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString());
                                        _db.HT_NGUOIDUNG_SDCollection.Add(new HT_NGUOIDUNG_SD { NGUOIDUNG_ID = nguoidung.ID, DOITUONG_LOAI = DsDoiTuong.DM_DANHMUC, CHUCNANG = DsChucNang.DonVi.ToString(), DOITUONG_ID = canbo.DVSD_ID.Value, ND_ID = nguoidung.ID });
                                    }
                                }
                            }
                            //-----
                            var checkCB = _db.TB_IMPORTCollection.Get(op, joData["id_can_bo"].Value<decimal>());
                            if (checkCB != null)
                            {
                                _db.DM_DANHMUC_ITEMCollection.RemoveByID((long)checkCB.ID_MOI);
                                _db.TB_IMPORTCollection.Remove(checkCB);
                            }
                            TB_IMPORT objTB_IMPORT = new TB_IMPORT() { ID_MOI = canbo.ID, LOAI = op, ID_CU = joData["id_can_bo"].Value<decimal>() };
                            //_db.TB_IMPORTCollection.Remove(objTB_IMPORT.LOAI + "", objTB_IMPORT.ID_CU.Value);
                            _db.TB_IMPORTCollection.Add(objTB_IMPORT);
                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message);
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "thietbi")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    Dictionary<string, int> lstTinhTrang = new Dictionary<string, int>();
                    lstTinhTrang.Add("Trong kho", 0);
                    lstTinhTrang.Add("trong kho", 0);
                    lstTinhTrang.Add("Đang cho mượn", 1);
                    lstTinhTrang.Add("Đã trang cấp", 2);
                    lstTinhTrang.Add("Kho mượn", 3);
                    lstTinhTrang.Add("Tốt", 0);
                    lstTinhTrang.Add("45678", 0);
                    lstTinhTrang.Add("46327", 0);
                    lstTinhTrang.Add("", 0);

                    Dictionary<string, int> lstPhanLoai = new Dictionary<string, int>();
                    lstPhanLoai.Add("Thiết bị cho thuê, mượn", 1);
                    lstPhanLoai.Add("Thiết bị trang cấp", 2);

                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            //if (joData["id_don_vi"] + "" == "" || joData["id_don_vi"] + "" == "NULL") continue;
                            stt++;

                            var objTB_THIETBI = item.ToObject<TB_THIETBI>();
                            objTB_THIETBI.NGAY_TAO = DateTime.Now;
                            objTB_THIETBI.NGUOIDUNG_ID = CurrentUser.ID;
                            objTB_THIETBI.DVQL_ID = DVQL_ID;
                            objTB_THIETBI.DVSD_ID = DVSD_ID;
                            //check donvi
                            var checkIDDV = _db.TB_IMPORTCollection.Get("donvi", joData["id_don_vi"].Value<decimal>());
                            if (checkIDDV != null) objTB_THIETBI.DVSD_ID = (long)checkIDDV.ID_MOI;
                            else continue;

                            var checkDV = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_THIETBI.DVSD_ID.Value);
                            if (checkDV == null) continue;

                            if (joData["loai_thiet_bi"] + "" != "" && joData["loai_thiet_bi"] + "" != "NULL") objTB_THIETBI.LOAI_THIET_BI = joData["loai_thiet_bi"] + "";
                            if (joData["ma_thiet_bi"] + "" != "" && joData["ma_thiet_bi"] + "" != "NULL") objTB_THIETBI.MA = joData["ma_thiet_bi"] + "";
                            if (joData["ten_thiet_bi"] + "" != "" && joData["ten_thiet_bi"] + "" != "NULL") objTB_THIETBI.TEN = joData["ten_thiet_bi"] + "";
                            if (joData["url_hinh_anh"] + "" != "" && joData["url_hinh_anh"] + "" != "NULL") objTB_THIETBI.ANH = joData["url_hinh_anh"] + "";
                            if (joData["thong_so_ky_thuat_chi_tiet"] + "" != "" && joData["thong_so_ky_thuat_chi_tiet"] + "" != "NULL") objTB_THIETBI.THONG_SO = joData["thong_so_ky_thuat_chi_tiet"] + "";
                            if (joData["ngay_mua"] + "" != "" && joData["ngay_mua"] + "" != "NULL") objTB_THIETBI.NGAY_MUA = ConvertClass.ExcelToDateTime(joData["ngay_mua"] + "");
                            if (joData["ngay_dua_vao_su_dung"] + "" != "" && joData["ngay_dua_vao_su_dung"] + "" != "NULL") objTB_THIETBI.NGAY_SD = ConvertClass.ExcelToDateTime(joData["ngay_dua_vao_su_dung"] + "");
                            if (joData["han_bao_hanh"] + "" != "" && joData["han_bao_hanh"] + "" != "NULL") objTB_THIETBI.HAN_BH = ConvertClass.ExcelToDateTime(joData["han_bao_hanh"] + "");
                            if (joData["thoi_han_khau_hao"] + "" != "" && joData["thoi_han_khau_hao"] + "" != "NULL") objTB_THIETBI.HAN_KH = ConvertClass.ExcelToDateTime(joData["thoi_han_khau_hao"] + "");
                            var tenncc = joData["ten_nha_cung_cap"] + "";
                            if (tenncc != "" && tenncc != "NULL")
                            {
                                var nhacc = _db.DM_DANHMUC_ITEMCollection.GetByTEN(tenncc, DsChucNang.NhaCungCap, objTB_THIETBI.DVSD_ID.Value);
                                if (nhacc == null)
                                {
                                    nhacc = new DM_DANHMUC_ITEM();
                                    nhacc.TEN = tenncc;
                                    if (joData["dia_chi"] + "" != "" && joData["dia_chi"] + "" != "NULL") nhacc.CT1 = joData["dia_chi"] + "";
                                    if (joData["dien_thoai"] + "" != "" && joData["dien_thoai"] + "" != "NULL") nhacc.CT2 = joData["dien_thoai"] + "";
                                    if (joData["email"] + "" != "" && joData["email"] + "" != "NULL") nhacc.CT3 = joData["email"] + "";
                                    nhacc.DANHMUC_ID = DsChucNang.NhaCungCap;
                                    nhacc.HETHONG = false;
                                    nhacc.DVQL_ID = DVQL_ID;
                                    nhacc.DVSD_ID = objTB_THIETBI.DVSD_ID;
                                    nhacc.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                    nhacc.SAP_XEP = 0;
                                    _db.DM_DANHMUC_ITEMCollection.Add(nhacc);
                                }
                                objTB_THIETBI.NHACC_ID = nhacc.ID;
                            }
                            if (joData["gia_luc_mua"] + "" != "" && joData["gia_luc_mua"] + "" != "NULL") objTB_THIETBI.GIA_MUA = joData["gia_luc_mua"].Value<decimal>();
                            if (joData["vat"] + "" != "" && joData["vat"] + "" != "NULL") objTB_THIETBI.VAT = joData["vat"].Value<decimal>();
                            if (joData["thanh_tien"] + "" != "" && joData["thanh_tien"] + "" != "NULL") objTB_THIETBI.THANH_TIEN = joData["thanh_tien"].Value<decimal>();
                            if (joData["ghi_chu"] + "" != "" && joData["ghi_chu"] + "" != "NULL") objTB_THIETBI.GHI_CHU = joData["ghi_chu"] + "";
                            if (joData["don_vi_tinh"] + "" != "" && joData["don_vi_tinh"] + "" != "NULL") objTB_THIETBI.DVT = joData["don_vi_tinh"] + "";
                            if (joData["so_seri"] + "" != "" && joData["url_hinh_so_serianh"] + "" != "NULL") objTB_THIETBI.SERI = joData["so_seri"] + "";
                            if (joData["is_mat"] + "" != "" && joData["is_mat"] + "" != "NULL") objTB_THIETBI.DATMAT = joData["is_mat"] + "" == "1" ? true : false;
                            if (joData["is_hong"] + "" != "" && joData["is_hong"] + "" != "NULL") objTB_THIETBI.DATHONG = joData["is_hong"] + "" == "1" ? true : false;
                            if (joData["is_thanh_ly"] + "" != "" && joData["is_thanh_ly"] + "" != "NULL") objTB_THIETBI.DATTL = joData["is_thanh_ly"] + "" == "1" ? true : false;
                            var tenkho = joData["ten_kho_thiet_bi"] + "";
                            if (tenkho != "" && tenkho != "NULL")
                            {
                                var khothietbi = _db.DM_DANHMUC_ITEMCollection.GetByTEN(tenkho, DsChucNang.KhoLuuTru, objTB_THIETBI.DVSD_ID.Value);
                                if (khothietbi == null)
                                {
                                    khothietbi = new DM_DANHMUC_ITEM();
                                    if (joData["ma_kho"] + "" != "" && joData["ma_kho"] + "" != "NULL") khothietbi.MA = joData["ma_kho"] + "";
                                    khothietbi.TEN = tenkho;
                                    khothietbi.DANHMUC_ID = DsChucNang.KhoLuuTru;
                                    khothietbi.HETHONG = false;
                                    khothietbi.DVQL_ID = DVQL_ID;
                                    khothietbi.DVSD_ID = objTB_THIETBI.DVSD_ID;
                                    khothietbi.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                    khothietbi.SAP_XEP = 0;
                                    _db.DM_DANHMUC_ITEMCollection.Add(khothietbi);
                                }
                                objTB_THIETBI.KHO_ID = khothietbi.ID;
                            }
                            if (joData["id_thiet_bi_cha"] + "" != "" && joData["id_thiet_bi_cha"] + "" != "NULL" && joData["id_thiet_bi_cha"] + "" != joData["id_thiet_bi"] + "")
                            {
                                var checkCHA = _db.TB_IMPORTCollection.Get("thietbi", joData["id_thiet_bi_cha"].Value<decimal>());
                                if (checkCHA != null) objTB_THIETBI.TBCHA_ID = (long)checkCHA.ID_MOI;
                            }
                            if (joData["id_can_bo_nhap"] + "" != "" && joData["id_can_bo_nhap"] + "" != "NULL")
                            {
                                var checkCANBO = _db.TB_IMPORTCollection.Get("canbo", joData["id_can_bo_nhap"].Value<decimal>());
                                if (checkCANBO != null) objTB_THIETBI.CANBO_ID = (long)checkCANBO.ID_MOI;
                            }
                            if (joData["tinh_trang_su_dung"] + "" != "" && joData["tinh_trang_su_dung"] + "" != "NULL") objTB_THIETBI.TT_SUDUNG = joData["tinh_trang_su_dung"] + "";
                            if (joData["chi_tiet_tinh_trang_su_dung"] + "" != "" && joData["chi_tiet_tinh_trang_su_dung"] + "" != "NULL") objTB_THIETBI.CT_SUDUNG = joData["chi_tiet_tinh_trang_su_dung"] + "";
                            if (joData["tinh_trang_luu_tru"] + "" != "NULL") objTB_THIETBI.TINH_TRANG = lstTinhTrang[joData["tinh_trang_luu_tru"] + ""];
                            if (joData["chi_tiet_tinh_trang_luu_tru"] + "" != "" && joData["chi_tiet_tinh_trang_luu_tru"] + "" != "NULL") objTB_THIETBI.CT_LUUTRU = joData["chi_tiet_tinh_trang_luu_tru"] + "";
                            if (joData["cb_loai_thiet_bi"] + "" != "" && joData["cb_loai_thiet_bi"] + "" != "NULL") objTB_THIETBI.PHAN_LOAI = lstPhanLoai[joData["cb_loai_thiet_bi"] + ""];
                            if (joData["ngay_mat"] + "" != "" && joData["ngay_mat"] + "" != "NULL") objTB_THIETBI.NGAYMAT = ConvertClass.ExcelToDateTime(joData["ngay_mat"] + "");
                            if (joData["ghi_chu_mat"] + "" != "" && joData["ghi_chu_mat"] + "" != "NULL") objTB_THIETBI.GHICHUMAT = joData["ghi_chu_mat"] + "";
                            if (joData["ngay_hong"] + "" != "" && joData["ngay_hong"] + "" != "NULL") objTB_THIETBI.NGAYHONG = ConvertClass.ExcelToDateTime(joData["ngay_hong"] + "");
                            if (joData["ghi_chu_hong"] + "" != "" && joData["ghi_chu_hong"] + "" != "NULL") objTB_THIETBI.GHICHUHONG = joData["ghi_chu_hong"] + "";
                            if (joData["ngay_thanh_ly"] + "" != "" && joData["ngay_thanh_ly"] + "" != "NULL") objTB_THIETBI.NGAYHONG = ConvertClass.ExcelToDateTime(joData["ngay_thanh_ly"] + "");
                            if (joData["ghi_chu_thanh_ly"] + "" != "" && joData["ghi_chu_thanh_ly"] + "" != "NULL") objTB_THIETBI.GHICHUTL = joData["ghi_chu_thanh_ly"] + "";
                            _db.TB_THIETBICollection.Add(objTB_THIETBI);

                            var checkTB = _db.TB_IMPORTCollection.Get(op, joData["id_thiet_bi"].Value<decimal>());
                            if (checkTB != null)
                            {
                                _db.TB_THIETBICollection.RemoveByID((long)checkTB.ID_MOI);
                                _db.TB_IMPORTCollection.Remove(checkTB);
                            }
                            TB_IMPORT objTB_IMPORT = new TB_IMPORT() { ID_MOI = objTB_THIETBI.ID, LOAI = op, ID_CU = joData["id_thiet_bi"].Value<decimal>() };
                            //_db.TB_IMPORTCollection.Remove(objTB_IMPORT.LOAI + "", objTB_IMPORT.ID_CU.Value);
                            _db.TB_IMPORTCollection.Add(objTB_IMPORT);

                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message + " [" + joData["id_thiet_bi"] + "]");
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "DM_CANBO")
            {
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    
                    int stt = 0;
                    foreach (var joData in item["data"])
                    {

                        try
                        {

                            stt++;
                            DM_DANHMUC_ITEM canbo = new DM_DANHMUC_ITEM();
                            canbo.DANHMUC_ID = DsChucNang.CanBo;
                            canbo.TEN = joData["ho_ten"].Value<string>();
                            //check chuc vu
                            var tencv = (joData["chuc_vu"] + "").Trim();
                            if (tencv != "" && tencv != "NULL" && tencv != "Admin" && tencv != "Sông Mã" && tencv != "ththcsbosinh")
                            {
                                if (tencv == "GV" || tencv == "GV" || tencv == "Giao vien" || tencv == "Giáo ciên" || tencv == "Giaoa viên") tencv = "Giáo viên";
                                if (tencv == "PHT" || tencv == "P. Hiệu Trưởng" || tencv == "Hiệu phó" || tencv == "Phó HT") tencv = "Phó Hiệu trưởng";
                                if (tencv == "HT") tencv = "Hiệu trưởng";
                                if (tencv == "Thư viện viên" || tencv == "nhân viên thư viện") tencv = "Thư viện";
                                if (tencv == "NV Văn Thư" || tencv == "VT") tencv = "Văn thư";
                                if (tencv == "KT") tencv = "Kế toán";
                                var chucvu = _db.DM_DANHMUC_ITEMCollection.GetByTEN(tencv, DsChucNang.ChucVu);
                                if (chucvu == null)
                                {
                                    chucvu = new DM_DANHMUC_ITEM();
                                    chucvu.TEN = tencv;
                                    chucvu.DANHMUC_ID = DsChucNang.ChucVu;
                                    chucvu.HETHONG = false;
                                    chucvu.DVQL_ID = DVQL_ID;
                                    chucvu.DVSD_ID = DVSD_ID;
                                    chucvu.TRANG_THAI = (int)TrangThai.DA_DUYET;
                                    chucvu.SAP_XEP = 0;
                                    _db.DM_DANHMUC_ITEMCollection.Add(chucvu);
                                }
                                canbo.CC1 = chucvu.ID;
                            }
                            if (joData["ngay_sinh"] + "" != "" && joData["ngay_sinh"] + "" != "NULL") canbo.CD1 = ConvertClass.ExcelToDateTime(joData["ngay_sinh"] + "");
                            canbo.CC2 = joData["gioi_tinh"] + "" == "Nữ" ? 2 : 1;
                            if (joData["dien_thoai"] + "" != "" && joData["dien_thoai"] + "" != "NULL") canbo.MO_TA = joData["dien_thoai"].Value<string>();
                            if (joData["email"] + "" != "" && joData["email"] + "" != "NULL") canbo.CT1 = joData["email"].Value<string>();
                            if (joData["cmnd_cccd"] + "" != "" && joData["cmnd_cccd"] + "" != "NULL") canbo.CT2 = joData["cmnd_cccd"].Value<string>();
                            if (joData["ngay_cap"] + "" != "" && joData["ngay_cap"] + "" != "NULL") canbo.CD2 = ConvertClass.ExcelToDateTime(joData["ngay_cap"] + "");
                            if (joData["noi_cap"] + "" != "" && joData["noi_cap"] + "" != "NULL") canbo.CT3 = joData["noi_cap"].Value<string>();
                            if (joData["ho_khau_tt"] + "" != "" && joData["ho_khau_tt"] + "" != "NULL") canbo.CT4 = joData["ho_khau_tt"].Value<string>();
                            if (joData["cho_o_hien_nay"] + "" != "" && joData["cho_o_hien_nay"] + "" != "NULL") canbo.CT5 = joData["cho_o_hien_nay"].Value<string>();
                            canbo.HETHONG = false;
                            canbo.TRANG_THAI = (int)TrangThai.DA_DUYET;
                            var trangthai = (joData["is_nghi_viec"] + "").Trim();
                            if (trangthai == "Đang làm việc" || trangthai == "đang làm việc" || trangthai == "Đang công tác") canbo.TRANG_THAI = (int)TrangThai.DA_DUYET;
                            if (trangthai == "Đã nghỉ" || trangthai == "nghỉ" || trangthai == "Nghỉ việc") canbo.TRANG_THAI = (int)TrangThai.TU_CHOI;
                            canbo.SAP_XEP = 0;
                            canbo.HASCHILD = false;
                            canbo.DVSD_ID = DVSD_ID;
                            canbo.DVQL_ID = DVQL_ID;
                            canbo.DATA = "{}";
                            _db.DM_DANHMUC_ITEMCollection.Add(canbo);
                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message);
                            break;
                        }

                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);

            }
            else if (op == "muontra")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    Dictionary<string, int> lstTrangThai = new Dictionary<string, int>();
                    lstTrangThai.Add("Đang mượn", 0);
                    lstTrangThai.Add("Đã trả", 1);

                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            stt++;

                            var objTB_MUONTRA = item.ToObject<TB_MUONTRA>();
                            objTB_MUONTRA.NGAY_TAO = DateTime.Now;
                            objTB_MUONTRA.NGUOIDUNG_ID = CurrentUser.ID;
                            objTB_MUONTRA.DVQL_ID = DVQL_ID;
                            objTB_MUONTRA.DVSD_ID = DVSD_ID;
                            //check donvi
                            var checkIDDV = _db.TB_IMPORTCollection.Get("donvi", joData["id_don_vi"].Value<decimal>());
                            if (checkIDDV != null) objTB_MUONTRA.DVSD_ID = (long)checkIDDV.ID_MOI;
                            else continue;

                            var checkDV = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_MUONTRA.DVSD_ID.Value);
                            if (checkDV == null) continue;

                            if (joData["so_bien_ban"] + "" != "" && joData["so_bien_ban"] + "" != "NULL") objTB_MUONTRA.SOBB = joData["so_bien_ban"] + "";
                            if (joData["ngay_bien_ban"] + "" != "" && joData["ngay_bien_ban"] + "" != "NULL")
                            {
                                objTB_MUONTRA.NGAY_BB = ConvertClass.ExcelToDateTime(joData["ngay_bien_ban"] + "");
                                objTB_MUONTRA.NGAY_TAO = objTB_MUONTRA.NGAY_BB;
                            }
                            if (joData["noi_ban_giao"] + "" != "" && joData["noi_ban_giao"] + "" != "NULL") objTB_MUONTRA.NOI_BAN_GIAO = joData["noi_ban_giao"] + "";
                            if (joData["id_can_bo_giao"] + "" != "" && joData["id_can_bo_giao"] + "" != "NULL")
                            {
                                var checkCANBO = _db.TB_IMPORTCollection.Get("canbo", joData["id_can_bo_giao"].Value<decimal>());
                                if (checkCANBO != null) objTB_MUONTRA.CBGIAO_ID = (long)checkCANBO.ID_MOI;
                            }
                            if (joData["id_can_bo_nhan"] + "" != "" && joData["id_can_bo_nhan"] + "" != "NULL")
                            {
                                var checkCANBO = _db.TB_IMPORTCollection.Get("canbo", joData["id_can_bo_nhan"].Value<decimal>());
                                if (checkCANBO != null) objTB_MUONTRA.CBNHAN_ID = (long)checkCANBO.ID_MOI;
                            }
                            if (joData["ly_do_giao_nhan"] + "" != "" && joData["ly_do_giao_nhan"] + "" != "NULL") objTB_MUONTRA.LY_DO = joData["ly_do_giao_nhan"] + "";
                            if (joData["thoi_gian_ban_giao_tu"] + "" != "" && joData["thoi_gian_ban_giao_tu"] + "" != "NULL") objTB_MUONTRA.NGAY_MUON = ConvertClass.ExcelToDateTime(joData["thoi_gian_ban_giao_tu"] + "");
                            if (joData["so_luong_bien_ban"] + "" != "" && joData["so_luong_bien_ban"] + "" != "NULL") objTB_MUONTRA.SO_LAP = joData["so_luong_bien_ban"].Value<int>();

                            if (joData["ngay_sinh_can_bo_giao"] + "" != "" && joData["ngay_sinh_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_NGAYSINH = ConvertClass.ExcelToDateTime(joData["ngay_sinh_can_bo_giao"] + "");
                            if (joData["shgt_can_bo_giao"] + "" != "" && joData["shgt_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_CCCD = joData["shgt_can_bo_giao"] + "";
                            if (joData["ngay_cap_giay_to_can_bo_giao"] + "" != "" && joData["ngay_cap_giay_to_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_NGAYCAP = ConvertClass.ExcelToDateTime(joData["ngay_cap_giay_to_can_bo_giao"] + "");
                            if (joData["noi_cap_giay_to_can_bo_giao"] + "" != "" && joData["noi_cap_giay_to_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_NOICAP = joData["noi_cap_giay_to_can_bo_giao"] + "";
                            if (joData["ho_khau_thuong_tru_can_bo_giao"] + "" != "" && joData["ho_khau_thuong_tru_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_HKTT = joData["ho_khau_thuong_tru_can_bo_giao"] + "";
                            if (joData["cho_o_hien_nay_can_bo_giao"] + "" != "" && joData["cho_o_hien_nay_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_CHOO = joData["cho_o_hien_nay_can_bo_giao"] + "";
                            if (joData["sdt_can_bo_giao"] + "" != "" && joData["sdt_can_bo_giao"] + "" != "NULL") objTB_MUONTRA.CBGIAO_DIENTHOAI = joData["sdt_can_bo_giao"] + "";

                            if (joData["ngay_sinh_can_bo_nhan"] + "" != "" && joData["ngay_sinh_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_NGAYSINH = ConvertClass.ExcelToDateTime(joData["ngay_sinh_can_bo_nhan"] + "");
                            if (joData["shgt_can_bo_nhan"] + "" != "" && joData["shgt_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_CCCD = joData["shgt_can_bo_nhan"] + "";
                            if (joData["ngay_cap_giay_to_can_bo_nhan"] + "" != "" && joData["ngay_cap_giay_to_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_NGAYCAP = ConvertClass.ExcelToDateTime(joData["ngay_cap_giay_to_can_bo_nhan"] + "");
                            if (joData["noi_cap_giay_to_can_bo_nhan"] + "" != "" && joData["noi_cap_giay_to_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_NOICAP = joData["noi_cap_giay_to_can_bo_nhan"] + "";
                            if (joData["ho_khau_thuong_tru_can_bo_nhan"] + "" != "" && joData["ho_khau_thuong_tru_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_HKTT = joData["ho_khau_thuong_tru_can_bo_nhan"] + "";
                            if (joData["cho_o_hien_nay_can_bo_nhan"] + "" != "" && joData["cho_o_hien_nay_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_CHOO = joData["cho_o_hien_nay_can_bo_nhan"] + "";
                            if (joData["sdt_can_bo_nhan"] + "" != "" && joData["sdt_can_bo_nhan"] + "" != "NULL") objTB_MUONTRA.CBNHAN_DIENTHOAI = joData["sdt_can_bo_nhan"] + "";

                            if (joData["ten_can_bo_phu_trach"] + "" != "" && joData["ten_can_bo_phu_trach"] + "" != "NULL") objTB_MUONTRA.PHU_TRACH_DV = joData["ten_can_bo_phu_trach"] + "";

                            if (joData["ben_a_giu"] + "" != "" && joData["ben_a_giu"] + "" != "NULL") objTB_MUONTRA.BEN_GIAO = joData["ben_a_giu"].Value<int>();
                            if (joData["ben_b_giu"] + "" != "" && joData["ben_b_giu"] + "" != "NULL") objTB_MUONTRA.BEN_NHAN = joData["ben_a_giu"].Value<int>();
                            if (joData["ngay_hen_tra"] + "" != "" && joData["ngay_hen_tra"] + "" != "NULL") objTB_MUONTRA.NGAY_TRA = ConvertClass.ExcelToDateTime(joData["ngay_hen_tra"] + "");

                            objTB_MUONTRA.TRANG_THAI = 0;
                            if (joData["cb_trang_thai"] + "" != "" && joData["cb_trang_thai"] + "" != "NULL") objTB_MUONTRA.TRANG_THAI = lstTrangThai[joData["cb_trang_thai"] + ""];

                            _db.TB_MUONTRACollection.Add(objTB_MUONTRA);

                            var checkTB = _db.TB_IMPORTCollection.Get(op, joData["id_bien_ban"].Value<decimal>());
                            if (checkTB != null)
                            {
                                _db.TB_MUONTRACollection.RemoveByID((long)checkTB.ID_MOI);
                                _db.TB_IMPORTCollection.Remove(checkTB);
                            }
                            TB_IMPORT objTB_IMPORT = new TB_IMPORT() { ID_MOI = objTB_MUONTRA.ID, LOAI = op, ID_CU = joData["id_bien_ban"].Value<decimal>() };
                            _db.TB_IMPORTCollection.Add(objTB_IMPORT);

                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message + " [" + joData["id_bien_ban"] + "]");
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "muontract")
            {
                if (!UserAccess(Quyen.Xem)) return UserAccessDenied();
                List<string> mess = new List<string>();
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    Dictionary<string, int> lstTrangThai = new Dictionary<string, int>();
                    lstTrangThai.Add("Đang mượn", 0);
                    lstTrangThai.Add("Đã trả", 1);

                    int stt = 0;
                    foreach (var joData in item["data"])
                    {
                        try
                        {
                            stt++;

                            var objTB_MUONTRACT = item.ToObject<TB_MUONTRACT>();
                            if (joData["id_thiet_bi"] + "" != "" && joData["id_thiet_bi"] + "" != "NULL")
                            {
                                var checkTB = _db.TB_IMPORTCollection.Get("thietbi", joData["id_thiet_bi"].Value<decimal>());
                                if (checkTB != null) objTB_MUONTRACT.THIETBI_ID = (long)checkTB.ID_MOI;
                            }
                            if (joData["id_bien_ban"] + "" != "" && joData["id_bien_ban"] + "" != "NULL")
                            {
                                var checkTB = _db.TB_IMPORTCollection.Get("muontra", joData["id_bien_ban"].Value<decimal>());
                                if (checkTB != null) objTB_MUONTRACT.MUONTRA_ID = (long)checkTB.ID_MOI;
                            }
                            //if (joData["tinh_trang_luc_muon"] + "" != "" && joData["tinh_trang_luc_muon"] + "" != "NULL") objTB_MUONTRACT.TINH_TRANG = joData["tinh_trang_luc_muon"] + "";
                            objTB_MUONTRACT.DA_TRA = false;
                            if (joData["is_da_tra"] + "" != "" && joData["is_da_tra"] + "" != "NULL") objTB_MUONTRACT.DA_TRA = joData["is_da_tra"] + "" == "1";
                            if (joData["tinh_trang_luc_tra"] + "" != "" && joData["tinh_trang_luc_tra"] + "" != "NULL") objTB_MUONTRACT.TINH_TRANG = joData["tinh_trang_luc_muon"] + "";
                            if (joData["ghi_chu"] + "" != "" && joData["ghi_chu"] + "" != "NULL") objTB_MUONTRACT.GHI_CHU = joData["ghi_chu"] + "";

                            _db.TB_MUONTRACTCollection.Add(objTB_MUONTRACT);
                        }
                        catch (Exception ex)
                        {
                            mess.Add("Vi trí " + stt + ": " + ex.Message + " [" + joData["id_bien_ban"] + "]");
                            break;
                        }
                    }
                    if (mess.Count == 0) dbContextTransaction.Commit();
                    else dbContextTransaction.Rollback();
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }

            return new NoContentResult();
        }
    }
}
