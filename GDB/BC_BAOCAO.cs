using GCommon;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class BC_BAOCAO
    {
        [Key]
        public long ID { get; set; }
        public string MA { get; set; }
        public string? TEN { get; set; }
        public string? THAM_SO { get; set; }
        public string? HAM_THUC_HIEN { get; set; }
        public string? BANG_DU_LIEU { get; set; }
        public int? TRANG_THAI { get; set; }
        public string? TEN_FILE { get; set; }
        public string? CAU_HINH { get; set; }
        public int? SAP_XEP { get; set; }
        public string? DIEU_KIEN_NHOM { get; set; }
        public string? GHI_CHU { get; set; }
        public string? KHOGIAY { get; set; }
        public string? NHOMBC { get; set; }
    }

    public class BC_BAOCAOCollection : TableCollection
    {
        public BC_BAOCAOCollection(GDBContext db) : base(db) { }

        public IQueryable<BC_BAOCAO> Get()
        {
            return _db.BC_BAOCAO.AsQueryable();
        }
        public BC_BAOCAO GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public BC_BAOCAO GetByMA(string MA)
        {
            return Get().FirstOrDefault(c => c.MA == MA);
        }
        public void Add(BC_BAOCAO item)
        {
            _db.BC_BAOCAO.Add(item);
            _db.SaveChanges();
        }

        public void Update(BC_BAOCAO item, bool save = true)
        {
            _db.BC_BAOCAO.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(BC_BAOCAO item)
        {
            if (item == null) return;
            _db.BC_BAOCAO.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveByMA(string MA)
        {
            Remove(GetByMA(MA));
        }
    }
}
