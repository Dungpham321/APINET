using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_TEP_SD
    {
        [Key]
        public long ID { get; set; }
        public long? TEP_ID { get; set; }
        public long? DOITUONG_ID { get; set; }
        public string? CHUCNANG { get; set; }
        public string? DOITUONG_LOAI { get; set; }
        public long? ND_ID { get; set; }
    }

    public class HT_TEP_SDCollection : TableCollection
    {
        public HT_TEP_SDCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_TEP_SD> Get()
        {
            return _db.HT_TEP_SD.AsQueryable();
        }
        public IQueryable<HT_TEP_SD> Get(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.CHUCNANG == CHUCNANG && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_TEP_SD> Get(long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI, long ND_ID)
        {
            return Get(DOITUONG_ID, CHUCNANG, DOITUONG_LOAI).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<HT_TEP_SD> Get(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.ND_ID == ND_ID && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_TEP_SD> Get(List<long> DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            return Get().Where(c => DOITUONG_ID.Contains(c.DOITUONG_ID.Value) && c.CHUCNANG == CHUCNANG && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_TEP_SD> Get(long DOITUONG_ID, string CHUCNANG)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_TEP_SD> Get(string CHUCNANG, string DOITUONG_LOAI)
        {
            return Get().Where(c => c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_TEP_SD> Get(long DOITUONG_ID, long ND_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.ND_ID == ND_ID);
        }
        public IQueryable<HT_TEP_SD> GetByTEP_ID(long TEP_ID)
        {
            return Get().Where(c => c.TEP_ID == TEP_ID);
        }
        public IQueryable<HT_TEP_SD> Get(long TEP_ID, long DOITUONG_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            return Get().Where(c => c.TEP_ID == TEP_ID && c.DOITUONG_ID == DOITUONG_ID && c.CHUCNANG == CHUCNANG && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_TEP_SD> GetByTEP_ID(long TEP_ID, string CHUCNANG, string DOITUONG_LOAI)
        {
            return Get().Where(c => c.TEP_ID == TEP_ID && c.CHUCNANG == CHUCNANG && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public void Add(HT_TEP_SD item)
        {
            _db.HT_TEP_SD.Add(item);
            _db.SaveChanges();
        }
        public void Update(List<HT_TEP_SD> items)
        {
            if (items == null || items.Count == 0) return;
            _db.HT_TEP_SD.UpdateRange(items);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<HT_TEP_SD> items)
        {
            if (items.Count() > 0)
            {
                _db.HT_TEP_SD.RemoveRange(items);
                _db.SaveChanges();
            }
        }
        public void Remove(HT_TEP_SD item)
        {
            if (item == null) return;
            _db.HT_TEP_SD.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByTEP_ID(long TEP_ID)
        {
            Remove(GetByTEP_ID(TEP_ID));
        }
    }
}
