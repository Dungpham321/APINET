using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_KIEMKE
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public DateTime? NGAY_SUA { get; set; }
        public long? DVQL_ID { get; set; }
        public long? DVSD_ID { get; set; }
        public DateTime? NGAY_KK { get; set; }
        public string? MA_BB { get; set; }
        public string? PHONG { get; set; }
        public string? NOI_DUNG { get; set; }
        public string? GHI_CHU { get; set; }
        public string? THANH_VIEN1 { get; set; }
        public string? THANH_VIEN2 { get; set; }
        public string? TRUONG_BAN1 { get; set; }
        public string? TRUONG_BAN2 { get; set; }
        public string? KHOKK { get; set; }
    }
    public class TB_KIEMKECollection : TableCollection
    {
        public TB_KIEMKECollection(GDBContext db) : base(db){}

        public IQueryable<TB_KIEMKE> Get()
        {
            return _db.TB_KIEMKE.AsQueryable();
        }
        public IQueryable<TB_KIEMKE> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_KIEMKE> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_KIEMKE> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
       
        public TB_KIEMKE GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public TB_KIEMKE GetByMA_BB(string MA_BB, long DVSD_ID)
        {
            return Get().FirstOrDefault(c => c.MA_BB == MA_BB && c.DVSD_ID == DVSD_ID);
        }
        public void Add(TB_KIEMKE item)
        {
            _db.TB_KIEMKE.Add(item);
            _db.SaveChanges();
        }

        public void Update(TB_KIEMKE item, bool save = true)
        {
            _db.TB_KIEMKE.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_KIEMKE item)
        {
            if (item == null) return;
            _db.TB_KIEMKE.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
