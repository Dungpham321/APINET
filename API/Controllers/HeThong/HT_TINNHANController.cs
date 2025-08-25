using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.AspNetCore.SignalR;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_TINNHAN")]
    [ApiController]
    public class HT_TINNHANController : BaseController
    {
        public HT_TINNHANController(GDBContext db, IHubContext<ChatHub> chatHub) : base(db, chatHub)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_TINNHAN;
        }
        public static List<QUYEN> Permission()
        {
            HT_TINNHANController mn = new HT_TINNHANController(null, null);
            return mn.QuyenCoBan(Quyen.Xem, Quyen.Them, Quyen.Xoa);
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
                    Delete = UserAccess(Quyen.Xoa),
                    Title = CurrentMenu?.NAME + "",
                    VAI_TRO = IntVaiTro
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.HT_TINNHANCollection.GetInfoByNGUOINHAN_ID(CurrentUser.ID));
            }
            else if (op == "HT_NGUOIDUNG")
            {
                return ListAll(GetDsHT_NGUOIDUNG(false));
            }
            else if (op == "GetNoiDung")
            {
                long ID = Convert.ToInt64(Request.Query["ID"]);
                var objHT_TINHAN_ND = _db.HT_TINNHAN_NDCollection.GetByID(ID);
                objHT_TINHAN_ND.TRANG_THAI = (int)TrangThaiTinNhan.DaXem;
                _db.HT_TINNHAN_NDCollection.Update(objHT_TINHAN_ND);
                var objHT_TINNHAN = _db.HT_TINNHANCollection.GetByID(objHT_TINHAN_ND.TINNHAN_ID);
                List<long> lstNguoiNhanId = new List<long>();
                if(objHT_TINNHAN.LOAI_TIN == (int)LoaiTinNhan.NguoiDung)
                {
                    lstNguoiNhanId = _db.HT_TINNHAN_NDCollection.GetAll().Where(c => c.TINNHAN_ID == objHT_TINNHAN.ID && c.NGUOINHAN_ID != objHT_TINNHAN.NGUOITAO_ID).Select(s => s.NGUOINHAN_ID).ToList();
                }
                else
                {
                    lstNguoiNhanId = _db.HT_TINNHAN_NDCollection.GetAll().Where(c => c.TINNHAN_ID == objHT_TINNHAN.ID).Select(s => s.NGUOINHAN_ID).ToList();
                }
                List<string> lstNguoiNhan = _db.HT_NGUOIDUNGCollection.GetByIDS(lstNguoiNhanId).Select(s=> s.TEN_DANG_NHAP).ToList();
                return ObjectResult(new
                {
                    NGUOITAO = objHT_TINNHAN.NGUOITAO_ID.HasValue?_db.HT_NGUOIDUNGCollection.GetByID(objHT_TINNHAN.NGUOITAO_ID.Value)?.TEN_DANG_NHAP:"",
                    objHT_TINNHAN.LOAI_TIN,
                    objHT_TINNHAN.NGAY_TAO,
                    objHT_TINNHAN.NOI_DUNG,
                    objHT_TINNHAN.TIEU_DE,
                    nguoinhan = lstNguoiNhan,
                });
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
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                
                //nguoi nhan
                List<long> lstNGUOI_NHAN = new List<long>();
                lstNGUOI_NHAN.Add(CurrentUser.ID);
                if (item["NGUOI_NHAN"] + "" != "")
                {
                    lstNGUOI_NHAN.AddRange(JsonConvert.DeserializeObject<List<long>>(item["NGUOI_NHAN"] + ""));
                }
                else if (item["NHOM_NHAN"] + "" != "")
                {
                    var lstNHOM_IDS = JsonConvert.DeserializeObject<List<long>>(item["NHOM_NHAN"] + "");
                    foreach (var NHOM_ID in lstNHOM_IDS)
                    {
                        var lstNGUOIDUNG_ID = _db.HT_NGUOIDUNG_SDCollection.GetByDoiTuong(NHOM_ID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString()).Select(s => s.NGUOIDUNG_ID).ToList();
                        if(lstNGUOIDUNG_ID != null) lstNGUOI_NHAN.AddRange(lstNGUOIDUNG_ID);
                    }
                }
                else
                {
                    lstNGUOI_NHAN.AddRange(GetDsHT_NGUOIDUNG(false).Select(s => s.ID).ToList());
                }
                if(lstNGUOI_NHAN.Count == 0)
                {
                    return ObjectResult(null, "Không có người dùng phù hợp");
                }
                else
                {
                    var itemNew = item.ToObject<HT_TINNHAN>();
                    itemNew.NGUOITAO_ID = CurrentUser.ID;
                    itemNew.NGAY_TAO = DateTime.Now;
                    itemNew.LOAI_TIN = (int)LoaiTinNhan.NguoiDung;
                    _db.HT_TINNHANCollection.Add(itemNew);
                    lstNGUOI_NHAN = lstNGUOI_NHAN.Distinct().ToList();
                    foreach (long NGUOIDUNG_ID in lstNGUOI_NHAN)
                    {
                        HT_TINNHAN_ND objHT_TINNHAN_ND = new HT_TINNHAN_ND();
                        objHT_TINNHAN_ND.NGUOINHAN_ID = NGUOIDUNG_ID;
                        objHT_TINNHAN_ND.TINNHAN_ID = itemNew.ID;
                        objHT_TINNHAN_ND.TRANG_THAI = NGUOIDUNG_ID == CurrentUser.ID ? (int)TrangThaiTinNhan.DaXem : (int)TrangThaiTinNhan.ChuaXem;
                        _db.HT_TINNHAN_NDCollection.Add(objHT_TINNHAN_ND);
                    }

                    //save file
                    SaveFile(itemNew.ID, "FILES", "HT_TINNHAN", CurrentUser.ID);

                    foreach (long NGUOIDUNG_ID in lstNGUOI_NHAN.Where(c => c != CurrentUser.ID).ToList())
                    {
                        SendChatTinNhan(NGUOIDUNG_ID);
                    }

                    HtLog(Quyen.Them, itemNew.ID);
                }
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var mid in jItem)
                {
                    var objHT_TINNHAN_ND = _db.HT_TINNHAN_NDCollection.GetByNGUOINHAN_ID(CurrentUser.ID, mid.Value<long>());
                    _db.HT_TINNHAN_NDCollection.Remove(objHT_TINNHAN_ND);
                    HtLog(Quyen.Xoa, objHT_TINNHAN_ND.TINNHAN_ID);
                }
            }
            return new NoContentResult();
        }
    }
}
