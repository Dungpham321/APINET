using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GDB
{
    public class TB_KEYPMCT
    {
        [Key]
        public long ID { get; set; }
        public long? KEYPM_ID { get; set; }
        public long? CANBO_ID { get; set; }
        public DateTime? NGAY_NHAP { get; set; }
        public long? THIETBI_ID { get; set; }
        public bool? DA_NHAP { get; set; }
        public string? GHI_CHU { get; set; }
        public string? DOI_MAY { get; set; }

    }
    public class TB_KEYPMCT_Info
    {
        public long ID { get; set; }
        public long? KEYPM_ID { get; set; }
        public long? CANBO_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? DOI_MAY { get; set; }
        public bool? DA_NHAP { get; set; }
        public string? GHI_CHU { get; set; }

    }
    public class TB_KEYPMCTCollection : TableCollection
    {
        public TB_KEYPMCTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_KEYPMCT> Get()
        {
            return _db.TB_KEYPMCT.AsQueryable();
        }
        public IQueryable<TB_KEYPMCT> GetByKEYPM_ID(long KEYPM_ID)
        {
            return Get().Where(e => e.KEYPM_ID == KEYPM_ID);
        }
        //info
        string sqlInfo = @"SELECT 
                            TB_THIETBI.ID, 
                            TB_THIETBI.MA, 
                            TB_THIETBI.TEN, 
                            TB_KEYPMCT.CANBO_ID, 
                            TB_KEYPMCT.DOI_MAY, 
                            TB_KEYPMCT.DA_NHAP, 
                            TB_KEYPMCT.GHI_CHU, 
                            TB_KEYPM.ID KEYPM_ID
                            FROM TB_KEYPMCT
                            LEFT JOIN  TB_KEYPM ON TB_KEYPMCT.KEYPM_ID = TB_KEYPM.ID
                            LEFT JOIN TB_THIETBI  ON TB_KEYPMCT.THIETBI_ID = TB_THIETBI.ID";
        public IQueryable<TB_KEYPMCT_Info> GetInfo(long KEYPM_ID)
        {
            string sql = sqlInfo + " WHERE TB_KEYPM.ID = {0}";
            return _db.TB_KEYPMCT_Info.FromSqlRaw(string.Format(sql, KEYPM_ID));
        }
        public void Add(TB_KEYPMCT item)
        {
            _db.TB_KEYPMCT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_KEYPMCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_KEYPMCT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_KEYPMCT item, bool save = true)
        {
            _db.TB_KEYPMCT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_KEYPMCT item)
        {
            if (item == null) return;
            _db.TB_KEYPMCT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_KEYPMCT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_KEYPMCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByKEYPM_ID(long KEYPM_ID)
        {
            Remove(GetByKEYPM_ID(KEYPM_ID));
        }
    }
}
