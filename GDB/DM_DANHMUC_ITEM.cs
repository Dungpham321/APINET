using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class DM_DANHMUC_ITEM
    {
        [Key]
        public long ID { get; set; }
        public long? DANHMUC_ID { get; set; }
        public string? MA { get; set; }
        public string? TEN { get; set; }
        public string? MO_TA { get; set; }
        public int? TRANG_THAI { get; set; }
        public long? PID { get; set; }
        public bool? HASCHILD { get; set; }
        public int? SAP_XEP { get; set; }
        public long? DVSD_ID { get; set; }
        public long? DVQL_ID { get; set; }
        public string? DATA { get; set; }
        public decimal? CC1 { get; set; }
        public decimal? CC2 { get; set; }
        public decimal? CC3 { get; set; }
        public decimal? CC4 { get; set; }
        public decimal? CC5 { get; set; }
        public string? CT1 { get; set; }
        public string? CT2 { get; set; }
        public string? CT3 { get; set; }
        public string? CT4 { get; set; }
        public string? CT5 { get; set; }
        public bool HETHONG { get; set; }
        public bool? CB1 { get; set; }
        public bool? CB2 { get; set; }
        public bool? CB3 { get; set; }
        public bool? CB4 { get; set; }
        public bool? CB5 { get; set; }
        public DateTime? CD1 { get; set; }
        public DateTime? CD2 { get; set; }
        public DateTime? CD3 { get; set; }
        public DateTime? CD4 { get; set; }
        public DateTime? CD5 { get; set; }

    }

    public class DM_DANHMUC_ITEMCollection : TableCollection
    {
        public DM_DANHMUC_ITEMCollection(GDBContext db) : base(db) { }

        public IQueryable<DM_DANHMUC_ITEM> Get()
        {
            return _db.DM_DANHMUC_ITEM.AsQueryable().OrderBy(o => o.SAP_XEP);
        }
        public DM_DANHMUC_ITEM GetByID(long ID)
        {
            return Get().FirstOrDefault(c => c.ID == ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetByID(List<long> ID)
        {
            return Get().Where(c => ID.Contains(c.ID));
        }
        public DM_DANHMUC_ITEM GetByMA(string MA, long DANHMUC_ID)
        {
            return Get().FirstOrDefault(c => c.MA.ToLower() == MA.ToLower() && c.DANHMUC_ID == DANHMUC_ID);
        }
        public DM_DANHMUC_ITEM GetByMA(string MA, long DANHMUC_ID, long DVSD_ID)
        {
            return Get().FirstOrDefault(c => c.MA.ToLower() == MA.ToLower() && c.DANHMUC_ID == DANHMUC_ID && c.DVSD_ID == DVSD_ID);
        }
        public DM_DANHMUC_ITEM GetByTEN(string TEN, long DANHMUC_ID)
        {
            return Get().FirstOrDefault(c => c.TEN.ToLower().Trim() == TEN.ToLower().Trim() && c.DANHMUC_ID == DANHMUC_ID);
        }
        public DM_DANHMUC_ITEM GetByTEN(string TEN, long DANHMUC_ID, long DVSD_ID)
        {
            return Get().FirstOrDefault(c => c.TEN.ToLower().Trim() == TEN.ToLower().Trim() && c.DANHMUC_ID == DANHMUC_ID && c.DVSD_ID == DVSD_ID);
        }
        public DM_DANHMUC_ITEM GetByField(string FIELD, string VALUE, long DANHMUC_ID)
        {
            return _db.DM_DANHMUC_ITEM.FromSqlRaw("SELECT * FROM DM_DANHMUC_ITEM as b WHERE b." + FIELD + " LIKE {0} AND b.DANHMUC_ID = {1}", VALUE, DANHMUC_ID).FirstOrDefault();
        }
        public DM_DANHMUC_ITEM GetByField(string FIELD, string VALUE, long DANHMUC_ID, long DVSD_ID)
        {
            return _db.DM_DANHMUC_ITEM.FromSqlRaw("SELECT * FROM DM_DANHMUC_ITEM as b WHERE b." + FIELD + " LIKE {0} AND b.DANHMUC_ID = {1} AND b.DVSD_ID = {2}", VALUE, DANHMUC_ID, DVSD_ID).FirstOrDefault();
        }
        //----
        public IQueryable<DM_DANHMUC_ITEM> GetByDANHMUC_ID(long DANHMUC_ID)
        {
            return Get().Where(c => c.DANHMUC_ID == DANHMUC_ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetByDANHMUC_ID(long DANHMUC_ID, long DVQL_ID)
        {
            return GetByDANHMUC_ID(DANHMUC_ID).Where(c => c.DVQL_ID == DVQL_ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetByDANHMUC_ID(long DANHMUC_ID, long DVQL_ID, long DVSD_ID)
        {
            return GetByDANHMUC_ID(DANHMUC_ID, DVQL_ID).Where(c => c.DVSD_ID == DVSD_ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetByMA_DANHMUC(string MA_DANHMUC)
        {
            DM_DANHMUC objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(MA_DANHMUC);
            return GetByDANHMUC_ID(objDM_DANHMUC.ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetByMA_DANHMUC(string MA_DANHMUC, long DVQL_ID)
        {
            DM_DANHMUC objDM_DANHMUC = _db.DM_DANHMUCCollection.GetByMA(MA_DANHMUC);
            return GetByDANHMUC_ID(objDM_DANHMUC.ID, DVQL_ID);
        }
        public IQueryable<DM_DANHMUC_ITEM> GetTree(long DANHMUC_ID, long ID, List<DM_DANHMUC_ITEM> tree = null, bool addP = true)
        {
            List<DM_DANHMUC_ITEM> child = new List<DM_DANHMUC_ITEM>();
            if (tree == null)
            {
                tree = GetByDANHMUC_ID(DANHMUC_ID).ToList();
            }
            if(addP) child.Add(tree.FirstOrDefault(c => c.ID == ID));
            foreach (var item in tree.Where(c => c.PID == ID))
            {
                var listChild = GetTree(DANHMUC_ID, item.ID, tree).ToList();
                if (listChild.Count > 0) child = child.Union(listChild).ToList();
            }
            return child.AsQueryable();
        }
        public IQueryable<DM_DANHMUC_ITEM> GetTree(long DANHMUC_ID, long ID, long DVQL_ID, List<DM_DANHMUC_ITEM> tree = null, bool addP = true)
        {
            List<DM_DANHMUC_ITEM> child = new List<DM_DANHMUC_ITEM>();
            if (tree == null)
            {
                tree = GetByDANHMUC_ID(DANHMUC_ID, DVQL_ID).ToList();
            }
            if (addP) child.Add(tree.FirstOrDefault(c => c.ID == ID));
            foreach (var item in tree.Where(c => c.PID == ID))
            {
                var listChild = GetTree(DANHMUC_ID, item.ID, DVQL_ID, tree).ToList();
                if (listChild.Count > 0) child = child.Union(listChild).ToList();
            }
            return child.AsQueryable();
        }
        //----
        public void Add(DM_DANHMUC_ITEM item)
        {
            _db.DM_DANHMUC_ITEM.Add(item);
            _db.SaveChanges();
        }

        public void Update(DM_DANHMUC_ITEM item, bool save = true)
        {
            _db.DM_DANHMUC_ITEM.Update(item);
            if (save) _db.SaveChanges();
        }
        public void Remove(DM_DANHMUC_ITEM item)
        {
            if (item == null) return;
            _db.DM_DANHMUC_ITEM.Remove(item);
            _db.SaveChanges();
        }
        public void Remove(IQueryable<DM_DANHMUC_ITEM> item)
        {
            if (item == null) return;
            _db.DM_DANHMUC_ITEM.RemoveRange(item);
            _db.SaveChanges();
        }
        public void RemoveByID(long ID)
        {
            Remove(GetByID(ID));
        }
        public void RemoveByDANHMUC_ID(long DANHMUC_ID)
        {
            Remove(GetByDANHMUC_ID(DANHMUC_ID));
        }
    }
}
