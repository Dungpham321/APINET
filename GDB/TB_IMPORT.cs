using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_IMPORT
    {
        [Key]
        public long ID { get; set; }
        public decimal? ID_MOI { get; set; }
        public string? LOAI { get; set; } 
        public decimal? ID_CU { get; set; }
        public string? GID_CU { get; set; }
    }
    public class TB_IMPORTCollection : TableCollection
    {
        public TB_IMPORTCollection(GDBContext db) : base(db){}

        public IQueryable<TB_IMPORT> Get()
        {
            return _db.TB_IMPORT.AsQueryable();
        }
        public TB_IMPORT Get(decimal ID_MOI, string LOAI)
        {
            return Get().FirstOrDefault(c => c.ID_MOI == ID_MOI && c.LOAI == LOAI);
        }
        public TB_IMPORT Get(string LOAI, decimal ID_CU)
        {
            return Get().FirstOrDefault(c => c.ID_CU == ID_CU && c.LOAI == LOAI);
        }
        public TB_IMPORT Get(string LOAI, string GID_CU)
        {
            return Get().FirstOrDefault(c => c.GID_CU == GID_CU && c.LOAI == LOAI);
        }
        public TB_IMPORT GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(TB_IMPORT item)
        {
            _db.TB_IMPORT.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_IMPORT item, bool save = true)
        {
            _db.TB_IMPORT.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_IMPORT item)
        {
            if (item == null) return;
            _db.TB_IMPORT.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(decimal ID_MOI, string LOAI)
        {
            Remove(Get(ID_MOI, LOAI));
        }
        public void Remove(string LOAI, decimal ID_CU)
        {
            Remove(Get(LOAI, ID_CU));
        }
        public void Remove(string LOAI, string GID_CU)
        {
            Remove(Get(LOAI, GID_CU));
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
