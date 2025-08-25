using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Drawing;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Security.Cryptography;

namespace API.Controllers
{
    [Consumes("application/json")]
    [Route("api/Dashboard")]
    [ApiController]
    public class DashboardController : BaseController
    {
        public DashboardController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) :
            base(db, hostingEnvironment, configuration)
        {
        }

        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            if (op == Resource.strMenuAdmin)
            {
                var pers = GetPermission();
                List<MenuItem> menuItems = new List<MenuItem>();
                if (pers.Count > 0 || IsAdmin)
                {
                    var items = _db.HT_VARIABLECollection.MenuItemGetAll(Resource.strMenuAdmin)?.Where(c => c.HIDEN == false).OrderBy("PID, WEIGHT").ToList();
                    if (items != null)
                        foreach (var item in items)
                        {
                            if (IsAdmin || item.HREF + "" == "" || item.HREF == "#" || (item.PERM != null && pers.FirstOrDefault(c => item.PERM.Split(',').ToList().Select(s => s.Trim().Split(';')[0]).Contains(c.QUYEN.ToString())) != null))
                                menuItems.Add(new MenuItem(item));
                        }
                }
                return ObjectResult(new { items = MenusTree.BuildToData(menuItems) });
            }
            else if (op == "ChanTrang")
            {
                string ft = HT_CAUHINH_Get<string>("FOOTER", "");
                ft = ft.Replace("{DONVI}", DVSD.TEN);
                return ObjectResult(ft);
            }
            else if (op == "TieuDeTrang")
            {
                long? lstIDS = _db.HT_TEP_SDCollection.Get(1000, "HT_CAUHINH", "LOGO").Select(s => s.TEP_ID.Value).FirstOrDefault();
                string logo = "";
                if (lstIDS.HasValue)
                {
                    var objHT_TEP = _db.HT_TEPCollection.GetByID(lstIDS.Value);
                    if (objHT_TEP != null)
                    {
                        string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                        try
                        {
                            Byte[] bytes = System.IO.File.ReadAllBytes(filePath);
                            logo = "data:" + objHT_TEP.KIEU_TEP.GetMimeTypeByWindowsRegistry() + ";base64," + Convert.ToBase64String(bytes);
                        }
                        catch
                        {
                        }
                    }

                }
                string td = HT_CAUHINH_Get<string>("TITLE", "");
                td = td.Replace("{DONVI}", DVSD.TEN);
                return ObjectResult(new { Title = td, Logo = logo });
            }
            else if (op == "Dashboard")
            {
                return ObjectResult(new { lstThongBao = _db.HT_THONGBAOCollection.Get(DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET && c.NGAY_GUI < DateTime.Now).OrderByDescending(c => c.NGAY_GUI).Select(s => new { s.ID, s.TIEU_DE, s.NGAY_GUI }).ToList() });
            }
            else if (op == "ThongKe")
            {
                var dsDVID = GetDsDM_DONVI_IDS();
                List<HT_NGUOIDUNG> lstHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.Get(DVQL_ID, dsDVID).ToList();
                var lstND_IDS = lstHT_NGUOIDUNG.Select(s => s.ID).ToList();
                List<HT_NGUOIDUNG_ON> lstHT_NGUOIDUNGON = _db.HT_NGUOIDUNG_ONCollection.Get().Where(c => lstND_IDS.Contains(c.NGUOIDUNG_ID.Value) && c.LONLINE.HasValue && c.LONLINE.Value).ToList();
                var nguoiDung = lstND_IDS.Count();
                var nguoiDungOn = lstHT_NGUOIDUNGON.GroupBy(g => g.NGUOIDUNG_ID).Count();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(DVQL_ID, dsDVID).ToList();
                return ObjectResult(new { 
                    nguoiDung = lstND_IDS.Count(), 
                    nguoiDungOn, 
                    donVi = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID).Where(c => dsDVID.Contains(c.ID)).ToList().Count, 
                    nhomNguoiDung = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.NhomQuyen, DVQL_ID).Where(c => dsDVID.Contains(c.DVSD_ID.Value)).ToList().Count,
                    soCanBo = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.CanBo, DVQL_ID).Where(c => dsDVID.Contains(c.DVSD_ID.Value)).ToList().Count,
                    soThietBi = lstTB_THIETBI.Count,
                    hetKhauHao = lstTB_THIETBI.Where(c => c.HAN_KH.HasValue && c.HAN_KH.Value < DateTime.Now).Count(),
                    hetBaoHanh = lstTB_THIETBI.Where(c => c.HAN_BH.HasValue && c.HAN_BH.Value < DateTime.Now).Count()
                });
            }
            return new BadRequestResult();
        }
    }
}
