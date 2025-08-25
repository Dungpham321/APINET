using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_CHUONGTRINHCT
    {
        [Key]
        public long ID { get; set; }
        public long? CHUONGTRINH_ID { get; set; }
        public long? THIETBI_ID { get; set; }
        public int? SO_LUONG { get; set; }
    }
    public class TB_CHUONGTRINHCT_info
    {
        public long ID { get; set; }
        public long? CHUONGTRINH_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? DVT { get; set; }
        public string? ANH { get; set; }
        public int? SO_LUONG { get; set; }
    }
    public class TB_CHUONGTRINHCTCollection : TableCollection
    {
        public TB_CHUONGTRINHCTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_CHUONGTRINHCT> Get()
        {
            return _db.TB_CHUONGTRINHCT.AsQueryable();
        }
        public IQueryable<TB_CHUONGTRINHCT> GetByCHUONGTRINH_ID(long CHUONGTRINH_ID)
        {
            return Get().Where(e => e.CHUONGTRINH_ID == CHUONGTRINH_ID);
        }

        string sqlInfo = @"SELECT 
                         TB_THIETBI.ID,
                         TB_THIETBI.MA,
                         TB_THIETBI.TEN,
                         TB_THIETBI.DVT,
                         TB_THIETBI.ANH,
                         TB_CHUONGTRINHCT.SO_LUONG,
                         TB_CHUONGTRINH.ID CHUONGTRINH_ID
                         FROM TB_CHUONGTRINHCT
                         LEFT JOIN TB_CHUONGTRINH ON TB_CHUONGTRINHCT.CHUONGTRINH_ID = TB_CHUONGTRINH.ID
                         LEFT JOIN TB_THIETBI ON TB_CHUONGTRINHCT.THIETBI_ID = TB_THIETBI.ID";
        public IQueryable<TB_CHUONGTRINHCT_info> GetInfo(long CHUONGTRINH_ID)
        {
            string sql = sqlInfo + " WHERE TB_CHUONGTRINH.ID = {0}";
            return _db.TB_CHUONGTRINHCT_info.FromSqlRaw(string.Format(sql, CHUONGTRINH_ID));
        }
        public void Add(TB_CHUONGTRINHCT item)
        {
            _db.TB_CHUONGTRINHCT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_CHUONGTRINHCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_CHUONGTRINHCT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_CHUONGTRINHCT item, bool save = true)
        {
            _db.TB_CHUONGTRINHCT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_CHUONGTRINHCT item)
        {
            if (item == null) return;
            _db.TB_CHUONGTRINHCT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_CHUONGTRINHCT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_CHUONGTRINHCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByCHUONGTRINH_ID(long CHUONGTRINH_ID)
        {
            Remove(GetByCHUONGTRINH_ID(CHUONGTRINH_ID));
        }
    }
}
