using API.Common;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Cryptography;
using GCommon;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_NGUOIDUNG_SD")]
    [ApiController]
    public class HT_NGUOIDUNG_SDController : BaseController
    {
        public HT_NGUOIDUNG_SDController(GDBContext db) : base(db)
        {
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {

            if (op == "List")
            {
                long DOITUONG_ID = ConvertClass.ToLong(Request.Query["DOITUONG_ID"] + "", 0);
                string CHUCNANG = Request.Query["CHUCNANG"] + "";
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"] + "";

                List<HT_NGUOIDUNG_SDInfo> lstHT_NGUOIDUNG_SDInfo = new List<HT_NGUOIDUNG_SDInfo>();
                List<HT_NGUOIDUNG_SD> lstHT_NGUOIDUNG_SD = new List<HT_NGUOIDUNG_SD>();
                if (DOITUONG_ID == 0)
                {
                    lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG, CurrentUser.ID).ToList();
                }
                else
                {
                    lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).ToList();
                }
                List<HT_NGUOIDUNG> lstHT_NGUOIDUNG = GetDsHT_NGUOIDUNG().ToList();
                foreach (var obj in lstHT_NGUOIDUNG)
                {
                    var ac = lstHT_NGUOIDUNG_SD.FirstOrDefault(c => c.NGUOIDUNG_ID == obj.ID);
                    lstHT_NGUOIDUNG_SDInfo.Add(new HT_NGUOIDUNG_SDInfo { ID = ac == null ? 0 : ac.ID, NGUOIDUNG_ID = obj.ID, DOITUONG_ID = DOITUONG_ID, TEN_DANG_NHAP = obj.TEN_DANG_NHAP, CHON = ac != null, DATA = ac?.DATA });
                }
                return ObjectResult(lstHT_NGUOIDUNG_SDInfo.OrderByDescending(x=> x.CHON).ToList());
            }
            else if (op == "ListAccess")
            {
                long NGUOIDUNG_ID = ConvertClass.ToLong(Request.Query["NGUOIDUNG_ID"] + "", 0);
                string CHUCNANG = Request.Query["CHUCNANG"] + "";
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"] + "";
                List<HT_NGUOIDUNG_SDInfo> lstHT_NGUOIDUNG_SDInfo = new List<HT_NGUOIDUNG_SDInfo>();
                List<HT_NGUOIDUNG_SD> lstHT_NGUOIDUNG_SD = new List<HT_NGUOIDUNG_SD>();
                if (NGUOIDUNG_ID == 0)
                {
                    lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(NGUOIDUNG_ID, DOITUONG_LOAI, CHUCNANG, CurrentUser.ID).ToList();
                }
                else
                {
                    lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(NGUOIDUNG_ID, DOITUONG_LOAI, CHUCNANG).ToList();
                }
                //doituong
                if(DOITUONG_LOAI == DsDoiTuong.DM_DANHMUC)
                {
                    List<DM_DANHMUC_ITEM> lstDM = new List<DM_DANHMUC_ITEM>();
                    if (CHUCNANG == DsChucNang.DonVi.ToString())
                    {
                        lstDM = GetDsDM_DONVI().ToList();
                    }
                    else if (CHUCNANG == DsChucNang.NhomQuyen.ToString())
                    {
                        lstDM = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(Convert.ToInt64(CHUCNANG)).ToList();
                        if (!IsQuanTriHeThong) lstDM = lstDM.Where(c => c.ID != 2).ToList();
                    }
                    else
                    {
                        lstDM = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(Convert.ToInt64(CHUCNANG)).ToList();
                    }
                    foreach (var obj in lstDM)
                    {
                        var ac = lstHT_NGUOIDUNG_SD.FirstOrDefault(c => c.DOITUONG_ID == obj.ID);
                        lstHT_NGUOIDUNG_SDInfo.Add(new HT_NGUOIDUNG_SDInfo { ID = ac == null ? 0 : ac.ID, NGUOIDUNG_ID = NGUOIDUNG_ID, DOITUONG_ID = obj.ID, TEN_DOI_TUONG = (obj.MA+""==""?"": obj.MA + " - ") + obj.TEN, CHON = ac != null, DATA = ac?.DATA });
                    }
                }

                return ObjectResult(lstHT_NGUOIDUNG_SDInfo.OrderByDescending(x => x.CHON).ToList());
            }
            return new BadRequestResult();
        }

        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] JObject item)
        {
            long DOITUONG_ID = ConvertClass.ToLong(item["DOITUONG_ID"] + "", 0);
            string CHUCNANG = item["CHUCNANG"] + "";
            string DOITUONG_LOAI = item["DOITUONG_LOAI"] + "";

            List<HT_NGUOIDUNG_SD> listInsert = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> listUpate = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> listDelete = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).ToList();

            List<HT_NGUOIDUNG_SDInfo> lstHT_NGUOIDUNG_SDInfo = JsonConvert.DeserializeObject<List<HT_NGUOIDUNG_SDInfo>>(item["data"] + "");
            foreach (var p in lstHT_NGUOIDUNG_SDInfo)
            {
                if (p.ID == 0)
                {
                    if (p.CHON) { 
                        listInsert.Add(new HT_NGUOIDUNG_SD { ID = 0, NGUOIDUNG_ID = p.NGUOIDUNG_ID, DOITUONG_ID = DOITUONG_ID, CHUCNANG = CHUCNANG, DOITUONG_LOAI = DOITUONG_LOAI, DATA = p.DATA, ND_ID = CurrentUser.ID });
                    }
                }
                else
                {
                    HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = lstHT_NGUOIDUNG_SD.FirstOrDefault(c => c.ID == p.ID);
                    if (objHT_NGUOIDUNG_SD == null) continue;
                    if (!p.CHON)
                    {
                        listDelete.Add(objHT_NGUOIDUNG_SD);
                    }
                    else if (objHT_NGUOIDUNG_SD != null)
                    {
                        if (objHT_NGUOIDUNG_SD.DATA != p.DATA)
                        {
                            objHT_NGUOIDUNG_SD.DATA = p.DATA;
                            listUpate.Add(objHT_NGUOIDUNG_SD);
                        }
                    }
                }
            }
            var ids = lstHT_NGUOIDUNG_SDInfo.Select(s => s.ID);
            listDelete.AddRange(lstHT_NGUOIDUNG_SD.Where(c => !ids.Contains(c.ID)));
            _db.HT_NGUOIDUNG_SDCollection.Add(listInsert);
            _db.HT_NGUOIDUNG_SDCollection.Update(listUpate);
            _db.HT_NGUOIDUNG_SDCollection.Remove(listDelete);

            return new NoContentResult();
        }

        [HttpPost("Access")]
        [Authorize("Bearer")]
        public IActionResult PostAccess([FromBody] JObject item)
        {
            long NGUOIDUNG_ID = ConvertClass.ToLong(item["NGUOIDUNG_ID"] + "", 0);
            string CHUCNANG = item["CHUCNANG"] + "";
            string DOITUONG_LOAI = item["DOITUONG_LOAI"] + "";

            List<HT_NGUOIDUNG_SD> listInsert = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> listUpate = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> listDelete = new List<HT_NGUOIDUNG_SD>();
            List<HT_NGUOIDUNG_SD> lstHT_NGUOIDUNG_SD = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(NGUOIDUNG_ID, DOITUONG_LOAI, CHUCNANG).ToList();

            List<HT_NGUOIDUNG_SDInfo> lstHT_NGUOIDUNG_SDInfo = JsonConvert.DeserializeObject<List<HT_NGUOIDUNG_SDInfo>>(item["data"] + "");
            foreach (var p in lstHT_NGUOIDUNG_SDInfo)
            {
                if (p.ID == 0)
                {
                    if (p.CHON)
                    {
                        listInsert.Add(new HT_NGUOIDUNG_SD { ID = 0, NGUOIDUNG_ID = NGUOIDUNG_ID, DOITUONG_ID = p.DOITUONG_ID, CHUCNANG = CHUCNANG, DOITUONG_LOAI = DOITUONG_LOAI, DATA = p.DATA, ND_ID = CurrentUser.ID });
                    }
                }
                else
                {
                    HT_NGUOIDUNG_SD objHT_NGUOIDUNG_SD = lstHT_NGUOIDUNG_SD.FirstOrDefault(c => c.ID == p.ID);
                    if (objHT_NGUOIDUNG_SD == null) continue;
                    if (!p.CHON)
                    {
                        listDelete.Add(objHT_NGUOIDUNG_SD);
                    }
                    else if (objHT_NGUOIDUNG_SD != null)
                    {
                        if (objHT_NGUOIDUNG_SD.DATA != p.DATA)
                        {
                            objHT_NGUOIDUNG_SD.DATA = p.DATA;
                            listUpate.Add(objHT_NGUOIDUNG_SD);
                        }
                    }
                }
            }
            var ids = lstHT_NGUOIDUNG_SDInfo.Select(s => s.ID);
            listDelete.AddRange(lstHT_NGUOIDUNG_SD.Where(c => !ids.Contains(c.ID)));
            _db.HT_NGUOIDUNG_SDCollection.Add(listInsert);
            _db.HT_NGUOIDUNG_SDCollection.Update(listUpate);
            _db.HT_NGUOIDUNG_SDCollection.Remove(listDelete);

            return new NoContentResult();
        }
    }
}
