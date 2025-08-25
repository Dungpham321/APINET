using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_TUDIEN_ITEM
    {
        [Key]
        public long ID { get; set; }
        public long? TUDIEN_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public int? SAP_XEP { get; set; }
    }

    public class DM_TUDIEN_ITEMCollection : TableCollection
    {
        public DM_TUDIEN_ITEMCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_TUDIEN_ITEM> Get()
        {
            return _db.DM_TUDIEN_ITEM.AsQueryable();
        }
        public DM_TUDIEN_ITEM GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public DM_TUDIEN_ITEM GetByMA(string MA, long TUDIEN_ID)
        {
            return Get().FirstOrDefault(c => c.MA == MA && c.TUDIEN_ID == TUDIEN_ID);
        }
        public IQueryable<DM_TUDIEN_ITEM> GetByTUDIEN_ID(long TUDIEN_ID)
        {
            return Get().Where(c => c.TUDIEN_ID == TUDIEN_ID);
        }
        public void Add(DM_TUDIEN_ITEM item)
        {
            _db.DM_TUDIEN_ITEM.Add(item);
            _db.SaveChanges();
        }

        public void Update(DM_TUDIEN_ITEM item, bool save = true)
        {
            _db.DM_TUDIEN_ITEM.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(DM_TUDIEN_ITEM item)
        {
            if (item == null) return;
            _db.DM_TUDIEN_ITEM.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<DM_TUDIEN_ITEM> item)
        {
            if (item.Count() == 0) return;
            _db.DM_TUDIEN_ITEM.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveByTUDIEN_ID(long TUDIEN_ID)
        {
            Remove(GetByTUDIEN_ID(TUDIEN_ID));
        }
    }
}
