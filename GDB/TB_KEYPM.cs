using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_KEYPM
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_SUA { get; set; }
        public long? DVQL_ID { get; set; }
        public long? DVSD_ID { get; set; }
        public string? MA { get; set; }
        public DateTime? NGAY_MUA { get; set; }
        public DateTime? NGAY_KICH_HOAT { get; set; }
        public int? NGAY_SU_DUNG { get; set; }
        public string? GHI_CHU { get; set; }

    }
    public class TB_KEYPMCollection : TableCollection
    {
        public TB_KEYPMCollection(GDBContext db) : base(db){}

        public IQueryable<TB_KEYPM> Get()
        {
            return _db.TB_KEYPM.AsQueryable();
        }
        public IQueryable<TB_KEYPM> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_KEYPM> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_KEYPM> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
       
        public TB_KEYPM GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(TB_KEYPM item)
        {
            _db.TB_KEYPM.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_KEYPM item, bool save = true)
        {
            _db.TB_KEYPM.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_KEYPM item)
        {
            if (item == null) return;
            _db.TB_KEYPM.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
