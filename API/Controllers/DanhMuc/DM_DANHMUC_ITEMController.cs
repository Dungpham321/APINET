using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/DanhMuc/DM_DANHMUC_ITEM")]
    [ApiController]
    public class DM_DANHMUC_ITEMController : BaseController
    {
        public DM_DANHMUC_ITEMController(GDBContext db) : base(db)
        {
            NhomChucNang = NhomChucNang.DanhMucDungChung;
            NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM;
        }
        public static void Permission_Alter(List<QUYEN> items, GDBContext _db)
        {
            var lstDM_DANHMUC = _db.DM_DANHMUCCollection.Get().ToList();
            foreach (var objDM_DANHMUC in lstDM_DANHMUC)
            {
                items.Add(new QUYEN(Quyen.Xem, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                items.Add(new QUYEN(Quyen.Them, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                items.Add(new QUYEN(Quyen.Sua, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                items.Add(new QUYEN(Quyen.Xoa, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                if (objDM_DANHMUC.CODUYET.HasValue && objDM_DANHMUC.CODUYET.Value)
                {
                    items.Add(new QUYEN(Quyen.Duyet, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                    items.Add(new QUYEN(Quyen.HuyDuyet, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                }
                if(objDM_DANHMUC.ID == DsChucNang.NhomQuyen)
                {
                    items.Add(new QUYEN(Quyen.PhanQuyen, Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN, NhomChucNang.DanhMucDungChung));
                }
            }
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == "Access")
            {
                _db.HT_NGUOIDUNG_SDCollection.RemoveByDoiTuongND(0, CurrentUser.ID);
                _db.DM_DANHMUC_SDCollection.RemoveByDoituong(0, CurrentUser.ID);
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(Request.Query["MA_DANHMUC"]);
                NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN;
                var lstDM_DANHMUC = _db.DM_DANHMUCCollection.Get().Where(c => c.PID.HasValue);
                return ObjectResult(new
                {
                    View = UserAccess(Quyen.Xem),
                    New = UserAccess(Quyen.Them),
                    Edit = UserAccess(Quyen.Sua),
                    Delete = UserAccess(Quyen.Xoa),
                    PhanQuyen = UserAccess(Quyen.PhanQuyen),
                    Duyet = UserAccess(Quyen.Duyet),
                    HuyDuyet = UserAccess(Quyen.HuyDuyet),
                    Title = objDM_DANHMUC?.TEN + "",
                    danhmuc = objDM_DANHMUC,
                    Cols = _db.DM_DANHMUC_COLCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID),
                    DmTree = lstDM_DANHMUC,
                });
            }
            else if (op == "List")
            {
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(DANHMUC_ID);
                if (objDM_DANHMUC == null) objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(Request.Query["DANHMUC_ID"] + "");
                if (objDM_DANHMUC == null) return new BadRequestResult();
                if (objDM_DANHMUC.ID == DsChucNang.DonVi)
                {
                    return ListAll(GetDsDM_DONVI());
                }
                else if (objDM_DANHMUC.ID == DsChucNang.NhomQuyen)
                {
                    return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID).Where(c => GetDsDM_DONVI_IDS().Contains(c.DVSD_ID.Value)));
                }
                if (objDM_DANHMUC.NOIBO.HasValue && objDM_DANHMUC.NOIBO.Value)
                {
                    return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID, DVSD_ID));
                }
                return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID));
            }
            else if (op == "ListSD")
            {
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(DANHMUC_ID);
                if (objDM_DANHMUC == null) objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(Request.Query["DANHMUC_ID"] + "");
                if (objDM_DANHMUC == null) return new BadRequestResult();
                if (objDM_DANHMUC.ID == DsChucNang.DonVi)
                {
                    return ListAll(GetDsDM_DONVI().Where(c => c.TRANG_THAI == (int) TrangThai.DA_DUYET));
                }
                else if (objDM_DANHMUC.ID == DsChucNang.NhomQuyen)
                {
                    return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET && GetDsDM_DONVI_IDS().Contains(c.DVSD_ID.Value)));
                } 
                else if (objDM_DANHMUC.ID == DsChucNang.DiaBan)
                {
                    string MA_DIABAN = HT_CAUHINH_Get<string>("MA_DIABAN", "");
                    if (MA_DIABAN == "")
                    {
                        ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET));
                    }
                    else
                    {
                        var objDM_DIABAN = _db.DM_DANHMUC_ITEMCollection.GetByMA(MA_DIABAN, objDM_DANHMUC.ID);
                        return ListAll(_db.DM_DANHMUC_ITEMCollection.GetTree(objDM_DANHMUC.ID, objDM_DIABAN.ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET));
                    }
                }
                if (objDM_DANHMUC.NOIBO.HasValue && objDM_DANHMUC.NOIBO.Value)
                {
                    if (objDM_DANHMUC.ID == DsChucNang.CanBo)
                    {
                        return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID).Where(c => GetDsDM_DONVI_IDS().Contains(c.DVSD_ID.Value) && c.TRANG_THAI == (int)TrangThai.DA_DUYET));
                    }
                    else
                    {
                        return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID, DVSD_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET));
                    }
                                    }
                return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET));
            }
            else if (op == "Check")
            {
                string MA = Request.Query["ID"] + "";
                string FIELD = Request.Query["Field"] + "";
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (MA == "") return new BadRequestResult();
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(DANHMUC_ID);
                DM_DANHMUC_ITEM objDM_DANHMUC_ITEM = null;
                if (objDM_DANHMUC.NOIBO.HasValue && objDM_DANHMUC.NOIBO.Value)
                {
                    objDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByField(FIELD, MA, DANHMUC_ID, DVSD_ID);
                }
                else
                {
                    objDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByField(FIELD, MA, DANHMUC_ID);
                }
                //DM_DANHMUC_ITEM objDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByField(FIELD, MA, DANHMUC_ID);//_db.DM_DANHMUC_ITEMCollection.GetByMA(MA, DANHMUC_ID);
                return ObjectResult(new { Check = objDM_DANHMUC_ITEM != null && objDM_DANHMUC_ITEM.ID != OldID });
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
                var itemNew = item.ToObject<DM_DANHMUC_ITEM>();
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(itemNew.DANHMUC_ID.Value);
                NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN;
                if (!UserAccess(Quyen.Them)) return UserAccessDenied();
                if (objDM_DANHMUC.CODUYET.HasValue && objDM_DANHMUC.CODUYET.Value) itemNew.TRANG_THAI = (int)TrangThai.CHO_DUYET;
                itemNew.DVSD_ID = DVSD_ID;
                itemNew.DVQL_ID = DVQL_ID;
                itemNew.HETHONG = false;
                if (objDM_DANHMUC.PID.HasValue && objDM_DANHMUC.PID.Value > 0 && !itemNew.PID.HasValue) itemNew.PID = 0;
                JObject thongtinkhac = new JObject();
                //Dictionary<string, object> thongtinkhac = new Dictionary<string, object>();
                foreach (var p in item)
                {
                    if (p.Key == "url") continue;
                    if (itemNew.GetType().GetProperty(p.Key) == null) thongtinkhac[p.Key] = p.Value;
                }
                itemNew.DATA = JsonConvert.SerializeObject(thongtinkhac);
                _db.DM_DANHMUC_ITEMCollection.Add(itemNew);
                UpdateHasChild("DM_DANHMUC_ITEM", "PID");
                _db.HT_NGUOIDUNG_SDCollection.UpdateByDoiTuong(itemNew.ID, DsDoiTuong.DM_DANHMUC, itemNew.DANHMUC_ID.ToString(), CurrentUser.ID);
                _db.DM_DANHMUC_SDCollection.UpdateByDoiTuong(itemNew.ID, DsDoiTuong.DM_DANHMUC_ITEM, CurrentUser.ID);
                HtLog(Quyen.Them, itemNew.ID, itemNew.MA);
            }
            else if (op == "Delete")
            {
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                List<string> mess = new List<string>();
                foreach (var id in jItem)
                {
                    var objDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByID(id.Value<long>());
                    var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(objDM_DANHMUC_ITEM.DANHMUC_ID.Value);
                    NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN;
                    if (!UserAccess(Quyen.Xoa))
                    {
                        mess.Add("Không có quyền sửa phân loại " + (objDM_DANHMUC_ITEM.MA + "" == "" ? objDM_DANHMUC_ITEM.TEN : objDM_DANHMUC_ITEM.MA));
                    }
                    else
                    {
                        if (objDM_DANHMUC_ITEM != null && !objDM_DANHMUC_ITEM.HETHONG)
                        {
                            _db.DM_DANHMUC_ITEMCollection.Remove(objDM_DANHMUC_ITEM);
                            _db.DM_DANHMUC_SDCollection.RemoveByDoituong(objDM_DANHMUC_ITEM.ID, DsDoiTuong.DM_DANHMUC_ITEM);
                            HtLog(Quyen.Xoa, objDM_DANHMUC_ITEM.ID, (objDM_DANHMUC_ITEM.MA + "" == "" ? objDM_DANHMUC_ITEM.TEN : objDM_DANHMUC_ITEM.MA));
                        }
                        else
                        {
                            mess.Add("Không thể xóa phân loại hệ thống " + (objDM_DANHMUC_ITEM.MA + "" == "" ? objDM_DANHMUC_ITEM.TEN : objDM_DANHMUC_ITEM.MA));
                        }
                    }
                    UpdateHasChild("DM_DANHMUC_ITEM", "PID");
                }
                if (mess.Count > 0) return ObjectResult(mess);
            }
            else if (op == "TrangThai")
            {
                List<string> mess = new List<string>();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var uid in jItem)
                {
                    var objDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByID(uid.Value<long>());
                    var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(objDM_DANHMUC_ITEM.DANHMUC_ID.Value);
                    NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN;
                    if (!UserAccess(Quyen.Duyet) && !UserAccess(Quyen.HuyDuyet))
                    {
                        mess.Add("Không có quyền sửa phân loại " + (objDM_DANHMUC_ITEM.MA + "" == "" ? objDM_DANHMUC_ITEM.TEN : objDM_DANHMUC_ITEM.MA));
                    }
                    else
                    {
                        objDM_DANHMUC_ITEM.TRANG_THAI = item.GetValue("trangthai").Value<int>();
                        _db.DM_DANHMUC_ITEMCollection.Update(objDM_DANHMUC_ITEM);
                        HtLog(objDM_DANHMUC_ITEM.TRANG_THAI == (int)TrangThai.DA_DUYET ? Quyen.Duyet : Quyen.HuyDuyet, objDM_DANHMUC_ITEM.ID, objDM_DANHMUC_ITEM.MA + "" == "" ? objDM_DANHMUC_ITEM.TEN : objDM_DANHMUC_ITEM.MA);
                    }
                }

                if (mess.Count > 0) return ObjectResult(mess);
            }
            return new NoContentResult();
        }

        //edit
        [HttpPut("{ID}")]
        [Authorize("Bearer")]
        public IActionResult Put(long ID, [FromBody] JObject item)
        {
            
            var oldItem = _db.DM_DANHMUC_ITEMCollection.GetByID(ID);
            if (oldItem != null)
            {
                var objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByID(oldItem.DANHMUC_ID.Value);
                NhomQuyen = Resource.QuyenDM_DANHMUC_ITEM + " " + objDM_DANHMUC.TEN;
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                var updateItem = (DM_DANHMUC_ITEM)UpdateObject(oldItem, item, new string[] { "id" });
                JObject thongtinkhac = new JObject();
                if (updateItem.DATA + ""!="") thongtinkhac = JsonConvert.DeserializeObject<JObject>(updateItem.DATA);
                foreach (var p in item)
                {
                    if (p.Key == "url") continue;
                    if (updateItem.GetType().GetProperty(p.Key) == null) thongtinkhac[p.Key] = p.Value;
                }
                updateItem.DATA = JsonConvert.SerializeObject(thongtinkhac);
                _db.DM_DANHMUC_ITEMCollection.Update(updateItem);
                UpdateHasChild("DM_DANHMUC_ITEM", "PID");
                HtLog(Quyen.Sua, updateItem.ID, updateItem.MA);
            }
            return new NoContentResult();
        }
    }
}
