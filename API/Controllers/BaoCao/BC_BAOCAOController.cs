using API.Common;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq.Dynamic.Core;

namespace API.Controllers.BaoCao
{
    [Consumes("application/json")]
    [Route("api/BaoCao/BC_BAOCAO")]
    [ApiController]
    public class BC_BAOCAOController : BaseController
    {
        public BC_BAOCAOController(GDBContext db, IWebHostEnvironment hostingEnvironment, IConfiguration configuration) : base(db, hostingEnvironment, configuration)
        {
            NhomChucNang = NhomChucNang.BaoCao;
            NhomQuyen = Resource.QuyenBC_BAOCAO;
        }

        public static List<QUYEN> Permission()
        {
            BC_BAOCAOController mn = new BC_BAOCAOController(null, null, null);
            return mn.QuyenCoBan(Quyen.Xem, Quyen.Sua, Quyen.Them, Quyen.Xoa);
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
                return ListAll(_db.BC_BAOCAOCollection.Get());
            }
            else if (op == "Check")
            {
                string ID = Request.Query["ID"];
                long OldID = ConvertClass.ToLong(Request.Query["OldID"] + "", 0);
                if (ID + "" == "") return new BadRequestResult();
                BC_BAOCAO objBC_BAOCAO = _db.BC_BAOCAOCollection.GetByMA(ID);
                return ObjectResult(new { Check = objBC_BAOCAO != null && objBC_BAOCAO.ID != OldID });
            }
            else if (op == "GetData")
            {
                var ID = ConvertClass.ToLong(Request.Query["ID"] + "", 0);
                var item = _db.BC_BAOCAOCollection.GetByID(ID);
                return ObjectResult(item?.CAU_HINH);
            }
            else if (op == "GetKhoGiay")
            {
                BC_BAOCAO baocao = _db.BC_BAOCAOCollection.GetByMA(Request.Query["MA"] + "");
                return ObjectResult((baocao?.KHOGIAY+"").Split(','));
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
                var itemNew = item.ToObject<BC_BAOCAO>();
                if ((itemNew.CAU_HINH + "").StartsWith("copy"))
                {
                    var itemOld = _db.BC_BAOCAOCollection.GetByID(Convert.ToInt64((itemNew.CAU_HINH + "").Replace("copy", "")));
                    itemNew.CAU_HINH = itemOld?.CAU_HINH + "";
                    itemNew.ID = 0;
                }
                _db.BC_BAOCAOCollection.Add(itemNew);
                HtLog(Quyen.Them, itemNew.ID, itemNew.MA);
            }
            else if (op == "Delete")
            {
                if (!UserAccess(Quyen.Xoa)) return UserAccessDenied();
                var jItem = JsonConvert.DeserializeObject<JArray>(item.GetValue("items").Value<string>());
                foreach (var id in jItem)
                {
                    BC_BAOCAO objBC_BAOCAO = _db.BC_BAOCAOCollection.GetByID(id.Value<long>());
                    _db.BC_BAOCAOCollection.Remove(objBC_BAOCAO);
                    HtLog(Quyen.Xoa, objBC_BAOCAO.ID, objBC_BAOCAO.MA);
                }
            }
            else if (op == "Design")
            {
                if (!UserAccess(Quyen.Sua)) return UserAccessDenied();
                var request = item["config"] + "";
                if (request != "")
                {
                    var UpdateReport = _db.BC_BAOCAOCollection.GetByID(item["ID"].Value<long>());
                    UpdateReport.CAU_HINH = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(request));
                    _db.BC_BAOCAOCollection.Update(UpdateReport);
                    HtLog(Quyen.Sua, UpdateReport.ID, UpdateReport.MA);
                }
            }
            else if(op == "TraCuu")
            {
                try
                {
                    BC_BAOCAO baocao = null;
                    if(item["KHOGIAY"] + "" != "") baocao = _db.BC_BAOCAOCollection.GetByMA(item["MA"] + "" + item["KHOGIAY"]);
                    if (baocao == null) baocao = _db.BC_BAOCAOCollection.GetByMA(item["MA"] + "");
                    if (baocao == null) return ObjectResult(null, Resource.strBaoCaoKhongTonTai, RequestState.message);
                    foreach (Type t in Globals.GetAllClass("API.Common"))
                    {
                        var m = t.GetMethods().FirstOrDefault(c => c.Name == baocao.HAM_THUC_HIEN + "_Pre_Alter");
                        if (m == null) continue;
                        baocao = (BC_BAOCAO) t.CallFunction(m.Name, item, _db);
                    }

                    var data = new Dictionary<string, object>();
                    Dictionary<string, object> variables = new Dictionary<string, object>();
                    Dictionary<string, object> datasource = new Dictionary<string, object>();
                    List<string> pars = new List<string>();
                    //pars.Add(String.Format("p_{0}:{1}", "MA_BAO_CAO", item["MA_BAO_CAO"]));
                    //if (item["ID"] + "" != "") pars.Add(String.Format("p_{0}:{1}", "ID", item["ID"]));

                    string[] thamso = (baocao.THAM_SO + "").Split(',');
                    foreach (string ts in thamso)
                    {
                        if (ts == "" || ts == "KHOGIAY") continue;
                        pars.Add(String.Format("p_{0}:{1}", ts, item[ts]));
                        //if (item[ts] != null)
                        //{

                        //}
                    }

                    Dictionary<string, object> lstDataSource = new Dictionary<string, object>();
                    Dictionary<string, object> dsDataSource = null;
                    if (baocao.HAM_THUC_HIEN + "" != "") dsDataSource = _db.BC_TONGHOPCollection.Get(baocao.HAM_THUC_HIEN, String.Join(";", pars), baocao.BANG_DU_LIEU);
                    if (dsDataSource != null)
                    {
                        if (baocao.DIEU_KIEN_NHOM + "" != "" && dsDataSource.Count > 0)
                        {
                            var subGroup = baocao.DIEU_KIEN_NHOM.Split('-');
                            bool checkResult = false;
                            foreach (KeyValuePair<string, object> dataS in dsDataSource)
                            {
                                lstDataSource.Add("ds" + dataS.Key, dataS.Value);
                                if (dataS.Key == "BC_TONGHOPA")
                                {
                                    List<BC_TONGHOPA> dtSource = (List<BC_TONGHOPA>)dataS.Value;
                                    if (dtSource.Count == 0) continue;
                                    checkResult = true;
                                    foreach (string sgroup in subGroup)
                                    {
                                        if (sgroup == "") continue;
                                        string jgroup = "";
                                        string select = "";
                                        var lsGroup = sgroup.Split(';');
                                        foreach (string group in lsGroup)
                                        {
                                            if (group == "") continue;
                                            jgroup += (jgroup == "" ? "" : ",") + group;
                                            select += (select == "" ? "" : ",") + "it.Key." + group.Replace(",", ",it.Key.");
                                            lstDataSource.Add("ds" + dataS.Key + group, dtSource.AsQueryable().GroupBy("new(" + jgroup + ")", "it").Select("new(" + select + ")").ToDynamicList());
                                        }
                                    }
                                }
                                else if (dataS.Key == "BC_TONGHOPB")
                                {
                                    List<BC_TONGHOPB> dtSource = (List<BC_TONGHOPB>)dataS.Value;
                                    if (dtSource.Count == 0) continue;
                                    checkResult = true;
                                    foreach (string sgroup in subGroup)
                                    {
                                        if (sgroup == "") continue;
                                        string jgroup = "";
                                        string select = "";
                                        var lsGroup = sgroup.Split(';');
                                        foreach (string group in lsGroup)
                                        {
                                            if (group == "") continue;
                                            jgroup += (jgroup == "" ? "" : ",") + group;
                                            select += (select == "" ? "" : ",") + "it.Key." + group.Replace(",", ",it.Key.");
                                            lstDataSource.Add("ds" + dataS.Key + group, dtSource.AsQueryable().GroupBy("new(" + jgroup + ")", "it").Select("new(" + select + ")").ToDynamicList());
                                        }
                                    }
                                }
                            }
                            if (!checkResult) return ObjectResult(null, "Không có dữ liệu phù hợp", RequestState.message);
                        }
                        else
                        {
                            foreach (KeyValuePair<string, object> dataS in dsDataSource)
                            {
                                lstDataSource.Add("ds" + dataS.Key, dataS.Value);
                            }
                        }
                    }

                    datasource.Add("Datasource", lstDataSource);
                    variables["RootPath"] = GetRootPath;
                    foreach (Type t in Globals.GetAllClass("API.Common"))
                    {
                        var m = t.GetMethods().FirstOrDefault(c => c.Name == baocao.HAM_THUC_HIEN + "_Report_Alter");
                        if (m == null) continue;
                        t.CallFunction(m.Name, item, variables, datasource, _db);
                    }
                    data.Add("Variables", variables);
                    data.Add("Resources", datasource);
                    data.Add("config", baocao?.CAU_HINH + "");
                    return ObjectResult(new { data });

                }
                catch (Exception ex)
                {
                    return ObjectResult(null, ex.Message, RequestState.message);
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
            var oldItem = _db.BC_BAOCAOCollection.GetByID(ID);
            if (oldItem != null)
            {
                var updateItem = (BC_BAOCAO)UpdateObject(oldItem, item, new string[] { "id" });
                _db.BC_BAOCAOCollection.Update(updateItem);
                HtLog(Quyen.Sua, updateItem.ID, updateItem.MA);
            }
            return new NoContentResult();
        }
    }
}
