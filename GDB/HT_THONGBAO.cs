using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GCommon;

namespace GDB
{
    public class HT_THONGBAO
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        public string? TIEU_DE { get; set; }
        public string? NOI_DUNG { get; set; }
        public long? DVQL_ID { get; set; }
        public DateTime? NGAY_GUI { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public DateTime? NGAY_SUA { get; set; }
        public bool? GAP { get; set; }
        public int? TRANG_THAI { get; set; }
    }

    public class HT_THONGBAOCollection : TableCollection
    {
        public HT_THONGBAOCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_THONGBAO> Get()
        {
            return _db.HT_THONGBAO.AsQueryable().Where(c => c.TRANG_THAI != (int)TrangThai.DAXOA);
        }
        public IQueryable<HT_THONGBAO> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public HT_THONGBAO GetById(long id)
        {
            return Get().Where(x => x.ID == id).FirstOrDefault();
        }
        public void Add(HT_THONGBAO item)
        {
            _db.HT_THONGBAO.Add(item);
            _db.SaveChanges();
        }
        public void Remove(HT_THONGBAO item)
        {
            if (item == null) return;
            _db.HT_THONGBAO.Remove(item);
            _db.SaveChanges();
        }
        public void Update(HT_THONGBAO item, bool save = true)
        {
            _db.HT_THONGBAO.Update(item);
            if (save) _db.SaveChanges();
        }
    }
}
