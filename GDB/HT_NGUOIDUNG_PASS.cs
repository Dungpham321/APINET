using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_NGUOIDUNG_PASS
    {
        [Key]
        public long NGUOIDUNG_ID { get; set; }
        public string? PASSCODE { get; set; }
        public DateTime? CREATED { get; set; }

    }

    public class HT_NGUOIDUNG_PASSCollection : TableCollection
    {
        public HT_NGUOIDUNG_PASSCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_NGUOIDUNG_PASS> Get()
        {
            return _db.HT_NGUOIDUNG_PASS.AsQueryable();
        }

        public HT_NGUOIDUNG_PASS GetByNGUOIDUNG_ID(long NGUOIDUNG_ID)
        {
            return Get().FirstOrDefault(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID);
        }

        public void Add(HT_NGUOIDUNG_PASS item)
        {
            _db.HT_NGUOIDUNG_PASS.Add(item);
            _db.SaveChanges();
        }

        public void Update(HT_NGUOIDUNG_PASS item)
        {
            _db.HT_NGUOIDUNG_PASS.Update(item);
            _db.SaveChanges();
        }

        public void Remove(HT_NGUOIDUNG_PASS item)
        {
            if (item == null) return;
            _db.HT_NGUOIDUNG_PASS.Remove(item);
            _db.SaveChanges();
        }

        public void Remove(long NGUOIDUNG_ID)
        {
            Remove(GetByNGUOIDUNG_ID(NGUOIDUNG_ID));
        }
    }
}
