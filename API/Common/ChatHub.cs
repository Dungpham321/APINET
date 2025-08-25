using GCommon;
using GDB;
using Microsoft.AspNetCore.SignalR;
using System.Linq.Dynamic.Core;
using System.Net.Sockets;

namespace API.Common
{
    public class ChatHub: Hub
    {
        private readonly GDBContext _db;
        private IConfiguration Configuration { get; }
        public ChatHub(GDBContext db, IConfiguration configuration)
        {
            this._db = db;
            Configuration = configuration;
        }
        public override Task OnConnectedAsync()
        {
            string domain = Context.GetHttpContext().Request.Query["domain"] + "";
            var sessionID = Context.GetHttpContext().Request.Query["sessionID"];
            HT_NGUOIDUNG_ON u = new HT_NGUOIDUNG_ON();
            u.CONNECTION_ID = Context.ConnectionId;
            u.CONNECTED = DateTime.Now;
            u.LONLINE = false;
            u.DEVICE = sessionID;
            u.URL = Context.GetHttpContext().Request.Query["url"];
            var oldSession = _db.HT_NGUOIDUNG_ONCollection.GetBysessionID(sessionID).FirstOrDefault();
            if (oldSession == null)
            {
                //get config
                var urlapi = (Configuration.GetValue<string>("AppSetings:URL_API") + "").Split(',');
                var urlrpt = (Configuration.GetValue<string>("AppSetings:URL_RPT") + "").Split(',');
                var userOn = _db.HT_NGUOIDUNG_ONCollection.Get().Where(c => urlapi.Contains(c.URL_API)).ToList();
                var userOnApi = userOn.GroupBy(g => new { g.URL_API }).Select(s => new { s.Key.URL_API, SOLUONG = s.Count() }).ToList();
                var userOnRpt = userOn.GroupBy(g => new { g.URL_RPT }).Select(s => new { s.Key.URL_RPT, SOLUONG = s.Count() }).ToList();
                if (userOn.Count > 0)
                {
                    var soluong = int.MaxValue;
                    foreach (string link in urlapi)
                    {
                        var fOn = userOnApi.FirstOrDefault(c => c.URL_API == link);
                        if (fOn == null)
                        {
                            u.URL_API = link;
                            break;
                        }
                        else if (soluong > fOn.SOLUONG)
                        {
                            u.URL_API = link;
                            soluong = fOn.SOLUONG;
                        }
                    }
                    soluong = int.MaxValue;
                    foreach (string link in urlrpt)
                    {
                        var fOn = userOnRpt.FirstOrDefault(c => c.URL_RPT == link);
                        if (fOn == null)
                        {
                            u.URL_RPT = link;
                            break;
                        }
                        else if (soluong > fOn.SOLUONG)
                        {
                            u.URL_RPT = link;
                            soluong = fOn.SOLUONG;
                        }
                    }
                }
                else
                {
                    u.URL_API = urlapi[0];
                    u.URL_RPT = urlrpt[0];
                }
            }
            else
            {
                u.URL_API = oldSession.URL_API + "";
                u.URL_RPT = oldSession.URL_RPT + "";
            }
            _db.HT_NGUOIDUNG_ONCollection.Add(u);
            
            if(domain != "")
            {
                var objHT_TENMIEN = _db.HT_TENMIENCollection.GetByDIA_CHI(domain);
                List<object> lsDM_DONVI = new List<object>();
                if (objHT_TENMIEN?.TEN_MIEN + "" == "LOCAL") lsDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, objHT_TENMIEN.DVQL_ID.Value).Where(c => c.PID == null || c.PID == 0).Select("new{ID,TEN}").ToDynamicList();
                Clients.Client(Context.ConnectionId).SendAsync("url", new { 
                    u.URL_API, 
                    u.URL_RPT,
                    Title = objHT_TENMIEN?.TEN_PHAN_MEM + "",
                    objHT_TENMIEN?.DVQL_ID,
                    DM_DVQL = lsDM_DONVI,
                });
            }
            else
            {
                Clients.Client(Context.ConnectionId).SendAsync("url", new { u.URL_API, u.URL_RPT });
            }
            return base.OnConnectedAsync();
        }

        //public async Task FormLogin(string sessionID, string domain)
        //{
        //    var objHT_TENMIEN = _db.HT_TENMIENCollection.GetByDIA_CHI(domain);
        //    List<object> lsDM_DONVI = new List<object>();
        //    if (objHT_TENMIEN?.TEN_MIEN + "" == "LOCAL") lsDM_DONVI = _db.DM_DONVICollection.Get().Where(c => c.DVQL_CHA_ID == null || c.DVQL_CHA_ID == 0).Select("new{ID,TEN_DVQL}").ToDynamicList();
        //    await Clients.Client(Context.ConnectionId).SendAsync("logindata", new
        //    {
        //        Title = objHT_TENMIEN?.TEN_PHAN_MEM + "",
        //        objHT_TENMIEN?.DVQL_ID,
        //        DM_DONVI = lsDM_DONVI,
        //    });
        //}

        public async Task ChangeUrl(string sessionID, string url)
        {
            HT_NGUOIDUNG_ON u = _db.HT_NGUOIDUNG_ONCollection.GetByCONNECTION_ID(Context.ConnectionId);
            if(u != null)
            {
                u.URL = url;
                u.DISCONNECTED = DateTime.Now;
                _db.HT_NGUOIDUNG_ONCollection.Update(u);
            }
        }

        public async Task UserLogin(long NGUOIDUNG_ID, string sessionID)
        {
            HT_NGUOIDUNG_ON u = _db.HT_NGUOIDUNG_ONCollection.GetByCONNECTION_ID(Context.ConnectionId);
            if (u != null)
            {
                u.NGUOIDUNG_ID = NGUOIDUNG_ID;
                u.LONLINE = true;
                u.CONNECTED = DateTime.Now;
                u.DEVICE = sessionID;
                _db.HT_NGUOIDUNG_ONCollection.Update(u);
            }
            
            await GetTinNhan(NGUOIDUNG_ID, sessionID);
            await GetThongBao(NGUOIDUNG_ID, sessionID);
            List<HT_NGUOIDUNG_ON> ds = _db.HT_NGUOIDUNG_ONCollection.GetBysessionID(sessionID).Where(c => c.CONNECTION_ID != Context.ConnectionId).ToList();
            foreach (var ui in ds)
            {
                await Clients.Client(ui.CONNECTION_ID).SendAsync("login", u);
            }
        }

        public async Task GetTinNhan(long NGUOIDUNG_ID, string sessionID)
        {
            var tinnhan = _db.HT_TINNHANCollection.GetInfoByNGUOINHAN_ID(NGUOIDUNG_ID).Where(c => (c.LOAI_TIN == (int)LoaiTinNhan.NguoiDung && c.NGUOITAO_ID != c.NGUOINHAN_ID) || (c.LOAI_TIN == (int)LoaiTinNhan.HeThong)).OrderByDescending(c =>c.NGAY_TAO).Take(10).ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("tinnhan", new { count = tinnhan.Where(c => c.TRANG_THAI == (int)TrangThaiTinNhan.ChuaXem).Count(), items = tinnhan });
        }

        private async Task GetThongBao(long NGUOIDUNG_ID, string sessionID)
        {
            var u = _db.HT_NGUOIDUNGCollection.GetByID(NGUOIDUNG_ID);
            var thongbao = _db.HT_THONGBAOCollection.Get(u.DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET && c.NGAY_GUI < DateTime.Now).OrderByDescending(c => c.NGAY_GUI).ToList();
            await Clients.Client(Context.ConnectionId).SendAsync("thongbao", new { count = thongbao.Count, items = thongbao });
        }

        //public async Task UpdateNotify(long NGUOIDUNG_ID, string sessionID)
        //{
        //    _db.HT_NOTIFYCollection.GetByNGUOI_NHAN_ID(NGUOIDUNG_ID).Where(c => c.DAXEM == false).ToList().ForEach(a => a.DAXEM = true);
        //    _db.SaveChanges();
        //    await SendNotify(NGUOIDUNG_ID, sessionID);
        //}

        public async Task UserLogout(long NGUOIDUNG_ID, string sessionID)
        {
            List<HT_NGUOIDUNG_ON> ds = _db.HT_NGUOIDUNG_ONCollection.Get(NGUOIDUNG_ID, sessionID).ToList();//.Where(c => c.CONNECTION_ID != Context.ConnectionId)
            foreach (var u in ds)
            {
                u.NGUOIDUNG_ID = null;
                u.LONLINE = false;
                _db.HT_NGUOIDUNG_ONCollection.Update(u);
                if(u.CONNECTION_ID != Context.ConnectionId) await Clients.Client(u.CONNECTION_ID).SendAsync("logout");
            }
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            HT_NGUOIDUNG_ON u = _db.HT_NGUOIDUNG_ONCollection.GetByCONNECTION_ID(Context.ConnectionId);
            _db.HT_NGUOIDUNG_ONCollection.Remove(u);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
