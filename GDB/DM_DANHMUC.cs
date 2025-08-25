using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_DANHMUC
    {
        [Key]
        public long ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public int? SAP_XEP { get; set; }
        public string? MO_TA { get; set; }
        public string? DATA { get; set; }
        public bool? IDTUTANG { get; set; }
        public long? PID { get; set; }
        public bool? CODUYET { get; set; }
        public bool HETHONG { get; set; }
        public bool? NOIBO { get; set; }
        public int? CHIEURONG { get; set; }
    }

    public class DM_DANHMUCCollection : TableCollection
    {
        public DM_DANHMUCCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_DANHMUC> Get()
        {
            return _db.DM_DANHMUC.AsQueryable();
        }
        public DM_DANHMUC GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public DM_DANHMUC GetByMA(string MA)
        {
            return Get().FirstOrDefault(c => c.MA == MA);
        }
        public void Add(DM_DANHMUC item)
        {
            _db.DM_DANHMUC.Add(item);
            _db.SaveChanges();
        }

        public void Update(DM_DANHMUC item, bool save = true)
        {
            _db.DM_DANHMUC.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(DM_DANHMUC item)
        {
            if (item == null) return;
            _db.DM_DANHMUC.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
