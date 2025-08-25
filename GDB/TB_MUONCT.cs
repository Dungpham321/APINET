using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_MUONCT
    {
        [Key]
        public long ID { get; set; }
        public long? MUON_ID { get; set; }
        public long? THIETBI_ID { get; set; }
    }
    public class TB_MUONCTCollection : TableCollection
    {
        public TB_MUONCTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_MUONCT> Get()
        {
            return _db.TB_MUONCT.AsQueryable();
        }
        public IQueryable<TB_MUONCT> GetByMUON_ID(long MUON_ID)
        {
            return Get().Where(e => e.MUON_ID == MUON_ID);
        }
        public IQueryable<TB_MUONCT> GetByTHIETBI_ID(long THIETBI_ID)
        {
            return Get().Where(e => e.THIETBI_ID == THIETBI_ID);
        }
        public void Add(TB_MUONCT item)
        {
            _db.TB_MUONCT.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_MUONCT> item)
        {
            if (item.Count == 0) return;
            _db.TB_MUONCT.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_MUONCT item, bool save = true)
        {
            _db.TB_MUONCT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_MUONCT item)
        {
            if (item == null) return;
            _db.TB_MUONCT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_MUONCT> item)
        {
            if (item.Count() == 0) return;
            _db.TB_MUONCT.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByMUON_ID(long MUON_ID)
        {
            Remove(GetByMUON_ID(MUON_ID));
        }
        public void RemoveByTHIETBI_ID(long THIETBI_ID)
        {
            Remove(GetByTHIETBI_ID(THIETBI_ID));
        }
    }
}
