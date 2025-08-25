using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_LICHSU : BaseTable
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public string? THAO_TAC { get; set; }
        public string? MA_TAC_DONG { get; set; }
        public string? MO_TA { get; set; }
        public string? DATA { get; set; }
        public long? DVQL_ID { get; set; }
        public long? DVSD_ID { get; set; }
    }

    public class HT_LICHSUCollection : TableCollection
    {
        public HT_LICHSUCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_LICHSU> Get()
        {
            return _db.HT_LICHSU.AsQueryable();
        }
        public IQueryable<HT_LICHSU> GetByDVQL_ID(long DVQL_ID)
        {
            return Get().Where(e => e.DVQL_ID == DVQL_ID);
        }
        public IQueryable<HT_LICHSU> GetByDVQL_ID(long DVQL_ID, List<long> DVSD_IDS)
        {
            return GetByDVQL_ID(DVQL_ID).Where(e => e.DVSD_ID.HasValue && DVSD_IDS.Contains(e.DVSD_ID.Value));
        }
        public HT_LICHSU GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(HT_LICHSU item)
        {
            _db.HT_LICHSU.Add(item);
            _db.SaveChanges();
        }

        public void Update(HT_LICHSU item, bool save = true)
        {
            _db.HT_LICHSU.Update(item);
            if (save) _db.SaveChanges();
        }

        public void Remove(HT_LICHSU item)
        {
            if (item == null) return;
            _db.HT_LICHSU.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
