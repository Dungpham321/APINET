using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_NGUOIDUNG_ON : BaseTable
    {
        [Key]
        [Required]
        public string CONNECTION_ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public bool? LONLINE { get; set; }
        public DateTime? CONNECTED { get; set; }
        public DateTime? DISCONNECTED { get; set; }
        public string? DEVICE { get; set; }
        public string? URL_API { get; set; }
        public string? URL_RPT { get; set; }
        public string? URL { get; set; }
    }

    public class HT_NGUOIDUNG_ONCollection : TableCollection
    {
        public HT_NGUOIDUNG_ONCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_NGUOIDUNG_ON> Get()
        {
            return _db.HT_NGUOIDUNG_ON.AsQueryable();
        }
        public IQueryable<HT_NGUOIDUNG_ON> Get(long NGUOIDUNG_ID)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID);
        }
        public IQueryable<HT_NGUOIDUNG_ON> Get(long NGUOIDUNG_ID, string sessionID)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID && c.DEVICE == sessionID);
        }
        public IQueryable<HT_NGUOIDUNG_ON> GetBysessionID(string sessionID)
        {
            return Get().Where(c => c.DEVICE == sessionID);
        }
        public HT_NGUOIDUNG_ON GetByCONNECTION_ID(string CONNECTION_ID)
        {
            return Get().FirstOrDefault(c => c.CONNECTION_ID == CONNECTION_ID);
        }
        public void Add(HT_NGUOIDUNG_ON item)
        {
            _db.HT_NGUOIDUNG_ON.Add(item);
            _db.SaveChanges();
        }
        public void Update(HT_NGUOIDUNG_ON item)
        {
            _db.HT_NGUOIDUNG_ON.Update(item);
            _db.SaveChanges();
        }
        public void Remove(HT_NGUOIDUNG_ON item)
        {
            if (item == null) return;
            _db.HT_NGUOIDUNG_ON.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByCONNECTION_ID(string CONNECTION_ID)
        {
            Remove(GetByCONNECTION_ID(CONNECTION_ID));
        }
        public void Remove(IQueryable<HT_NGUOIDUNG_ON> item)
        {
            _db.HT_NGUOIDUNG_ON.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByNGUOIDUNG_ID(long NGUOIDUNG_ID)
        {
            Remove(Get(NGUOIDUNG_ID));
        }
    }
}
