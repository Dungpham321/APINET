using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class TB_THIETBI
    {
        [Key]
        public long ID { get; set; }
        public long? NGUOIDUNG_ID { get; set; } // Nullable long
        public DateTime? NGAY_TAO { get; set; } // Nullable DateTime
        public DateTime? NGAY_SUA { get; set; } // Nullable DateTime
        public long? DVQL_ID { get; set; } // Nullable long
        public long? DVSD_ID { get; set; } // Nullable long
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public int? TINH_TRANG { get; set; } // Nullable int
        public string? LOAI_THIET_BI { get; set; }
        public string? SERI { get; set; }
        public string? DVT { get; set; }
        public DateTime? NGAY_MUA { get; set; } // Nullable DateTime
        public DateTime? NGAY_SD { get; set; } // Nullable DateTime
        public DateTime? HAN_BH { get; set; } // Nullable DateTime
        public DateTime? HAN_KH { get; set; } // Nullable DateTime
        public long? NHACC_ID { get; set; } // Nullable long
        public string? THONG_SO { get; set; }
        public string? GHI_CHU { get; set; }
        public decimal? GIA_MUA { get; set; } // Nullable decimal
        public decimal? VAT { get; set; } // Nullable decimal
        public decimal? THANH_TIEN { get; set; } // Nullable decimal
        public string? TT_SUDUNG { get; set; }
        public string? CT_SUDUNG { get; set; }
        public long? TBCHA_ID { get; set; } // Nullable long
        public string? CT_LUUTRU { get; set; }
        public bool? DATMAT { get; set; } // Nullable bool
        public DateTime? NGAYMAT { get; set; } // Nullable DateTime
        public string? GHICHUMAT { get; set; }
        public bool? DATHONG { get; set; } // Nullable bool
        public DateTime? NGAYHONG { get; set; } // Nullable DateTime
        public string? GHICHUHONG { get; set; }
        public bool? DATTL { get; set; } // Nullable bool
        public DateTime? NGAYTL { get; set; } // Nullable DateTime
        public string? GHICHUTL { get; set; }
        public long? KHO_ID { get; set; } // Nullable long
        public string? ANH { get; set; }
        public int? PHAN_LOAI { get; set; }
        public long? CANBO_ID { get; set; } // Nullable long
    }
    public class TB_THIETBICollection : TableCollection
    {
        public TB_THIETBICollection(GDBContext db) : base(db){}

        public IQueryable<TB_THIETBI> Get()
        {
            return _db.TB_THIETBI.AsQueryable();
        }
        public IQueryable<TB_THIETBI> Get(long DVQL_ID)
        {
            return Get().Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<TB_THIETBI> Get(long DVQL_ID, long DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<TB_THIETBI> Get(long DVQL_ID, List<long> DVSD_ID)
        {
            return Get(DVQL_ID).Where(c => DVSD_ID.Contains(c.DVSD_ID.Value));
        }
        public TB_THIETBI GetByMa(long DVQL_ID, long DVSD_ID, string MA)
        {
            return Get(DVQL_ID, DVSD_ID).FirstOrDefault(e => e.MA == MA);
        }
        public TB_THIETBI GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public IQueryable<TB_THIETBI> GetByIDS(List<long> IDS)
        {
            return Get().Where(c =>  IDS.Contains(c.ID));
        }
        public void Add(TB_THIETBI item)
        {
            _db.TB_THIETBI.Add(item);
            _db.SaveChanges();
        }
        public void Update(TB_THIETBI item, bool save = true)
        {
            _db.TB_THIETBI.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(TB_THIETBI item)
        {
            if (item == null) return;
            _db.TB_THIETBI.Remove(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
    }
}
