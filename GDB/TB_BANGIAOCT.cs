using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GDB
{
    public class TB_BANGIAOCT
    {
        [Key]
        public long ID { get; set; }
        public long? BANGIAO_ID { get; set; }
        public long? THIETBI_ID { get; set; }
        public decimal? MENH_GIA { get; set; }
        public int? SO_LUONG { get; set; }
        public string? DVT { get; set; }
        public decimal? THANH_TIEN { get; set; }
        public string? GHI_CHU { get; set; }
        public string? TINH_TRANG { get; set; }

    }
    public class TB_BANGIAOCT_Info
    {
        public long? ID { get; set; }
        public long? BANGIAO_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? TINH_TRANG { get; set; }
        public string? GHI_CHU { get; set; }
        public decimal? MENH_GIA { get; set; }
        public int? SO_LUONG { get; set; }
        public string? DVT { get; set; }
        public decimal? THANH_TIEN { get; set; }

    }
   
    public class TB_BANGIAOCTCollection : TableCollection
    {
        public TB_BANGIAOCTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_BANGIAOCT> Get()
        {
            return _db.TB_BANGIAOCT.AsQueryable();
        }
        public IQueryable<TB_BANGIAOCT> GetByBANGIAO_ID(long BANGIAO_ID)
        {
            return Get().Where(e => e.BANGIAO_ID == BANGIAO_ID);
        }
        public IQueryable<TB_BANGIAOCT> GetByBANGIAO_ID(List<long> BANGIAO_ID)
        {
            return Get().Where(e => BANGIAO_ID.Contains(e.BANGIAO_ID.Value));
        }

        string sqlInfo = @"SELECT TB_THIETBI.ID, TB_THIETBI.MA, TB_THIETBI.TEN, TB_BANGIAOCT.TINH_TRANG, 
                        TB_BANGIAOCT.GHI_CHU, TB_BANGIAOCT.MENH_GIA, TB_BANGIAOCT.SO_LUONG, TB_BANGIAOCT.THANH_TIEN, 
                        TB_BANGIAOCT.DVT, TB_BANGIAO.ID BANGIAO_ID
                        FROM TB_BANGIAOCT
                        LEFT JOIN  TB_BANGIAO ON TB_BANGIAOCT.BANGIAO_ID = TB_BANGIAO.ID
                        LEFT JOIN TB_THIETBI  ON TB_BANGIAOCT.THIETBI_ID = TB_THIETBI.ID";
        public IQueryable<TB_BANGIAOCT_Info> GetInfo(long BANGIAO_ID)
        {
            string sql = sqlInfo + " WHERE TB_BANGIAO.ID = {0}";
            return _db.TB_BANGIAOCT_Info.FromSqlRaw(string.Format(sql, BANGIAO_ID));
        }
        public void Add(TB_BANGIAOCT item)
        {
            _db.TB_BANGIAOCT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_BANGIAOCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_BANGIAOCT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_BANGIAOCT item, bool save = true)
        {
            _db.TB_BANGIAOCT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_BANGIAOCT item)
        {
            if (item == null) return;
            _db.TB_BANGIAOCT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(List<TB_BANGIAOCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_BANGIAOCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_BANGIAOCT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_BANGIAOCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByBANGIAO_ID(long BANGIAO_ID)
        {
            Remove(GetByBANGIAO_ID(BANGIAO_ID));
        }
    }
}
