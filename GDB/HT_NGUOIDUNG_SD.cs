using GCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_NGUOIDUNG_SD
    {
        public long ID { get; set; }
        public long NGUOIDUNG_ID { get; set; }
        public long DOITUONG_ID { get; set; }
        public string DOITUONG_LOAI { get; set; }
        public string CHUCNANG { get; set; }
        public long ND_ID { get; set; }
        public string? DATA { get; set; }
    }

    public class HT_NGUOIDUNG_SDCollection : TableCollection
    {
        public HT_NGUOIDUNG_SDCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_NGUOIDUNG_SD> Get()
        {
            return _db.HT_NGUOIDUNG_SD.AsQueryable();
        }
        //doi tuong
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(long DOITUONG_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI)
        {
            return GetByDoiTuong(DOITUONG_ID).Where(c => c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI).Where(c => c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG, long ND_ID)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(List<long> DOITUONG_ID, string DOITUONG_LOAI)
        {
            return Get().Where(c => DOITUONG_ID.Contains(c.DOITUONG_ID) && c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuong(List<long> DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI).Where(c => c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByDoiTuongND(long DOITUONG_ID, long ND_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.ND_ID == ND_ID);
        }
        //nguoi dung
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, string CHUCNANG, long ND_ID)
        {
            return GetByNguoiDung(NGUOIDUNG_ID, DOITUONG_LOAI, CHUCNANG).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID && c.DOITUONG_LOAI == DOITUONG_LOAI && c.ND_ID == ND_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI)
        {
            return GetByNguoiDung(NGUOIDUNG_ID).Where(c => c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(List<long> NGUOIDUNG_ID)
        {
            return Get().Where(c => NGUOIDUNG_ID.Contains(c.NGUOIDUNG_ID));
        }
        public IQueryable<HT_NGUOIDUNG_SD> GetByNguoiDung(long NGUOIDUNG_ID, long ND_ID)
        {
            return Get().Where(c => c.NGUOIDUNG_ID == NGUOIDUNG_ID);
        }
        //----
        public HT_NGUOIDUNG_SD GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(HT_NGUOIDUNG_SD item)
        {
            _db.HT_NGUOIDUNG_SD.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<HT_NGUOIDUNG_SD> items)
        {
            if (items == null || items.Count == 0) return;
            _db.HT_NGUOIDUNG_SD.AddRange(items);
            _db.SaveChanges();
        }
        public void Update(HT_NGUOIDUNG_SD item, bool save = true)
        {
            _db.HT_NGUOIDUNG_SD.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Update(List<HT_NGUOIDUNG_SD> items)
        {
            if (items == null || items.Count == 0) return;
            _db.HT_NGUOIDUNG_SD.UpdateRange(items);
            _db.SaveChanges();
        }
        public void UpdateByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG, long ND_ID)
        {
            List<HT_NGUOIDUNG_SD> items = GetByDoiTuong(0, DOITUONG_LOAI, CHUCNANG, ND_ID).ToList();
            items.ForEach(c => c.DOITUONG_ID = DOITUONG_ID);
            Update(items);
        }
        public void UpdateByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            List<HT_NGUOIDUNG_SD> items = GetByDoiTuong(0, DOITUONG_LOAI, ND_ID).ToList();
            items.ForEach(c => c.DOITUONG_ID = DOITUONG_ID);
            Update(items);
        }
        public void UpdateByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, string CHUCNANG, long ND_ID)
        {
            List<HT_NGUOIDUNG_SD> items = GetByNguoiDung(0, DOITUONG_LOAI, CHUCNANG, ND_ID).ToList();
            items.ForEach(c => c.NGUOIDUNG_ID = NGUOIDUNG_ID);
            Update(items);
        }
        public void UpdateByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            List<HT_NGUOIDUNG_SD> items = GetByNguoiDung(0, DOITUONG_LOAI, ND_ID).ToList();
            items.ForEach(c => c.NGUOIDUNG_ID = NGUOIDUNG_ID);
            Update(items);
        }
        //remove
        public void Remove(HT_NGUOIDUNG_SD item)
        {
            if (item == null) return;
            _db.HT_NGUOIDUNG_SD.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<HT_NGUOIDUNG_SD> items)
        {
            if (items.Count() == 0) return;
            _db.HT_NGUOIDUNG_SD.RemoveRange(items);
            _db.SaveChanges();
        }
        public void Remove(List<HT_NGUOIDUNG_SD> items)
        {
            if (items.Count() == 0) return;
            _db.HT_NGUOIDUNG_SD.RemoveRange(items);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        //RemoveByDoiTuong
        public void RemoveByDoiTuong(long DOITUONG_ID)
        {
            Remove(GetByDoiTuong(DOITUONG_ID));
        }
        public void RemoveByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            Remove(GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG));
        }
        public void RemoveByDoiTuong(List<long> DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            Remove(GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG));
        }
        public void RemoveByDoiTuongND(long DOITUONG_ID, long ND_ID)
        {
            Remove(GetByDoiTuongND(DOITUONG_ID, ND_ID));
        }
        //RemoveByNguoiDung
        public void RemoveByNguoiDung(long NGUOIDUNG_ID)
        {
            Remove(GetByNguoiDung(NGUOIDUNG_ID));
        }

        public void RemoveByNguoiDung(long NGUOIDUNG_ID, long ND_ID)
        {
            Remove(GetByNguoiDung(NGUOIDUNG_ID, ND_ID));
        }

        public void RemoveByNguoiDung(long NGUOIDUNG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            Remove(GetByNguoiDung(NGUOIDUNG_ID, DOITUONG_LOAI, CHUCNANG));
        }
    }
}
