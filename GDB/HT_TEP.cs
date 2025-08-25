using GDB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_TEP
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public string? TEN_TEP { get; set; }
        public string? DUONG_DAN { get; set; }
        public string? KIEU_TEP { get; set; }
        public long? KICH_THUOC { get; set; }
        public int? TRANG_THAI { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public string? TEN_BAN_DAU { get; set; }
        public bool? KY_SO { get; set; }
        public string? TGROUP { get; set; }
    }
    public class HT_TEP_INFO
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public string? TEN_TEP { get; set; }
        public string? DUONG_DAN { get; set; }
        public string? KIEU_TEP { get; set; }
        public long? KICH_THUOC { get; set; }
        public int? TRANG_THAI { get; set; }
        public DateTime? NGAY_TAO { get; set; }
        public string? TEN_BAN_DAU { get; set; }
        public string? NGUOITAO { get; set; }
        public bool? Q_XEM { get; set; }
        public bool? Q_TAI { get; set; }
        public bool? Q_XOA { get; set; }
        public bool? KY_SO { get; set; }
        public string? TGROUP { get; set; }
    }
    public class HT_TEPCollection : TableCollection
    {
        public HT_TEPCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_TEP> Get()
        {
            return _db.HT_TEP.AsQueryable();
        }
        public IQueryable<HT_TEP_INFO> GetInfo()
        {
            string sql = @"SELECT HT_TEP.ID, HT_TEP.NGUOIDUNG_ID, TEN_TEP,DUONG_DAN,KIEU_TEP,KICH_THUOC,HT_TEP.TRANG_THAI,HT_TEP.NGAY_TAO,TEN_BAN_DAU,TEN_DANG_NHAP NGUOITAO,
                CAST(1 AS BIT) Q_XEM, CAST(1 AS BIT) Q_XOA, CAST(1 AS BIT) Q_TAI, KY_SO, TGROUP
                FROM HT_TEP LEFT JOIN HT_NGUOIDUNG ON HT_TEP.NGUOIDUNG_ID = HT_NGUOIDUNG.ID";
            return _db.HT_TEP_INFO.FromSqlRaw(sql);
        }
        public IQueryable<HT_TEP> Get(List<long> IDS)
        {
            return Get().Where(c => IDS.Contains(c.ID));
        }
        public IQueryable<HT_TEP> Get(List<long?> IDS)
        {
            return Get().Where(c => IDS.Contains(c.ID));
        }
        public IQueryable<HT_TEP_INFO> GetInfo(List<long> IDS)
        {
            return GetInfo().Where(c => IDS.Contains(c.ID));
        }
        public HT_TEP GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(HT_TEP item)
        {
            _db.HT_TEP.Add(item);
            _db.SaveChanges();
        }

        public void Update(HT_TEP item, bool save = true)
        {
            _db.HT_TEP.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Update(List<HT_TEP> items)
        {
            if (items == null || items.Count == 0) return;
            _db.HT_TEP.UpdateRange(items);
            _db.SaveChanges();
        }
        public void Remove(HT_TEP item)
        {
            if (item == null) return;
            _db.HT_TEP.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
