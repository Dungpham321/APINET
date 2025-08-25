using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_GRID
    {
        [Key]
        public string GRID_ID { get; set; }
        public string? CAUHINH { get; set; }

    }

    public class HT_GRIDCollection : TableCollection
    {
        public HT_GRIDCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_GRID> Get()
        {
            return _db.HT_GRID.AsQueryable();
        }
        public HT_GRID GetByID(string GRID_ID)
        {
            return Get().FirstOrDefault(c => c.GRID_ID == GRID_ID);
        }
        public void Add(HT_GRID item)
        {
            _db.HT_GRID.Add(item);
            _db.SaveChanges();
        }
        public void Update(HT_GRID item, bool save = true)
        {
            _db.HT_GRID.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(HT_GRID item)
        {
            if (item == null) return;
            _db.HT_GRID.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(string ID)
        {
            Remove(GetByID(ID));
        }
    }
}
