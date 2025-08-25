using API.Common;
using Azure.Core;
using GCommon;
using GDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace API.Controllers.DanhMuc
{
    [Consumes("application/json")]
    [Route("api/DanhMuc/DM_HETHONG")]
    [ApiController]
    public class DM_HETHONGController : BaseController
    {
        public DM_HETHONGController(GDBContext db) : base(db) {}
        [HttpGet("{op}")]
        [Authorize("Bearer")]
        public IActionResult Get(string op)
        {
            long PHANLOAI = ConvertClass.ToLong(Request.Query["PHAN_LOAI"] + "", 0);
            if (op == "HT_NGUOIDUNG")
            {
                if(PHANLOAI == 1) return ListAll(GetDsHT_NGUOIDUNG());
                else if (PHANLOAI == 2) return ListAll(GetDsHT_NGUOIDUNG(false));
                return ListAll(_db.HT_NGUOIDUNGCollection.Get());
            }
            else if (op == "HT_NGUOIDUNGByKey")
            {
                return ObjectResult(_db.HT_NGUOIDUNGCollection.GetByID(ConvertClass.ToLong(Request.Query["key"] + "", 0)));
            }
            else if (op == "Quyen")
            {
                var HtLogTypes = Enum.GetNames(typeof(Quyen)).Select(e => new { Key = e, Name = ConvertClass.GetEnumDescription((Quyen)GetEnumValue(typeof(Quyen), e)) }).ToList();
                return ListAll(HtLogTypes.AsQueryable());
            }
            else if (op == "NHOMQUYEN")
            {
                if (IsQuanTriHeThong)
                {
                    return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.NhomQuyen, DVQL_ID));
                }
                else if (IsQuanTriDonVi)
                {
                    return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.NhomQuyen, DVQL_ID, DVSD_ID));
                }
            }
            else if (op == "DONVI")
            {
                if (PHANLOAI == 1) return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.DonVi, DVQL_ID));
                return ListAll(GetDsDM_DONVI().ToList().AsQueryable());
            }
            else if (op == "DONVIByKey")
            {
                return ObjectResult(_db.DM_DANHMUC_ITEMCollection.GetByID(ConvertClass.ToLong(Request.Query["key"] + "", 0)));
            }
            else if(op == "DANHMUC_ITEM")
            {
                long DANHMUC_ID = ConvertClass.ToLong(Request.Query["DANHMUC_ID"] + "", 0);
                return ListAll(_db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DANHMUC_ID));

            }
            return new BadRequestResult();
        }
    }
}
