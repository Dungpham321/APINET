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
    [Route("api/ThietBi/TB_THIETBI")]
    [ApiController]
    public class TB_THIETBIController : BaseController
    {
        public TB_THIETBIController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.ThietBi;
            NhomQuyen = Resource.QuyenTB_THIETBI;
        }

        public static List<QUYEN> Permission()
        {
            TB_THIETBIController mn = new TB_THIETBIController(null, null, null);
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
                int PHANLOAI = Convert.ToInt32(Request.Query["PHANLOAI"]);
                return ListAll(_db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID).Where(e => e.PHAN_LOAI == PHANLOAI));
            }
            else if (op == "ListChon")
            {
                string Type = Request.Query["type"] + "";
                string mode = Request.Query["mode"] + "";
                string loai = Request.Query["loai"] + "";
                if (mode == "")
                {
                    var data = _db.TB_THIETBICollection.Get(DVQL_ID, DVSD_ID).Where(e => e.TINH_TRANG == (int)TinhTrangLuuTru.TRONG_KHO);
                    if (Type == "THANHLY")
                    {
                        if (loai == "1") data = data.Where(c => (c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value));
                        else data = data.Where(c => !((c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value)));
                        return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.GHI_CHU, CHON = false }));
                    }
                    else if (Type == "TRANGCAP")
                    {
                        data = data.Where(c => c.PHAN_LOAI == (int)PhanLoai.TrangCap);
                        data = data.Where(c => !((c.DATTL.HasValue && c.DATTL.Value) || (c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value)));
                        return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.GHI_CHU, CHON = false }));
                    }
                    else if (Type == "MATHONG")
                    {
                        if (loai == "1") data = data.Where(c => (c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value));
                        else data = data.Where(c => !((c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value)));
                        return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.GHI_CHU, CHON = false }));
                    }
                    else if (Type == "TB_CHUONGTRINH")
                    {
                        data = data.Where(c => c.PHAN_LOAI == (int)PhanLoai.ThueMuon);
                        data = data.Where(c => !((c.DATTL.HasValue && c.DATTL.Value) || (c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value)));
                        return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.LOAI_THIET_BI, s.GHI_CHU, s.ANH, SO_LUONG = 1, CHON = false }));
                    }
                    else
                    {
                        data = data.Where(c => c.PHAN_LOAI == (int)PhanLoai.ThueMuon);
                        data = data.Where(c => !((c.DATTL.HasValue && c.DATTL.Value) || (c.DATHONG.HasValue && c.DATHONG.Value) || (c.DATMAT.HasValue && c.DATMAT.Value)));
                        return ObjectResult(data.Select(s => new { s.ID, s.MA, s.TEN, s.DVT, s.TT_SUDUNG, s.GHI_CHU, CHON = false }));
                    }
                }
                else
                {
                    var lstTB = new List<TB_THIETBI>();
                    return ObjectResult(lstTB);
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
                var itemNew = item.ToObject<TB_THIETBI>();
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.DVSD_ID = DVSD_ID;
                itemNew.CANBO_ID = CANBO_ID;
                _db.TB_THIETBICollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID, itemNew.MA);

            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                string mathietbi = "";
                using (var dbContextTransaction = _db.Database.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in jItem)
                        {
                            var obgTB_THIETBI = _db.TB_THIETBICollection.GetByID(id.Value<long>());
                            mathietbi = obgTB_THIETBI.MA;
                            if (obgTB_THIETBI != null)
                            {
                                _db.TB_THIETBICollection.Remove(obgTB_THIETBI);
                                HtLog(Quyen.Xoa, obgTB_THIETBI.ID, obgTB_THIETBI.MA);
                            }
                      
                        }
                        dbContextTransaction.Commit();
                    }
                    catch (Exception e)
                    {
                        dbContextTransaction.Rollback();
                        mess.Add($"Thiết bị " + mathietbi+ " đang có dữ liệu liên quan.");
                    }
                }
                if (mess.Count() > 0) return ObjectResult(mess);

            }
            return new NoContentResult();
        }
        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {

            var obgTB_THIETBI = _db.TB_THIETBICollection.GetByID(ID);
            if (obgTB_THIETBI != null)
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                if (obgTB_THIETBI.DVSD_ID != DVSD_ID) return UserAccessDenied();
                if (item["ANH"] + "" != "" && obgTB_THIETBI.ANH + "" != "" && item["ANH"] + "" != obgTB_THIETBI.ANH)
                {
                    string filePath = GetRootPath + obgTB_THIETBI.ANH;
                    try
                    {
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }
                    catch { }
                }
                UpdateObject(obgTB_THIETBI, item, new string[] { "id ma phan_loai" });
                obgTB_THIETBI.NGAY_SUA = DateTime.Now;
                _db.TB_THIETBICollection.Update(obgTB_THIETBI);
                HtLog(Quyen.Sua, obgTB_THIETBI.ID, obgTB_THIETBI.MA);
            }
            return new NoContentResult();
        }
    }
}
