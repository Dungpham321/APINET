using API.Common;
using GoogleAuthenticatorService.Core;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Microsoft.AspNetCore.Authorization;

namespace API.Controllers.HeThong
{
    [Consumes("application/json")]
    [Route("api/HeThong/HT_DANGNHAP")]
    [ApiController]
    public class HT_DANGNHAPController : BaseController
    {
        public HT_DANGNHAPController(GDBContext db, IConfiguration configuration) : base(db, configuration) { }

        [HttpGet("{op}")]
        public IActionResult Get(string op)
        {
            if (op == "Info")
            {
                //var fDv = _db.DM_DONVICollection.Get().FirstOrDefault(c => c.DON_VI_CHA_ID == 0);
                long longDVQL_ID = Convert.ToInt64(Request.Query["DVQL_ID"]);
                var fDv = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, longDVQL_ID).FirstOrDefault(c => c.PID == 0 || c.PID == null);
                return ObjectResult(new {zalolink = HT_CAUHINH_Get<string>("HOTRO_ZALOLINK", "", fDv.ID) , zalophone = HT_CAUHINH_Get<string>("HOTRO_ZALOPHONE", "", fDv.ID) });
            }
            return new BadRequestResult();
        }

        [HttpPost]
        public IActionResult GetAuthToken([FromBody] JObject item)
        {
            HT_NGUOIDUNG objHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.GetByTEN_DANG_NHAP(item["TenDangNhap"].Value<string>().Trim(), item["DVQL_ID"].Value<long>());
            if (objHT_NGUOIDUNG != null)
            {
                if (objHT_NGUOIDUNG.TRANG_THAI == (int)TrangThai.DA_DUYET)
                {
                    int solansai = HT_CAUHINH_Get<int>("TS_DANG_NHAP_SAI", 0, objHT_NGUOIDUNG.DVQL_ID);
                    var decode = item["MatKhau"].Value<string>().Base64Decode(false);
                    if (objHT_NGUOIDUNG.MAT_KHAU == item["MatKhau"].Value<string>().Base64Decode(false).EncodePassword())
                    {
                        var dsDonVi = GetLcDM_DONVI(objHT_NGUOIDUNG).ToList();
                        if (dsDonVi.FirstOrDefault(c => c.ID == objHT_NGUOIDUNG.DVSD_ID) == null) objHT_NGUOIDUNG.DVSD_ID = dsDonVi.FirstOrDefault().ID;

                        DateTime requestAt = DateTime.Now;
                        DateTime expiresIn = requestAt + TokenAuthOption.ExpiresSpan;
                        string token = GenerateToken(objHT_NGUOIDUNG, expiresIn);
                        HtLog_ID(Quyen.DangNhap, objHT_NGUOIDUNG.TEN_DANG_NHAP, null, objHT_NGUOIDUNG.ID, objHT_NGUOIDUNG.DVQL_ID, objHT_NGUOIDUNG.DVSD_ID);
                        objHT_NGUOIDUNG.SAI_MAT_KHAU = 0;
                        objHT_NGUOIDUNG.NGAY_DANG_NHAP = DateTime.Now;
                        if (!objHT_NGUOIDUNG.NGAY_MAT_KHAU.HasValue) objHT_NGUOIDUNG.NGAY_MAT_KHAU = DateTime.Now;
                        _db.HT_NGUOIDUNGCollection.Update(objHT_NGUOIDUNG);
                        int thoigianmatkhau = HT_CAUHINH_Get<int>("THOI_GIAN_MAT_KHAU", 0, objHT_NGUOIDUNG.DVQL_ID);
                        bool DOI_MAT_KHAU = false;
                        if (thoigianmatkhau > 0 && objHT_NGUOIDUNG.NGAY_MAT_KHAU.Value.AddDays(thoigianmatkhau) < DateTime.Now)
                        {
                            DOI_MAT_KHAU = true;
                        }
                        return ObjectResult(new
                        {
                            requertAt = requestAt,
                            expiresIn = TokenAuthOption.ExpiresSpan.TotalSeconds,
                            tokeyType = TokenAuthOption.TokenType,
                            accessToken = token,
                            objHT_NGUOIDUNG.EMAIL,
                            objHT_NGUOIDUNG.ID,
                            objHT_NGUOIDUNG.TEN_DANG_NHAP,
                            objHT_NGUOIDUNG.TEN_DAY_DU,
                            objHT_NGUOIDUNG.DVQL_ID,
                            objHT_NGUOIDUNG.DVSD_ID,
                            objHT_NGUOIDUNG.CANBO_ID,
                            VAI_TRO = IntVaiTro,
                            GD_MAU_SAC = HT_CAUHINH_Get<string>("GD_MAU_SAC", "", objHT_NGUOIDUNG.DVQL_ID),
                            TEN_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(objHT_NGUOIDUNG.DVSD_ID)?.TEN + "",
                            TEN_DONVI_CHA = _db.DM_DANHMUC_ITEMCollection.GetByID(objHT_NGUOIDUNG.DVQL_ID)?.TEN + "",
                            DOI_MAT_KHAU,
                            objHT_NGUOIDUNG.TFA,
                        });
                    }
                    else if (solansai > 0 && objHT_NGUOIDUNG.TEN_DANG_NHAP != "superadmin")
                    {
                        if (!objHT_NGUOIDUNG.SAI_MAT_KHAU.HasValue) objHT_NGUOIDUNG.SAI_MAT_KHAU = 0;
                        objHT_NGUOIDUNG.SAI_MAT_KHAU += 1;
                        if (objHT_NGUOIDUNG.SAI_MAT_KHAU > solansai)
                        {
                            objHT_NGUOIDUNG.TRANG_THAI = (int)TrangThai.KHOA;
                        }
                        _db.HT_NGUOIDUNGCollection.Update(objHT_NGUOIDUNG);
                        if (objHT_NGUOIDUNG.TRANG_THAI == (int)TrangThai.KHOA) return ObjectResult(null, Resource.strQuaLanDangNhap, RequestState.Failed);
                    }
                }
            }

            return ObjectResult(null, Resource.strDangNhapSai, RequestState.Failed);
        }

        [HttpPost("{op}")]
        public IActionResult Post(string op, [FromBody] JObject item)
        {
            if (item == null || op == "") return new BadRequestResult();
            if (op == "Password")
            {
                long dvql_id = item["DVQL_ID"].Value<long>();
                HT_NGUOIDUNG user = _db.HT_NGUOIDUNGCollection.GetByTEN_DANG_NHAP(item["TenDangNhap"].Value<string>(), dvql_id);
                if (user == null) return ObjectResult(null, Resource.strSaiTenDangNhap);
                if (user.EMAIL + "" == "") return ObjectResult(null, Resource.strKhongEmail);
                if (item["bxl"].Value<int>() == 0)
                {
                    string passcode = ConvertClass.RandomString(4, true);
                    HT_NGUOIDUNG_PASS pass = _db.HT_NGUOIDUNG_PASSCollection.GetByNGUOIDUNG_ID(user.ID);
                    if (pass == null)
                    {
                        pass = new HT_NGUOIDUNG_PASS();
                        pass.CREATED = DateTime.Now;
                        pass.NGUOIDUNG_ID = user.ID;
                        pass.PASSCODE = passcode;
                        _db.HT_NGUOIDUNG_PASSCollection.Add(pass);
                    }
                    else
                    {
                        pass.CREATED = DateTime.Now;
                        pass.PASSCODE = passcode;
                        _db.HT_NGUOIDUNG_PASSCollection.Update(pass);
                    }
                    //gui mail
                    string urlSMTP = HT_CAUHINH_Get<string>("SMTP_SERVER", "", dvql_id);
                    if (urlSMTP != "")
                    {
                        System.Net.Mail.SmtpClient Mclient = new System.Net.Mail.SmtpClient(urlSMTP);
                        Mclient.Port = ConvertClass.ToInt(HT_CAUHINH_Get<string>("SMTP_PORT", "", dvql_id), 587);
                        if (HT_CAUHINH_Get<string>("SMTP_SSL", "", dvql_id) == "1") Mclient.EnableSsl = true;
                        else Mclient.EnableSsl = false;
                        
                        Mclient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        Mclient.UseDefaultCredentials = false;
                        Mclient.Credentials = new System.Net.NetworkCredential(HT_CAUHINH_Get<string>("SMTP_USER", "", dvql_id), HT_CAUHINH_Get<string>("SMTP_PASS", "", dvql_id));
                        System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();
                        mailMessage.Sender = new MailAddress(HT_CAUHINH_Get<string>("SMTP_MAIL", "", dvql_id), HT_CAUHINH_Get<string>("SMTP_FROM", "", dvql_id), System.Text.Encoding.UTF8);
                        mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
                        mailMessage.SubjectEncoding = System.Text.Encoding.UTF8;
                        mailMessage.From = new System.Net.Mail.MailAddress(HT_CAUHINH_Get<string>("SMTP_MAIL", "", dvql_id));
                        
                        mailMessage.To.Add(user.EMAIL);
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = "Mã xác nhận @code".Replace("@code", passcode);
                        mailMessage.Subject = "Quên mật khẩu";

                        //System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
                        //ServicePointManager.ServerCertificateValidationCallback =
                        //    delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
                        try
                        {
                            Mclient.Send(mailMessage);
                        }
                        catch (Exception ex)
                        {
                            return ObjectResult(null, ex.Message);
                        }
                    }

                }
                else if (item["bxl"].Value<int>() == 1)
                {
                    HT_NGUOIDUNG_PASS pass = _db.HT_NGUOIDUNG_PASSCollection.GetByNGUOIDUNG_ID(user.ID);
                    if (pass?.PASSCODE + "" != item["MaXacNhan"].Value<string>()) return ObjectResult(null, Resource.strSaiMaXacNhan);
                }
                else if (item["bxl"].Value<int>() == 2)
                {
                    user.MAT_KHAU = ConvertClass.EncodePassword(item["MatKhau"].Value<string>());
                    _db.HT_NGUOIDUNGCollection.Update(user);
                    _db.HT_NGUOIDUNG_PASSCollection.Remove(user.ID);
                    HtLog_ID(Quyen.DoiMatKhau, user.TEN_DANG_NHAP, null, user.ID, user.DVQL_ID, user.DVSD_ID);
                }
            }
            else if (op == "Auth")
            {
                long NGUOIDUNG_ID = item["NGUOIDUNG_ID"].Value<long>();
                string CODE = item["CODE"].Value<string>();
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                TimeSpan sp = new TimeSpan(0, 0, 30);
                var objHT_NGUOIDUNG = _db.HT_NGUOIDUNGCollection.GetByID(NGUOIDUNG_ID);
                return ObjectResult(tfa.ValidateTwoFactorPIN(objHT_NGUOIDUNG.TEN_DANG_NHAP + "QLHTNV2BUOC", CODE, sp));
            }
            return new NoContentResult();
        }
    }
}
