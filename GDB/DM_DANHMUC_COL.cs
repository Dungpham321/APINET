using GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_DANHMUC_COL
    {
        [Key]
        public long ID { get; set; }
        public long? DANHMUC_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public int? SAP_XEP { get; set; }
        public string? KIEUDULIEU { get; set; }
        public int? CHIEURONG { get; set; }
        public long? DM_ID { get; set; }
        public int? THAPPHAN { get; set; }
        public decimal? TOITHIEU { get; set; }
        public decimal? TOIDA { get; set; }
        public bool? HIENTHI { get; set; }
        public bool? CODINH { get; set; }
        public bool? BATBUOC { get; set; }
        public int? COLSPAN { get; set; }
        public bool? CHECKTRUNG { get; set; }
        public string? MACDINH { get; set; }
        public long? TUDIEN_ID { get; set; }
    }

    public class DM_DANHMUC_COLCollection : TableCollection
    {
        public DM_DANHMUC_COLCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_DANHMUC_COL> Get()
        {
            return _db.DM_DANHMUC_COL.AsQueryable();
        }
        public DM_DANHMUC_COL GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public DM_DANHMUC_COL GetByMA(string MA, long DANHMUC_ID)
        {
            return Get().FirstOrDefault(c => c.MA == MA && c.DANHMUC_ID == DANHMUC_ID);
        }
        public IQueryable<DM_DANHMUC_COL> GetByDANHMUC_ID(long DANHMUC_ID)
        {
            return Get().Where(c => c.DANHMUC_ID == DANHMUC_ID);
        }
        public void Add(DM_DANHMUC_COL item)
        {
            _db.DM_DANHMUC_COL.Add(item);
            _db.SaveChanges();
        }

        public void Update(DM_DANHMUC_COL item, bool save = true)
        {
            _db.DM_DANHMUC_COL.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(DM_DANHMUC_COL item)
        {
            if (item == null) return;
            _db.DM_DANHMUC_COL.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<DM_DANHMUC_COL> item)
        {
            if (item.Count() == 0) return;
            _db.DM_DANHMUC_COL.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveByDANHMUC_ID(long DANHMUC_ID)
        {
            Remove(GetByDANHMUC_ID(DANHMUC_ID));
        }
    }
}
