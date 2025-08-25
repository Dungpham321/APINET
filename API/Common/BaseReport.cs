using Azure.Core;
using GCommon;
using GDB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using static OfficeOpenXml.ExcelErrorValue;

namespace API.Common
{
    public class BaseReport
    {
        //Nhãn thiết bị
        public static void NHAN_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var IDS = JsonConvert.DeserializeObject<JArray>(item.GetValue("ID").Value<string>()).ToObject<List<long>>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.GetByIDS(IDS);
                foreach (var objTB_THIETBI in lstTB_THIETBI)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo tổng hợp tình hình mượn, trả thiết bị
        public static void BC01_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            int TRANG_THAI = ConvertClass.ToInt(item["TRANG_THAI"], 0);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            DEN_NGAY = DEN_NGAY.Date.Add(new TimeSpan(23, 59, 59));
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            variables["TRANG_THAI"] = TRANG_THAI;
            variables["TRANG_THAI_TEXT"] = TRANG_THAI == 0 ? "ĐANG MƯỢN" : "ĐÃ TRẢ";
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_MUONTRA = _db.TB_MUONTRACollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).ToList();
                if (TRANG_THAI == 0)
                {
                    lstTB_MUONTRA = lstTB_MUONTRA.Where(c => c.NGAY_MUON.HasValue && TU_NGAY <= c.NGAY_MUON.Value && c.NGAY_MUON.Value <= DEN_NGAY && c.TRANG_THAI == TRANG_THAI).ToList();
                }
                else
                {
                   
                    var lstTB_MUONTRA_ID = lstTB_MUONTRA.Where(e=>e.TRANG_THAI == TRANG_THAI).Select(s => s.ID).ToList();
                    var lstTB_MUONTRACTsss = _db.TB_MUONTRACTCollection.Get().Where(c => lstTB_MUONTRA_ID.Contains(c.MUONTRA_ID.Value)).ToList();
                    var lstTB_MUONTRACT = _db.TB_MUONTRACTCollection.Get().Where(c => lstTB_MUONTRA_ID.Contains(c.MUONTRA_ID.Value) && c.NGAY_TRA.HasValue && TU_NGAY <= c.NGAY_TRA.Value && c.NGAY_TRA.Value <= DEN_NGAY).ToList();
                    lstTB_MUONTRA_ID = lstTB_MUONTRACT.Select(s => s.MUONTRA_ID.Value).ToList();
                    lstTB_MUONTRA = lstTB_MUONTRA.Where(c => lstTB_MUONTRA_ID.Contains(c.ID)).ToList();
                }
                var lstTB_MUONTRA_ID_BC = lstTB_MUONTRA.Select(s => s.ID).ToList();
                var lstTB_MUONTRACT_BC = _db.TB_MUONTRACTCollection.Get().Where(c => lstTB_MUONTRA_ID_BC.Contains(c.MUONTRA_ID.Value)).ToList();
                foreach (var objTB_MUONTRACT in lstTB_MUONTRACT_BC)
                {
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_MUONTRACT.THIETBI_ID.Value);
                    if (objTB_THIETBI == null) continue;
                    var objTB_MUONTRA = lstTB_MUONTRA.FirstOrDefault(c => c.ID == objTB_MUONTRACT.MUONTRA_ID);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT3 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_MUONTRA.CBGIAO_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT4 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_MUONTRA.CBNHAN_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT5 = objTB_MUONTRA.LY_DO;
                    objBC_TONGHOPA.CT6 = objTB_MUONTRA.NGAY_MUON?.ToString("dd/MM/yyyy");           
                    objBC_TONGHOPA.CT7 = objTB_MUONTRA.NGAY_TRA?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT8 = objTB_MUONTRACT.NGAY_TRA?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT9 = objTB_MUONTRACT.TINH_TRANG;
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo tổng hợp tình hình bàn giao tiền mặt
        public static void BC02_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_BANGIAO = _db.TB_BANGIAOCollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.LOAIBG == (int) LoaiBanGiao.TienMat && c.NGAY_BG.HasValue && TU_NGAY <= c.NGAY_BG.Value && c.NGAY_BG.Value <= DEN_NGAY).ToList();
                foreach (var objTB_BANGIAO in lstTB_BANGIAO)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_BANGIAO.SOBG;
                    objBC_TONGHOPA.CT2 = objTB_BANGIAO.NGAY_BG?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT3 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_BANGIAO.CBGIAO_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT4 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_BANGIAO.CBNHAN_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT5 = objTB_BANGIAO.TIEU_DE;
                    objBC_TONGHOPA.C6 = objTB_BANGIAO.SO_TIEN;
                    objBC_TONGHOPA.CT7 = objTB_BANGIAO.LY_DO;
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo danh sách thiết bị đến hạn khấu hao, thanh lý
        public static void BC03_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.HAN_KH.HasValue && TU_NGAY <= c.HAN_KH.Value && c.HAN_KH.Value <= DEN_NGAY).ToList();
                foreach (var objTB_THIETBI in lstTB_THIETBI)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.LOAI_THIET_BI;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.CT5 = objTB_THIETBI.HAN_KH?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT6 = objTB_THIETBI.THONG_SO;
                    objBC_TONGHOPA.CT7 = objTB_THIETBI.PHAN_LOAI.HasValue ? ((PhanLoai)objTB_THIETBI.PHAN_LOAI).GetEnumDescription() : "";
                    objBC_TONGHOPA.CT8 = objTB_THIETBI.TINH_TRANG.HasValue ? ((TinhTrangLuuTru)objTB_THIETBI.TINH_TRANG).GetEnumDescription() : "";
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }
        
        //Báo cáo danh sách thiết bị hết hạn bảo hành
        public static void BC04_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.HAN_BH.HasValue && TU_NGAY <= c.HAN_BH.Value && c.HAN_BH.Value <= DEN_NGAY).ToList();
                foreach (var objTB_THIETBI in lstTB_THIETBI)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.LOAI_THIET_BI;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.CT5 = objTB_THIETBI.HAN_BH?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT6 = objTB_THIETBI.THONG_SO;
                    objBC_TONGHOPA.CT7 = objTB_THIETBI.PHAN_LOAI.HasValue ? ((PhanLoai)objTB_THIETBI.PHAN_LOAI).GetEnumDescription() : "";
                    objBC_TONGHOPA.CT8 = objTB_THIETBI.TINH_TRANG.HasValue ? ((TinhTrangLuuTru)objTB_THIETBI.TINH_TRANG).GetEnumDescription() : "";
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo hiện trạng thiết bị
        public static void BC05_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            int TINH_TRANG = ConvertClass.ToInt(item["TINH_TRANG"], -1);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.NGAY_TAO.HasValue && TU_NGAY <= c.NGAY_TAO.Value && c.NGAY_TAO.Value <= DEN_NGAY).ToList();
                if (TINH_TRANG >= 0) lstTB_THIETBI = lstTB_THIETBI.Where(c => c.TINH_TRANG == TINH_TRANG).ToList();
                foreach (var objTB_THIETBI in lstTB_THIETBI)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.LOAI_THIET_BI;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.CT5 = objTB_THIETBI.HAN_BH?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT6 = objTB_THIETBI.THONG_SO;
                    objBC_TONGHOPA.CT7 = objTB_THIETBI.PHAN_LOAI.HasValue ? ((PhanLoai)objTB_THIETBI.PHAN_LOAI).GetEnumDescription() : "";
                    objBC_TONGHOPA.CT8 = objTB_THIETBI.TINH_TRANG.HasValue ? ((TinhTrangLuuTru)objTB_THIETBI.TINH_TRANG).GetEnumDescription() : "";
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo tổng hợp số lượt mượn trong năm
        public static void BC06_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            int NAM = ConvertClass.ToInt(item["NAM"], 0);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_MUONTRA = _db.TB_MUONTRACollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.NGAY_TAO.HasValue && c.NGAY_TAO.Value.Year == NAM).ToList();
                var lstCANBO = _db.DM_DANHMUC_ITEMCollection.GetByDANHMUC_ID(DsChucNang.CanBo, objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).ToList();
                foreach (var objCANBO in lstCANBO)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objCANBO.TEN;
                    objBC_TONGHOPA.C2 = lstTB_MUONTRA.Where(c => c.CBNHAN_ID == objCANBO.ID).Count();
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo hiện trạng thiết bị
        public static void BC07_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            int NAM = ConvertClass.ToInt(item["NAM"], 0);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.NGAY_MUA.HasValue && c.NGAY_MUA.Value.Year == NAM).ToList();
                foreach (var objTB_THIETBI in lstTB_THIETBI)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.C3 = objTB_THIETBI.VAT;
                    objBC_TONGHOPA.C4 = objTB_THIETBI.GIA_MUA;
                    objBC_TONGHOPA.C5 = objTB_THIETBI.THANH_TIEN;
                    objBC_TONGHOPA.CT6 = objTB_THIETBI.GHI_CHU;
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo chi tiết tình hình mượn, trả thiết bị
        public static void BC08_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            DEN_NGAY = DEN_NGAY.Date.Add(new TimeSpan(23, 59, 59));
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");

            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_MUONTRA = _db.TB_MUONTRACollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.NGAY_MUON.HasValue && TU_NGAY <= c.NGAY_MUON.Value && c.NGAY_MUON.Value <= DEN_NGAY).ToList();
                var lstTB_MUONTRA_ID_BC = lstTB_MUONTRA.Select(s => s.ID).ToList();
                var lstTB_MUONTRACT_BC = _db.TB_MUONTRACTCollection.Get().Where(c => lstTB_MUONTRA_ID_BC.Contains(c.MUONTRA_ID.Value)).ToList();
                foreach (var objTB_MUONTRACT in lstTB_MUONTRACT_BC)
                {
                    if (objTB_MUONTRACT.THIETBI_ID == null) continue;
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_MUONTRACT.THIETBI_ID.Value);
                    if (objTB_THIETBI == null) continue;
                    var objTB_MUONTRA = lstTB_MUONTRA.FirstOrDefault(c => c.ID == objTB_MUONTRACT.MUONTRA_ID);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT3 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_MUONTRA.CBGIAO_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT4 = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_MUONTRA.CBNHAN_ID.Value)?.TEN;
                    objBC_TONGHOPA.CT5 = objTB_MUONTRA.LY_DO;
                    objBC_TONGHOPA.CT6 = objTB_MUONTRA.NGAY_MUON?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT7 = objTB_MUONTRA.NGAY_TRA?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT8 = objTB_MUONTRACT.NGAY_TRA?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT9 = objTB_MUONTRACT.TINH_TRANG;
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }
        //Danh sách đăng ký mã key
        public static void BC09_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_KEYPM = _db.TB_KEYPMCollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).ToList();
                var lstTB_KEYPM_BC = lstTB_KEYPM.Select(s => s.ID).ToList();
                var lstTB_KEYPMCT_BC = _db.TB_KEYPMCTCollection.Get().Where(c => lstTB_KEYPM_BC.Contains(c.KEYPM_ID.Value)).ToList();
                foreach (var objTB_KEYPMCT in lstTB_KEYPMCT_BC)
                {
                    var objCANBO = _db.DM_DANHMUC_ITEMCollection.GetByID(objTB_KEYPMCT.CANBO_ID.Value);
                    if (objCANBO == null) continue;
                    var objTB_KEYPM = lstTB_KEYPM.FirstOrDefault(c => c.ID == objTB_KEYPMCT.KEYPM_ID);
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_KEYPMCT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_KEYPM.MA;
                    objBC_TONGHOPA.CT2 = objCANBO.TEN;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI?.MA + " - " + objTB_THIETBI?.TEN;
                    objBC_TONGHOPA.CT4 = objTB_KEYPMCT.NGAY_NHAP?.ToString("dd/MM/yyyy");
                    objBC_TONGHOPA.CT5 = objTB_KEYPMCT.DA_NHAP.HasValue && objTB_KEYPMCT.DA_NHAP.Value ? "x" : "";
                    objBC_TONGHOPA.CT6 = objTB_KEYPMCT.DOI_MAY;
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Báo cáo danh sách thiết bị đã thanh lý
        public static void BC10_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            DateTime TU_NGAY = ConvertClass.ToDateTime(item["TU_NGAY"], DateTime.Now);
            DateTime DEN_NGAY = ConvertClass.ToDateTime(item["DEN_NGAY"], DateTime.Now);
            variables["TU_NGAY"] = TU_NGAY.ToString("dd/MM/yyyy");
            variables["DEN_NGAY"] = DEN_NGAY.ToString("dd/MM/yyyy");
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_BANGIAO = _db.TB_BANGIAOCollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => c.NGAY_BG.HasValue && TU_NGAY <= c.NGAY_BG.Value && c.NGAY_BG.Value <= DEN_NGAY).ToList();
                var lstTB_BANGIAO_ID = lstTB_BANGIAO.Select(s => s.ID).ToList();
                var lst_TB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(lstTB_BANGIAO_ID).ToList();
                var lst_TB_BANGIAOCT_TBID = lst_TB_BANGIAOCT.Select(s => s.THIETBI_ID).ToList();
                var lstTB_THIETBI = _db.TB_THIETBICollection.Get(objDM_DONVI.DVQL_ID.Value, objDM_DONVI.ID).Where(c => lst_TB_BANGIAOCT_TBID.Contains(c.ID)).ToList();
                foreach (var objTB_BANGIAO in lstTB_BANGIAO)
                {
                    var sub_TB_BANGIAOCT = lst_TB_BANGIAOCT.Where(c => c.BANGIAO_ID == objTB_BANGIAO.ID);
                    var subTB_THIETBI = lstTB_THIETBI.Where(c => sub_TB_BANGIAOCT.Select(c => c.THIETBI_ID).Contains(c.ID));
                    foreach (var objTB_THIETBI in subTB_THIETBI)
                    {
                        BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                        objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                        objBC_TONGHOPA.CT2 = objTB_THIETBI.LOAI_THIET_BI;
                        objBC_TONGHOPA.CT3 = objTB_THIETBI.TEN;
                        objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                        objBC_TONGHOPA.CT5 = objTB_BANGIAO.NGAY_BG?.ToString("dd/MM/yyyy");
                        objBC_TONGHOPA.CT6 = objTB_THIETBI.THONG_SO;
                        objBC_TONGHOPA.CT7 = objTB_THIETBI.PHAN_LOAI.HasValue ? ((PhanLoai)objTB_THIETBI.PHAN_LOAI).GetEnumDescription() : "";
                        objBC_TONGHOPA.CT8 = objTB_THIETBI.TINH_TRANG.HasValue ? ((TinhTrangLuuTru)objTB_THIETBI.TINH_TRANG).GetEnumDescription() : "";
                        lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                    }
                }

                //foreach (var objTB_THIETBI in lstTB_THIETBI)
                //{
                //    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                //    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                //    objBC_TONGHOPA.CT2 = objTB_THIETBI.LOAI_THIET_BI;
                //    objBC_TONGHOPA.CT3 = objTB_THIETBI.TEN;
                //    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                //    objBC_TONGHOPA.CT5 = objTB_THIETBI.HAN_KH?.ToString("dd/MM/yyyy");
                //    objBC_TONGHOPA.CT6 = objTB_THIETBI.THONG_SO;
                //    objBC_TONGHOPA.CT7 = objTB_THIETBI.PHAN_LOAI.HasValue ? ((PhanLoai)objTB_THIETBI.PHAN_LOAI).GetEnumDescription() : "";
                //    objBC_TONGHOPA.CT8 = objTB_THIETBI.TINH_TRANG.HasValue ? ((TinhTrangLuuTru)objTB_THIETBI.TINH_TRANG).GetEnumDescription() : "";
                //    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                //}

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Biên bản mượn thiết bị
        public static void BBBANGIAO_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_MUONTRA = _db.TB_MUONTRACollection.GetByID(ID);
            ProcessMuonTra(variables, objTB_MUONTRA, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_MUONTRACT = _db.TB_MUONTRACTCollection.GetByMUONTRA_ID(objTB_MUONTRA.ID).ToList();
                foreach (var objTB_MUONTRACT in lstTB_MUONTRACT)
                {
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_MUONTRACT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.C3 = 1;
                    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.CT5 = objTB_MUONTRACT.TINH_TRANG;
                    objBC_TONGHOPA.CT6 = objTB_MUONTRACT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }
        
        //Phiếu mượn thiết bị
        public static void PHIEUMUON_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_MUONTRA = _db.TB_MUONTRACollection.GetByID(ID);
            ProcessMuonTra(variables, objTB_MUONTRA, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_MUONTRACT = _db.TB_MUONTRACTCollection.GetByMUONTRA_ID(objTB_MUONTRA.ID).ToList();
                foreach (var objTB_MUONTRACT in lstTB_MUONTRACT)
                {
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_MUONTRACT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI.SERI;
                    objBC_TONGHOPA.CT4 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.C5 = 1;
                    objBC_TONGHOPA.CT6 = objTB_MUONTRACT.TINH_TRANG;
                    objBC_TONGHOPA.CT7 = "x";
                    objBC_TONGHOPA.CT8 = objTB_MUONTRACT.DA_TRA.HasValue && objTB_MUONTRACT.DA_TRA.Value ? "x" : "";
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        private static void ProcessMuonTra(Dictionary<string, object> variables, TB_MUONTRA objTB_MUONTRA, GDBContext _db)
        {
            foreach (var p in objTB_MUONTRA.GetType().GetProperties())
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t == typeof(DateTime))
                {
                    var gtd = objTB_MUONTRA.GetValueByName<DateTime>(p.Name);
                    if (gtd != DateTime.MinValue) variables[p.Name] = gtd.ToString("dd/MM/yyyy");
                    else variables[p.Name] = "";
                }
                else
                {
                    variables[p.Name] = objTB_MUONTRA.GetValueByName(p.Name);
                }
            }
            variables["CBGIAO_TEN"] = _db.DM_DANHMUC_ITEMCollection.GetByID((long)variables["CBGIAO_ID"])?.TEN;
            variables["CBNHAN_TEN"] = _db.DM_DANHMUC_ITEMCollection.GetByID((long)variables["CBNHAN_ID"])?.TEN;
        }

        //Biên bản bàn giao tiền mặt
        public static void BBBANGIAOTM_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(ID);
            ProcessBanGiao(variables, objTB_BANGIAO, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(objTB_BANGIAO.ID).ToList();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                foreach (var objTB_BANGIAOCT in lstTB_BANGIAOCT)
                {
                    //var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_BANGIAOCT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_BANGIAOCT.MENH_GIA?.ToString("0,0", elGR);
                    objBC_TONGHOPA.C2 = objTB_BANGIAOCT.SO_LUONG;
                    objBC_TONGHOPA.CT3 = objTB_BANGIAOCT.DVT;
                    objBC_TONGHOPA.C4 = objTB_BANGIAOCT.THANH_TIEN;
                    objBC_TONGHOPA.CT5 = objTB_BANGIAOCT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        public static void BBBANGIAOTB_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(ID);
            ProcessBanGiao(variables, objTB_BANGIAO, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(objTB_BANGIAO.ID).ToList();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                foreach (var objTB_BANGIAOCT in lstTB_BANGIAOCT)
                {
                    if (!objTB_BANGIAOCT.THIETBI_ID.HasValue) continue;
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_BANGIAOCT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.C3 = objTB_BANGIAOCT.SO_LUONG;
                    objBC_TONGHOPA.CT4 = objTB_BANGIAOCT.DVT;
                    objBC_TONGHOPA.CT5 = objTB_BANGIAOCT.TINH_TRANG;
                    objBC_TONGHOPA.CT6 = objTB_BANGIAOCT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //BBTHANHLY - Biên bản thanh lý thiết bị
        public static void BBTHANHLY_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_BANGIAO = _db.TB_BANGIAOCollection.GetByID(ID);
            ProcessBanGiao(variables, objTB_BANGIAO, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_BANGIAOCT = _db.TB_BANGIAOCTCollection.GetByBANGIAO_ID(objTB_BANGIAO.ID).ToList();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                foreach (var objTB_BANGIAOCT in lstTB_BANGIAOCT)
                {
                    if (!objTB_BANGIAOCT.THIETBI_ID.HasValue) continue;
                    var objTB_THIETBI = _db.TB_THIETBICollection.GetByID(objTB_BANGIAOCT.THIETBI_ID.Value);
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.C3 = objTB_BANGIAOCT.SO_LUONG;
                    objBC_TONGHOPA.CT4 = objTB_BANGIAOCT.DVT;
                    objBC_TONGHOPA.CT5 = objTB_BANGIAOCT.TINH_TRANG;
                    objBC_TONGHOPA.CT6 = objTB_BANGIAOCT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            List<string> strHoiDong = new List<string>();
            var lstTB_BANGIAOHD = _db.TB_BANGIAOHDCollection.GetByBANGIAO_ID(objTB_BANGIAO.ID).ToList();
            foreach (var objTB_BANGIAOHD in lstTB_BANGIAOHD)
            {
                string hd = "- Ông/bà: " + objTB_BANGIAOHD.HO_TEN + (objTB_BANGIAOHD.VAI_TRO + "" != "" ? "&emsp;&emsp; Vai trò: " + objTB_BANGIAOHD.VAI_TRO : "");
                strHoiDong.Add(hd);
            }
            variables["HOIDONG"] = String.Join("<br />", strHoiDong);
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        private static void ProcessBanGiao(Dictionary<string, object> variables, TB_BANGIAO objTB_BANGIAO, GDBContext _db)
        {
            foreach (var p in objTB_BANGIAO.GetType().GetProperties())
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t == typeof(DateTime))
                {
                    var gtd = objTB_BANGIAO.GetValueByName<DateTime>(p.Name);
                    if (gtd != DateTime.MinValue) variables[p.Name] = gtd.ToString("dd/MM/yyyy");
                    else variables[p.Name] = "";
                }
                else
                {
                    variables[p.Name] = objTB_BANGIAO.GetValueByName(p.Name);
                }
            }
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            if (variables["SO_TIEN"] + "" != "")
            {
                variables["SO_TIEN_CHU"] = ((decimal)variables["SO_TIEN"]).ChuyenSo();
                variables["SO_TIEN"] = ((decimal)variables["SO_TIEN"]).ToString("0,0", elGR);
                
            }
            if(variables["CBGIAO_ID"]+ "" != "") variables["CBGIAO_TEN"] = _db.DM_DANHMUC_ITEMCollection.GetByID((long)variables["CBGIAO_ID"])?.TEN;
            if (variables["CBNHAN_ID"] + "" != "") variables["CBNHAN_TEN"] = _db.DM_DANHMUC_ITEMCollection.GetByID((long)variables["CBNHAN_ID"])?.TEN;
        }

        //Phiếu đề xuất mua thiết bị
        public static void DEXUAT_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_DEXUAT = _db.TB_DEXUATCollection.GetByID(ID);
            ProcessTB_DEXUAT(variables, objTB_DEXUAT, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_DEXUATCT = _db.TB_DEXUATCTCollection.GetByDEXUAT_ID(objTB_DEXUAT.ID).ToList();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                foreach (var objTB_DEXUATCT in lstTB_DEXUATCT)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = "";
                    objBC_TONGHOPA.CT2 = objTB_DEXUATCT.TEN;
                    objBC_TONGHOPA.C3 = objTB_DEXUATCT.GIA_TIEN;
                    objBC_TONGHOPA.C4 = objTB_DEXUATCT.SO_LUONG;
                    objBC_TONGHOPA.C5 = objTB_DEXUATCT.THANH_TIEN;
                    objBC_TONGHOPA.CT6 = objTB_DEXUATCT.NGUOI_DUNG;
                    objBC_TONGHOPA.CT7 = objTB_DEXUATCT.BAN_SU_DUNG;
                    objBC_TONGHOPA.CT8 = objTB_DEXUATCT.LY_DO;
                    objBC_TONGHOPA.CT9 = objTB_DEXUATCT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Giấy đề nghị tạm ứng
        public static void TAMUNG_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_DEXUAT = _db.TB_DEXUATCollection.GetByID(ID);
            ProcessTB_DEXUAT(variables, objTB_DEXUAT, _db);
            ProcessVariable(variables, objDM_DONVI, _db);
        }
        //Giấy đề nghị thanh toán
        public static void THANHTOAN_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_DEXUAT = _db.TB_DEXUATCollection.GetByID(ID);
            ProcessTB_DEXUAT(variables, objTB_DEXUAT, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                objBC_TONGHOPA.CT1 = objTB_DEXUAT.TIEU_DE;
                objBC_TONGHOPA.C2 = objTB_DEXUAT.TONG_TIEN;
                //---
                lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        //Giấy thanh toán tiền tạm ứng
        public static void THANHTOANTU_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_DEXUAT = _db.TB_DEXUATCollection.GetByID(ID);
            ProcessTB_DEXUAT(variables, objTB_DEXUAT, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_DEXUATCT = _db.TB_DEXUATCTCollection.GetByDEXUAT_ID(objTB_DEXUAT.ID).ToList();
                CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
                foreach (var objTB_DEXUATCT in lstTB_DEXUATCT)
                {
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_DEXUATCT.TEN;
                    objBC_TONGHOPA.C2 = objTB_DEXUATCT.THANH_TIEN;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        private static void ProcessTB_DEXUAT(Dictionary<string, object> variables, TB_DEXUAT objTB_DEXUAT, GDBContext _db)
        {
            foreach (var p in objTB_DEXUAT.GetType().GetProperties())
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t == typeof(DateTime))
                {
                    var gtd = objTB_DEXUAT.GetValueByName<DateTime>(p.Name);
                    if (gtd != DateTime.MinValue) variables[p.Name] = gtd.ToString("dd/MM/yyyy");
                    else variables[p.Name] = "";
                }
                else
                {
                    variables[p.Name] = objTB_DEXUAT.GetValueByName(p.Name);
                }
            }
            CultureInfo elGR = CultureInfo.CreateSpecificCulture("el-GR");
            if (variables["TONG_TIEN"] + "" != "")
            {
                variables["TONG_TIEN_CHU"] = ((decimal)variables["TONG_TIEN"]).ChuyenSo();
                variables["TONG_TIEN"] = ((decimal)variables["TONG_TIEN"]).ToString("0,0", elGR);
              
            }
            if (variables["CANBO_ID"] + "" != "")
            {
                var cb = _db.DM_DANHMUC_ITEMCollection.GetByID((long)variables["CANBO_ID"]);
                variables["CANBO"] = cb?.TEN;
                variables["CCCD"] = cb?.CT4;
                variables["CHUCVU"] = cb != null && cb.CC1.HasValue ? _db.DM_DANHMUC_ITEMCollection.GetByID((long)cb.CC1.Value)?.TEN : "";
            }
        }

        //Biên bản kiểm kê
        public static void KIEMKE_Report_Alter(JObject item, Dictionary<string, object> variables, Dictionary<string, object> datasource, GDBContext _db)
        {
            long DVSD_ID = ConvertClass.ToLong(item["DONVI"], 0);
            var objDM_DONVI = _db.DM_DANHMUC_ITEMCollection.GetByID(DVSD_ID);
            long ID = ConvertClass.ToLong(item["ID"], 0);
            var objTB_KIEMKE = _db.TB_KIEMKECollection.GetByID(ID);
            ProcessTB_KIEMKE(variables, objTB_KIEMKE, _db);
            Dictionary<string, object> lstDataSource = (Dictionary<string, object>)datasource["Datasource"];
            if (!lstDataSource.ContainsKey("dsBC_TONGHOPA"))
            {
                List<BC_TONGHOPA> lstBC_TONGHOPA = new List<BC_TONGHOPA>();
                var lstTB_KIEMKECT = _db.TB_KIEMKECTCollection.GetByKIEMKE_ID(objTB_KIEMKE.ID).ToList();
                var lstTB_ID = lstTB_KIEMKECT.Select(s => s.THIETBI_ID.Value).ToList();
                var lstTB_THIETBI = _db.TB_THIETBICollection.GetByIDS(lstTB_ID).ToList();
                foreach (var objTB_KIEMKECT in lstTB_KIEMKECT)
                {
                    var objTB_THIETBI = lstTB_THIETBI.FirstOrDefault(c => c.ID == objTB_KIEMKECT.THIETBI_ID.Value);
                    if (objTB_THIETBI == null) continue;
                    BC_TONGHOPA objBC_TONGHOPA = new BC_TONGHOPA();
                    objBC_TONGHOPA.CT1 = objTB_THIETBI.MA;
                    objBC_TONGHOPA.CT2 = objTB_THIETBI.TEN;
                    objBC_TONGHOPA.CT3 = objTB_THIETBI.LOAI_THIET_BI;
                    objBC_TONGHOPA.CT5 = objTB_THIETBI.DVT;
                    objBC_TONGHOPA.C6 = objTB_KIEMKECT.SO_LUONG;
                    objBC_TONGHOPA.C7 = objTB_KIEMKECT.TRUOC_KK_HONG;
                    objBC_TONGHOPA.C8 = objTB_KIEMKECT.TRUOC_KK_MAT;
                    objBC_TONGHOPA.C9 = objTB_KIEMKECT.SAU_KK_HONG;
                    objBC_TONGHOPA.C10 = objTB_KIEMKECT.SAU_KK_MAT;
                    objBC_TONGHOPA.C11 = objTB_KIEMKECT.CON_DUNG;
                    objBC_TONGHOPA.C12 = objTB_KIEMKECT.THUA;
                    objBC_TONGHOPA.CT13 = objTB_KIEMKECT.GHI_CHU;
                    //---
                    lstBC_TONGHOPA.Add(objBC_TONGHOPA);
                }

                lstDataSource.Add("dsBC_TONGHOPA", lstBC_TONGHOPA);
            }
            ProcessVariable(variables, objDM_DONVI, _db);
        }

        private static void ProcessTB_KIEMKE(Dictionary<string, object> variables, TB_KIEMKE objTB_KIEMKE, GDBContext _db)
        {
            foreach (var p in objTB_KIEMKE.GetType().GetProperties())
            {
                var t = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                if (t == typeof(DateTime))
                {
                    var gtd = objTB_KIEMKE.GetValueByName<DateTime>(p.Name);
                    if (gtd != DateTime.MinValue) variables[p.Name] = gtd.ToString("dd/MM/yyyy");
                    else variables[p.Name] = "";
                }
                else
                {
                    variables[p.Name] = objTB_KIEMKE.GetValueByName(p.Name);
                }
            }
            if (variables["KHOKK"] + "" != "")
            {
                var lstKHOKK = (variables["KHOKK"] + "").Split(";").Select(Int64.Parse).ToList();
                var lstKHO = _db.DM_DANHMUC_ITEMCollection.GetByID(lstKHOKK);
                variables["KHOKK"] = String.Join(", ", lstKHO.Select(s => s.TEN));
            }
        }

        //---------------------------
        protected static void ProcessVariable(Dictionary<string, object> variables, DM_DANHMUC_ITEM objDM_DONVI, GDBContext _db)
        {
            if (objDM_DONVI == null) return;
            DM_DANHMUC_ITEM objDM_DONVI_TREN = _db.DM_DANHMUC_ITEMCollection.GetByID(objDM_DONVI.PID.HasValue ? objDM_DONVI.PID.Value : 0);
            string tendv = HT_CAUHINHDV_Get<string>("DONVI", "", objDM_DONVI.ID, _db);
            string tendvtren = HT_CAUHINHDV_Get<string>("COQUAN", "", objDM_DONVI.ID, _db);
            variables["DIADANH"] = HT_CAUHINHDV_Get<string>("DIADANH", "", objDM_DONVI.ID, _db);
            variables["DON_VI"] = tendv != "" ? tendv : objDM_DONVI?.TEN;
            variables["DON_VI_CHA"] = tendvtren != "" ? tendvtren : objDM_DONVI_TREN?.TEN;
            string ngthang = DateTime.Now.ToString("dd") + " tháng " + DateTime.Now.ToString("MM") + " năm " + DateTime.Now.ToString("yyyy");
            variables["DD_NGAY_THANG"] = variables["DIADANH"] + ", ngày " + ngthang;
            variables["NGAY_THANG"] = "Ngày " + ngthang;
            variables["THUTRUONG"] = HT_CAUHINHDV_Get<string>("THUTRUONG", "", objDM_DONVI.ID, _db);
            variables["THUTRUONGCV"] = HT_CAUHINHDV_Get<string>("THUTRUONGCV", "", objDM_DONVI.ID, _db);
            variables["NGUOILAP"] = HT_CAUHINHDV_Get<string>("NGUOILAP", "", objDM_DONVI.ID, _db);
        }
        protected static T HT_CAUHINH_Get<T>(string name, object defaultValue, long dvql_id, GDBContext _db)
        {
            JObject data = _db.HT_VARIABLECollection.VariableGet<JObject>("SystemConfig", "HT_CAUHINH_" + dvql_id, new JObject());
            return data[name] == null ? (defaultValue == null ? default(T) : (T)defaultValue) : data[name].Value<T>();
        }

        protected static T HT_CAUHINHDV_Get<T>(string name, object defaultValue, long dvsd_id, GDBContext _db)
        {
            JObject data = _db.HT_VARIABLECollection.VariableGet<JObject>("DvConfig", "HT_CAUHINHDV_" + dvsd_id, new JObject());
            return data[name] == null ? (defaultValue == null ? default(T) : (T)defaultValue) : data[name].Value<T>();
        }
    }
}
