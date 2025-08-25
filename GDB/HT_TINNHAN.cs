using GCommon;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_TINNHAN
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOITAO_ID { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public string? NOI_DUNG { get; set; }
        public int? LOAI_TIN { get; set; }
        public string? TIEU_DE { get; set; }
    }

    public class HT_TINNHAN_INFO
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOITAO_ID { get; set; }
        public string? NGUOITAO { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public int? LOAI_TIN { get; set; }
        public string? TIEU_DE { get; set; }
        public long TINNHAN_ND_ID { get; set; }
        public int TRANG_THAI { get; set; }
        public long NGUOINHAN_ID { get; set; }
    }

    public class HT_TINNHANCollection : TableCollection
    {
        public HT_TINNHANCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_TINNHAN> Get()
        {
            return _db.HT_TINNHAN.AsQueryable();
        }
        public HT_TINNHAN GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public IQueryable<HT_TINNHAN_INFO> GetInfoByNGUOINHAN_ID(long NGUOINHAN_ID)
        {
            string sql = @"SELECT HT_TINNHAN.ID,HT_TINNHAN.NGUOITAO_ID,HT_TINNHAN.NGAY_TAO,HT_TINNHAN.LOAI_TIN,HT_TINNHAN.TIEU_DE,HT_TINNHAN_ND.TRANG_THAI,
                HT_NGUOIDUNG.TEN_DANG_NHAP NGUOITAO,HT_TINNHAN_ND.ID TINNHAN_ND_ID,HT_TINNHAN_ND.NGUOINHAN_ID FROM HT_TINNHAN
                LEFT JOIN HT_NGUOIDUNG ON HT_TINNHAN.NGUOITAO_ID = HT_NGUOIDUNG.ID JOIN HT_TINNHAN_ND ON HT_TINNHAN.ID = HT_TINNHAN_ND.TINNHAN_ID
                WHERE HT_TINNHAN_ND.NGUOINHAN_ID={0} AND HT_TINNHAN_ND.TRANG_THAI != " + (int)TrangThai.DAXOA;
            return _db.HT_TINNHAN_INFO.FromSqlRaw(sql, NGUOINHAN_ID);
        }
        public void Add(HT_TINNHAN item)
        {
            _db.HT_TINNHAN.Add(item);
            _db.SaveChanges();
        }
    }
}
