using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_TUDIEN
    {
        [Key]
        public long ID { get; set; }
        public string? TEN { get; set; }
    }

    public class DM_TUDIENCollection : TableCollection
    {
        public DM_TUDIENCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_TUDIEN> Get()
        {
            return _db.DM_TUDIEN.AsQueryable();
        }
        public DM_TUDIEN GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(DM_TUDIEN item)
        {
            _db.DM_TUDIEN.Add(item);
            _db.SaveChanges();
        }

        public void Update(DM_TUDIEN item, bool save = true)
        {
            _db.DM_TUDIEN.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(DM_TUDIEN item)
        {
            if (item == null) return;
            _db.DM_TUDIEN.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
