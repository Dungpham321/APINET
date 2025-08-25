using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.IdentityModel.Tokens;

namespace GCommon
{
    public class QUYEN
    {
        public string MA { get; set; }
        public string TEN { get; set; }
        public string NHOM_QUYEN { get; set; }
        public string CHUC_NANG { get; set; }
        public long SAP_XEP { get; set; }

        public QUYEN() { }
        public QUYEN(Quyen quyen, string nhomquyen, NhomChucNang chucnang, long sapxep = 0)
        {
            MA = (quyen.ToString() + nhomquyen + chucnang.ToString()).CleanKey().ToLower();
            TEN = quyen.GetEnumDescription();
            NHOM_QUYEN = nhomquyen;
            CHUC_NANG = chucnang.GetEnumDescription();
            SAP_XEP = sapxep;
        }
    }


    public class RequestResult
    {
        public RequestState State { get; set; }
        public string Msg { get; set; }
        public Object Data { get; set; }
    }

    public class QueryResult
    {
        public int totalCount { get; set; }
        public List<dynamic> items { get; set; }
    }


    public class TokenAuthOption
    {
        public static string Audience { get; } = "GAudience";
        public static string Issuer { get; } = "GIssuer";
        public static RsaSecurityKey Key { get; } = new RsaSecurityKey(Auth.GenerateKey());
        public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);

        public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromHours(24);

        public static TimeSpan IdleTimeout { get; } = TimeSpan.FromSeconds(60);
        public static string TokenType { get; } = "Bearer";
    }

    public class Auth
    {
        public static RSAParameters GenerateKey()
        {
            using (var key = new RSACryptoServiceProvider(2048))
            {
                return key.ExportParameters(true);
            }
        }
    }

    public class BaseData
    {
        public long ID { get; set; }
        public string CODE { get; set; }
        public string TITLE { get; set; }
        public decimal? C1 { get; set; }
        public decimal? C2 { get; set; }
        public string? CT1 { get; set; }
        public BaseData() { }
        public BaseData(long id, string code, string title)
        {
            ID = id; CODE = code; TITLE = title;
        }
    }

    public class HT_NGUOIDUNG_SDInfo
    {
        public long ID { get; set; }
        public long NGUOIDUNG_ID { get; set; }
        public string TEN_DANG_NHAP { get; set; }
        public long DOITUONG_ID { get; set; }
        public string TEN_DOI_TUONG { get; set; }
        public bool CHON { get; set; }
        public string? DATA { get; set; }
    }

    public class DM_DANHMUC_SDInfo
    {
        public long ID { get; set; }
        public long DANHMUCITEM_ID { get; set; }
        public string TEN { get; set; }
        public bool CHON { get; set; }
        public string? DATA { get; set; }
        public string? C1 { get; set; }
        public string? C2 { get; set; }
    }

    public class DataChart
    {
        public string Title { get; set; }
        public decimal Area { get; set; }
    }

    public class HT_BACKUP_Info
    {
        public string ID { get; set; }
        public string? TEN_FILE { get; set; }
        public DateTime? NGAY_SAO_LUU { get; set; }
        public DateTime? NGAY_PHUC_HOI { get; set; }
        public string? DUONG_DAN { get; set; }
        public long? NGUOIDUNG_ID { get; set; }

    }

    public class FOLDERTREE
    {
        public string PATH { get; set; }
        public string NAME { get; set; }
        public string PID { get; set; }
    }

    public class FILETREE
    {
        public string PATH { get; set; }
        public string FILENAME { get; set; }
        public long FILESIZE { get; set; }
        public DateTime CREATED { get; set; }
        public int WIDTH { get; set; }
        public int HEIGHT { get; set; }
    }
}
