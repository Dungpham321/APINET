using GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_TINNHAN_ND
    {
        [Key]
        public long ID { get; set; }
        public long TINNHAN_ID { get; set; }
        public long NGUOINHAN_ID { get; set; }
        public int TRANG_THAI { get; set; }
    }

    public class HT_TINNHAN_NDCollection : TableCollection
    {
        public HT_TINNHAN_NDCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_TINNHAN_ND> Get()
        {
            return _db.HT_TINNHAN_ND.AsQueryable().Where(c => c.TRANG_THAI != (int)TrangThai.DAXOA);
        }
        public IQueryable<HT_TINNHAN_ND> GetAll()
        {
            return _db.HT_TINNHAN_ND.AsQueryable();
        }
        public HT_TINNHAN_ND GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public HT_TINNHAN_ND GetByNGUOINHAN_ID(long NGUOINHAN_ID, long TINNHAN_ID)
        {
            return Get().FirstOrDefault(c => c.NGUOINHAN_ID == NGUOINHAN_ID && c.TINNHAN_ID == TINNHAN_ID);
        }
        public IQueryable<HT_TINNHAN_ND> GetByTINNHAN_ID(long TINNHAN_ID)
        {
            return Get().Where(c => c.TINNHAN_ID == TINNHAN_ID);
        }

        public void Add(HT_TINNHAN_ND item)
        {
            _db.HT_TINNHAN_ND.Add(item);
            _db.SaveChanges();
        }
        public void Update(HT_TINNHAN_ND item)
        {
            _db.HT_TINNHAN_ND.Update(item);
            _db.SaveChanges();
        }
        public void Remove(HT_TINNHAN_ND item)
        {
            if (item == null) return;
            item.TRANG_THAI = (int)TrangThai.DAXOA;
            _db.HT_TINNHAN_ND.Update(item);
            _db.SaveChanges();
        }
    }
}
