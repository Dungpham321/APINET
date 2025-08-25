using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_KIEMKECT
    {
        [Key]
        public long ID { get; set; }
        public long? KIEMKE_ID { get; set; }
        public long? THIETBI_ID { get; set; }
        public int? SO_LUONG { get; set; }
        public int? TRUOC_KK_HONG { get; set; }
        public int? TRUOC_KK_MAT { get; set; }
        public int? SAU_KK_HONG { get; set; }
        public int? SAU_KK_MAT { get; set; }
        public int? CON_DUNG { get; set; }
        public int? THUA { get; set; }
        public string? GHI_CHU { get; set; }


    }
    public class TB_KIEMKECT_info
    {
        public long ID { get; set; }
        public long? KIEMKE_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? LOAI_THIET_BI { get; set; }
        public string? DVT { get; set; }
        public int? SO_LUONG { get; set; }
        public int? TRUOC_KK_HONG { get; set; }
        public int? TRUOC_KK_MAT { get; set; }
        public int? SAU_KK_HONG { get; set; }
        public int? SAU_KK_MAT { get; set; }
        public int? CON_DUNG { get; set; }
        public int? THUA { get; set; }
        public string? GHI_CHU { get; set; }


    }
    public class TB_KIEMKECTCollection : TableCollection
    {
        public TB_KIEMKECTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_KIEMKECT> Get()
        {
            return _db.TB_KIEMKECT.AsQueryable();
        }
        public IQueryable<TB_KIEMKECT> GetByKIEMKE_ID(long KIEMKE_ID)
        {
            return Get().Where(e => e.KIEMKE_ID == KIEMKE_ID);
        }

        string sqlInfo = @"SELECT 
                         TB_THIETBI.ID,
                         TB_THIETBI.MA,
                         TB_THIETBI.TEN,
                         TB_THIETBI.LOAI_THIET_BI,
                         TB_THIETBI.DVT,
                         TB_KIEMKECT.SO_LUONG,
                         TB_KIEMKECT.TRUOC_KK_HONG,
                         TB_KIEMKECT.TRUOC_KK_MAT,
                         TB_KIEMKECT.SAU_KK_HONG,
                         TB_KIEMKECT.SAU_KK_MAT,
                         TB_KIEMKECT.CON_DUNG,
                         TB_KIEMKECT.THUA,
                         TB_KIEMKECT.GHI_CHU,
                         TB_KIEMKE.ID KIEMKE_ID
                         FROM TB_KIEMKECT
                         LEFT JOIN TB_KIEMKE ON TB_KIEMKECT.KIEMKE_ID = TB_KIEMKE.ID
                         LEFT JOIN TB_THIETBI ON TB_KIEMKECT.THIETBI_ID = TB_THIETBI.ID";
        public IQueryable<TB_KIEMKECT_info> GetInfo(long KIEMKE_ID)
        {
            string sql = sqlInfo + " WHERE TB_KIEMKE.ID = {0}";
            return _db.TB_KIEMKECT_info.FromSqlRaw(string.Format(sql, KIEMKE_ID));
        }
        public void Add(TB_KIEMKECT item)
        {
            _db.TB_KIEMKECT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_KIEMKECT> item)
        {
            if (item.Count == 0) return;
            _db.TB_KIEMKECT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_KIEMKECT item, bool save = true)
        {
            _db.TB_KIEMKECT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_KIEMKECT item)
        {
            if (item == null) return;
            _db.TB_KIEMKECT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_KIEMKECT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_KIEMKECT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByKIEMKE_ID(long KIEMKE_ID)
        {
            Remove(GetByKIEMKE_ID(KIEMKE_ID));
        }
    }
}
