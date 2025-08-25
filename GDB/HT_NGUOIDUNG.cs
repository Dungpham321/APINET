using Microsoft.EntityFrameworkCore;
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
    public class HT_NGUOIDUNG : BaseTable
    {
        [Key]
        public long ID { get; set; }
        public string? TEN_DANG_NHAP { get; set; }
        public string? MAT_KHAU { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NGAY_TAO { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NGAY_SUA { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NGAY_DANG_NHAP { get; set; }
        public int? TRANG_THAI { get; set; }
        public string? TEN_DAY_DU { get; set; }
        public string? DIEN_THOAI { get; set; }
        public string? EMAIL { get; set; }
        public string? MO_TA { get; set; }
        public long DVQL_ID { get; set; }
        public long DVSD_ID { get; set; }
        public int? SAI_MAT_KHAU { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? NGAY_MAT_KHAU { get; set; }
        public bool? TFA { get; set; }
        public long? NGUOIDUNG_ID { get; set; }
        public long? CANBO_ID { get; set; }
    }

    public class HT_NGUOIDUNGCollection : TableCollection
    {
        public HT_NGUOIDUNGCollection(GDBContext db) : base(db) { }
        public IQueryable<HT_NGUOIDUNG> GetAll()
        {
            return _db.HT_NGUOIDUNG.AsQueryable();
        }

        public IQueryable<HT_NGUOIDUNG> Get(bool khach = false)
        {
            if(khach) return GetAll().Where(c => c.TRANG_THAI != (int)TrangThai.DAXOA);
            return GetAll().Where(c => c.TRANG_THAI != (int)TrangThai.DAXOA && c.TEN_DANG_NHAP.ToLower() != "khach");
        }
        public IQueryable<HT_NGUOIDUNG> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<HT_NGUOIDUNG> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<HT_NGUOIDUNG> Get(long DVQL_ID, List<long> DVSD_IDS)
        {
            return Get(DVQL_ID).Where(c => DVSD_IDS.Contains(c.DVSD_ID));
        }
        public HT_NGUOIDUNG GetByKhach()
        {
            return Get(true).FirstOrDefault(c => c.TEN_DANG_NHAP.ToLower() == "khach");
        }
        public HT_NGUOIDUNG GetByTEN_DANG_NHAP(string TEN_DANG_NHAP, long DVQL_ID)
        {
            return Get(DVQL_ID).FirstOrDefault(c => c.TEN_DANG_NHAP.ToLower() == TEN_DANG_NHAP.ToLower());
        }
        public HT_NGUOIDUNG GetByEMAIL(string EMAIL, long DVQL_ID)
        {
            return Get(DVQL_ID).FirstOrDefault(c => c.EMAIL.ToLower() == EMAIL.ToLower());
        }
        public HT_NGUOIDUNG GetByDIEN_THOAI(string DIEN_THOAI, long DVQL_ID)
        {
            return Get(DVQL_ID).FirstOrDefault(c => c.DIEN_THOAI.ToLower() == DIEN_THOAI.ToLower());
        }
        public HT_NGUOIDUNG GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public IQueryable<HT_NGUOIDUNG> GetByIDS(List<long> IDS)
        {
            return Get().Where(c => IDS.Contains(c.ID));
        }
        public IQueryable<HT_NGUOIDUNG> GetByQuyen(string quyen)
        {
            var dsNHOMQUYEN_ID = _db.HT_DOITUONG_QUYENCollection.GetByQUYEN(DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString(), quyen).Select(s => s.DOITUONG_ID).ToList();
            var dsNGUOIDUNG_ID = _db.HT_NGUOIDUNG_SDCollection.GetByDoiTuong(dsNHOMQUYEN_ID, DsDoiTuong.DM_DANHMUC, DsChucNang.NhomQuyen.ToString()).Select(s => s.NGUOIDUNG_ID).ToList();
            return Get().Where(c => dsNGUOIDUNG_ID.Contains(c.ID));
        }
        public void Add(HT_NGUOIDUNG item)
        {
            _db.HT_NGUOIDUNG.Add(item);
            _db.SaveChanges();
        }

        public void Update(HT_NGUOIDUNG item, bool save = true)
        {
            _db.HT_NGUOIDUNG.Update(item);
            if (save) _db.SaveChanges();
        }

        public void Remove(HT_NGUOIDUNG item)
        {
            if (item == null) return;
            item.TRANG_THAI = (int)TrangThai.DAXOA;
            Update(item, false);
            _db.SaveChanges();
        }

        public void RemoveAllByID(long ID)
        {
            _db.Database.ExecuteSqlRaw("EXECUTE HETHONG$SP_XOA_NGUOIDUNG @p_NGUOIDUNG_ID={0}", ID);
        }

        public void RemoveByID(long NGUOIDUNG_ID)
        {
            Remove(GetByID(NGUOIDUNG_ID));
        }
    }
}
