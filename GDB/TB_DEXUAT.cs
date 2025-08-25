using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_DEXUAT
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_SUA { get; set; }
        public long? DVQL_ID { get; set; }
        public long? DVSD_ID { get; set; }
        public string? SO_PHIEU { get; set; }
        public long? CANBO_ID { get; set; }
        public DateTime? NGAY_DE_XUAT { get; set; }
        public string? TIEU_DE { get; set; }
        public string? NOI_DUNG { get; set; }
        public decimal? TONG_TIEN { get; set; }
        public string? GHI_CHU { get; set; }
        public DateTime? NGAY_NHAN { get; set; }
        public decimal? TIEN_THUC_TE { get; set; }
        public decimal? TIEN_THUA { get; set; }
        public bool? DA_NOP { get; set; }
        public bool? NHAN_TIEN { get; set; }
        public bool? LIEN_HE { get; set; }
        public bool? DA_MUA { get; set; }
        public bool? DA_NHAN { get; set; }
        public bool? DA_THANH_TOAN { get; set; }
        public bool? DA_NHAP { get; set; }
        public bool? DA_XONG { get; set; }

    }
    public class TB_DEXUATCollection : TableCollection
    {
        public TB_DEXUATCollection(GDBContext db) : base(db){}

        public IQueryable<TB_DEXUAT> Get()
        {
            return _db.TB_DEXUAT.AsQueryable();
        }
        public IQueryable<TB_DEXUAT> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_DEXUAT> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_DEXUAT> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
       
        public TB_DEXUAT GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(TB_DEXUAT item)
        {
            _db.TB_DEXUAT.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_DEXUAT item, bool save = true)
        {
            _db.TB_DEXUAT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_DEXUAT item)
        {
            if (item == null) return;
            _db.TB_DEXUAT.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
