using GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_DANHMUC_SD
    {
        [Key]
        public long ID { get; set; }
        public long DANHMUCITEM_ID { get; set; }
        public long DOITUONG_ID { get; set; }
        public string DOITUONG_LOAI { get; set; }
        public string CHUCNANG { get; set; }
        public string? DATA { get; set; }
        public long ND_ID { get; set; }
        public string? C1 { get; set; }
        public string? C2 { get; set; }

    }

    public class DM_DANHMUC_SDCollection : TableCollection
    {
        public DM_DANHMUC_SDCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_DANHMUC_SD> Get()
        {
            return _db.DM_DANHMUC_SD.AsQueryable();
        }
        public DM_DANHMUC_SD Get(long DANHMUCITEM_ID, long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().FirstOrDefault(c => c.DANHMUCITEM_ID == DANHMUCITEM_ID && c.DOITUONG_ID == DOITUONG_ID && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        //doituong
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI)
        {
            return GetByDoiTuong(DOITUONG_ID).Where(c => c.DOITUONG_LOAI == DOITUONG_LOAI);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI).Where(c => c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG, long ND_ID)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            return GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI).Where(c => c.ND_ID == ND_ID);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c => c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(string DOITUONG_LOAI, string CHUCNANG, long DANHMUC_ID)
        {
            return GetByDoiTuong(DANHMUC_ID, DOITUONG_LOAI).Where(c => c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDoiTuong(long DOITUONG_ID, long ND_ID)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.ND_ID == ND_ID);
        }
        //danhmuc
        public IQueryable<DM_DANHMUC_SD> GetByDanhMuc(long DANHMUCITEM_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c =>  c.DANHMUCITEM_ID == DANHMUCITEM_ID && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<DM_DANHMUC_SD> GetByDanhMuc(List<long> DANHMUCITEM_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c => DANHMUCITEM_ID.Contains(c.DANHMUCITEM_ID) && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        //nguoidung
        //public IQueryable<DM_DANHMUC_SD> GetByNguoiDung(long ND_ID)
        //{
        //    return Get().Where(c => c.ND_ID == ND_ID);
        //}

        public DM_DANHMUC_SD GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public void Add(DM_DANHMUC_SD item)
        {
            _db.DM_DANHMUC_SD.Add(item);
            _db.SaveChanges();
        }
        public void Add(List<DM_DANHMUC_SD> items)
        {
            if (items == null || items.Count == 0) return;
            _db.DM_DANHMUC_SD.AddRange(items);
            _db.SaveChanges();
        }
        public void Update(DM_DANHMUC_SD item, bool save = true)
        {
            _db.DM_DANHMUC_SD.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Update(List<DM_DANHMUC_SD> items)
        {
            if (items == null || items.Count == 0) return;
            _db.DM_DANHMUC_SD.UpdateRange(items);
            _db.SaveChanges();
        }
        public void UpdateByDoiTuong(long DOITUONG_ID, string DOITUONG_LOAI, long ND_ID)
        {
            List<DM_DANHMUC_SD> items = GetByDoiTuong(0, DOITUONG_LOAI, ND_ID).ToList();
            items.ForEach(c => c.DOITUONG_ID = DOITUONG_ID);
            Update(items);
        }
        public void Remove(DM_DANHMUC_SD item)
        {
            if (item == null) return;
            _db.DM_DANHMUC_SD.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<DM_DANHMUC_SD> items)
        {
            if (items.Count() == 0) return;
            _db.DM_DANHMUC_SD.RemoveRange(items);
            _db.SaveChanges();
        }
        public void Remove(List<DM_DANHMUC_SD> items)
        {
            if (items.Count() == 0) return;
            _db.DM_DANHMUC_SD.RemoveRange(items);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveByDoituong(long DOITUONG_ID)
        {
            Remove(GetByDoiTuong(DOITUONG_ID));
        }
        public void RemoveByDoituong(long DOITUONG_ID, long ND_ID)
        {
            Remove(GetByDoiTuong(DOITUONG_ID, ND_ID));
        }
        public void RemoveByDoituong(long DOITUONG_ID, string DOITUONG_LOAI)
        {
            Remove(GetByDoiTuong(DOITUONG_ID, DOITUONG_LOAI));
        }
    }
}
