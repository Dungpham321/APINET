using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_BANGIAOHD
    {
        [Key]
        public long ID { get; set; }
        public long? BANGIAO_ID { get; set; }
        public string? HO_TEN { get; set; }
        public string? CHUC_VU { get; set; }
        public string? VAI_TRO { get; set; }
        public bool? DAI_DIEN { get; set; }
    }

    public class TB_BANGIAOHDCollection : TableCollection
    {
        public TB_BANGIAOHDCollection(GDBContext db) : base(db) { }

        public IQueryable<TB_BANGIAOHD> Get()
        {
            return _db.TB_BANGIAOHD.AsQueryable();
        }
        public IQueryable<TB_BANGIAOHD> GetByBANGIAO_ID(long BANGIAO_ID)
        {
            return Get().Where(e => e.BANGIAO_ID == BANGIAO_ID);
        }
        public void Add(TB_BANGIAOHD item)
        {
            _db.TB_BANGIAOHD.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<TB_BANGIAOHD> item)
        {
            if (item.Count == 0) return;
            _db.TB_BANGIAOHD.AddRange(item);
            _db.SaveChanges();
        }
        public void Update(TB_BANGIAOHD item, bool save = true)
        {
            _db.TB_BANGIAOHD.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_BANGIAOHD item)
        {
            if (item == null) return;
            _db.TB_BANGIAOHD.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<TB_BANGIAOHD> item)
        {
            if (item.Count() == 0) return;
            _db.TB_BANGIAOHD.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByBANGIAO_ID(long BANGIAO_ID)
        {
            Remove(GetByBANGIAO_ID(BANGIAO_ID));
        }
    }
}
