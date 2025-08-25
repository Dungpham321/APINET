using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Controllers.DanhMuc
{
    [Produces("application/json")]
    [Route("api/DanhMuc/DM_DANHMUC_SD")]
    [ApiController]
    public class DM_DANHMUC_SDController : BaseController
    {
        public DM_DANHMUC_SDController(GDBContext db) : base(db) { }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {

            if (op == "List")
            {
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                long DOITUONG_ID = ConvertClass.ToLong(Request.Query["DOITUONG_ID"] + "", 0);
                string CHUCNANG = Request.Query["CHUCNANG"] + "";
                string DOITUONG_LOAI = Request.Query["DOITUONG_LOAI"] + "";
                List<DM_DANHMUC_SDInfo> lstDM_DANHMUC_SDInfo = new List<DM_DANHMUC_SDInfo>();
                List<DM_DANHMUC_SD> lstDM_DANHMUC_SD = new List<DM_DANHMUC_SD>();
                if (DOITUONG_ID == 0)
                {
                    lstDM_DANHMUC_SD = _db.DM_DANHMUC_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG, CurrentUser.ID).ToList();
                }
                else
                {
                    lstDM_DANHMUC_SD = _db.DM_DANHMUC_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).ToList();
                }
                var lstDM_DANHMUC_ITEM = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DANHMUC_ID).ToList();
                foreach (var obj in lstDM_DANHMUC_ITEM)
                {
                    var ac = lstDM_DANHMUC_SD.FirstOrDefault(c => c.DANHMUCITEM_ID == obj.ID);
                    lstDM_DANHMUC_SDInfo.Add(new DM_DANHMUC_SDInfo { ID = ac == null ? 0 : ac.ID, DANHMUCITEM_ID = obj.ID, TEN = obj.MA + (obj.MA+""==""?"":" - ") +  obj.TEN, CHON = ac != null, DATA = ac?.DATA, C1 = ac?.C1, C2 = ac?.C2 });
                }
                return ObjectResult(lstDM_DANHMUC_SDInfo);
            }
            return new BadRequestResult();
        }
        [HttpPost]
        [Authorize("Bearer")]
        public IActionResult Post([FromBody] JObject item)
        {
            string BANG = item["BANG"] + "";
            long DOITUONG_ID = ConvertClass.ToLong(item["DOITUONG_ID"] + "", 0);
            string CHUCNANG = item["CHUCNANG"] + "";
            string DOITUONG_LOAI = item["DOITUONG_LOAI"] + "";

            List<DM_DANHMUC_SD> listInsert = new List<DM_DANHMUC_SD>();
            List<DM_DANHMUC_SD> listUpate = new List<DM_DANHMUC_SD>();
            List<DM_DANHMUC_SD> listDelete = new List<DM_DANHMUC_SD>();
            List<DM_DANHMUC_SD> lstDM_DANHMUC_SD = _db.DM_DANHMUC_SDCollection.GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).ToList();

            List<DM_DANHMUC_SDInfo> lstDM_DANHMUC_SDInfo = JsonConvert.DeserializeObject<List<DM_DANHMUC_SDInfo>>(item["data"] + "");
            foreach (var p in lstDM_DANHMUC_SDInfo)
            {
                if (p.ID == 0)
                {
                    if (p.CHON)
                    {
                        listInsert.Add(new DM_DANHMUC_SD { ID = 0, DANHMUCITEM_ID = p.DANHMUCITEM_ID, DOITUONG_ID = DOITUONG_ID, CHUCNANG = CHUCNANG, DOITUONG_LOAI = DOITUONG_LOAI, DATA = p.DATA, C1 = p.C1, C2 = p.C2, ND_ID = CurrentUser.ID });
                    }
                }
                else
                {
                    DM_DANHMUC_SD objDM_DANHMUC_SD = lstDM_DANHMUC_SD.FirstOrDefault(c => c.ID == p.ID);
                    if (objDM_DANHMUC_SD == null) continue;
                    if (!p.CHON)
                    {
                        listDelete.Add(objDM_DANHMUC_SD);
                    }
                    else if (objDM_DANHMUC_SD != null)
                    {
                        if (objDM_DANHMUC_SD.DATA != p.DATA || objDM_DANHMUC_SD.C1 != p.C1 || objDM_DANHMUC_SD.C2 != p.C2)
                        {
                            if(objDM_DANHMUC_SD.DATA != p.DATA) objDM_DANHMUC_SD.DATA = p.DATA;
                            if (objDM_DANHMUC_SD.C1 != p.C1) objDM_DANHMUC_SD.C1 = p.C1;
                            if (objDM_DANHMUC_SD.C2 != p.C2) objDM_DANHMUC_SD.C2 = p.C2;
                            listUpate.Add(objDM_DANHMUC_SD);
                        }
                    }
                }
            }
            var ids = lstDM_DANHMUC_SDInfo.Select(s => s.ID);
            listDelete.AddRange(lstDM_DANHMUC_SD.Where(c => !ids.Contains(c.ID)));
            _db.DM_DANHMUC_SDCollection.Add(listInsert);
            _db.DM_DANHMUC_SDCollection.Update(listUpate);
            _db.DM_DANHMUC_SDCollection.Remove(listDelete);

            return new NoContentResult();
        }
    }
}
