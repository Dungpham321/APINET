using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDB
{
    public class BC_TONGHOPA
    {
        public decimal? C1 { get; set; }
        public decimal? C2 { get; set; }
        public decimal? C3 { get; set; }
        public decimal? C4 { get; set; }
        public decimal? C5 { get; set; }
        public decimal? C6 { get; set; }
        public decimal? C7 { get; set; }
        public decimal? C8 { get; set; }
        public decimal? C9 { get; set; }
        public decimal? C10 { get; set; }
        public decimal? C11 { get; set; }
        public decimal? C12 { get; set; }
        public decimal? C13 { get; set; }
        public decimal? C14 { get; set; }
        public decimal? C15 { get; set; }
        public decimal? C16 { get; set; }
        public decimal? C17 { get; set; }
        public decimal? C18 { get; set; }
        public decimal? C19 { get; set; }
        public decimal? C20 { get; set; }
        public decimal? C21 { get; set; }
        public decimal? C22 { get; set; }
        public decimal? C23 { get; set; }
        public decimal? C24 { get; set; }
        public decimal? C25 { get; set; }
        public decimal? C26 { get; set; }
        public decimal? C27 { get; set; }
        public decimal? C28 { get; set; }
        public decimal? C29 { get; set; }
        public decimal? C30 { get; set; }
        public decimal? C31 { get; set; }
        public decimal? C32 { get; set; }
        public decimal? C33 { get; set; }
        public decimal? C34 { get; set; }
        public decimal? C35 { get; set; }
        public decimal? C36 { get; set; }
        public decimal? C37 { get; set; }
        public decimal? C38 { get; set; }
        public decimal? C39 { get; set; }
        public decimal? C40 { get; set; }
        public decimal? C41 { get; set; }
        public decimal? C42 { get; set; }
        public decimal? C43 { get; set; }
        public decimal? C44 { get; set; }
        public decimal? C45 { get; set; }
        public string? CT1 { get; set; }
        public string? CT2 { get; set; }
        public string? CT3 { get; set; }
        public string? CT4 { get; set; }
        public string? CT5 { get; set; }
        public string? CT6 { get; set; }
        public string? CT7 { get; set; }
        public string? CT8 { get; set; }
        public string? CT9 { get; set; }
        public string? CT10 { get; set; }
        public string? CT11 { get; set; }
        public string? CT12 { get; set; }
        public string? CT13 { get; set; }
        public string? CT14 { get; set; }
        public string? CT15 { get; set; }
        public string? CT16 { get; set; }
        public string? CT17 { get; set; }
        public string? CT18 { get; set; }
        public string? CT19 { get; set; }
        public string? CT20 { get; set; }
        public string? CT21 { get; set; }
        public string? CT22 { get; set; }
        public string? CT23 { get; set; }
        public string? CT24 { get; set; }
        public string? CT25 { get; set; }
        public string? CT26 { get; set; }
        public string? CT27 { get; set; }
        public string? CT28 { get; set; }
        public string? CT29 { get; set; }
        public string? CT30 { get; set; }
        public string? CT31 { get; set; }
        public string? CT32 { get; set; }
        public string? CT33 { get; set; }
        public string? CT34 { get; set; }
        public string? CT35 { get; set; }
        public string? CT36 { get; set; }
        public string? CT37 { get; set; }
        public string? CT38 { get; set; }
        public string? CT39 { get; set; }
        public string? CT40 { get; set; }
        public string? CT41 { get; set; }
        public string? CT42 { get; set; }
        public string? CT43 { get; set; }
        public string? CT44 { get; set; }
        public string? CT45 { get; set; }
    }

    public class BC_TONGHOPCollection : TableCollection
    {
        public BC_TONGHOPCollection(GDBContext db) : base(db) { }

        public Dictionary<string, object> Get(string proc, string pars, string table)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            using (var dbContextTransaction = _db.Database.BeginTransaction())
            {
                try
                {
                    string fPars = "";
                    List<SqlParameter> lPars = new List<SqlParameter>();
                    string[] lsparams = pars.Split(';');
                    if (lsparams.Length > 0)
                    {
                        foreach (string str in lsparams)
                        {
                            string[] giatri = str.Split(':');
                            fPars += (fPars == "" ? "" : ",") + "@" + giatri[0];
                            lPars.Add(new SqlParameter("@" + giatri[0], giatri[1]));
                        }

                    }
                    string[] dsTable = table.Split(',');
                    foreach (string tbl in dsTable)
                    {
                        if (tbl == "BC_TONGHOPA") result.Add("BC_TONGHOPA", _db.BC_TONGHOPA.FromSqlRaw("EXECUTE BAOCAO$" + proc + "_A " + fPars, lPars.ToArray()).ToList());
                        else if (tbl == "BC_TONGHOPB") result.Add("BC_TONGHOPB", _db.BC_TONGHOPB.FromSqlRaw("EXECUTE BAOCAO$" + proc + "_B " + fPars, lPars.ToArray()));
                    }
                    dbContextTransaction.Commit();
                }
                catch(Exception ex)
                {
                    result = null;
                    dbContextTransaction.Rollback();
                }
            }
            return result;
        }

    }
}
