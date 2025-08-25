using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using static System.Net.WebRequestMethods;

namespace GDB
{
    public class GDBContext : DbContext
    {
        public GDBContext(DbContextOptions<GDBContext> options) : base(options) { }

        //dbset
        public DbSet<BC_BAOCAO> BC_BAOCAO { get; set; }
        public DbSet<BC_TONGHOPA> BC_TONGHOPA { get; set; }
        public DbSet<BC_TONGHOPB> BC_TONGHOPB { get; set; }
        //DM
        public DbSet<DM_DANHMUC> DM_DANHMUC { get; set; }
        public DbSet<DM_DANHMUC_COL> DM_DANHMUC_COL { get; set; }
        public DbSet<DM_DANHMUC_ITEM> DM_DANHMUC_ITEM { get; set; }
        public DbSet<DM_DANHMUC_SD> DM_DANHMUC_SD { get; set; }
        public DbSet<DM_TUDIEN> DM_TUDIEN { get; set; }
        public DbSet<DM_TUDIEN_ITEM> DM_TUDIEN_ITEM { get; set; }
        //HT
        public DbSet<HT_DOITUONG_QUYEN> HT_DOITUONG_QUYEN { get; set; }
        public DbSet<HT_GRID> HT_GRID { get; set; }
        public DbSet<HT_LICHSU> HT_LICHSU { get; set; }
        public DbSet<HT_NGUOIDUNG> HT_NGUOIDUNG { get; set; }
        public DbSet<HT_NGUOIDUNG_ON> HT_NGUOIDUNG_ON { get; set; }
        public DbSet<HT_NGUOIDUNG_PASS> HT_NGUOIDUNG_PASS { get; set; }
        public DbSet<HT_NGUOIDUNG_SD> HT_NGUOIDUNG_SD { get; set; }
        public DbSet<HT_TENMIEN> HT_TENMIEN { get; set; }
        public DbSet<HT_TEP> HT_TEP { get; set; }
        public DbSet<HT_TEP_INFO> HT_TEP_INFO { get; set; }
        public DbSet<HT_TEP_SD> HT_TEP_SD { get; set; }
        public DbSet<HT_THONGBAO> HT_THONGBAO { get; set; }
        public DbSet<HT_TINNHAN> HT_TINNHAN { get; set; }
        public DbSet<HT_TINNHAN_INFO> HT_TINNHAN_INFO { get; set; }
        public DbSet<HT_TINNHAN_ND> HT_TINNHAN_ND { get; set; }
        public DbSet<HT_VARIABLE> HT_VARIABLE { get; set; }
        // Thiết bị
        public DbSet<TB_THIETBI> TB_THIETBI {  get; set; }
        public DbSet<TB_MUON> TB_MUON {  get; set; }
        public DbSet<TB_MUONCT> TB_MUONCT {  get; set; }
        public DbSet<TB_MUONTRA> TB_MUONTRA {  get; set; }
        public DbSet<TB_MUONTRACT> TB_MUONTRACT {  get; set; }
        public DbSet<TB_BANGIAO> TB_BANGIAO { get; set; }
        public DbSet<TB_BANGIAOCT> TB_BANGIAOCT { get; set; }
        public DbSet<TB_BANGIAOHD> TB_BANGIAOHD { get; set; }
        public DbSet<TB_MUONTRACT_Info> TB_MUONTRACT_Info {  get; set; }
        public DbSet<TB_BANGIAOCT_Info> TB_BANGIAOCT_Info {  get; set; }
        public DbSet<TB_KEYPMCT_Info> TB_KEYPMCT_Info {  get; set; }
        public DbSet<TB_KEYPM> TB_KEYPM { get; set; }
        public DbSet<TB_KEYPMCT> TB_KEYPMCT { get; set; }
        public DbSet<TB_DEXUAT> TB_DEXUAT { get; set; }
        public DbSet<TB_DEXUATCT> TB_DEXUATCT { get; set; }
        public DbSet<TB_KIEMKE> TB_KIEMKE { get; set; }
        public DbSet<TB_KIEMKECT> TB_KIEMKECT { get; set; }
        public DbSet<TB_IMPORT> TB_IMPORT { get; set; }
        public DbSet<TB_KIEMKECT_info> TB_KIEMKECT_info { get; set; }
        public DbSet<TB_CHUONGTRINH> TB_CHUONGTRINH { get; set; }
        public DbSet<TB_CHUONGTRINHCT> TB_CHUONGTRINHCT { get; set; }
        public DbSet<TB_CHUONGTRINHCT_info> TB_CHUONGTRINHCT_info { get; set; }

        //-----------------
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<HT_LICHSU>().Property(p => p.NGAY_TAO).HasColumnType("datetime");
            modelBuilder.Entity<BC_TONGHOPA>().HasNoKey();
            modelBuilder.Entity<BC_TONGHOPB>().HasNoKey();
            modelBuilder.Entity<TB_MUONTRACT_Info>().HasNoKey();
            modelBuilder.Entity<TB_BANGIAOCT_Info>().HasNoKey();
            modelBuilder.Entity<TB_KEYPMCT_Info>().HasNoKey();
            modelBuilder.Entity<TB_KIEMKECT_info>().HasNoKey();
            modelBuilder.Entity<TB_CHUONGTRINHCT_info>().HasNoKey();
        }

        //collections
        private BC_BAOCAOCollection _BC_BAOCAOCollection { get; set; }
        public BC_BAOCAOCollection BC_BAOCAOCollection { get { if (null == _BC_BAOCAOCollection) _BC_BAOCAOCollection = new BC_BAOCAOCollection(this); return _BC_BAOCAOCollection; } }
        private BC_TONGHOPCollection _BC_TONGHOPCollection { get; set; }
        public BC_TONGHOPCollection BC_TONGHOPCollection { get { if (null == _BC_TONGHOPCollection) _BC_TONGHOPCollection = new BC_TONGHOPCollection(this); return _BC_TONGHOPCollection; } }
        //DM
        private DM_DANHMUCCollection _DM_DANHMUCCollection { get; set; }
        public DM_DANHMUCCollection DM_DANHMUCCollection { get { if (null == _DM_DANHMUCCollection) _DM_DANHMUCCollection = new DM_DANHMUCCollection(this); return _DM_DANHMUCCollection; } }
        private DM_DANHMUC_COLCollection _DM_DANHMUC_COLCollection { get; set; }
        public DM_DANHMUC_COLCollection DM_DANHMUC_COLCollection { get { if (null == _DM_DANHMUC_COLCollection) _DM_DANHMUC_COLCollection = new DM_DANHMUC_COLCollection(this); return _DM_DANHMUC_COLCollection; } }
        private DM_DANHMUC_ITEMCollection _DM_DANHMUC_ITEMCollection { get; set; }
        public DM_DANHMUC_ITEMCollection DM_DANHMUC_ITEMCollection { get { if (null == _DM_DANHMUC_ITEMCollection) _DM_DANHMUC_ITEMCollection = new DM_DANHMUC_ITEMCollection(this); return _DM_DANHMUC_ITEMCollection; } }
        private DM_DANHMUC_SDCollection _DM_DANHMUC_SDCollection { get; set; }
        public DM_DANHMUC_SDCollection DM_DANHMUC_SDCollection { get { if (null == _DM_DANHMUC_SDCollection) _DM_DANHMUC_SDCollection = new DM_DANHMUC_SDCollection(this); return _DM_DANHMUC_SDCollection; } }
        private DM_TUDIENCollection _DM_TUDIENCollection { get; set; }
        public DM_TUDIENCollection DM_TUDIENCollection { get { if (null == _DM_TUDIENCollection) _DM_TUDIENCollection = new DM_TUDIENCollection(this); return _DM_TUDIENCollection; } }
        private DM_TUDIEN_ITEMCollection _DM_TUDIEN_ITEMCollection { get; set; }
        public DM_TUDIEN_ITEMCollection DM_TUDIEN_ITEMCollection { get { if (null == _DM_TUDIEN_ITEMCollection) _DM_TUDIEN_ITEMCollection = new DM_TUDIEN_ITEMCollection(this); return _DM_TUDIEN_ITEMCollection; } }
        //HT
        private HT_DOITUONG_QUYENCollection _HT_DOITUONG_QUYENCollection { get; set; }
        public HT_DOITUONG_QUYENCollection HT_DOITUONG_QUYENCollection { get { if (null == _HT_DOITUONG_QUYENCollection) _HT_DOITUONG_QUYENCollection = new HT_DOITUONG_QUYENCollection(this); return _HT_DOITUONG_QUYENCollection; } }
        private HT_GRIDCollection _HT_GRIDCollection { get; set; }
        public HT_GRIDCollection HT_GRIDCollection { get { if (null == _HT_GRIDCollection) _HT_GRIDCollection = new HT_GRIDCollection(this); return _HT_GRIDCollection; } }
        private HT_LICHSUCollection _HT_LICHSUCollection { get; set; }
        public HT_LICHSUCollection HT_LICHSUCollection { get { if (null == _HT_LICHSUCollection) _HT_LICHSUCollection = new HT_LICHSUCollection(this); return _HT_LICHSUCollection; } }
        private HT_NGUOIDUNGCollection _HT_NGUOIDUNGCollection { get; set; }
        public HT_NGUOIDUNGCollection HT_NGUOIDUNGCollection { get { if (null == _HT_NGUOIDUNGCollection) _HT_NGUOIDUNGCollection = new HT_NGUOIDUNGCollection(this); return _HT_NGUOIDUNGCollection; } }
        private HT_NGUOIDUNG_ONCollection _HT_NGUOIDUNG_ONCollection { get; set; }
        public HT_NGUOIDUNG_ONCollection HT_NGUOIDUNG_ONCollection { get { if (null == _HT_NGUOIDUNG_ONCollection) _HT_NGUOIDUNG_ONCollection = new HT_NGUOIDUNG_ONCollection(this); return _HT_NGUOIDUNG_ONCollection; } }
        private HT_NGUOIDUNG_PASSCollection _HT_NGUOIDUNG_PASSCollection { get; set; }
        public HT_NGUOIDUNG_PASSCollection HT_NGUOIDUNG_PASSCollection { get { if (null == _HT_NGUOIDUNG_PASSCollection) _HT_NGUOIDUNG_PASSCollection = new HT_NGUOIDUNG_PASSCollection(this); return _HT_NGUOIDUNG_PASSCollection; } }
        private HT_NGUOIDUNG_SDCollection _HT_NGUOIDUNG_SDCollection { get; set; }
        public HT_NGUOIDUNG_SDCollection HT_NGUOIDUNG_SDCollection { get { if (null == _HT_NGUOIDUNG_SDCollection) _HT_NGUOIDUNG_SDCollection = new HT_NGUOIDUNG_SDCollection(this); return _HT_NGUOIDUNG_SDCollection; } }
        private HT_TENMIENCollection _HT_TENMIENCollection { get; set; }
        public HT_TENMIENCollection HT_TENMIENCollection { get { if (null == _HT_TENMIENCollection) _HT_TENMIENCollection = new HT_TENMIENCollection(this); return _HT_TENMIENCollection; } }
        private HT_TEPCollection _HT_TEPCollection { get; set; }
        public HT_TEPCollection HT_TEPCollection { get { if (null == _HT_TEPCollection) _HT_TEPCollection = new HT_TEPCollection(this); return _HT_TEPCollection; } }
        private HT_TEP_SDCollection _HT_TEP_SDCollection { get; set; }
        public HT_TEP_SDCollection HT_TEP_SDCollection { get { if (null == _HT_TEP_SDCollection) _HT_TEP_SDCollection = new HT_TEP_SDCollection(this); return _HT_TEP_SDCollection; } }
        private HT_THONGBAOCollection _HT_THONGBAOCollection { get; set; }
        public HT_THONGBAOCollection HT_THONGBAOCollection { get { if (null == _HT_THONGBAOCollection) _HT_THONGBAOCollection = new HT_THONGBAOCollection(this); return _HT_THONGBAOCollection; } }
        private HT_TINNHANCollection _HT_TINNHANCollection { get; set; }
        public HT_TINNHANCollection HT_TINNHANCollection { get { if (null == _HT_TINNHANCollection) _HT_TINNHANCollection = new HT_TINNHANCollection(this); return _HT_TINNHANCollection; } }
        private HT_TINNHAN_NDCollection _HT_TINNHAN_NDCollection { get; set; }
        public HT_TINNHAN_NDCollection HT_TINNHAN_NDCollection { get { if (null == _HT_TINNHAN_NDCollection) _HT_TINNHAN_NDCollection = new HT_TINNHAN_NDCollection(this); return _HT_TINNHAN_NDCollection; } }
        private HT_VARIABLECollection _HT_VARIABLECollection { get; set; }
        public HT_VARIABLECollection HT_VARIABLECollection { get { if (null == _HT_VARIABLECollection) _HT_VARIABLECollection = new HT_VARIABLECollection(this); return _HT_VARIABLECollection; } }
        //Thiết bị
        private TB_THIETBICollection _TB_THIETBICollection { get; set; }
        public TB_THIETBICollection TB_THIETBICollection { get { if (null == _TB_THIETBICollection) _TB_THIETBICollection = new TB_THIETBICollection(this); return _TB_THIETBICollection; } }
        private TB_MUONCollection _TB_MUONCollection { get; set; }
        public TB_MUONCollection TB_MUONCollection { get { if (null == _TB_MUONCollection) _TB_MUONCollection = new TB_MUONCollection(this); return _TB_MUONCollection; } }

        private TB_MUONCTCollection _TB_MUONCTCollection { get; set; }
        public TB_MUONCTCollection TB_MUONCTCollection { get { if (null == _TB_MUONCTCollection) _TB_MUONCTCollection = new TB_MUONCTCollection(this); return _TB_MUONCTCollection; } }

        private TB_MUONTRACollection _TB_MUONTRACollection { get; set; }
        public TB_MUONTRACollection TB_MUONTRACollection { get { if (null == _TB_MUONTRACollection) _TB_MUONTRACollection = new TB_MUONTRACollection(this); return _TB_MUONTRACollection; } }

        private TB_MUONTRACTCollection _TB_MUONTRACTCollection { get; set; }
        public TB_MUONTRACTCollection TB_MUONTRACTCollection { get { if (null == _TB_MUONTRACTCollection) _TB_MUONTRACTCollection = new TB_MUONTRACTCollection(this); return _TB_MUONTRACTCollection; } }

        private TB_BANGIAOCollection _TB_BANGIAOCollection { get; set; }
        public TB_BANGIAOCollection TB_BANGIAOCollection { get { if (null == _TB_BANGIAOCollection) _TB_BANGIAOCollection = new TB_BANGIAOCollection(this); return _TB_BANGIAOCollection; } }

        private TB_BANGIAOCTCollection _TB_BANGIAOCTCollection { get; set; }
        public TB_BANGIAOCTCollection TB_BANGIAOCTCollection { get { if (null == _TB_BANGIAOCTCollection) _TB_BANGIAOCTCollection = new TB_BANGIAOCTCollection(this); return _TB_BANGIAOCTCollection; } }
        private TB_BANGIAOHDCollection _TB_BANGIAOHDCollection { get; set; }
        public TB_BANGIAOHDCollection TB_BANGIAOHDCollection { get { if (null == _TB_BANGIAOHDCollection) _TB_BANGIAOHDCollection = new TB_BANGIAOHDCollection(this); return _TB_BANGIAOHDCollection; } }

        private TB_KEYPMCollection _TB_KEYPMCollection { get; set; }
        public TB_KEYPMCollection TB_KEYPMCollection { get { if (null == _TB_KEYPMCollection) _TB_KEYPMCollection = new TB_KEYPMCollection(this); return _TB_KEYPMCollection; } }

        private TB_CHUONGTRINHCollection _TB_CHUONGTRINHCollection { get; set; }
        public TB_CHUONGTRINHCollection TB_CHUONGTRINHCollection { get { if (null == _TB_CHUONGTRINHCollection) _TB_CHUONGTRINHCollection = new TB_CHUONGTRINHCollection(this); return _TB_CHUONGTRINHCollection; } }
        private TB_CHUONGTRINHCTCollection _TB_CHUONGTRINHCTCollection { get; set; }
        public TB_CHUONGTRINHCTCollection TB_CHUONGTRINHCTCollection { get { if (null == _TB_CHUONGTRINHCTCollection) _TB_CHUONGTRINHCTCollection = new TB_CHUONGTRINHCTCollection(this); return _TB_CHUONGTRINHCTCollection; } }

        private TB_KEYPMCTCollection _TB_KEYPMCTCollection { get; set; }
        public TB_KEYPMCTCollection TB_KEYPMCTCollection { get { if (null == _TB_KEYPMCTCollection) _TB_KEYPMCTCollection = new TB_KEYPMCTCollection(this); return _TB_KEYPMCTCollection; } }
        private TB_DEXUATCollection _TB_DEXUATCollection { get; set; }
        public TB_DEXUATCollection TB_DEXUATCollection { get { if (null == _TB_DEXUATCollection) _TB_DEXUATCollection = new TB_DEXUATCollection(this); return _TB_DEXUATCollection; } }
        private TB_DEXUATCTCollection _TB_DEXUATCTCollection { get; set; }
        public TB_DEXUATCTCollection TB_DEXUATCTCollection { get { if (null == _TB_DEXUATCTCollection) _TB_DEXUATCTCollection = new TB_DEXUATCTCollection(this); return _TB_DEXUATCTCollection; } }
        private TB_KIEMKECollection _TB_KIEMKECollection { get; set; }
        public TB_KIEMKECollection TB_KIEMKECollection { get { if (null == _TB_KIEMKECollection) _TB_KIEMKECollection = new TB_KIEMKECollection(this); return _TB_KIEMKECollection; } }
        private TB_KIEMKECTCollection _TB_KIEMKECTCollection { get; set; }
        public TB_KIEMKECTCollection TB_KIEMKECTCollection { get { if (null == _TB_KIEMKECTCollection) _TB_KIEMKECTCollection = new TB_KIEMKECTCollection(this); return _TB_KIEMKECTCollection; } }
        private TB_IMPORTCollection _TB_IMPORTCollection { get; set; }
        public TB_IMPORTCollection TB_IMPORTCollection { get { if (null == _TB_IMPORTCollection) _TB_IMPORTCollection = new TB_IMPORTCollection(this); return _TB_IMPORTCollection; } }
        //methods
        public T CallFunction<T>(string function, params object[] pars)
        {
            FormattableString sql = FormattableStringFactory.Create(String.Format("SELECT dbo.{0}({1})", function, String.Join(",", pars)));
            return this.Database.SqlQuery<T>(sql).ToList().FirstOrDefault();
        }
    }

    public class TableCollection
    {
        protected GDBContext _db;
        public TableCollection(GDBContext db)
        {
            _db = db;
        }
    }

    public class BaseTable
    {
        public BaseTable() { }
        public BaseTable(object item)
        {
            UpdateObject(item);
        }
        protected void UpdateObject(object item, string[] exfield = null)
        {
            if (item == null) return;
            var cItem = JObject.FromObject(item).ToObject(GetType());
            foreach (var p in cItem.GetType().GetProperties())
                if (exfield == null || (exfield != null && !exfield.Contains(p.Name)))
                    GetType().GetProperty(p.Name).SetValue(this, p.GetValue(cItem, null));
        }
    }

    public static class DbExClass
    {
        public static bool DbLike(this string toSearch, string toFind)
        {
            return new Regex(@"\A" + new Regex(@"\.|\$|\^|\{|\[|\(|\||\)|\*|\+|\?|\\").Replace(toFind, ch => @"\" + ch).Replace('_', '.').Replace("%", ".*") + @"\z", RegexOptions.Singleline).IsMatch(toSearch);
        }
        public static bool DbLike(this string toSearch, string[] toFind)
        {
            foreach (string f in toFind)
            {
                if (toSearch.DbLike(f)) return true;
            }
            return false;
        }
    }
}
