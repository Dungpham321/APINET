using GDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using GCommon;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace API.Common
{
    public class BaseController : Controller
    {
        private List<HT_DOITUONG_QUYEN> _lstHT_DOITUONG_QUYEN;
        private static List<QUYEN> _lstQUYEN;

        protected readonly GDBContext _db;
        protected IWebHostEnvironment _hostingEnvironment;
        protected readonly IHubContext<ChatHub> _chatHub;
        protected IConfiguration _configuration { get; }
        protected readonly IMemoryCache _memoryCache;

        protected string MenuAdmin = Resource.strMenuAdmin;

        public BaseController(GDBContext db)
        {
            _db = db;
        }

        public BaseController(GDBContext db, IWebHostEnvironment hostingEnvironment)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
        }

        public BaseController(GDBContext db, IHubContext<ChatHub> chatHub)
        {
            _db = db;
            _chatHub = chatHub;
        }

        public BaseController(GDBContext db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        public BaseController(GDBContext db, IMemoryCache memoryCache)
        {
            _db = db;
            _memoryCache = memoryCache;
        }

        public BaseController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            _db = db;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
        }

        private NhomChucNang _nhomChucNang;
        protected NhomChucNang NhomChucNang
        {
            get { return _nhomChucNang; }
            set { _nhomChucNang = value; }
        }

        private string _nhomQuyen;
        protected string NhomQuyen
        {
            get { return _nhomQuyen; }
            set { _nhomQuyen = value; }
        }

        protected List<QUYEN> QuyenCoBan(params Quyen[] p)
        {
            List<QUYEN> items = new List<QUYEN>();
            if (p.Length == 0)
            {
                items.Add(new QUYEN(Quyen.Xem, NhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Them, NhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Sua, NhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Xoa, NhomQuyen, NhomChucNang));
            }
            else
            {
                foreach (Quyen q in p)
                {
                    items.Add(new QUYEN(q, NhomQuyen, NhomChucNang));
                }
            }
            return items;
        }
        protected List<QUYEN> QuyenCoBanThem(params Quyen[] p)
        {
            List<QUYEN> items = QuyenCoBan();
            foreach (Quyen q in p)
            {
                items.Add(new QUYEN(q, NhomQuyen, NhomChucNang));
            }
            return items;
        }

        protected List<QUYEN> QuyenCoBan(string nhomQuyen, params Quyen[] p)
        {
            List<QUYEN> items = new List<QUYEN>();
            if (p.Length == 0)
            {
                items.Add(new QUYEN(Quyen.Xem, nhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Them, nhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Sua, nhomQuyen, NhomChucNang));
                items.Add(new QUYEN(Quyen.Xoa, nhomQuyen, NhomChucNang));
            }
            else
            {
                foreach (Quyen q in p)
                {
                    items.Add(new QUYEN(q, nhomQuyen, NhomChucNang));
                }
            }
            return items;
        }

        protected MenuItem CurrentMenu
        {
            get
            {
                return _db.HT_VARIABLECollection.MenuItemGetByPath(Request.Query["url"] + "", Resource.strMenuAdmin);
            }
        }

        protected bool IsQuanTriHeThong
        {
            get
            {
                return IsAdmin || UserAccess("quantrihethongcauhinhhethongquantrihethong");
            }
        }

        protected bool IsQuanTriDonVi
        {
            get
            {
                return UserAccess("quantridonvicauhinhhethongquantrihethong");
            }
        }

        protected int IntVaiTro
        {
            get
            {
                return IsQuanTriHeThong ? 1 : IsQuanTriDonVi ? 2 : 3;
            }
        }

        private long? _DVQL_ID;
        protected long DVQL_ID
        {
            get
            {
                if (_DVQL_ID == null)
                {
                    ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
                    _DVQL_ID = claimsIdentity.Name == null ? 0 : ConvertClass.ToLong(claimsIdentity.Claims.FirstOrDefault(s => s.Type == "DVQL_ID").Value + "", 0);
                }
                if (_DVQL_ID == 0) _DVQL_ID = CurrentUser.DVQL_ID;
                return _DVQL_ID.Value;
            }
        }

        private long? _DVSD_ID;
        protected long DVSD_ID
        {
            get
            {
                if (_DVSD_ID == null)
                {
                    ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
                    _DVSD_ID = claimsIdentity.Name == null ? 0 : ConvertClass.ToLong(claimsIdentity.Claims.FirstOrDefault(s => s.Type == "DVSD_ID").Value + "", 0);
                }
                if (_DVSD_ID == 0) _DVSD_ID = CurrentUser.DVSD_ID;
                return _DVSD_ID.Value;
            }
        }

        private DM_DANHMUC_ITEM _DVQL;
        protected DM_DANHMUC_ITEM DVQL
        {
            get
            {
                if (_DVQL == null) _DVQL = _db.DM_DANHMUC_ITEMCollection.GetByID(DVQL_ID);
                return _DVQL;
            }
        }

        private DM_DANHMUC_ITEM _DVSD;
        protected DM_DANHMUC_ITEM DVSD
        {
            get
            {
                if (_DVSD == null) _DVSD = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
                return _DVSD;
            }
        }

        private long? _NGUOIDUNG_ID;
        protected long NGUOIDUNG_ID
        {
            get
            {
                if (_NGUOIDUNG_ID == null)
                {
                    ClaimsIdentity claimsIdentity = User.Identity as ClaimsIdentity;
                    _NGUOIDUNG_ID = claimsIdentity.Name == null ? 0 : ConvertClass.ToLong(claimsIdentity.Name, 0);
                }
                return _NGUOIDUNG_ID.Value;
            }
        }

        private HT_NGUOIDUNG _currentUser;
        protected HT_NGUOIDUNG CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = _db.HT_NGUOIDUNGCollection.GetByID(NGUOIDUNG_ID).Clone<HT_NGUOIDUNG>();
                    if (_currentUser == null) _currentUser = _db.HT_NGUOIDUNGCollection.GetByKhach();
                    if (_currentUser == null) _currentUser = new HT_NGUOIDUNG();
                    _currentUser.DVQL_ID = DVQL_ID;
                    _currentUser.DVSD_ID = DVSD_ID;
                }
                return _currentUser;
            }
        }

        private long? _CANBO_ID;
        protected long? CANBO_ID
        {
            get
            {
                if(_CANBO_ID == null) _CANBO_ID = _currentUser.CANBO_ID;
                return _CANBO_ID;
            }
        }

        private DM_DANHMUC_ITEM _CANBO;
        protected DM_DANHMUC_ITEM? CANBO
        {
            get
            {
                if (_CANBO == null)
                {
                    if (_currentUser.CANBO_ID.HasValue)
                    {
                        _CANBO = _db.DM_DANHMUC_ITEMCollection.GetByID(_currentUser.CANBO_ID.Value);
                    }
                }
                return _CANBO;
            }
        }

        protected bool IsAdmin
        {
            get
            {
                return CurrentUser.TEN_DANG_NHAP.ToLower() == "superadmin";
            }
        }

        protected bool UserAccess(Quyen ma)
        {
            return UserAccess(String.Format("{0}{1}{2}", ma.ToString(), _nhomQuyen.ToString(), _nhomChucNang.ToString()).CleanKey());
        }
        protected bool UserAccess(Quyen ma, string nhomquyen)
        {
            return UserAccess(String.Format("{0}{1}{2}", ma.ToString(), nhomquyen, _nhomChucNang.ToString()).CleanKey());
        }
        protected bool UserAccess(Quyen ma, string nhomquyen, NhomChucNang nhomchucnang)
        {
            return UserAccess(String.Format("{0}{1}{2}", ma.ToString(), nhomquyen, nhomchucnang.ToString()).CleanKey());
        }
        protected bool UserAccess(params Quyen[] ma)
        {
            bool checkAccess = false;
            foreach (Quyen q in ma)
            {
                checkAccess = checkAccess || UserAccess(q);
            }
            return checkAccess;
        }
        protected bool UserAccess(string name)
        {
            if (IsAdmin) return true;
            var userPermission = GetPermission();
            if (userPermission != null && userPermission.FirstOrDefault(c => c.QUYEN.ToLower() == name.ToLower()) != null) return true;
            return false;
        }

        protected List<HT_DOITUONG_QUYEN> GetPermission(HT_NGUOIDUNG curentUser = null)
        {
            if (_lstHT_DOITUONG_QUYEN == null)
            {
                if (curentUser == null) curentUser = CurrentUser;
                _lstHT_DOITUONG_QUYEN = _db.HT_DOITUONG_QUYENCollection.GetByNGUOIDUNG_ID(curentUser.ID).ToList();
            }
            return _lstHT_DOITUONG_QUYEN;
        }

        protected IEnumerable<QUYEN> GetSystemPermissionTree(List<string> InQuyen = null)
        {
            List<QUYEN> data = new List<QUYEN>();
            List<QUYEN> listPerm = GetSystemPermission();
            List<string> listChucNang = listPerm.Select(s => s.CHUC_NANG).ToList().Distinct().OrderBy(o => o).ToList();
            foreach (var chucnang in listChucNang)
            {
                string chucnangkey = chucnang.ToLower().CleanKey();
                data.Add(new QUYEN { MA = chucnangkey, TEN = chucnang, CHUC_NANG = "" });
                List<string> listNhomQuyen = listPerm.Where(c => c.CHUC_NANG == chucnang).Select(s => s.NHOM_QUYEN).ToList().Distinct().OrderBy(o => o).ToList();
                foreach (var nhomquyen in listNhomQuyen)
                {
                    string nhomquyenkey = nhomquyen.ToLower().CleanKey();
                    data.Add(new QUYEN { MA = nhomquyenkey + chucnangkey, TEN = nhomquyen, CHUC_NANG = chucnangkey });
                    var check = listPerm.Where(c => c.CHUC_NANG == chucnang && c.NHOM_QUYEN == nhomquyen).ToList();
                    List<QUYEN> listQuyen = listPerm.Where(c => c.CHUC_NANG == chucnang && c.NHOM_QUYEN == nhomquyen).ToList().Distinct().OrderBy(o => o.SAP_XEP).ToList();
                    foreach (var quyen in listQuyen)
                    {
                        if (InQuyen != null && !InQuyen.Contains(quyen.MA)) continue;
                        data.Add(new QUYEN { MA = quyen.MA, TEN = quyen.TEN, CHUC_NANG = nhomquyenkey + chucnangkey });
                    }
                }
            }
            return data;
        }

        protected void HtLog_ID(Quyen thaotac, object MA_TAC_DONG, string MO_TA, long NGUOIDUNG_ID, long DVQL_ID, long DVSD_ID)
        {
            _db.HT_LICHSUCollection.Add(new HT_LICHSU
            {
                NGUOIDUNG_ID = NGUOIDUNG_ID,
                DVQL_ID = DVQL_ID,
                DVSD_ID = DVSD_ID,
                NGAY_TAO = DateTime.Now,
                THAO_TAC = thaotac.GetEnumDescription(),
                MA_TAC_DONG = MA_TAC_DONG + "",
                MO_TA = MO_TA != null && MO_TA.Length > 500 ? MO_TA.Substring(0, 500) : MO_TA,
            });
        }
        protected void HtLog(Quyen thaotac, object MA_TAC_DONG, string mota = "")
        {
            HtLog_ID(thaotac, MA_TAC_DONG, NhomQuyen + (mota!=""? " "+mota:""), CurrentUser.ID, CurrentUser.DVQL_ID, CurrentUser.DVSD_ID);
        }
        protected void HtLog(Quyen thaotac, string nhomquyen, object MA_TAC_DONG)
        {
            HtLog_ID(thaotac, MA_TAC_DONG, nhomquyen, CurrentUser.ID, CurrentUser.DVQL_ID, CurrentUser.DVSD_ID);
        }
        protected void HtLog_T(string thaotac, string noidung)
        {
            _db.HT_LICHSUCollection.Add(new HT_LICHSU
            {
                NGUOIDUNG_ID = CurrentUser.ID,
                DVQL_ID = CurrentUser.DVQL_ID,
                DVSD_ID = CurrentUser.DVSD_ID,
                NGAY_TAO = DateTime.Now,
                THAO_TAC = thaotac,
                MA_TAC_DONG = null,
                MO_TA = noidung != null && noidung.Length > 500 ? noidung.Substring(0, 500) : noidung,
            });
        }
        protected void ResetPermission()
        {
            _lstQUYEN = null;
        }
        protected List<QUYEN> GetSystemPermission()
        {
            if (_lstQUYEN == null)
            {
                _lstQUYEN = new List<QUYEN>();
                foreach (Type t in Globals.GetAllClass())
                {
                    var m = t.GetMethods().FirstOrDefault(c => c.Name == "Permission");
                    if (m == null) continue;
                    List<QUYEN> funcs = (List<QUYEN>)t.CallFunction(m.Name);
                    _lstQUYEN = _lstQUYEN.Concat(funcs.Select(c => c)).ToList();
                }

                foreach (Type t in Globals.GetAllClass())
                {
                    var m = t.GetMethods().FirstOrDefault(c => c.Name == "Permission_Alter");
                    if (m == null) continue;
                    t.CallFunction(m.Name, _lstQUYEN, _db);
                }
            }
            return _lstQUYEN;
        }

        protected IActionResult ListAll(IQueryable items)
        {
            return ListAll(items, new string[] { });
        }

        protected IActionResult ListAll(IQueryable items, string[] acFields)
        {
            return ObjectResult(ListAllItems(items, acFields));
        }

        protected QueryResult ListAllItems(IQueryable items)
        {
            return ListAllItems(items, new string[] { });
        }

        protected QueryResult ListAllItems(IQueryable items, string[] acFields)
        {
            int totalCount = 0;
            int skip = Convert.ToInt32(Request.Query["skip"]);
            int take = Convert.ToInt32(Request.Query["take"]);
            string sort = Request.Query["sort"];
            string defaultSort = Request.Query["defaultSort"];
            string keys = Request.Query["keys"];
            string fields = Request.Query["fields"];
            string searchValue = Request.Query["searchValue"];
            JArray jFields = JsonConvert.DeserializeObject<JArray>(fields);
            string filter = Request.Query["filter"];
            //todo:
            string treeList = Request.Query["treeList"] + "";
            //string mid = Request.Query["mid"];
            if (items == null) return new QueryResult { totalCount = 0, items = new List<object>() };
            string requireTotalCount = Request.Query["requireTotalCount"];
            string dataField = Request.Query["dataField"];
            //fitelr
            if (filter != null)
            {
                JArray oFilter = JsonConvert.DeserializeObject<JArray>(filter);
                HttpContext.Session.SetInt32("posParas", 0);
                var wh = DoWhere(oFilter, jFields);
                if (wh != null && wh.Item1 != "") items = items.Where(wh.Item1, wh.Item2.ToArray());
            }
            //search
            else if (searchValue != null)
            {
                HttpContext.Session.SetInt32("posParas", 0);
                string sfilter = "";
                foreach (var jf in jFields)
                {
                    sfilter += (sfilter == "" ? "" : ",\"or\",") + "[\"" + jf + "\",\"contains\",\"" + searchValue + "\"]";
                }
                JArray oFilter = JsonConvert.DeserializeObject<JArray>("[" + sfilter + "]");
                var wh = DoWhere(oFilter, jFields);
                if (wh != null && wh.Item1 != "") items = items.Where(wh.Item1, wh.Item2.ToArray());
            }
            //sort
            if (sort != null)
            {
                List<JObject> joSort = JsonConvert.DeserializeObject<List<JObject>>(sort);
                string oD = "";
                foreach (JObject obSort in joSort)
                {
                    if (jFields.FirstOrDefault(c => c + "" == obSort["selector"].Value<string>()) == null) continue;
                    oD += (oD != "" ? "," : "") + obSort["selector"].Value<string>() + (obSort["desc"].Value<bool>() ? " desc" : "");
                }
                if (oD != "") items = items.OrderBy(oD);
            }
            else
            {
                JArray jDefaultSort = JsonConvert.DeserializeObject<JArray>(defaultSort);
                if (jDefaultSort.Count > 0) items = items.OrderBy(String.Join(",", jDefaultSort));
            }
            //return
            if (dataField != null)
            {
                if (jFields.FirstOrDefault(c => c + "" == dataField) != null) items = items.Select(String.Format("new({0})", dataField));
            }
            else
            {
                if (requireTotalCount == null && dataField == null && take == 0)
                {
                    JArray jKeys = JsonConvert.DeserializeObject<JArray>(keys);
                    items = items.Select(String.Format("new({0})", String.Join(", ", jKeys)));
                }
                else
                {
                    if (acFields.Length > 0)
                        foreach (var jf in jFields)
                            if (!acFields.Contains(jf.Value<string>())) jFields.Remove(jf);
                    JArray jKeys = JsonConvert.DeserializeObject<JArray>(keys);
                    for (int i = 0; i < jKeys.Count; i++)
                    {
                        if (!jFields.Any(t => t.Value<string>() == jKeys[i] + "")) jFields.Add(jKeys[i]);
                    }
                    items = items.Select(String.Format("new({0})", String.Join(", ", jFields)));
                }
            }
            if (dataField == null && requireTotalCount != null && requireTotalCount == "true") totalCount = items.Count();
            if (take > 0) items = items.Skip(skip).Take(take);
            return new QueryResult { totalCount = totalCount, items = items.ToDynamicList() };
        }

        private Tuple<string, List<object>> DoWhere(JArray oFilter, JArray fields)
        {
            string sql = "";
            List<object> pars = new List<object>();
            if (oFilter[0].GetType() == typeof(JArray))
            {
                string nextAnd = "";
                foreach (object f in oFilter)
                {
                    if (f.GetType() == typeof(JArray))
                    {
                        var cf = (JArray)f;
                        var res = DoWhere(cf, fields);
                        if (res == null) continue;
                        sql += (nextAnd != "" ? nextAnd : "") + res.Item1;
                        pars.AddRange(res.Item2);
                    }
                    else
                    {
                        if (sql != "") nextAnd = f.ToString() == "and" ? " AND " : " OR ";
                    }
                }
                if (nextAnd != "") sql = "(" + sql + ")";
            }
            else
            {
                string field = oFilter[0].ToString();
                if (field == "!")
                {
                    var res = DoWhere((JArray)oFilter[1], fields);
                    if (res != null)
                    {
                        sql += "NOT (" + res.Item1 + ")";
                        pars.AddRange(res.Item2);
                    }
                }
                else
                {

                    if (!fields.Any(t => t.Value<string>() == field)) return null;
                    var posParas = HttpContext.Session.GetInt32("posParas").Value;
                    bool typeString = true;
                    switch (oFilter[1].ToString())
                    {
                        case "contains":
                            sql = String.Format((oFilter[2].ToString().Trim() != "" ? "{0} != null && " : "") + "{0}.ToLower().Contains(@{1})", field, posParas);
                            break;
                        case "notcontains":
                            sql = String.Format((oFilter[2].ToString().Trim() != "" ? "{0} != null && " : "") + "!{0}.ToLower().Contains(@{1})", field, posParas);
                            break;
                        case "startswith":
                            sql = String.Format((oFilter[2].ToString().Trim() != "" ? "{0} != null && " : "") + "{0}.ToLower().StartsWith(@{1})", field, posParas);
                            break;
                        case "endswith":
                            sql = String.Format((oFilter[2].ToString().Trim() != "" ? "{0} != null && " : "") + "{0}.ToLower().EndsWith(@{1})", field, posParas);
                            break;
                        case "=":
                        case "<":
                        case ">":
                        case "<=":
                        case ">=":
                            sql = String.Format((oFilter[2].ToString().Trim() != ""? "{0} != null && " : "") + "{0} {2} @{1}", field, posParas, oFilter[1].ToString());
                            typeString = false;
                            break;
                        case "<>":
                            sql = String.Format((oFilter[2].ToString().Trim() != "" ? "{0} != null && " : "") + "{0} != (@{1})", field, posParas);
                            typeString = false;
                            break;
                    }
                    if (oFilter[2].Type == JTokenType.String) typeString = true;
                    if (typeString) pars.Add(oFilter[2].ToString().ToLower().Trim());
                    else
                    {
                        if (oFilter[2].ToString().Trim() == "") pars.Add(null);
                        else
                        {
                            try
                            {
                                var a = oFilter[2].Value<DateTime>();
                                pars.Add(a);
                            }
                            catch
                            {
                                try
                                {
                                    pars.Add(Convert.ToDecimal(oFilter[2].ToString()));
                                }
                                catch { pars.Add(oFilter[2].ToString()); }
                            }
                        }
                    }
                    HttpContext.Session.SetInt32("posParas", posParas + 1);
                }

            }
            return Tuple.Create(sql, pars);
        }

        protected object UpdateObject(object oldItem, JObject item, string[] key, bool inField = false)
        {
            var pos = oldItem.GetType().GetProperties();
            foreach (var po in pos)
            {
                if (!inField && key.Contains(po.Name.ToLower())) continue;
                if (inField && !key.Contains(po.Name.ToLower())) continue;
                if (item[po.Name] != null)
                {
                    if (po.PropertyType.BaseType == typeof(object))
                    {
                        po.SetValue(oldItem, item[po.Name].ToObject(po.PropertyType));
                    }
                    else
                    {
                        var t = Nullable.GetUnderlyingType(po.PropertyType) ?? po.PropertyType;
                        var v = item[po.Name];
                        if (v + "" == "" && (t == typeof(int) || t == typeof(long) || t == typeof(bool) || t == typeof(decimal)))
                        {
                            var nullable = System.Nullable.GetUnderlyingType(po.PropertyType);
                            if (nullable != null) v = null;
                            else v = "0";
                        }
                        else if (t == typeof(DateTime) && v + "" == "")
                        {
                            v = null;
                        }
                        po.SetValue(oldItem, v == null ? v : Convert.ChangeType(v, t));
                    }
                }

            }
            return oldItem;
        }

        protected JObject UpdateJObject(JObject oldItem, JObject item, string[] key, bool inField = false)
        {
            foreach (var p in item.Properties())
            {
                if (!inField && key.Contains(p.Name.ToLower())) continue;
                if (inField && !key.Contains(p.Name.ToLower())) continue;
                oldItem[p.Name] = p.Value;
            }
            return oldItem;
        }

        protected ObjectResult ObjectResult(object data, string mess = "", RequestState res = RequestState.Success)
        {
            return new ObjectResult(new RequestResult
            {
                State = res,
                Data = data,
                Msg = mess,
            });
        }

        protected ObjectResult UserAccessDenied()
        {
            return ObjectResult(null, Resource.strKhongCoQuyen, RequestState.NotAuth);
        }
      
        protected string GetEnumName(object num)
        {
            return Enum.GetName(num.GetType(), num) + "";
        }
        protected string GetEnumName(Type type, object value)
        {
            return Enum.GetName(type, value) + "";
        }

        protected int GetEnumValue(Type type, string name)
        {
            return (int)Enum.Parse(type, name);
        }

        protected string CreateToken()
        {
            JObject playload = new JObject();
            playload["user"] = CurrentUser.TEN_DANG_NHAP;
            playload["time"] = DateTime.Now.Ticks * 5 / 2;
            using (var hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes("GSToken")))
            {
                string playloadEnCode = JsonConvert.SerializeObject(playload).Base64Encode();
                return Convert.ToBase64String(hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(playloadEnCode))) + "." + playloadEnCode;
            }
        }

        protected void RemoveFolder(string folder)
        {
            DirectoryInfo dir = new DirectoryInfo(folder);
            foreach (FileInfo file in dir.GetFiles())
            {
                file.Delete();
            }
            Directory.Delete(folder);
        }

        protected string GenerateToken(HT_NGUOIDUNG objHT_NGUOIDUNG, DateTime expires)
        {
            var handler = new JwtSecurityTokenHandler();

            ClaimsIdentity identity = new ClaimsIdentity(
                new GenericIdentity(objHT_NGUOIDUNG.ID.ToString(), "TokenAuth"),
                new[] {
                    new Claim("DVQL_ID", objHT_NGUOIDUNG.DVQL_ID+""),
                    new Claim("DVSD_ID", objHT_NGUOIDUNG.DVSD_ID+""),
                }
            );

            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = TokenAuthOption.Issuer,
                Audience = TokenAuthOption.Audience,
                SigningCredentials = TokenAuthOption.SigningCredentials,
                Subject = identity,
                Expires = expires
            });
            return handler.WriteToken(securityToken);
        }

        protected T HT_CAUHINH_Get<T>(string name, object defaultValue = null, long dvql_id = 0)
        {
            if (dvql_id == 0) dvql_id = DVQL_ID;
            JObject data = _db.HT_VARIABLECollection.VariableGet<JObject>("SystemConfig", "HT_CAUHINH_" + dvql_id, new JObject());
            return data[name] == null ? (defaultValue == null ? default(T) : (T)defaultValue) : data[name].Value<T>();
        }

        protected T HT_CAUHINHDV_Get<T>(string name, object defaultValue = null, long dvsd_id = 0)
        {
            if (dvsd_id == 0) dvsd_id = DVSD_ID;
            JObject data = _db.HT_VARIABLECollection.VariableGet<JObject>("DvConfig", "HT_CAUHINHDV_" + dvsd_id, new JObject());
            return data[name] == null ? (defaultValue == null ? default(T) : (T)defaultValue) : data[name].Value<T>();
        }

        //tinnhan
        protected void CreateTinNhan(string TieuDe, string NoiDung, long NguoiNhan)
        {
            List<long> lstNguoiNhan = new List<long>();
            lstNguoiNhan.Add(NguoiNhan);
            CreateTinNhan(TieuDe, NoiDung, lstNguoiNhan);
        }
        protected void CreateTinNhan(string TieuDe, string NoiDung, List<long> NguoiNhan)
        {
            HT_TINNHAN tinnhan = new HT_TINNHAN();
            tinnhan.NGUOITAO_ID = CurrentUser.ID;
            tinnhan.NGAY_TAO = DateTime.Now;
            tinnhan.TIEU_DE = TieuDe;
            tinnhan.NOI_DUNG = NoiDung;
            tinnhan.LOAI_TIN = (int)LoaiTinNhan.HeThong;
            _db.HT_TINNHANCollection.Add(tinnhan);
            foreach (long NGUOIDUNG_ID in NguoiNhan)
            {
                HT_TINNHAN_ND objHT_TINNHAN_ND = new HT_TINNHAN_ND();
                objHT_TINNHAN_ND.NGUOINHAN_ID = NGUOIDUNG_ID;
                objHT_TINNHAN_ND.TINNHAN_ID = tinnhan.ID;
                objHT_TINNHAN_ND.TRANG_THAI = (int)TrangThaiTinNhan.ChuaXem;
                _db.HT_TINNHAN_NDCollection.Add(objHT_TINNHAN_ND);
            }
            foreach (long NGUOIDUNG_ID in NguoiNhan)
            {
                SendChatTinNhan(NGUOIDUNG_ID);
            }
        }
        protected void SendChatTinNhan(long NGUOI_NHAN_ID)
        {
            var tinnhan = _db.HT_TINNHANCollection.GetInfoByNGUOINHAN_ID(NGUOI_NHAN_ID).Where(c => (c.LOAI_TIN == (int)LoaiTinNhan.NguoiDung && c.NGUOITAO_ID != c.NGUOINHAN_ID) || (c.LOAI_TIN == (int)LoaiTinNhan.HeThong)).OrderByDescending(c => c.NGAY_TAO).Take(10).ToList();
            AddChatMessage(NGUOI_NHAN_ID, "tinnhan", new { count = tinnhan.Where(c => c.TRANG_THAI == (int)TrangThaiTinNhan.ChuaXem).Count(), items = tinnhan });
        }
        protected void SendChatThongBao(long NGUOI_NHAN_ID)
        {
            var thongbao = _db.HT_THONGBAOCollection.Get(DVSD_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET && c.NGAY_GUI < DateTime.Now).OrderByDescending(c => c.NGAY_GUI).ToList();
            AddChatMessage(NGUOI_NHAN_ID, "thongbao", new { count = thongbao.Count, items = thongbao });
        }
        private void AddChatMessage(long NGUOIDUNG_ID, string action, object data)
        {
            if (_chatHub == null) return;
            var ds = _db.HT_NGUOIDUNG_ONCollection.Get(NGUOIDUNG_ID);
            foreach (var u in ds)
            {
                _chatHub.Clients.Client(u.CONNECTION_ID).SendAsync(action, data);
            }
        }

        protected IQueryable<DM_DANHMUC_ITEM> GetLcDM_DONVI(HT_NGUOIDUNG nguoidung = null)
        {
            if (nguoidung != null) _currentUser = nguoidung;
            if (IsQuanTriHeThong)
            {
                return _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID);
            }
            else if (IsQuanTriDonVi)
            {
                List<long> lsDM_DVSDTS = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(CurrentUser.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString()).Select(s => s.DOITUONG_ID).ToList();
                if (lsDM_DVSDTS.Count == 0) lsDM_DVSDTS.Add(DVSD_ID);
                List<DM_DANHMUC_ITEM> lstDM_DONVI = new List<DM_DANHMUC_ITEM>();
                foreach (long dvid in lsDM_DVSDTS)
                {
                    lstDM_DONVI = lstDM_DONVI.Union(_db.DM_DANHMUC_ITEMCollection.GetTree(DsChucNang.DonVi, dvid, DVQL_ID).ToList()).ToList();
                }
                return lstDM_DONVI.Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET).AsQueryable();
            }
            else
            {
                List<long> lsDM_DVSDTS = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(CurrentUser.ID, DsDoiTuong.DM_DANHMUC, DsChucNang.DonVi.ToString()).Select(s => s.DOITUONG_ID).ToList();
                if (lsDM_DVSDTS.Count == 0) lsDM_DVSDTS.Add(DVSD_ID);
                return _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID).Where(c => c.TRANG_THAI == (int)TrangThai.DA_DUYET && lsDM_DVSDTS.Contains(c.ID));
            }
        }
        protected IQueryable<DM_DANHMUC_ITEM> GetDsDM_DONVI()
        {
            if (IsQuanTriHeThong)
            {
                return _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID);
            }
            else if (IsQuanTriDonVi)
            {
                return _db.DM_DANHMUC_ITEMCollection.GetTree(DsChucNang.DonVi, DVSD_ID, DVQL_ID);
            }
            else
            {
                return _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID).Where(c => c.ID == DVSD_ID);
            }
        }

        protected List<long> GetDsDM_DONVI_IDS()
        {
            var ds = GetDsDM_DONVI().ToList();
            if (ds != null) return ds.Select(s => s.ID).ToList();
            else return new List<long>();
        }

        protected List<HT_TEP> GetFile(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).ToList();
            return _db.HT_TEPCollection.Get(lstHT_TEP_SD.Select(s => s.TEP_ID).ToList()).ToList();
        }

        protected void SaveFile(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)//, List<long> DT_ACCESS = null, int DOITUONG_LOAI_ACCESS = (int)LOAI_DOI_TUONG.NGUOI_DUNG
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(0, DOITUONG_LOAI, ND_ID).ToList();
            if (lstHT_TEP_SD.Count > 0)
            {
                lstHT_TEP_SD.ForEach(c => c.DOITUONG_ID = DOITUONG_ID);
                _db.HT_TEP_SDCollection.Update(lstHT_TEP_SD);
                List<HT_TEP> lstHT_TEP = _db.HT_TEPCollection.Get(lstHT_TEP_SD.Select(s => s.TEP_ID).ToList()).ToList();
                lstHT_TEP.ForEach(c => c.TRANG_THAI = (int)TrangThaiTep.DANG_SU_DUNG);
                _db.HT_TEPCollection.Update(lstHT_TEP);
            }
        }

        protected void SaveFile(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI, long ND_ID)//, List<long> DT_ACCESS = null, int DOITUONG_LOAI_ACCESS = (int)LOAI_DOI_TUONG.NGUOI_DUNG
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(0, CHUCNANG, DOITUONG_LOAI, ND_ID).ToList();
            if (lstHT_TEP_SD.Count > 0)
            {
                lstHT_TEP_SD.ForEach(c => c.DOITUONG_ID = DOITUONG_ID);
                _db.HT_TEP_SDCollection.Update(lstHT_TEP_SD);
                List<HT_TEP> lstHT_TEP = _db.HT_TEPCollection.Get(lstHT_TEP_SD.Select(s => s.TEP_ID).ToList()).ToList();
                lstHT_TEP.ForEach(c => c.TRANG_THAI = (int)TrangThaiTep.DANG_SU_DUNG);
                _db.HT_TEPCollection.Update(lstHT_TEP);
            }
        }

        protected void SaveFile(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI, string NewCHUCNANG)
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(0, CHUCNANG, DOITUONG_LOAI).ToList();
            if (lstHT_TEP_SD.Count > 0)
            {
                lstHT_TEP_SD.ForEach(c => { c.DOITUONG_ID = DOITUONG_ID; c.CHUCNANG = NewCHUCNANG; });
                _db.HT_TEP_SDCollection.Update(lstHT_TEP_SD);
                List<HT_TEP> lstHT_TEP = _db.HT_TEPCollection.Get(lstHT_TEP_SD.Select(s => s.TEP_ID).ToList()).ToList();
                lstHT_TEP.ForEach(c => c.TRANG_THAI = (int)TrangThaiTep.DANG_SU_DUNG);
                _db.HT_TEPCollection.Update(lstHT_TEP);
            }
        }

        protected void RemoveFile(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).ToList();
            foreach (var objHT_TEP_SD in lstHT_TEP_SD)
            {
                _db.HT_TEP_SDCollection.Remove(objHT_TEP_SD);
            }
            //check trang thai file
            foreach (long fid in lstHT_TEP_SD.Select(s => s.TEP_ID).ToList())
            {
                var checkSD = _db.HT_TEP_SDCollection.GetByTEP_ID(fid).ToList();
                if (checkSD.Count == 0)
                {
                    HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(fid);
                    _db.HT_TEPCollection.Remove(objHT_TEP);
                    
                    _db.HT_TEP_SDCollection.RemoveByTEP_ID(objHT_TEP.ID);
                    string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                    try
                    {
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }
                    catch { }
                    HtLog(Quyen.Xoa, objHT_TEP.ID);
                }
            }
        }

        protected void RemoveFile(long DOITUONG_ID, string CHUCNANG)
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, CHUCNANG).ToList();
            foreach (var objHT_TEP_SD in lstHT_TEP_SD)
            {
                _db.HT_TEP_SDCollection.Remove(objHT_TEP_SD);
            }
            //check trang thai file
            foreach (long fid in lstHT_TEP_SD.Select(s => s.TEP_ID).ToList())
            {
                var checkSD = _db.HT_TEP_SDCollection.GetByTEP_ID(fid).ToList();
                if (checkSD.Count == 0)
                {
                    HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(fid);
                    _db.HT_TEPCollection.Remove(objHT_TEP);

                    _db.HT_TEP_SDCollection.RemoveByTEP_ID(objHT_TEP.ID);
                    string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                    try
                    {
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }
                    catch { }
                    HtLog(Quyen.Xoa, objHT_TEP.ID);
                }
            }
        }

        protected void RemoveFile(long DOITUONG_ID, long ND_ID)
        {
            var lstHT_TEP_SD = _db.HT_TEP_SDCollection.Get(DOITUONG_ID, ND_ID).ToList();
            foreach (var objHT_TEP_SD in lstHT_TEP_SD)
            {
                _db.HT_TEP_SDCollection.Remove(objHT_TEP_SD);
            }
            //check trang thai file
            foreach (long fid in lstHT_TEP_SD.Select(s => s.TEP_ID).ToList())
            {
                var checkSD = _db.HT_TEP_SDCollection.GetByTEP_ID(fid).ToList();
                if (checkSD.Count == 0)
                {
                    HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(fid);
                    _db.HT_TEPCollection.Remove(objHT_TEP);

                    _db.HT_TEP_SDCollection.RemoveByTEP_ID(objHT_TEP.ID);
                    string filePath = GetRootPath + objHT_TEP.DUONG_DAN;
                    try
                    {
                        if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
                    }
                    catch { }
                }
            }
        }

        protected HT_TEP AccessFile(long ID)
        {
            HT_TEP objHT_TEP = _db.HT_TEPCollection.GetByID(ID);
            if (objHT_TEP == null) return null;
            //if (objHT_TEP.NGUOITAO_ID == CurrentUser.ID) return objHT_TEP;
            //if (_db.HT_TEP_ACCollection.Get(CurrentUser.ID, (int)LOAI_DOI_TUONG.NGUOI_DUNG, ID).Where(c => c.Q_XEM == true).ToList().Count() > 0) return objHT_TEP;
            return objHT_TEP;
        }

        protected IQueryable<HT_NGUOIDUNG> GetDsHT_NGUOIDUNG(bool all = true)
        {
            IQueryable<HT_NGUOIDUNG> result = _db.HT_NGUOIDUNGCollection.Get(DVQL_ID, GetDsDM_DONVI_IDS());
            if (!all) result = result.Where(c => c.ID != CurrentUser.ID);
            return result;
        }
       
        protected void UpdateHasChild(string table, string fieldCha = "PID")
        {
            string sql = String.Format("UPDATE {0} SET HASCHILD = (SELECT CASE WHEN count(*) > 0 THEN 1 ELSE 0 END FROM {0} dt WHERE dt.{1}={0}.ID)", table, fieldCha);
            _db.Database.ExecuteSqlRaw(sql);
        }

        protected void UpdateColumnConfig(JObject item)
        {
            var objHT_GRID = _db.HT_GRIDCollection.GetByID(item["id"] + "");
            if (objHT_GRID == null)
            {
                objHT_GRID = new HT_GRID { GRID_ID = item["id"] + "" };
                objHT_GRID.CAUHINH = JsonConvert.SerializeObject(item["config"].Value<JArray>());
                _db.HT_GRIDCollection.Add(objHT_GRID);
            }
            else
            {
                objHT_GRID.CAUHINH = JsonConvert.SerializeObject(item["config"].Value<JArray>());
                _db.HT_GRIDCollection.Update(objHT_GRID);
            }
        }

        protected string GetRootPath
        {
            get
            {
                string filepath = _configuration.GetValue<string>("AppSetings:FILE_PATH") + "";
                if (filepath == "") filepath = _hostingEnvironment.WebRootPath;
                return filepath;
            }
        }

        //protected string TaoMaHoSo(HS_HOSO item, string thamso = "MA_HO_SO")
        //{
        //    string mahoso = HT_CAUHINH_Get<string>(thamso, "@madonvi-@lv-@ngay@thang@nam-@soluonghs");
        //    if (mahoso.IndexOf("@madonvi") >= 0) mahoso = mahoso.Replace("@madonvi", DVSD.MA + "");
        //    if(mahoso.IndexOf("@lv") >= 0)
        //    {
        //        var objDM_LINHVUC = _db.DM_DANHMUC_ITEMCollection.GetByID(item.LINHVUC_ID.Value);
        //        mahoso = mahoso.Replace("@lv", objDM_LINHVUC.MA + "");
        //    }
        //    DateTime ngaytao = item.NGAY_TAO.HasValue ? item.NGAY_TAO.Value : DateTime.Now;
        //    mahoso = mahoso.Replace("@nam", ngaytao.Year + "");
        //    mahoso = mahoso.Replace("@ngay", (ngaytao.Day + "").PadLeft(2, '0'));
        //    mahoso = mahoso.Replace("@thang", (ngaytao.Month + "").PadLeft(2, '0'));
        //    if (mahoso.IndexOf("@sotutang") >= 0)
        //    {
        //        long soluong = _db.HS_HOSOCollection.GetSoHoSo();
        //        mahoso = mahoso.Replace("@sotutang", (soluong < 999 ? soluong.ToString().PadLeft(4, '0') : soluong) + "");
        //    }
        //    if (mahoso.IndexOf("@soluonghs") >= 0)
        //    {
        //        long soluong = _db.HS_HOSOCollection.GetSoHoSo(DVSD_ID) + 1;
        //        mahoso = mahoso.Replace("@soluonghs", (soluong < 999 ? soluong.ToString().PadLeft(4, '0') : soluong) + "");
        //    }
        //    return mahoso;
        //}

        //protected void CALL_HS_FORM_DATA_Alter(HS_FORM_DATA objHS_FORM_DATA)
        //{
        //    foreach (Type t in Globals.GetAllClass("API.Controllers.HoSo"))
        //    {
        //        var m = t.GetMethods().FirstOrDefault(c => c.Name == "HS_FORM_DATA_Alter");
        //        if (m == null) continue;
        //        t.CallFunction(m.Name, objHS_FORM_DATA, _db, CurrentUser);
        //    }
        //}
    }
}
