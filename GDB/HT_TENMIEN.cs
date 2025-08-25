using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class HT_TENMIEN
    {
        [Key]
        public long ID { get; set; }
        public string? TEN_MIEN { get; set; }
        public string? DIA_CHI { get; set; }
        public long? DVQL_ID { get; set; }
        public string? TEN_PHAN_MEM { get; set; }

    }
    public class HT_TENMIENCollection : TableCollection
    {
        public HT_TENMIENCollection(GDBContext db) : base(db) { }

        public IQueryable<HT_TENMIEN> Get()
        {
            return _db.HT_TENMIEN.AsQueryable();
        }

        public HT_TENMIEN GetByDIA_CHI(string DIA_CHI)
        {
            return Get().FirstOrDefault(c => c.DIA_CHI == DIA_CHI);
        }
    }
}
