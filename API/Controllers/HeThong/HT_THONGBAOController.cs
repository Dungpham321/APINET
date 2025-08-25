using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/HETHONG/HT_THONGBAO")]
    [ApiController]
    public class HT_THONGBAOController : BaseController
    {
        public HT_THONGBAOController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.QuanTriHeThong;
            NhomQuyen = Resource.QuyenHT_THONGBAO;
        }

        public static List<QUYEN> Permission()
        {
            HT_THONGBAOController mn = new HT_THONGBAOController(null);
            return mn.QuyenCoBan(Quyen.Xem, Quyen.Sua, Quyen.Them, Quyen.Xoa,Quyen.Duyet, Quyen.HuyDuyet);
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
                    Duyet = UserAccess(Quyen.Duyet),
                    HuyDuyet = UserAccess(Quyen.HuyDuyet),
                    Title = CurrentMenu?.NAME + "",
                });
            }
            else if (op == "List")
            {
                return ListAll(_db.HT_THONGBAOCollection.Get(DVQL_ID));
            }
            else if (op == "GetNoiDung")
            {
                long ID = ConvertClass.ToLong(Request.Query["ID"] + "", 0);
                return ObjectResult(_db.HT_THONGBAOCollection.GetById(ID));
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
                var itemNew = item.ToObject<HT_THONGBAO>();
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.NGAY_TAO = DateTime.Now;
                itemNew.TRANG_THAI = (int)TrangThai.CHO_DUYET;
                itemNew.NGUOIDUNG_ID = CurrentUser.ID;
                _db.HT_THONGBAOCollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID);
                if(itemNew.NGAY_GUI <= DateTime.Now)
                {
                    foreach(var NGUOIDUNG_ID in _db.HT_NGUOIDUNGCollection.Get().Select(s => s.ID).ToList())
                    {
                        SendChatThongBao(NGUOIDUNG_ID);
                    }
                }
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var uid in jItem)
                {
                    HT_THONGBAO u = _db.HT_THONGBAOCollection.GetById(uid.Value<long>());
                    _db.HT_THONGBAOCollection.Remove(u);
                    HtLog(Quyen.Xoa, u.ID);
                }
            }else if(op == "TrangThai")
            {
                if (!UserAccess(Quyen.Duyet) && !UserAccess(Quyen.HuyDuyet)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var uid in jItem)
                {
                    HT_THONGBAO u = _db.HT_THONGBAOCollection.GetById(uid.Value<long>());
                    u.TRANG_THAI = item.GetValue("trangthai").Value<int>();
                    _db.HT_THONGBAOCollection.Update(u);
                    HtLog(u.TRANG_THAI == (int)TrangThai.DA_DUYET ? Quyen.Duyet : Quyen.HuyDuyet, u.ID);
                }
            }
            return new NoContentResult();
        }

        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {
            if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
            var oldItem = _db.HT_THONGBAOCollection.GetById(ID);
            if (oldItem != null)
            {
                UpdateObject(oldItem, item, new string[] { "id" });
                 oldItem.NGAY_SUA = DateTime.Now;
                _db.HT_THONGBAOCollection.Update(oldItem);
                HtLog(Quyen.Sua, oldItem.ID);
            }
            return new NoContentResult();
        }
    }
}
