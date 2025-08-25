using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_BANGIAO
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_SUA { get; set; }
        public long? DVQL_ID { get; set; }
        public long? DVSD_ID { get; set; }
        public string? SOBG { get; set; }
        public DateTime? NGAY_BG { get; set; }
        public string? TIEU_DE { get; set; }
        public string? NOI_BAN_GIAO { get; set; }
        public int? LOAIBG { get; set; }
        public long? CBGIAO_ID { get; set; }
        public DateTime? CBGIAO_NGAYSINH { get; set; }
        public string? CBGIAO_CCCD { get; set; }
        public DateTime? CBGIAO_NGAYCAP { get; set; }
        public string? CBGIAO_NOICAP { get; set; }
        public string? CBGIAO_HKTT { get; set; }
        public string? CBGIAO_CHOO { get; set; }
        public string? CBGIAO_DIENTHOAI { get; set; }
        public long? CBNHAN_ID { get; set; }
        public DateTime? CBNHAN_NGAYSINH { get; set; }
        public string? CBNHAN_CCCD { get; set; }
        public DateTime? CBNHAN_NGAYCAP { get; set; }
        public string? CBNHAN_NOICAP { get; set; }
        public string? CBNHAN_HKTT { get; set; }
        public string? CBNHAN_CHOO { get; set; }
        public string? CBNHAN_DIENTHOAI { get; set; }
        public string? LY_DO { get; set; }
        public int? SO_LAP { get; set; }
        public int? BEN_GIAO { get; set; }
        public int? BEN_NHAN { get; set; }
        public string? PHU_TRACH_DV { get; set; }
        public decimal? SO_TIEN { get; set; }


    }
    public class TB_BANGIAOCollection : TableCollection
    {
        public TB_BANGIAOCollection(GDBContext db) : base(db){}

        public IQueryable<TB_BANGIAO> Get()
        {
            return _db.TB_BANGIAO.AsQueryable();
        }
        public IQueryable<TB_BANGIAO> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_BANGIAO> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_BANGIAO> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
       
        public TB_BANGIAO GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(TB_BANGIAO item)
        {
            _db.TB_BANGIAO.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_BANGIAO item, bool save = true)
        {
            _db.TB_BANGIAO.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_BANGIAO item)
        {
            if (item == null) return;
            _db.TB_BANGIAO.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
