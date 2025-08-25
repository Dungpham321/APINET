using GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_DOITUONG_QUYEN : BaseTable
    {
        [Key]
        public long ID { get; set; }
        public long DOITUONG_ID { get; set; }
        public string DOITUONG_LOAI { get; set; }
        [StringLength(100)]
        public string QUYEN { get; set; }
        public string? CHUCNANG { get; set; }
    }

    public class HT_DOITUONG_QUYENCollection : TableCollection
    {
        public HT_DOITUONG_QUYENCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_DOITUONG_QUYEN> Get()
        {
            return _db.HT_DOITUONG_QUYEN.AsQueryable();
        }
        public IQueryable<HT_DOITUONG_QUYEN> GetByDsDOITUONG_ID(List<long> DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c => DOITUONG_ID.Contains(c.DOITUONG_ID) && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_DOITUONG_QUYEN> GetByDOITUONG_ID(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            return Get().Where(c => c.DOITUONG_ID == DOITUONG_ID && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public IQueryable<HT_DOITUONG_QUYEN> GetByNGUOIDUNG_ID(long NGUOIDUNG_ID)
        {
            var nhomquyen = _db.HT_NGUOIDUNG_SDCollection.GetByNguoiDung(NGUOIDUNG_ID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString()).Select(s => s.DOITUONG_ID).ToList();
            return GetByDsDOITUONG_ID(nhomquyen, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString());
        }
        public IQueryable<HT_DOITUONG_QUYEN> GetByQUYEN(string DOITUONG_LOAI, string CHUCNANG, string QUYEN)
        {
            return Get().Where(c => c.QUYEN == QUYEN && c.DOITUONG_LOAI == DOITUONG_LOAI && c.CHUCNANG == CHUCNANG);
        }
        public void Add(HT_DOITUONG_QUYEN item)
        {
            _db.HT_DOITUONG_QUYEN.Add(item);
            _db.SaveChanges();
        }
        public void Update(HT_DOITUONG_QUYEN item, bool save = true)
        {
            _db.HT_DOITUONG_QUYEN.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(HT_DOITUONG_QUYEN item)
        {
            if (item == null) return;
            _db.HT_DOITUONG_QUYEN.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<HT_DOITUONG_QUYEN> item)
        {
            if (item == null) return;
            _db.HT_DOITUONG_QUYEN.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByDOITUONG_ID(long DOITUONG_ID, string DOITUONG_LOAI, string CHUCNANG)
        {
            Remove(GetByDOITUONG_ID(DOITUONG_ID, DOITUONG_LOAI, CHUCNANG));
        }
        public void AddBulk(List<HT_DOITUONG_QUYEN> item, bool save = false)
        {
            _db.HT_DOITUONG_QUYEN.AddRange(item);
            if (save) _db.SaveChanges();
        }
        public void RemoveBulk(List<HT_DOITUONG_QUYEN> item, bool save = false)
        {
            _db.HT_DOITUONG_QUYEN.RemoveRange(item);
            if (save) _db.SaveChanges();
        }
    }
}
