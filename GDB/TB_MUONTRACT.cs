using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_MUONTRACT
    {
        [Key]
        public long ID { get; set; }
        public long? MUONTRA_ID { get; set; }
        public long? THIETBI_ID { get; set; }
        public string? TINH_TRANG { get; set; } 
        public string? GHI_CHU { get; set; } 
        public bool? DA_TRA { get; set; }
        public DateTime? NGAY_TRA { get; set; }
     
    }
    public class TB_MUONTRACT_Info
    {
        public long? ID { get; set; }
        public long? MUONTRA_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? TINH_TRANG { get; set; }
        public DateTime? NGAY_TRA { get; set; }
        public string? GHI_CHU { get; set; }
        public bool? DA_TRA { get; set; }
       
    }
    public class TB_MUONTRACTCollection : TableCollection
    {
        public TB_MUONTRACTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_MUONTRACT> Get()
        {
            return _db.TB_MUONTRACT.AsQueryable();
        }
        public IQueryable<TB_MUONTRACT> GetByMUONTRA_ID(long MUONTRA_ID)
        {
            return Get().Where(e => e.MUONTRA_ID == MUONTRA_ID);
        }
        public TB_MUONTRACT GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }

        // mượn trả info
        string sqlInfo = @"SELECT TB_THIETBI.ID, TB_THIETBI.MA, TB_THIETBI.TEN, TB_MUONTRACT.TINH_TRANG, TB_MUONTRACT.NGAY_TRA, TB_MUONTRACT.GHI_CHU, TB_MUONTRACT.DA_TRA, TB_MUONTRA.ID MUONTRA_ID
                        FROM TB_MUONTRACT
                        LEFT JOIN  TB_MUONTRA ON TB_MUONTRACT.MUONTRA_ID = TB_MUONTRA.ID
                        LEFT JOIN TB_THIETBI  ON TB_MUONTRACT.THIETBI_ID = TB_THIETBI.ID";
        public IQueryable<TB_MUONTRACT_Info> GetInfo(long MUONTRA_ID)
        {
            string sql = sqlInfo + " WHERE TB_MUONTRA.ID = {0}";
            return _db.TB_MUONTRACT_Info.FromSqlRaw(string.Format(sql, MUONTRA_ID));
        }
        public void Add(TB_MUONTRACT item)
        {
            _db.TB_MUONTRACT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_MUONTRACT> items)
        {
            _db.TB_MUONTRACT.AddRange(items);
            _db.SaveChanges();
        }
        public void Update(TB_MUONTRACT item, bool save = true)
        {
            _db.TB_MUONTRACT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_MUONTRACT item)
        {
            if (item == null) return;
            _db.TB_MUONTRACT.Remove(item);
            _db.SaveChanges();
        }

        public void Remove(IQueryable<TB_MUONTRACT> item)
        {
            if (item == null) return;
            _db.TB_MUONTRACT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveGetByMUONTRA(long MUONTRA_ID)
        {
            Remove(GetByMUONTRA_ID(MUONTRA_ID));
        }
    }
}
