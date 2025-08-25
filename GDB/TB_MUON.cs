using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_MUON
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_SUA { get; set; } 
        public long? DVQL_ID { get; set; } 
        public long? DVSD_ID { get; set; }
        public long? CANBO_ID { get; set; }
        public DateTime? NGAY_MUON { get; set; }
        public string? LY_DO { get; set; } 
        public DateTime? NGAY_TRA { get; set; }
        public string? GHI_CHU { get; set; }
     
    }
    public class TB_MUONCollection : TableCollection
    {
        public TB_MUONCollection(GDBContext db) : base(db){}

        public IQueryable<TB_MUON> Get()
        {
            return _db.TB_MUON.AsQueryable();
        }
        public IQueryable<TB_MUON> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_MUON> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_MUON> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
       
        public TB_MUON GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(TB_MUON item)
        {
            _db.TB_MUON.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_MUON item, bool save = true)
        {
            _db.TB_MUON.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_MUON item)
        {
            if (item == null) return;
            _db.TB_MUON.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
