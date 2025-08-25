using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_DEXUATCT
    {
        [Key]
        public long ID { get; set; }
        public long? PHIEU_ID { get; set; }
        public int? PHAN_LOAI { get; set; }
        public int? CONG_TRU { get; set; }
        public string? ANH { get; set; }
        public string? TEN { get; set; }
        public decimal? GIA_TIEN { get; set; }
        public int? SO_LUONG { get; set; }
        public decimal? VAT { get; set; }
        public string? DVT { get; set; }
        public decimal? THANH_TIEN { get; set; }
        public string? DE_XUAT { get; set; }
        public string? BAN_SU_DUNG { get; set; }
        public string? LY_DO { get; set; }
        public string? KHO_KHAN { get; set; }
        public string? NGUOI_DUNG { get; set; }
        public string? UNG_DUNG { get; set; }
        public string? THOI_GIAN { get; set; }
        public string? GHI_CHU { get; set; }

    }
    public class TB_DEXUATCTCollection : TableCollection
    {
        public TB_DEXUATCTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_DEXUATCT> Get()
        {
            return _db.TB_DEXUATCT.AsQueryable();
        }
        public IQueryable<TB_DEXUATCT> GetByDEXUAT_ID(long DEXUAT_ID)
        {
            return Get().Where(e => e.PHIEU_ID == DEXUAT_ID);
        }
        public void Add(TB_DEXUATCT item)
        {
            _db.TB_DEXUATCT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_DEXUATCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_DEXUATCT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_DEXUATCT item, bool save = true)
        {
            _db.TB_DEXUATCT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_DEXUATCT item)
        {
            if (item == null) return;
            _db.TB_DEXUATCT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_DEXUATCT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_DEXUATCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByDEXUAT_ID(long DEXUAT_ID)
        {
            Remove(GetByDEXUAT_ID(DEXUAT_ID));
        }
    }
}
