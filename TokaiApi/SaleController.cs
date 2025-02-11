using Microsoft.Graph.Models;
using Microsoft.Graph.Models.TermStore;
using Microsoft.Kiota.Abstractions;
using Mysqlx.Crud;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using static ERP.Controllers.PurchaseController;
using static ERP.Controllers.SaleController;

namespace ERP.Controllers
{
    public class SaleController : Controller
    {
        private string vt_pkey;
        string _BillPkey = "";
        private string acCode;
        private string BILL_PKEY;

        //private readonly string connectionString = "Data Source=DESKTOP-B3QR5MS;Initial Catalog=TOKAIN2425;Persist Security Info=True;User ID=sa;Password=sa;TrustServerCertificate=True;";
       // private readonly string connectionString = "Data Source=103.89.45.71;Initial Catalog=YHPLPB2324;Persist Security Info=True;User ID=SofgenApi;Password=SofgenApi@123;TrustServerCertificate=True;";

        public string VTBILL_PKEY { get; private set; }
        public string requestBody { get; private set; }

        [HttpPost]
        public async Task<ContentResult> GetInvoice()
        {
            DataTable dtinvoiceS = new DataTable();
            string sql = @"SELECT (SELECT TOP 1 DOCU_TYPE  FROM FVTMID WITH(NOLOCK) WHERE VT_PKEY = T.BILL_PKEY ORDER BY DOCU_TYPE ASC) AS DOCU_TYPE,T.BILL_PKEY, T.TYPE, T.VCH_SER, T.VCH_NO, T.VCH_DATE, T.PREP_TIME, 
        T.REM_DATE, T.REM_TIME, T.TAX_CODE, T.AC_CODE, T.AC_NAME, T.ADDRESS, T.CITY, T.CONS_NAME, T.CONS_ADDR, T.DESTINATION, T.CARRIER_NAME, T.VEH_NO, T.RRGR_NO, T.RRGR_DATE, T.DOCU_THRU, T.TQNTY, T.TCASES, 
        T.TWT, T.TBASIC, T.TASS_AMT, T.TBED_AMT, T.TAED_AMT, T.TTAXABLE_AMT, T.TRND_AMT, T.TBILL_AMT, T.SERCONTROL, T.SPL_INS, T.PVT_MARKS, T.SALE_TYPE, T.NARRATION, T.CR_DAYS, T.AMT1, T.AMT2, T.AMT3, T.AMT4, 
        T.AMT5, T.AMT6, T.AMT7, T.AMT8, T.AMT9, T.AMT10, T.AMT11, T.AMT12, T.AMT13, T.AMT14, T.AMT15, T.AMT16, T.PER1, T.PER2, T.PER3, T.PER4, T.PER5, T.PER6, T.PER7, T.PER8, T.PER9, T.PER10, T.PER11, T.PER12, 
        T.PER13, T.PER14, T.PER15, T.PER16, T.ADESC1, T.ADESC2, T.ADESC3, T.ADESC4, T.ADESC5, T.ADESC6, T.ADESC7, T.ADESC8, T.ADESC9, T.ADESC10, T.ADESC11, T.ADESC12, T.ADESC13, T.ADESC14, T.ADESC15, T.ADESC16, 
        T.CASH_CREDIT, T.TPKG_AMT, T.TFRT_AMT, T.TINS_AMT, T.CPO_NO, T.CPO_DATE, T.CONS_CITY, T.CONS_COUNTRY, T.CONS_PIN, T.CONS_STATE, T.COUNTRY, T.PIN, T.STATE, T.CONS_CODE, T.DRIVER_NAME, T.CLRNC_TYPE, T.VCODE, 
        T.TPT_MODE, T.EXCH_RATE, T.EINV_TYPE, T.TTCS_AMT, T.OBILL_PKEY, T.TPACKING, T.CATEGORY, T.SE_CODE, T.PLANT, T.CANCELLED_YN, T.TAED_AMT2, T.DAC_CODE, T.TTOOLING_AMT, T.TFOC_AMT, T.FRT_TYPE, 
        T.CFLAG, T.TNET_WT, T.DESIG_BILL_TO, T.FAC_CODE, T.BANK_CODE, T.PRE_C_BY, T.POR_BY_PC, T.PO_DELIVERY, T.NOTIFY, T.POL, T.POD, T.COFD, T.TERMS, T.USRNAME, T.TSOP_GALF, T.GS_FLAG, T.LCO_FLAG, 
        T.GSTIN_NO, T.CONS_GSTIN_NO, T.GST_CLRNC_TYPE, T.GST_REGN_TYPE, T.SER_PREFIX, T.DISTANCE, T.TTCS_RATE, T.NTCS_AMT, T.TCS_BASIC_AMT, T.EB_YN, T.CHANGE_POS, T.PROJ_CODE, 
        E.KIND_ATTN, E.EINR_AMT, E.EASS_AMT, E.FRTSGST_RATE, E.FRTSGST_AMT, E.FRTCGST_RATE, E.FRTCGST_AMT, E.FRTIGST_RATE, E.FRTIGST_AMT, E.PKGSGST_RATE, E.PKGSGST_AMT, E.PKGCGST_RATE, E.PKGCGST_AMT, 
        E.PKGIGST_RATE, E.PKGIGST_AMT, E.INSSGST_RATE, E.INSSGST_AMT, E.INSCGST_RATE, E.INSCGST_AMT, E.INSIGST_RATE, E.INSIGST_AMT, E.RCSGST_AMT, E.RCCGST_AMT, E.RCIGST_AMT, E.EB_NO, E.EB_DATE, 
        E.EB_TIME, E.FPIGSTSLAB_CODE, E.SHIPPINGBILL_NO, E.SHIPPINGBILL_DATE, E.PORT_CODE, E.WBGR_WT, E.WBSLIP_NO, E.OVER_WEIGHT_REASON, E.RFID_SEAL_NO, E.BC417_IMAGE, E.IRN, E.ACK_NO, E.ACK_DATE, 
        E.ACK_TIME, E.CANCEL_DATE, E.CANCEL_TIME, E.EVU_TIME, E.EVU_DATE, E.QR_CODE, E.EBCANCEL_DATE, E.EBCANCEL_TIME, E.EIFPIGSTSLAB_CODE, E.EIFPICOM_CODE, E.BC417_IMAGE_EINVOICE, E.TASN_NO, E.EB_VALID_TILL_DATE, 
        E.EB_VALID_TILL_TIME, E.FRTCESS_RATE, E.FRTCESS_AMT, E.PKGCESS_RATE, E.PKGCESS_AMT, E.INSCESS_RATE, E.INSCESS_AMT FROM TBILLTOP T WITH(NOLOCK) JOIN TBILLTOPE E WITH(NOLOCK) ON 
        T.BILL_PKEY = E.BILL_PKEY WHERE  (T.TALLY_ID IS NULL OR T.TALLY_ID = '') AND T.TALLY_SYNC_YN = 'Y';";
            dtinvoiceS = GetData(sql);
            data d = new data();
            d.DOC_DETAIL = new List<DOC_DETAIL>();
            sql = @"SELECT M.BILL_PKEY, M.ENT_NO, M.ITEM_CODE, M.ITEM_NAME, M.QNTY, M.RATE, M.DIS_RATE1, M.DIS_AMT1, M.DIS_RATE2, M.DIS_AMT2, M.AMOUNT, M.ASS_AMT, M.BED_RATE, M.AED_RATE, M.BED_AMT, M.AED_AMT, M.TAXABLE_AMT, M.BILL_AMT, 
                M.MILL_COST, M.REMARKS, M.SUNIT, M.SQNTY, M.UNIT, M.NOD, M.PKG_AMT, M.FRT_AMT, M.INS_AMT, M.DI_NO, M.DI_DATE, M.DI_TIME, M.DN_NO, M.ITEM_DESC, M.BATCH_NO, M.CPO_NO, M.CPO_DATE, M.CAO_NO, M.CAO_DATE, 
                M.TAR_NO, M.COM_CODE, M.CPROD_CODE, M.CO_FLAG, M.OSC_FLAG, M.PKG_QNTY1, M.BOXES1, M.PKG_QNTY2, M.BOXES2, M.GR_WT, M.NET_WT, M.UNL_LOC, M.USG_LOC, M.SO_PKEY, M.SRV_NO, M.SRV_DATE, M.HUNDI_NO, 
                M.HUNDI_DATE, M.TCS_RATE, M.TCS_AMT, M.MODEL, M.RATE_BASIS, M.WEIGHT, M.ITEM_NO, M.RQNTY, M.AQNTY, M.SOA_NO, M.IPKG_TYPE, M.IPKG_QNTY1, M.IBOXES1, M.IPKG_QNTY2, M.IBOXES2, M.MRP_RATE, 
                M.AED_RATE2, M.AED_AMT2, M.TOOLING_RATE, M.TOOLING_AMT, M.FOC_RATE, M.FOC_AMT, M.ABED_RATE, M.ABED_AMT, M.OPKG_TYPE, M.SSO_PKEY, M.BI_RATE, M.CASENO_FROM, M.CASENO_TO, M.WQ_FLAG, M.SSA_PKEY, 
                M.DIS_RATE3, M.DIS_AMT3, M.PALLET_NO, M.GSTSLAB_CODE, M.SERVICE_CODE, M.SERVICE_NAME, M.SCOM_CODE, M.STAR_NO, M.FRTSGST_AMT, M.FRTCGST_AMT, M.FRTIGST_AMT, M.PKGSGST_AMT, M.PKGCGST_AMT, M.PKGIGST_AMT, 
                M.INSSGST_AMT, M.INSCGST_AMT, M.INSIGST_AMT, M.MISC_AMT, M.OBILL_PKEY, M.SSCH_PKEY, M.MCPROD_NAME, M.CASE_NO, M.PROJ_CODE, M.SLOC_CODE, M.BATCH_DATE
            FROM TBILLMID M WITH(NOLOCK) INNER JOIN TBILLTOP T WITH(NOLOCK) ON M.BILL_PKEY = T.BILL_PKEY WHERE(T.TALLY_ID IS NULL OR T.TALLY_ID = '') AND T.TALLY_SYNC_YN = 'Y'; ";
            var dtmidAll = GetData(sql);

            sql = @"SELECT M.VT_PKEY,M.VCH_SER,M.VCH_NO,M.VCH_DATE,M.CHEQUE_AMT,M.SERCONTROL FROM FVTTOP M WITH (NOLOCK) INNER JOIN TBILLTOP T WITH (NOLOCK) 
                    ON M.VT_PKEY = T.BILL_PKEY WHERE  (T.TALLY_ID IS NULL OR T.TALLY_ID = '') AND T.TALLY_SYNC_YN = 'Y';";
            var dtFvttopAll = GetData(sql);

            sql = @" SELECT F.Ac_Name, F.GRP_NAME, F.ADDRESS, F.CITY,F.COUNTRY, F.STATE, F.PIN_CODE, F.PAN_NO,F.PGSTIN_NO, F.AC_TYPE, M.VT_PKEY,M.DOCU_TYPE,M.VCH_DATE,M.ENT_NO,
                    M.BOOK_CODE,M.AC_CODE,M.BILL_NO,M.BILL_DATE,M.NARRATION,M.AMT,M.QNTY,M.OAC_CODE,M.CD_FLAG,M.VEXCH_RATE,M.VT_SER_PREFIX,M.VT_SER_SUFFIX,M.PLANT  FROM 
                    FVTMID M WITH (NOLOCK) INNER JOIN FACMAS F WITH (NOLOCK) ON M.AC_CODE = F.AC_CODE INNER JOIN TBILLTOP T WITH (NOLOCK) ON M.VT_PKEY =
                    T.BILL_PKEY WHERE T.TALLY_ID = '' AND T.TALLY_SYNC_YN = 'Y'";
            var dtFVTMIDAll = GetData(sql);

            sql = @"SELECT A.DOC_PKEY,A.AC_CODE,A.ENT_NO,A.REF_NO,A.DOCU_TYPE,A.AMT,A.DC_FLAG,A.DUE_DATE,A.ADJ_AMT,A.AN_FLAG,A.REF_DATE,A.PLANT,A.CR_DAYS,A.DOCREF_NO,A.ORD_PKEY,A.FREXCH_RATE,A.SER_PREFIX,A.PROJ_CODE
                FROM FREFDETAIL A(NOLOCK) JOIN TBILLTOP T(NOLOCK) ON A.DOC_PKEY = T.BILL_PKEY WHERE(T.TALLY_ID IS NULL OR T.TALLY_ID = '') AND T.TALLY_SYNC_YN = 'Y';";

            var dtRefAll = GetData(sql);
            if (dtinvoiceS.Rows.Count == 0)
            {
                return Content("Data not found in table");
            }
            if (dtinvoiceS.Rows.Count > 0)
            {
                foreach (DataRow row in dtinvoiceS.Rows)
                {
                    DOC_DETAIL DOC_DETAIL = new DOC_DETAIL();


                    DOC_DETAIL.BILL_PKEY = row["BILL_PKEY"].ToString();
                    DOC_DETAIL.TYPE = row["TYPE"].ToString();
                    DOC_DETAIL.VCH_SER = row["VCH_SER"].ToString();
                    DOC_DETAIL.VCH_NO = row["VCH_NO"] != DBNull.Value ? row["VCH_NO"].ToString() : "";
                    DOC_DETAIL.VCH_DATE = row["VCH_DATE"] == DBNull.Value ? null : ((DateTime)row["VCH_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.SER_PREFIX = row["SER_PREFIX"].ToString();
                    DOC_DETAIL.TALLYDOCU_TYPE = row["DOCU_TYPE"].ToString();
                    DOC_DETAIL.TALLYVCH_NO = $"{DOC_DETAIL.SER_PREFIX}{DOC_DETAIL.VCH_SER}N{DOC_DETAIL.VCH_NO}";
                    DOC_DETAIL.PREP_TIME = row["PREP_TIME"].ToString();
                    DOC_DETAIL.REM_DATE = row["REM_DATE"] == DBNull.Value ? null : ((DateTime)row["REM_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.REM_TIME = row["REM_TIME"].ToString();
                    DOC_DETAIL.CFLAG = row["CFLAG"].ToString();
                    DOC_DETAIL.PLANT = row["PLANT"].ToString();
                    DOC_DETAIL.CATEGORY = row["CATEGORY"].ToString();
                    DOC_DETAIL.CASH_CREDIT = row["CASH_CREDIT"].ToString();
                    DOC_DETAIL.SALE_TYPE = row["SALE_TYPE"].ToString();
                    DOC_DETAIL.SERCONTROL = row["SERCONTROL"].ToString();
                    DOC_DETAIL.GS_FLAG = row["GS_FLAG"].ToString();
                    DOC_DETAIL.LCO_FLAG = row["LCO_FLAG"].ToString();
                    DOC_DETAIL.BANK_CODE = row["BANK_CODE"].ToString();
                    DOC_DETAIL.USRNAME = row["USRNAME"].ToString();
                    DOC_DETAIL.VCODE = row["VCODE"].ToString();
                    DOC_DETAIL.AC_CODE = row["AC_CODE"].ToString();
                    DOC_DETAIL.STATE = row["STATE"].ToString();
                    DOC_DETAIL.CR_DAYS = row["CR_DAYS"].ToString();
                    DOC_DETAIL.CONS_CODE = row["CONS_CODE"].ToString();
                    DOC_DETAIL.CONS_NAME = row["CONS_NAME"].ToString();
                    DOC_DETAIL.CONS_ADDR = row["CONS_ADDR"].ToString();
                    DOC_DETAIL.CONS_CITY = row["CONS_CITY"].ToString();
                    DOC_DETAIL.CONS_COUNTRY = row["CONS_COUNTRY"].ToString();
                    DOC_DETAIL.CONS_PIN = row["CONS_PIN"].ToString();
                    DOC_DETAIL.CONS_STATE = row["CONS_STATE"].ToString();
                    DOC_DETAIL.CONS_GSTIN_NO = row["CONS_GSTIN_NO"].ToString();
                    DOC_DETAIL.CLRNC_TYPE = row["CLRNC_TYPE"].ToString();
                    DOC_DETAIL.GST_CLRNC_TYPE = row["GST_CLRNC_TYPE"].ToString();
                    DOC_DETAIL.GST_REGN_TYPE = row["GST_REGN_TYPE"].ToString();
                    DOC_DETAIL.FAC_CODE = row["FAC_CODE"].ToString();
                    DOC_DETAIL.DAC_CODE = row["DAC_CODE"].ToString();
                    DOC_DETAIL.CPO_NO = row["CPO_NO"].ToString();
                    DOC_DETAIL.CPO_DATE = row["CPO_DATE"] == DBNull.Value ? null : ((DateTime)row["CPO_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.SE_CODE = row["SE_CODE"].ToString();
                    DOC_DETAIL.KIND_ATTN = row["KIND_ATTN"].ToString();
                    DOC_DETAIL.SPL_INS = row["SPL_INS"].ToString();
                    DOC_DETAIL.NARRATION = row["NARRATION"].ToString();
                    DOC_DETAIL.WBGR_WT = row["WBGR_WT"].ToString();
                    DOC_DETAIL.WBSLIP_NO = row["WBSLIP_NO"].ToString();
                    DOC_DETAIL.FRT_TYPE = row["FRT_TYPE"].ToString();
                    DOC_DETAIL.TPT_MODE = row["TPT_MODE"].ToString();
                    DOC_DETAIL.DRIVER_NAME = row["DRIVER_NAME"].ToString();
                    DOC_DETAIL.CARRIER_NAME = row["CARRIER_NAME"].ToString();
                    DOC_DETAIL.VEH_NO = row["VEH_NO"].ToString();
                    DOC_DETAIL.RRGR_NO = row["RRGR_NO"].ToString();
                    DOC_DETAIL.RRGR_DATE = row["RRGR_DATE"] == DBNull.Value ? null : ((DateTime)row["RRGR_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.DOCU_THRU = row["DOCU_THRU"].ToString();
                    DOC_DETAIL.DISTANCE = row["DISTANCE"].ToString();
                    DOC_DETAIL.EINV_TYPE = row["EINV_TYPE"].ToString();
                    DOC_DETAIL.PVT_MARKS = row["PVT_MARKS"].ToString();
                    DOC_DETAIL.DESTINATION = row["DESTINATION"].ToString();
                    DOC_DETAIL.PRE_C_BY = row["PRE_C_BY"].ToString();
                    DOC_DETAIL.POR_BY_PC = row["POR_BY_PC"].ToString();
                    DOC_DETAIL.PO_DELIVERY = row["PO_DELIVERY"].ToString();
                    DOC_DETAIL.NOTIFY = row["NOTIFY"].ToString();
                    DOC_DETAIL.POL = row["POL"].ToString();
                    DOC_DETAIL.POD = row["POD"].ToString();
                    DOC_DETAIL.COFD = row["COFD"].ToString();
                    DOC_DETAIL.TERMS = row["TERMS"].ToString();
                    DOC_DETAIL.EXCH_RATE = row["EXCH_RATE"].ToString();
                    DOC_DETAIL.TQNTY = row["TQNTY"].ToString();
                    DOC_DETAIL.TCASES = row["TCASES"].ToString();
                    DOC_DETAIL.AMT1 = row["AMT1"].ToString();
                    DOC_DETAIL.AMT2 = row["AMT2"].ToString();
                    DOC_DETAIL.AMT3 = row["AMT3"].ToString();
                    DOC_DETAIL.AMT13 = row["AMT13"].ToString();
                    DOC_DETAIL.PER1 = row["PER1"].ToString();
                    DOC_DETAIL.PER2 = row["PER2"].ToString();
                    DOC_DETAIL.PER3 =row["PER3"].ToString();
                    DOC_DETAIL.PER13 =row["PER13"].ToString();
                    DOC_DETAIL.ADESC1 =row["ADESC1"].ToString();
                    DOC_DETAIL.ADESC2 =row["ADESC2"].ToString();
                    DOC_DETAIL.ADESC3 =row["ADESC3"].ToString();
                    DOC_DETAIL.FRTSGST_RATE =row["FRTSGST_RATE"].ToString();
                    DOC_DETAIL.FRTSGST_AMT =row["FRTSGST_AMT"].ToString();
                    DOC_DETAIL.FRTCGST_RATE =row["FRTCGST_RATE"].ToString();
                    DOC_DETAIL.FRTCGST_AMT =row["FRTCGST_AMT"].ToString();
                    DOC_DETAIL.FRTIGST_RATE =row["FRTIGST_RATE"].ToString();
                    DOC_DETAIL.FRTIGST_AMT =row["FRTIGST_AMT"].ToString();
                    DOC_DETAIL.PKGSGST_RATE =row["PKGSGST_RATE"].ToString();
                    DOC_DETAIL.PKGSGST_AMT =row["PKGSGST_AMT"].ToString();
                    DOC_DETAIL.PKGCGST_RATE =row["PKGCGST_RATE"].ToString();
                    DOC_DETAIL.PKGCGST_AMT =row["PKGCGST_AMT"].ToString();
                    DOC_DETAIL.PKGIGST_RATE =row["PKGIGST_RATE"].ToString();
                    DOC_DETAIL.PKGIGST_AMT =row["PKGIGST_AMT"].ToString();
                    DOC_DETAIL.INSSGST_RATE =row["INSSGST_RATE"].ToString();
                    DOC_DETAIL.INSSGST_AMT =row["INSSGST_AMT"].ToString();
                    DOC_DETAIL.INSCGST_RATE =row["INSCGST_RATE"].ToString();
                    DOC_DETAIL.INSCGST_AMT =row["INSCGST_AMT"].ToString();
                    DOC_DETAIL.INSIGST_RATE =row["INSIGST_RATE"].ToString();
                    DOC_DETAIL.INSIGST_AMT =row["INSIGST_AMT"].ToString();
                    DOC_DETAIL.RCSGST_AMT =row["RCSGST_AMT"].ToString();
                    DOC_DETAIL.RCCGST_AMT =row["RCCGST_AMT"].ToString();
                    DOC_DETAIL.RCIGST_AMT =row["RCIGST_AMT"].ToString();
                    DOC_DETAIL.FRTCESS_RATE =row["FRTCESS_RATE"].ToString();
                    DOC_DETAIL.FRTCESS_AMT =row["FRTCESS_AMT"].ToString();
                    DOC_DETAIL.PKGCESS_RATE =row["PKGCESS_RATE"].ToString();
                    DOC_DETAIL.PKGCESS_AMT =row["PKGCESS_AMT"].ToString();
                    DOC_DETAIL.INSCESS_RATE =row["INSCESS_RATE"].ToString();
                    DOC_DETAIL.INSCESS_AMT =row["INSCESS_AMT"].ToString();
                    DOC_DETAIL.TTCS_RATE =row["TTCS_RATE"].ToString();
                    DOC_DETAIL.TBASIC =row["TBASIC"].ToString();
                    DOC_DETAIL.TPKG_AMT =row["TPKG_AMT"].ToString();
                    DOC_DETAIL.TFRT_AMT =row["TFRT_AMT"].ToString();
                    DOC_DETAIL.TINS_AMT =row["TINS_AMT"].ToString();
                    DOC_DETAIL.TCS_BASIC_AMT =row["TCS_BASIC_AMT"].ToString();
                    DOC_DETAIL.TTCS_AMT =row["TTCS_AMT"].ToString();
                    DOC_DETAIL.NTCS_AMT =row["NTCS_AMT"].ToString();
                    DOC_DETAIL.TTOOLING_AMT =row["TTOOLING_AMT"].ToString();
                    DOC_DETAIL.TFOC_AMT =row["TFOC_AMT"].ToString();
                    DOC_DETAIL.TASS_AMT =row["TASS_AMT"].ToString();
                    DOC_DETAIL.TBED_AMT =row["TBED_AMT"].ToString();
                    DOC_DETAIL.TAED_AMT =row["TAED_AMT"].ToString();
                    DOC_DETAIL.TAED_AMT2 =row["TAED_AMT2"].ToString();
                    DOC_DETAIL.TTAXABLE_AMT =row["TTAXABLE_AMT"].ToString();
                    DOC_DETAIL.TRND_AMT =row["TRND_AMT"].ToString();
                    DOC_DETAIL.TBILL_AMT =row["TBILL_AMT"].ToString();
                    DOC_DETAIL.TASN_NO =row["TASN_NO"].ToString();
                    DOC_DETAIL.TWT =row["TWT"].ToString();
                    DOC_DETAIL.TNET_WT =row["TNET_WT"].ToString();
                    DOC_DETAIL.TPACKING =row["TPACKING"].ToString();
                    DOC_DETAIL.OBILL_PKEY =row["OBILL_PKEY"].ToString();
                    DOC_DETAIL.PORT_CODE =row["PORT_CODE"].ToString();
                    DOC_DETAIL.EINR_AMT =row["EINR_AMT"].ToString();
                    DOC_DETAIL.EASS_AMT =row["EASS_AMT"].ToString();
                    DOC_DETAIL.SHIPPINGBILL_NO = row["SHIPPINGBILL_NO"].ToString();
                    DOC_DETAIL.SHIPPINGBILL_DATE = row["SHIPPINGBILL_DATE"] == DBNull.Value ? null : ((DateTime)row["SHIPPINGBILL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_NO = row["EB_NO"].ToString();
                    DOC_DETAIL.EB_DATE = row["EB_DATE"] == DBNull.Value ? null : ((DateTime)row["EB_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_TIME = row["EB_TIME"].ToString();
                    DOC_DETAIL.EB_VALID_TILL_DATE = row["EB_VALID_TILL_DATE"] == DBNull.Value ? null : ((DateTime)row["EB_VALID_TILL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_VALID_TILL_TIME = row["EB_VALID_TILL_TIME"].ToString();
                    DOC_DETAIL.EBCANCEL_DATE = row["EBCANCEL_DATE"] == DBNull.Value ? null : ((DateTime)row["EBCANCEL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EBCANCEL_TIME = row["EBCANCEL_TIME"].ToString();
                    DOC_DETAIL.QR_CODE = row["QR_CODE"].ToString();
                    DOC_DETAIL.BC417_IMAGE = row["BC417_IMAGE"].ToString();
                    DOC_DETAIL.BC417_IMAGE_EINVOICE = row["BC417_IMAGE_EINVOICE"].ToString();
                    DOC_DETAIL.IRN = row["IRN"].ToString();
                    DOC_DETAIL.ACK_NO = row["ACK_NO"].ToString();
                    DOC_DETAIL.ACK_DATE = row["ACK_DATE"] == DBNull.Value ? null : ((DateTime)row["ACK_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.ACK_TIME = row["ACK_TIME"].ToString();
                    DOC_DETAIL.CANCEL_DATE = row["CANCEL_DATE"] == DBNull.Value ? null : ((DateTime)row["CANCEL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.CANCEL_TIME = row["CANCEL_TIME"].ToString();
                    DOC_DETAIL.ITEMS = new List<ITEM>();
                    BILL_PKEY = DOC_DETAIL.BILL_PKEY;
                    DataTable dtinvoiceVT = new DataTable();
                    //string sqlV = "select * from FVTTOP WHERE VT_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "'  ";
                    //dtinvoiceVT = GetData(sqlV);

                    try
                    {
                        dtinvoiceVT = dtFvttopAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    }
                    catch { }
                    DataTable dtinvoiceM = new DataTable();
                    //string sqlM = "SELECT * FROM TBILLMID WHERE BILL_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "' ";
                    //dtinvoiceM = GetData(sqlM);
                    try
                    {
                        dtinvoiceM = dtmidAll.AsEnumerable().Where(w => w["BILL_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    }
                    catch { }
                    foreach (DataRow dr in dtinvoiceM.Rows)
                    {

                        ITEM item = new ITEM();
                        {
                            item.BILL_PKEY = dr["BILL_PKEY"].ToString();
                            item.ITEM_CODE = dr["ITEM_CODE"].ToString();
                            item.ITEM_NAME = dr["ITEM_NAME"].ToString();
                            item.ITEM_DESC = dr["ITEM_DESC"].ToString();
                            item.ITEM_NO = dr["ITEM_NO"].ToString();
                            item.MODEL = dr["MODEL"].ToString();
                            item.CPROD_CODE = dr["CPROD_CODE"].ToString();
                            item.MCPROD_NAME = dr["MCPROD_NAME"].ToString();
                            item.CO_FLAG = dr["CO_FLAG"].ToString();
                            item.OSC_FLAG = dr["OSC_FLAG"].ToString();
                            item.PROJ_CODE = dr["PROJ_CODE"].ToString();
                            item.UNIT = dr["UNIT"].ToString();
                            item.QNTY = dr["QNTY"].ToString();
                            item.SUNIT = dr["SUNIT"].ToString();
                            item.SQNTY = dr["SQNTY"].ToString();
                            item.WQ_FLAG = dr["WQ_FLAG"].ToString();
                            item.MRP_RATE = dr["MRP_RATE"].ToString();
                            item.RATE = dr["RATE"].ToString();
                            item.DIS_RATE1 = dr["DIS_RATE1"].ToString();
                            item.DIS_AMT1 = dr["DIS_AMT1"].ToString();
                            item.DIS_RATE2 = dr["DIS_RATE2"].ToString();
                            item.DIS_AMT2 = dr["DIS_AMT2"].ToString();
                            item.DIS_RATE3 = dr["DIS_RATE3"].ToString();
                            item.DIS_AMT3 = dr["DIS_AMT3"].ToString();
                            item.BED_RATE = dr["BED_RATE"].ToString();
                            item.AED_RATE = dr["AED_RATE"].ToString();
                            item.AED_RATE2 = dr["AED_RATE2"].ToString();
                            item.BED_AMT = dr["BED_AMT"].ToString();
                            item.AED_AMT = dr["AED_AMT"].ToString();
                            item.AED_AMT2 = dr["AED_AMT2"].ToString();
                            item.TOOLING_RATE = dr["TOOLING_RATE"].ToString();
                            item.TOOLING_AMT = dr["TOOLING_AMT"].ToString();
                            item.FOC_RATE = dr["FOC_RATE"].ToString();
                            item.FOC_AMT = dr["FOC_AMT"].ToString();
                            item.TCS_RATE = dr["TCS_RATE"].ToString();
                            item.TCS_AMT = dr["TCS_AMT"].ToString();
                            item.PKG_AMT = dr["PKG_AMT"].ToString();
                            item.FRT_AMT = dr["FRT_AMT"].ToString();
                            item.INS_AMT = dr["INS_AMT"].ToString();
                            item.PKGSGST_AMT = dr["PKGSGST_AMT"].ToString();
                            item.PKGCGST_AMT = dr["PKGCGST_AMT"].ToString();
                            item.PKGIGST_AMT = dr["PKGIGST_AMT"].ToString();
                            item.FRTSGST_AMT = dr["FRTSGST_AMT"].ToString();
                            item.FRTCGST_AMT = dr["FRTCGST_AMT"].ToString();
                            item.FRTIGST_AMT = dr["FRTIGST_AMT"].ToString();
                            item.INSSGST_AMT = dr["INSSGST_AMT"].ToString();
                            item.INSCGST_AMT = dr["INSCGST_AMT"].ToString();
                            item.INSIGST_AMT = dr["INSIGST_AMT"].ToString();
                            item.MISC_AMT = dr["MISC_AMT"].ToString();
                            item.AMOUNT = dr["AMOUNT"].ToString();
                            item.ASS_AMT = dr["ASS_AMT"].ToString();
                            item.TAXABLE_AMT = dr["TAXABLE_AMT"].ToString();
                            item.BILL_AMT = dr["BILL_AMT"].ToString();
                            item.MILL_COST = dr["MILL_COST"].ToString();
                            item.BI_RATE = dr["BI_RATE"].ToString();
                            item.REMARKS = dr["REMARKS"].ToString();
                            item.SO_PKEY = dr["SO_PKEY"].ToString();
                            item.SOA_NO = dr["SOA_NO"].ToString();
                            item.SSCH_PKEY = dr["SSCH_PKEY"].ToString();
                            item.CPO_NO = dr["CPO_NO"].ToString();
                            item.CPO_DATE = dr["CPO_DATE"] == DBNull.Value ? null : ((DateTime)dr["CPO_DATE"]).ToString("dd/MM/yyyy");
                            item.CAO_NO = dr["CAO_NO"].ToString();
                            item.CAO_DATE = dr["CAO_DATE"] == DBNull.Value ? null : ((DateTime)dr["CAO_DATE"]).ToString("dd/MM/yyyy");
                            item.DN_NO = dr["DN_NO"].ToString();
                            item.BATCH_NO = dr["BATCH_NO"].ToString();
                            //item.BATCH_DATE = dtinvoiceM.Rows[0]["BATCH_DATE"] == DBNull.Value ? null : DateTime.Parse(dtinvoiceM.Rows[0]["BATCH_DATE"].ToString()).ToString("dd/MM/yyyy");
                            item.SSO_PKEY = dr["SSO_PKEY"].ToString();
                            item.SSA_PKEY = dr["SSA_PKEY"].ToString();
                            item.OBILL_PKEY = dr["OBILL_PKEY"].ToString();
                            item.TAR_NO = dr["TAR_NO"].ToString();
                            item.Service = new Service();
                            item.Service.SERVICE_NAME = dr["SERVICE_NAME"] == DBNull.Value ? string.Empty : dr["SERVICE_NAME"].ToString();
                            item.Service.STAR_NO = dr["STAR_NO"].ToString();
                            item.Service.SERVICE_CODE = dr["SERVICE_CODE"].ToString();
                            item.OPKG_TYPE = (string)dr["OPKG_TYPE"].ToString();
                            item.PKG_QNTY1 = dr["PKG_QNTY1"].ToString();
                            item.BOXES1 = dr["BOXES1"].ToString();
                            item.PKG_QNTY2 = dr["PKG_QNTY2"].ToString();
                            item.BOXES2 = dr["BOXES2"].ToString();
                            item.IPKG_TYPE = dr["IPKG_TYPE"].ToString();
                            item.IPKG_QNTY1 = dr["IPKG_QNTY1"].ToString();
                            item.IBOXES1 = dr["IBOXES1"].ToString();
                            item.IPKG_QNTY2 = dr["IPKG_QNTY2"].ToString();
                            item.IBOXES2 = dr["IBOXES2"].ToString();
                            item.GR_WT = dr["GR_WT"].ToString();
                            item.NET_WT = dr["NET_WT"].ToString();
                            item.WEIGHT = dr["WEIGHT"].ToString();
                            item.UNL_LOC = dr["UNL_LOC"].ToString();
                            item.USG_LOC = dr["USG_LOC"].ToString();
                            item.SRV_NO = dr["SRV_NO"].ToString();
                            item.SRV_DATE = dr["SRV_DATE"] == DBNull.Value ? null : ((DateTime)dr["SRV_DATE"]).ToString("dd/MM/yyyy");
                            item.CASENO_FROM = dr["CASENO_FROM"].ToString();
                            item.CASENO_TO = dr["CASENO_TO"].ToString();
                            item.PALLET_NO = dr["PALLET_NO"].ToString();
                            item.CASE_NO = dr["CASE_NO"].ToString();

                        };
                        DOC_DETAIL.ITEMS.Add(item);
                    }


                    //DataTable dtinvoiceVT = new DataTable();
                    ////string sqlV = "select * from FVTTOP WHERE VT_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "'  ";
                    ////dtinvoiceVT = GetData(sqlV);

                    //try
                    //{
                    //    dtinvoiceVT = dtFvttopAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    //}
                    //catch { }
                    List<VoucherDetail> voucherList = new List<VoucherDetail>();
                    foreach (DataRow drv in dtinvoiceVT.Rows)
                    {
                        VoucherDetail voucher = new VoucherDetail();
                        voucher.TOP = new TOP();
                        List<MID> midList = new List<MID>();
                        List<Reference> referenceList = new List<Reference>();
                        voucher.TOP.VT_PKEY = drv["VT_PKEY"].ToString();
                        voucher.TOP.VCH_SER = drv["VCH_SER"].ToString();
                        voucher.TOP.VCH_NO = drv["VCH_NO"].ToString();
                        voucher.TOP.VCH_DATE = drv["VCH_DATE"] == DBNull.Value ? null : ((DateTime)drv["VCH_DATE"]).ToString("dd/MM/yyyy");
                        voucher.TOP.CHEQUE_AMT = drv["CHEQUE_AMT"].ToString();
                        voucher.TOP.SERCONTROL = drv["SERCONTROL"].ToString();
                        vt_pkey = voucher.TOP.VT_PKEY;
                        //string sqlVM = "SELECT F.Ac_Name,F.GRP_NAME,F.ADDRESS,F.CITY,F.COUNTRY,F.STATE,F.PIN_CODE,F.PAN_NO,F.PGSTIN_NO,F.AC_TYPE, M.*
                        //FROM FVTMID M,FACMAS F WHERE M.AC_CODE=F.AC_CODE AND VT_PKEY =  '" + vt_pkey.Replace("'", "''") + "' ";
                        //DataTable dtInvoiceVM = GetData(sqlVM);
                        DataTable dtInvoiceVM = new DataTable();
                        try
                        {
                            dtInvoiceVM = dtFVTMIDAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == vt_pkey.Replace("'", "''")).CopyToDataTable();
                        }
                        catch { }
                        foreach (DataRow drvMid in dtInvoiceVM.Rows)
                        {
                            MID mid = new MID();
                            mid.VT_PKEY = drvMid["VT_PKEY"].ToString();
                            mid.DOCU_TYPE = drvMid["DOCU_TYPE"].ToString();
                            mid.VCH_DATE = drvMid["VCH_DATE"] == DBNull.Value ? null : ((DateTime)drvMid["VCH_DATE"]).ToString("dd/MM/yyyy");
                            mid.ENT_NO = drvMid["ENT_NO"].ToString();
                            mid.BOOK_CODE = drvMid["BOOK_CODE"].ToString();
                            mid.AC_CODE = drvMid["AC_CODE"].ToString();
                            mid.LEDGERNAME = drvMid["Ac_Name"].ToString();
                            mid.GROUP = drvMid["GRP_NAME"].ToString();
                            mid.ADDRESS = drvMid["ADDRESS"].ToString();
                            mid.CITY = drvMid["CITY"].ToString();
                            mid.COUNTRY = drvMid["COUNTRY"].ToString();
                            mid.STATE = drvMid["STATE"].ToString();
                            mid.PIN = drvMid["PIN_CODE"].ToString();
                            mid.PAN_NO = drvMid["PAN_NO"].ToString();
                            mid.GSTIN_NO = drvMid["PGSTIN_NO"].ToString();
                            mid.AC_TYPE = drvMid["AC_TYPE"].ToString();
                            mid.BILL_NO = drvMid["BILL_NO"].ToString();
                            mid.BILL_DATE = drvMid["BILL_DATE"] == DBNull.Value ? null : ((DateTime)drvMid["BILL_DATE"]).ToString("dd/MM/yyyy");
                            mid.NARRATION = drvMid["NARRATION"].ToString();
                            mid.AMT = drvMid["AMT"].ToString();
                            mid.QNTY = drvMid["QNTY"].ToString();
                            mid.OAC_CODE = drvMid["OAC_CODE"].ToString();
                            mid.CD_FLAG = drvMid["CD_FLAG"].ToString();
                            mid.VEXCH_RATE = drvMid["VEXCH_RATE"].ToString();
                            //  mid.VT_SER_PREFIX = (string) drvMid["VT_SER_PREFIX"].ToString();
                            //  mid.VT_SER_SUFFIX = (string) drvMid["VT_SER_SUFFIX"].ToString();
                            mid.PLANT = drvMid["PLANT"].ToString();
                            mid.REFERENCE = new List<Reference>();

                            //string sqlVRef = $"SELECT * FROM FREFDETAIL WHERE DOC_PKEY = '{mid.VT_PKEY}' AND AC_CODE = '{drvMid["AC_CODE"].ToString().Replace("'", "''")}'";
                            DataTable dtInvoiceVRef = new DataTable();

                            try
                            {
                                dtInvoiceVRef = dtRefAll.AsEnumerable().Where(w => w["DOC_PKEY"].ToString().Trim().Replace("'", "''") == mid.VT_PKEY.Replace("'", "''")
                                 && w["AC_CODE"].ToString().Trim().Replace("'", "''") == drvMid["AC_CODE"].ToString().Replace("'", "''")).CopyToDataTable();
                            }
                            catch { }
                            foreach (DataRow drvRef in dtInvoiceVRef.Rows)
                            {
                                Reference reference = new Reference();
                                reference.REF_NO = drvRef["REF_NO"].ToString();
                                reference.DOC_PKEY = drvRef["DOC_PKEY"].ToString();
                                reference.AC_CODE = drvRef["AC_CODE"].ToString();
                                reference.ENT_NO = drvRef["ENT_NO"].ToString();
                                reference.DOCU_TYPE = drvRef["DOCU_TYPE"].ToString();
                                reference.AMT = drvRef["AMT"].ToString();
                                reference.DC_FLAG = drvRef["DC_FLAG"].ToString();
                                reference.DUE_DATE = drvRef["DUE_DATE"] == DBNull.Value ? null : ((DateTime)drvRef["DUE_DATE"]).ToString("dd/MM/yyyy");
                                reference.ADJ_AMT = drvRef["ADJ_AMT"].ToString();
                                reference.AN_FLAG = drvRef["AN_FLAG"].ToString();
                                reference.REF_DATE = drvRef["REF_DATE"] == DBNull.Value ? null : ((DateTime)drvRef["REF_DATE"]).ToString("dd/MM/yyyy");
                                reference.PLANT = drvRef["PLANT"].ToString();
                                reference.CR_DAYS = drvRef["CR_DAYS"].ToString();
                                reference.DOCREF_NO = drvRef["DOCREF_NO"].ToString();
                                reference.ORD_PKEY = drvRef["ORD_PKEY"].ToString();
                                reference.FREXCH_RATE = drvRef["FREXCH_RATE"].ToString();
                                reference.SER_PREFIX = drvRef["SER_PREFIX"].ToString();
                                reference.PROJ_CODE = drvRef["PROJ_CODE"].ToString();
                                mid.REFERENCE.Add(reference);
                            }
                            voucher.TOP.MID.Add(mid);
                        }
                        voucherList.Add(voucher);
                    }
                    DOC_DETAIL.VoucherDetail = voucherList;
                    d.DOC_DETAIL.Add(DOC_DETAIL);
                }
                var json = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
                return Content(json, "application/json");
            }
            return Content("");
        }

        [HttpPost] 
        public async Task<ContentResult> GetEditedInvoice()
        {
            DataTable dtinvoiceS = new DataTable();
            string sql = @"SELECT (SELECT TOP 1 DOCU_TYPE  FROM FVTMID WITH(NOLOCK) WHERE VT_PKEY = T.BILL_PKEY ORDER BY DOCU_TYPE ASC) AS DOCU_TYPE,T.BILL_PKEY, T.TYPE, T.VCH_SER, T.VCH_NO, T.VCH_DATE, T.PREP_TIME, 
        T.REM_DATE, T.REM_TIME, T.TAX_CODE, T.AC_CODE, T.AC_NAME, T.ADDRESS, T.CITY, T.CONS_NAME, T.CONS_ADDR, T.DESTINATION, T.CARRIER_NAME, T.VEH_NO, T.RRGR_NO, T.RRGR_DATE, T.DOCU_THRU, T.TQNTY, T.TCASES, 
        T.TWT, T.TBASIC, T.TASS_AMT, T.TBED_AMT, T.TAED_AMT, T.TTAXABLE_AMT, T.TRND_AMT, T.TBILL_AMT, T.SERCONTROL, T.SPL_INS, T.PVT_MARKS, T.SALE_TYPE, T.NARRATION, T.CR_DAYS, T.AMT1, T.AMT2, T.AMT3, T.AMT4, 
        T.AMT5, T.AMT6, T.AMT7, T.AMT8, T.AMT9, T.AMT10, T.AMT11, T.AMT12, T.AMT13, T.AMT14, T.AMT15, T.AMT16, T.PER1, T.PER2, T.PER3, T.PER4, T.PER5, T.PER6, T.PER7, T.PER8, T.PER9, T.PER10, T.PER11, T.PER12, 
        T.PER13, T.PER14, T.PER15, T.PER16, T.ADESC1, T.ADESC2, T.ADESC3, T.ADESC4, T.ADESC5, T.ADESC6, T.ADESC7, T.ADESC8, T.ADESC9, T.ADESC10, T.ADESC11, T.ADESC12, T.ADESC13, T.ADESC14, T.ADESC15, T.ADESC16, 
        T.CASH_CREDIT, T.TPKG_AMT, T.TFRT_AMT, T.TINS_AMT, T.CPO_NO, T.CPO_DATE, T.CONS_CITY, T.CONS_COUNTRY, T.CONS_PIN, T.CONS_STATE, T.COUNTRY, T.PIN, T.STATE, T.CONS_CODE, T.DRIVER_NAME, T.CLRNC_TYPE, T.VCODE, 
        T.TPT_MODE, T.EXCH_RATE, T.EINV_TYPE, T.TTCS_AMT, T.OBILL_PKEY, T.TPACKING, T.CATEGORY, T.SE_CODE, T.PLANT, T.CANCELLED_YN, T.TAED_AMT2, T.DAC_CODE, T.TTOOLING_AMT, T.TFOC_AMT, T.FRT_TYPE, 
        T.CFLAG, T.TNET_WT, T.DESIG_BILL_TO, T.FAC_CODE, T.BANK_CODE, T.PRE_C_BY, T.POR_BY_PC, T.PO_DELIVERY, T.NOTIFY, T.POL, T.POD, T.COFD, T.TERMS, T.USRNAME, T.TSOP_GALF, T.GS_FLAG, T.LCO_FLAG, 
        T.GSTIN_NO, T.CONS_GSTIN_NO, T.GST_CLRNC_TYPE, T.GST_REGN_TYPE, T.SER_PREFIX, T.DISTANCE, T.TTCS_RATE, T.NTCS_AMT, T.TCS_BASIC_AMT, T.EB_YN, T.CHANGE_POS, T.PROJ_CODE, 
        E.KIND_ATTN, E.EINR_AMT, E.EASS_AMT, E.FRTSGST_RATE, E.FRTSGST_AMT, E.FRTCGST_RATE, E.FRTCGST_AMT, E.FRTIGST_RATE, E.FRTIGST_AMT, E.PKGSGST_RATE, E.PKGSGST_AMT, E.PKGCGST_RATE, E.PKGCGST_AMT, 
        E.PKGIGST_RATE, E.PKGIGST_AMT, E.INSSGST_RATE, E.INSSGST_AMT, E.INSCGST_RATE, E.INSCGST_AMT, E.INSIGST_RATE, E.INSIGST_AMT, E.RCSGST_AMT, E.RCCGST_AMT, E.RCIGST_AMT, E.EB_NO, E.EB_DATE, 
        E.EB_TIME, E.FPIGSTSLAB_CODE, E.SHIPPINGBILL_NO, E.SHIPPINGBILL_DATE, E.PORT_CODE, E.WBGR_WT, E.WBSLIP_NO, E.OVER_WEIGHT_REASON, E.RFID_SEAL_NO, E.BC417_IMAGE, E.IRN, E.ACK_NO, E.ACK_DATE, 
        E.ACK_TIME, E.CANCEL_DATE, E.CANCEL_TIME, E.EVU_TIME, E.EVU_DATE, E.QR_CODE, E.EBCANCEL_DATE, E.EBCANCEL_TIME, E.EIFPIGSTSLAB_CODE, E.EIFPICOM_CODE, E.BC417_IMAGE_EINVOICE, E.TASN_NO, E.EB_VALID_TILL_DATE, 
        E.EB_VALID_TILL_TIME, E.FRTCESS_RATE, E.FRTCESS_AMT, E.PKGCESS_RATE, E.PKGCESS_AMT, E.INSCESS_RATE, E.INSCESS_AMT FROM TBILLTOP T WITH(NOLOCK) JOIN TBILLTOPE E WITH(NOLOCK) ON 
        T.BILL_PKEY = E.BILL_PKEY WHERE  (T.TALLY_ID IS NULL OR T.TALLY_ID <> '') AND T.TALLY_SYNC_YN = 'Y';";
            dtinvoiceS = GetData(sql);
            data d = new data();
            d.DOC_DETAIL = new List<DOC_DETAIL>();
            sql = @"SELECT M.BILL_PKEY, M.ENT_NO, M.ITEM_CODE, M.ITEM_NAME, M.QNTY, M.RATE, M.DIS_RATE1, M.DIS_AMT1, M.DIS_RATE2, M.DIS_AMT2, M.AMOUNT, M.ASS_AMT, M.BED_RATE, M.AED_RATE, M.BED_AMT, M.AED_AMT, M.TAXABLE_AMT, M.BILL_AMT, 
                M.MILL_COST, M.REMARKS, M.SUNIT, M.SQNTY, M.UNIT, M.NOD, M.PKG_AMT, M.FRT_AMT, M.INS_AMT, M.DI_NO, M.DI_DATE, M.DI_TIME, M.DN_NO, M.ITEM_DESC, M.BATCH_NO, M.CPO_NO, M.CPO_DATE, M.CAO_NO, M.CAO_DATE, 
                M.TAR_NO, M.COM_CODE, M.CPROD_CODE, M.CO_FLAG, M.OSC_FLAG, M.PKG_QNTY1, M.BOXES1, M.PKG_QNTY2, M.BOXES2, M.GR_WT, M.NET_WT, M.UNL_LOC, M.USG_LOC, M.SO_PKEY, M.SRV_NO, M.SRV_DATE, M.HUNDI_NO, 
                M.HUNDI_DATE, M.TCS_RATE, M.TCS_AMT, M.MODEL, M.RATE_BASIS, M.WEIGHT, M.ITEM_NO, M.RQNTY, M.AQNTY, M.SOA_NO, M.IPKG_TYPE, M.IPKG_QNTY1, M.IBOXES1, M.IPKG_QNTY2, M.IBOXES2, M.MRP_RATE, 
                M.AED_RATE2, M.AED_AMT2, M.TOOLING_RATE, M.TOOLING_AMT, M.FOC_RATE, M.FOC_AMT, M.ABED_RATE, M.ABED_AMT, M.OPKG_TYPE, M.SSO_PKEY, M.BI_RATE, M.CASENO_FROM, M.CASENO_TO, M.WQ_FLAG, M.SSA_PKEY, 
                M.DIS_RATE3, M.DIS_AMT3, M.PALLET_NO, M.GSTSLAB_CODE, M.SERVICE_CODE, M.SERVICE_NAME, M.SCOM_CODE, M.STAR_NO, M.FRTSGST_AMT, M.FRTCGST_AMT, M.FRTIGST_AMT, M.PKGSGST_AMT, M.PKGCGST_AMT, M.PKGIGST_AMT, 
                M.INSSGST_AMT, M.INSCGST_AMT, M.INSIGST_AMT, M.MISC_AMT, M.OBILL_PKEY, M.SSCH_PKEY, M.MCPROD_NAME, M.CASE_NO, M.PROJ_CODE, M.SLOC_CODE, M.BATCH_DATE
            FROM TBILLMID M WITH(NOLOCK) INNER JOIN TBILLTOP T WITH(NOLOCK) ON M.BILL_PKEY = T.BILL_PKEY WHERE(T.TALLY_ID IS NULL OR T.TALLY_ID <> '') AND T.TALLY_SYNC_YN = 'Y'; ";
            var dtmidAll = GetData(sql);

            sql = @"SELECT M.VT_PKEY,M.VCH_SER,M.VCH_NO,M.VCH_DATE,M.CHEQUE_AMT,M.SERCONTROL FROM FVTTOP M WITH (NOLOCK) INNER JOIN TBILLTOP T WITH (NOLOCK) 
                    ON M.VT_PKEY = T.BILL_PKEY WHERE  (T.TALLY_ID IS NULL OR T.TALLY_ID <> '') AND T.TALLY_SYNC_YN = 'Y';";
            var dtFvttopAll = GetData(sql);

            sql = @" SELECT F.Ac_Name, F.GRP_NAME, F.ADDRESS, F.CITY,F.COUNTRY, F.STATE, F.PIN_CODE, F.PAN_NO,F.PGSTIN_NO, F.AC_TYPE, M.VT_PKEY,M.DOCU_TYPE,M.VCH_DATE,M.ENT_NO,
                    M.BOOK_CODE,M.AC_CODE,M.BILL_NO,M.BILL_DATE,M.NARRATION,M.AMT,M.QNTY,M.OAC_CODE,M.CD_FLAG,M.VEXCH_RATE,M.VT_SER_PREFIX,M.VT_SER_SUFFIX,M.PLANT  FROM 
                    FVTMID M WITH (NOLOCK) INNER JOIN FACMAS F WITH (NOLOCK) ON M.AC_CODE = F.AC_CODE INNER JOIN TBILLTOP T WITH (NOLOCK) ON M.VT_PKEY =
                    T.BILL_PKEY WHERE T.TALLY_ID <> '' AND T.TALLY_SYNC_YN = 'Y'";
            var dtFVTMIDAll = GetData(sql);

            sql = @"SELECT A.DOC_PKEY,A.AC_CODE,A.ENT_NO,A.REF_NO,A.DOCU_TYPE,A.AMT,A.DC_FLAG,A.DUE_DATE,A.ADJ_AMT,A.AN_FLAG,A.REF_DATE,A.PLANT,A.CR_DAYS,A.DOCREF_NO,A.ORD_PKEY,A.FREXCH_RATE,A.SER_PREFIX,A.PROJ_CODE
                FROM FREFDETAIL A(NOLOCK) JOIN TBILLTOP T(NOLOCK) ON A.DOC_PKEY = T.BILL_PKEY WHERE(T.TALLY_ID IS NULL OR T.TALLY_ID <> '') AND T.TALLY_SYNC_YN = 'Y';";

            var dtRefAll = GetData(sql);
            if (dtinvoiceS.Rows.Count == 0)
            {
                return Content("Data not found in table");
            }
            if (dtinvoiceS.Rows.Count > 0)
            {
                foreach (DataRow row in dtinvoiceS.Rows)
                {
                    DOC_DETAIL DOC_DETAIL = new DOC_DETAIL();


                    DOC_DETAIL.BILL_PKEY = row["BILL_PKEY"].ToString();
                    DOC_DETAIL.TYPE = row["TYPE"].ToString();
                    DOC_DETAIL.VCH_SER = row["VCH_SER"].ToString();
                    DOC_DETAIL.VCH_NO = row["VCH_NO"] != DBNull.Value ? row["VCH_NO"].ToString() : "";
                    DOC_DETAIL.VCH_DATE = row["VCH_DATE"] == DBNull.Value ? null : ((DateTime)row["VCH_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.SER_PREFIX = row["SER_PREFIX"].ToString();
                    DOC_DETAIL.TALLYDOCU_TYPE = row["DOCU_TYPE"].ToString();
                    DOC_DETAIL.TALLYVCH_NO = $"{DOC_DETAIL.SER_PREFIX}{DOC_DETAIL.VCH_SER}N{DOC_DETAIL.VCH_NO}";
                    DOC_DETAIL.PREP_TIME = row["PREP_TIME"].ToString();
                    DOC_DETAIL.REM_DATE = row["REM_DATE"] == DBNull.Value ? null : ((DateTime)row["REM_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.REM_TIME = row["REM_TIME"].ToString();
                    DOC_DETAIL.CFLAG = row["CFLAG"].ToString();
                    DOC_DETAIL.PLANT = row["PLANT"].ToString();
                    DOC_DETAIL.CATEGORY = row["CATEGORY"].ToString();
                    DOC_DETAIL.CASH_CREDIT = row["CASH_CREDIT"].ToString();
                    DOC_DETAIL.SALE_TYPE = row["SALE_TYPE"].ToString();
                    DOC_DETAIL.SERCONTROL = row["SERCONTROL"].ToString();
                    DOC_DETAIL.GS_FLAG = row["GS_FLAG"].ToString();
                    DOC_DETAIL.LCO_FLAG = row["LCO_FLAG"].ToString();
                    DOC_DETAIL.BANK_CODE = row["BANK_CODE"].ToString();
                    DOC_DETAIL.USRNAME = row["USRNAME"].ToString();
                    DOC_DETAIL.VCODE = row["VCODE"].ToString();
                    DOC_DETAIL.AC_CODE = row["AC_CODE"].ToString();
                    DOC_DETAIL.STATE = row["STATE"].ToString();
                    DOC_DETAIL.CR_DAYS = row["CR_DAYS"].ToString();
                    DOC_DETAIL.CONS_CODE = row["CONS_CODE"].ToString();
                    DOC_DETAIL.CONS_NAME = row["CONS_NAME"].ToString();
                    DOC_DETAIL.CONS_ADDR = row["CONS_ADDR"].ToString();
                    DOC_DETAIL.CONS_CITY = row["CONS_CITY"].ToString();
                    DOC_DETAIL.CONS_COUNTRY = row["CONS_COUNTRY"].ToString();
                    DOC_DETAIL.CONS_PIN = row["CONS_PIN"].ToString();
                    DOC_DETAIL.CONS_STATE = row["CONS_STATE"].ToString();
                    DOC_DETAIL.CONS_GSTIN_NO = row["CONS_GSTIN_NO"].ToString();
                    DOC_DETAIL.CLRNC_TYPE = row["CLRNC_TYPE"].ToString();
                    DOC_DETAIL.GST_CLRNC_TYPE = row["GST_CLRNC_TYPE"].ToString();
                    DOC_DETAIL.GST_REGN_TYPE = row["GST_REGN_TYPE"].ToString();
                    DOC_DETAIL.FAC_CODE = row["FAC_CODE"].ToString();
                    DOC_DETAIL.DAC_CODE = row["DAC_CODE"].ToString();
                    DOC_DETAIL.CPO_NO = row["CPO_NO"].ToString();
                    DOC_DETAIL.CPO_DATE = row["CPO_DATE"] == DBNull.Value ? null : ((DateTime)row["CPO_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.SE_CODE = row["SE_CODE"].ToString();
                    DOC_DETAIL.KIND_ATTN = row["KIND_ATTN"].ToString();
                    DOC_DETAIL.SPL_INS = row["SPL_INS"].ToString();
                    DOC_DETAIL.NARRATION = row["NARRATION"].ToString();
                    DOC_DETAIL.WBGR_WT = row["WBGR_WT"].ToString();
                    DOC_DETAIL.WBSLIP_NO = row["WBSLIP_NO"].ToString();
                    DOC_DETAIL.FRT_TYPE = row["FRT_TYPE"].ToString();
                    DOC_DETAIL.TPT_MODE = row["TPT_MODE"].ToString();
                    DOC_DETAIL.DRIVER_NAME = row["DRIVER_NAME"].ToString();
                    DOC_DETAIL.CARRIER_NAME = row["CARRIER_NAME"].ToString();
                    DOC_DETAIL.VEH_NO = row["VEH_NO"].ToString();
                    DOC_DETAIL.RRGR_NO = row["RRGR_NO"].ToString();
                    DOC_DETAIL.RRGR_DATE = row["RRGR_DATE"] == DBNull.Value ? null : ((DateTime)row["RRGR_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.DOCU_THRU = row["DOCU_THRU"].ToString();
                    DOC_DETAIL.DISTANCE = row["DISTANCE"].ToString();
                    DOC_DETAIL.EINV_TYPE = row["EINV_TYPE"].ToString();
                    DOC_DETAIL.PVT_MARKS = row["PVT_MARKS"].ToString();
                    DOC_DETAIL.DESTINATION = row["DESTINATION"].ToString();
                    DOC_DETAIL.PRE_C_BY = row["PRE_C_BY"].ToString();
                    DOC_DETAIL.POR_BY_PC = row["POR_BY_PC"].ToString();
                    DOC_DETAIL.PO_DELIVERY = row["PO_DELIVERY"].ToString();
                    DOC_DETAIL.NOTIFY = row["NOTIFY"].ToString();
                    DOC_DETAIL.POL = row["POL"].ToString();
                    DOC_DETAIL.POD = row["POD"].ToString();
                    DOC_DETAIL.COFD = row["COFD"].ToString();
                    DOC_DETAIL.TERMS = row["TERMS"].ToString();
                    DOC_DETAIL.EXCH_RATE = row["EXCH_RATE"].ToString();
                    DOC_DETAIL.TQNTY = row["TQNTY"].ToString();
                    DOC_DETAIL.TCASES = row["TCASES"].ToString();
                    DOC_DETAIL.AMT1 = row["AMT1"].ToString();
                    DOC_DETAIL.AMT2 = row["AMT2"].ToString();
                    DOC_DETAIL.AMT3 = row["AMT3"].ToString();
                    DOC_DETAIL.AMT13 = row["AMT13"].ToString();
                    DOC_DETAIL.PER1 = row["PER1"].ToString();
                    DOC_DETAIL.PER2 = row["PER2"].ToString();
                    DOC_DETAIL.PER3 = row["PER3"].ToString();
                    DOC_DETAIL.PER13 = row["PER13"].ToString();
                    DOC_DETAIL.ADESC1 = row["ADESC1"].ToString();
                    DOC_DETAIL.ADESC2 = row["ADESC2"].ToString();
                    DOC_DETAIL.ADESC3 = row["ADESC3"].ToString();
                    DOC_DETAIL.FRTSGST_RATE = row["FRTSGST_RATE"].ToString();
                    DOC_DETAIL.FRTSGST_AMT = row["FRTSGST_AMT"].ToString();
                    DOC_DETAIL.FRTCGST_RATE = row["FRTCGST_RATE"].ToString();
                    DOC_DETAIL.FRTCGST_AMT = row["FRTCGST_AMT"].ToString();
                    DOC_DETAIL.FRTIGST_RATE = row["FRTIGST_RATE"].ToString();
                    DOC_DETAIL.FRTIGST_AMT = row["FRTIGST_AMT"].ToString();
                    DOC_DETAIL.PKGSGST_RATE = row["PKGSGST_RATE"].ToString();
                    DOC_DETAIL.PKGSGST_AMT = row["PKGSGST_AMT"].ToString();
                    DOC_DETAIL.PKGCGST_RATE = row["PKGCGST_RATE"].ToString();
                    DOC_DETAIL.PKGCGST_AMT = row["PKGCGST_AMT"].ToString();
                    DOC_DETAIL.PKGIGST_RATE = row["PKGIGST_RATE"].ToString();
                    DOC_DETAIL.PKGIGST_AMT = row["PKGIGST_AMT"].ToString();
                    DOC_DETAIL.INSSGST_RATE = row["INSSGST_RATE"].ToString();
                    DOC_DETAIL.INSSGST_AMT = row["INSSGST_AMT"].ToString();
                    DOC_DETAIL.INSCGST_RATE = row["INSCGST_RATE"].ToString();
                    DOC_DETAIL.INSCGST_AMT = row["INSCGST_AMT"].ToString();
                    DOC_DETAIL.INSIGST_RATE = row["INSIGST_RATE"].ToString();
                    DOC_DETAIL.INSIGST_AMT = row["INSIGST_AMT"].ToString();
                    DOC_DETAIL.RCSGST_AMT = row["RCSGST_AMT"].ToString();
                    DOC_DETAIL.RCCGST_AMT = row["RCCGST_AMT"].ToString();
                    DOC_DETAIL.RCIGST_AMT = row["RCIGST_AMT"].ToString();
                    DOC_DETAIL.FRTCESS_RATE = row["FRTCESS_RATE"].ToString();
                    DOC_DETAIL.FRTCESS_AMT = row["FRTCESS_AMT"].ToString();
                    DOC_DETAIL.PKGCESS_RATE = row["PKGCESS_RATE"].ToString();
                    DOC_DETAIL.PKGCESS_AMT = row["PKGCESS_AMT"].ToString();
                    DOC_DETAIL.INSCESS_RATE = row["INSCESS_RATE"].ToString();
                    DOC_DETAIL.INSCESS_AMT = row["INSCESS_AMT"].ToString();
                    DOC_DETAIL.TTCS_RATE = row["TTCS_RATE"].ToString();
                    DOC_DETAIL.TBASIC = row["TBASIC"].ToString();
                    DOC_DETAIL.TPKG_AMT = row["TPKG_AMT"].ToString();
                    DOC_DETAIL.TFRT_AMT = row["TFRT_AMT"].ToString();
                    DOC_DETAIL.TINS_AMT = row["TINS_AMT"].ToString();
                    DOC_DETAIL.TCS_BASIC_AMT = row["TCS_BASIC_AMT"].ToString();
                    DOC_DETAIL.TTCS_AMT = row["TTCS_AMT"].ToString();
                    DOC_DETAIL.NTCS_AMT = row["NTCS_AMT"].ToString();
                    DOC_DETAIL.TTOOLING_AMT = row["TTOOLING_AMT"].ToString();
                    DOC_DETAIL.TFOC_AMT = row["TFOC_AMT"].ToString();
                    DOC_DETAIL.TASS_AMT = row["TASS_AMT"].ToString();
                    DOC_DETAIL.TBED_AMT = row["TBED_AMT"].ToString();
                    DOC_DETAIL.TAED_AMT = row["TAED_AMT"].ToString();
                    DOC_DETAIL.TAED_AMT2 = row["TAED_AMT2"].ToString();
                    DOC_DETAIL.TTAXABLE_AMT = row["TTAXABLE_AMT"].ToString();
                    DOC_DETAIL.TRND_AMT = row["TRND_AMT"].ToString();
                    DOC_DETAIL.TBILL_AMT = row["TBILL_AMT"].ToString();
                    DOC_DETAIL.TASN_NO = row["TASN_NO"].ToString();
                    DOC_DETAIL.TWT = row["TWT"].ToString();
                    DOC_DETAIL.TNET_WT = row["TNET_WT"].ToString();
                    DOC_DETAIL.TPACKING = row["TPACKING"].ToString();
                    DOC_DETAIL.OBILL_PKEY = row["OBILL_PKEY"].ToString();
                    DOC_DETAIL.PORT_CODE = row["PORT_CODE"].ToString();
                    DOC_DETAIL.EINR_AMT = row["EINR_AMT"].ToString();
                    DOC_DETAIL.EASS_AMT = row["EASS_AMT"].ToString();
                    DOC_DETAIL.SHIPPINGBILL_NO = row["SHIPPINGBILL_NO"].ToString();
                    DOC_DETAIL.SHIPPINGBILL_DATE = row["SHIPPINGBILL_DATE"] == DBNull.Value ? null : ((DateTime)row["SHIPPINGBILL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_NO = row["EB_NO"].ToString();
                    DOC_DETAIL.EB_DATE = row["EB_DATE"] == DBNull.Value ? null : ((DateTime)row["EB_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_TIME = row["EB_TIME"].ToString();
                    DOC_DETAIL.EB_VALID_TILL_DATE = row["EB_VALID_TILL_DATE"] == DBNull.Value ? null : ((DateTime)row["EB_VALID_TILL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EB_VALID_TILL_TIME = row["EB_VALID_TILL_TIME"].ToString();
                    DOC_DETAIL.EBCANCEL_DATE = row["EBCANCEL_DATE"] == DBNull.Value ? null : ((DateTime)row["EBCANCEL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.EBCANCEL_TIME = row["EBCANCEL_TIME"].ToString();
                    DOC_DETAIL.QR_CODE = row["QR_CODE"].ToString();
                    DOC_DETAIL.BC417_IMAGE = row["BC417_IMAGE"].ToString();
                    DOC_DETAIL.BC417_IMAGE_EINVOICE = row["BC417_IMAGE_EINVOICE"].ToString();
                    DOC_DETAIL.IRN = row["IRN"].ToString();
                    DOC_DETAIL.ACK_NO = row["ACK_NO"].ToString();
                    DOC_DETAIL.ACK_DATE = row["ACK_DATE"] == DBNull.Value ? null : ((DateTime)row["ACK_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.ACK_TIME = row["ACK_TIME"].ToString();
                    DOC_DETAIL.CANCEL_DATE = row["CANCEL_DATE"] == DBNull.Value ? null : ((DateTime)row["CANCEL_DATE"]).ToString("dd/MM/yyyy");
                    DOC_DETAIL.CANCEL_TIME = row["CANCEL_TIME"].ToString();
                    DOC_DETAIL.ITEMS = new List<ITEM>();
                    BILL_PKEY = DOC_DETAIL.BILL_PKEY;
                    DataTable dtinvoiceVT = new DataTable();
                    //string sqlV = "select * from FVTTOP WHERE VT_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "'  ";
                    //dtinvoiceVT = GetData(sqlV);

                    try
                    {
                        dtinvoiceVT = dtFvttopAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    }
                    catch { }
                    DataTable dtinvoiceM = new DataTable();
                    //string sqlM = "SELECT * FROM TBILLMID WHERE BILL_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "' ";
                    //dtinvoiceM = GetData(sqlM);
                    try
                    {
                        dtinvoiceM = dtmidAll.AsEnumerable().Where(w => w["BILL_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    }
                    catch { }
                    foreach (DataRow dr in dtinvoiceM.Rows)
                    {

                        ITEM item = new ITEM();
                        {
                            item.BILL_PKEY = dr["BILL_PKEY"].ToString();
                            item.ITEM_CODE = dr["ITEM_CODE"].ToString();
                            item.ITEM_NAME = dr["ITEM_NAME"].ToString();
                            item.ITEM_DESC = dr["ITEM_DESC"].ToString();
                            item.ITEM_NO = dr["ITEM_NO"].ToString();
                            item.MODEL = dr["MODEL"].ToString();
                            item.CPROD_CODE = dr["CPROD_CODE"].ToString();
                            item.MCPROD_NAME = dr["MCPROD_NAME"].ToString();
                            item.CO_FLAG = dr["CO_FLAG"].ToString();
                            item.OSC_FLAG = dr["OSC_FLAG"].ToString();
                            item.PROJ_CODE = dr["PROJ_CODE"].ToString();
                            item.UNIT = dr["UNIT"].ToString();
                            item.QNTY = dr["QNTY"].ToString();
                            item.SUNIT = dr["SUNIT"].ToString();
                            item.SQNTY = dr["SQNTY"].ToString();
                            item.WQ_FLAG = dr["WQ_FLAG"].ToString();
                            item.MRP_RATE = dr["MRP_RATE"].ToString();
                            item.RATE = dr["RATE"].ToString();
                            item.DIS_RATE1 = dr["DIS_RATE1"].ToString();
                            item.DIS_AMT1 = dr["DIS_AMT1"].ToString();
                            item.DIS_RATE2 = dr["DIS_RATE2"].ToString();
                            item.DIS_AMT2 = dr["DIS_AMT2"].ToString();
                            item.DIS_RATE3 = dr["DIS_RATE3"].ToString();
                            item.DIS_AMT3 = dr["DIS_AMT3"].ToString();
                            item.BED_RATE = dr["BED_RATE"].ToString();
                            item.AED_RATE = dr["AED_RATE"].ToString();
                            item.AED_RATE2 = dr["AED_RATE2"].ToString();
                            item.BED_AMT = dr["BED_AMT"].ToString();
                            item.AED_AMT = dr["AED_AMT"].ToString();
                            item.AED_AMT2 = dr["AED_AMT2"].ToString();
                            item.TOOLING_RATE = dr["TOOLING_RATE"].ToString();
                            item.TOOLING_AMT = dr["TOOLING_AMT"].ToString();
                            item.FOC_RATE = dr["FOC_RATE"].ToString();
                            item.FOC_AMT = dr["FOC_AMT"].ToString();
                            item.TCS_RATE = dr["TCS_RATE"].ToString();
                            item.TCS_AMT = dr["TCS_AMT"].ToString();
                            item.PKG_AMT = dr["PKG_AMT"].ToString();
                            item.FRT_AMT = dr["FRT_AMT"].ToString();
                            item.INS_AMT = dr["INS_AMT"].ToString();
                            item.PKGSGST_AMT = dr["PKGSGST_AMT"].ToString();
                            item.PKGCGST_AMT = dr["PKGCGST_AMT"].ToString();
                            item.PKGIGST_AMT = dr["PKGIGST_AMT"].ToString();
                            item.FRTSGST_AMT = dr["FRTSGST_AMT"].ToString();
                            item.FRTCGST_AMT = dr["FRTCGST_AMT"].ToString();
                            item.FRTIGST_AMT = dr["FRTIGST_AMT"].ToString();
                            item.INSSGST_AMT = dr["INSSGST_AMT"].ToString();
                            item.INSCGST_AMT = dr["INSCGST_AMT"].ToString();
                            item.INSIGST_AMT = dr["INSIGST_AMT"].ToString();
                            item.MISC_AMT = dr["MISC_AMT"].ToString();
                            item.AMOUNT = dr["AMOUNT"].ToString();
                            item.ASS_AMT = dr["ASS_AMT"].ToString();
                            item.TAXABLE_AMT = dr["TAXABLE_AMT"].ToString();
                            item.BILL_AMT = dr["BILL_AMT"].ToString();
                            item.MILL_COST = dr["MILL_COST"].ToString();
                            item.BI_RATE = dr["BI_RATE"].ToString();
                            item.REMARKS = dr["REMARKS"].ToString();
                            item.SO_PKEY = dr["SO_PKEY"].ToString();
                            item.SOA_NO = dr["SOA_NO"].ToString();
                            item.SSCH_PKEY = dr["SSCH_PKEY"].ToString();
                            item.CPO_NO = dr["CPO_NO"].ToString();
                            item.CPO_DATE = dr["CPO_DATE"] == DBNull.Value ? null : ((DateTime)dr["CPO_DATE"]).ToString("dd/MM/yyyy");
                            item.CAO_NO = dr["CAO_NO"].ToString();
                            item.CAO_DATE = dr["CAO_DATE"] == DBNull.Value ? null : ((DateTime)dr["CAO_DATE"]).ToString("dd/MM/yyyy");
                            item.DN_NO = dr["DN_NO"].ToString();
                            item.BATCH_NO = dr["BATCH_NO"].ToString();
                            //item.BATCH_DATE = dtinvoiceM.Rows[0]["BATCH_DATE"] == DBNull.Value ? null : DateTime.Parse(dtinvoiceM.Rows[0]["BATCH_DATE"].ToString()).ToString("dd/MM/yyyy");
                            item.SSO_PKEY = dr["SSO_PKEY"].ToString();
                            item.SSA_PKEY = dr["SSA_PKEY"].ToString();
                            item.OBILL_PKEY = dr["OBILL_PKEY"].ToString();
                            item.TAR_NO = dr["TAR_NO"].ToString();
                            item.Service = new Service();
                            item.Service.SERVICE_NAME = dr["SERVICE_NAME"] == DBNull.Value ? string.Empty : dr["SERVICE_NAME"].ToString();
                            item.Service.STAR_NO = dr["STAR_NO"].ToString();
                            item.Service.SERVICE_CODE = dr["SERVICE_CODE"].ToString();
                            item.OPKG_TYPE = (string)dr["OPKG_TYPE"].ToString();
                            item.PKG_QNTY1 = dr["PKG_QNTY1"].ToString();
                            item.BOXES1 = dr["BOXES1"].ToString();
                            item.PKG_QNTY2 = dr["PKG_QNTY2"].ToString();
                            item.BOXES2 = dr["BOXES2"].ToString();
                            item.IPKG_TYPE = dr["IPKG_TYPE"].ToString();
                            item.IPKG_QNTY1 = dr["IPKG_QNTY1"].ToString();
                            item.IBOXES1 = dr["IBOXES1"].ToString();
                            item.IPKG_QNTY2 = dr["IPKG_QNTY2"].ToString();
                            item.IBOXES2 = dr["IBOXES2"].ToString();
                            item.GR_WT = dr["GR_WT"].ToString();
                            item.NET_WT = dr["NET_WT"].ToString();
                            item.WEIGHT = dr["WEIGHT"].ToString();
                            item.UNL_LOC = dr["UNL_LOC"].ToString();
                            item.USG_LOC = dr["USG_LOC"].ToString();
                            item.SRV_NO = dr["SRV_NO"].ToString();
                            item.SRV_DATE = dr["SRV_DATE"] == DBNull.Value ? null : ((DateTime)dr["SRV_DATE"]).ToString("dd/MM/yyyy");
                            item.CASENO_FROM = dr["CASENO_FROM"].ToString();
                            item.CASENO_TO = dr["CASENO_TO"].ToString();
                            item.PALLET_NO = dr["PALLET_NO"].ToString();
                            item.CASE_NO = dr["CASE_NO"].ToString();

                        };
                        DOC_DETAIL.ITEMS.Add(item);
                    }


                    //DataTable dtinvoiceVT = new DataTable();
                    ////string sqlV = "select * from FVTTOP WHERE VT_PKEY =  '" + BILL_PKEY.Replace("'", "''") + "'  ";
                    ////dtinvoiceVT = GetData(sqlV);

                    //try
                    //{
                    //    dtinvoiceVT = dtFvttopAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == BILL_PKEY.Replace("'", "''")).CopyToDataTable();
                    //}
                    //catch { }
                    List<VoucherDetail> voucherList = new List<VoucherDetail>();
                    foreach (DataRow drv in dtinvoiceVT.Rows)
                    {
                        VoucherDetail voucher = new VoucherDetail();
                        voucher.TOP = new TOP();
                        List<MID> midList = new List<MID>();
                        List<Reference> referenceList = new List<Reference>();
                        voucher.TOP.VT_PKEY = drv["VT_PKEY"].ToString();
                        voucher.TOP.VCH_SER = drv["VCH_SER"].ToString();
                        voucher.TOP.VCH_NO = drv["VCH_NO"].ToString();
                        voucher.TOP.VCH_DATE = drv["VCH_DATE"] == DBNull.Value ? null : ((DateTime)drv["VCH_DATE"]).ToString("dd/MM/yyyy");
                        voucher.TOP.CHEQUE_AMT = drv["CHEQUE_AMT"].ToString();
                        voucher.TOP.SERCONTROL = drv["SERCONTROL"].ToString();
                        vt_pkey = voucher.TOP.VT_PKEY;
                        //string sqlVM = "SELECT F.Ac_Name,F.GRP_NAME,F.ADDRESS,F.CITY,F.COUNTRY,F.STATE,F.PIN_CODE,F.PAN_NO,F.PGSTIN_NO,F.AC_TYPE, M.*
                        //FROM FVTMID M,FACMAS F WHERE M.AC_CODE=F.AC_CODE AND VT_PKEY =  '" + vt_pkey.Replace("'", "''") + "' ";
                        //DataTable dtInvoiceVM = GetData(sqlVM);
                        DataTable dtInvoiceVM = new DataTable();
                        try
                        {
                            dtInvoiceVM = dtFVTMIDAll.AsEnumerable().Where(w => w["VT_PKEY"].ToString().Trim().Replace("'", "''") == vt_pkey.Replace("'", "''")).CopyToDataTable();
                        }
                        catch { }
                        foreach (DataRow drvMid in dtInvoiceVM.Rows)
                        {
                            MID mid = new MID();
                            mid.VT_PKEY = drvMid["VT_PKEY"].ToString();
                            mid.DOCU_TYPE = drvMid["DOCU_TYPE"].ToString();
                            mid.VCH_DATE = drvMid["VCH_DATE"] == DBNull.Value ? null : ((DateTime)drvMid["VCH_DATE"]).ToString("dd/MM/yyyy");
                            mid.ENT_NO = drvMid["ENT_NO"].ToString();
                            mid.BOOK_CODE = drvMid["BOOK_CODE"].ToString();
                            mid.AC_CODE = drvMid["AC_CODE"].ToString();
                            mid.LEDGERNAME = drvMid["Ac_Name"].ToString();
                            mid.GROUP = drvMid["GRP_NAME"].ToString();
                            mid.ADDRESS = drvMid["ADDRESS"].ToString();
                            mid.CITY = drvMid["CITY"].ToString();
                            mid.COUNTRY = drvMid["COUNTRY"].ToString();
                            mid.STATE = drvMid["STATE"].ToString();
                            mid.PIN = drvMid["PIN_CODE"].ToString();
                            mid.PAN_NO = drvMid["PAN_NO"].ToString();
                            mid.GSTIN_NO = drvMid["PGSTIN_NO"].ToString();
                            mid.AC_TYPE = drvMid["AC_TYPE"].ToString();
                            mid.BILL_NO = drvMid["BILL_NO"].ToString();
                            mid.BILL_DATE = drvMid["BILL_DATE"] == DBNull.Value ? null : ((DateTime)drvMid["BILL_DATE"]).ToString("dd/MM/yyyy");
                            mid.NARRATION = drvMid["NARRATION"].ToString();
                            mid.AMT = drvMid["AMT"].ToString();
                            mid.QNTY = drvMid["QNTY"].ToString();
                            mid.OAC_CODE = drvMid["OAC_CODE"].ToString();
                            mid.CD_FLAG = drvMid["CD_FLAG"].ToString();
                            mid.VEXCH_RATE = drvMid["VEXCH_RATE"].ToString();
                            //  mid.VT_SER_PREFIX = (string) drvMid["VT_SER_PREFIX"].ToString();
                            //  mid.VT_SER_SUFFIX = (string) drvMid["VT_SER_SUFFIX"].ToString();
                            mid.PLANT = drvMid["PLANT"].ToString();
                            mid.REFERENCE = new List<Reference>();

                            //string sqlVRef = $"SELECT * FROM FREFDETAIL WHERE DOC_PKEY = '{mid.VT_PKEY}' AND AC_CODE = '{drvMid["AC_CODE"].ToString().Replace("'", "''")}'";
                            DataTable dtInvoiceVRef = new DataTable();

                            try
                            {
                                dtInvoiceVRef = dtRefAll.AsEnumerable().Where(w => w["DOC_PKEY"].ToString().Trim().Replace("'", "''") == mid.VT_PKEY.Replace("'", "''")
                                 && w["AC_CODE"].ToString().Trim().Replace("'", "''") == drvMid["AC_CODE"].ToString().Replace("'", "''")).CopyToDataTable();
                            }
                            catch { }
                            foreach (DataRow drvRef in dtInvoiceVRef.Rows)
                            {
                                Reference reference = new Reference();
                                reference.REF_NO = drvRef["REF_NO"].ToString();
                                reference.DOC_PKEY = drvRef["DOC_PKEY"].ToString();
                                reference.AC_CODE = drvRef["AC_CODE"].ToString();
                                reference.ENT_NO = drvRef["ENT_NO"].ToString();
                                reference.DOCU_TYPE = drvRef["DOCU_TYPE"].ToString();
                                reference.AMT = drvRef["AMT"].ToString();
                                reference.DC_FLAG = drvRef["DC_FLAG"].ToString();
                                reference.DUE_DATE = drvRef["DUE_DATE"] == DBNull.Value ? null : ((DateTime)drvRef["DUE_DATE"]).ToString("dd/MM/yyyy");
                                reference.ADJ_AMT = drvRef["ADJ_AMT"].ToString();
                                reference.AN_FLAG = drvRef["AN_FLAG"].ToString();
                                reference.REF_DATE = drvRef["REF_DATE"] == DBNull.Value ? null : ((DateTime)drvRef["REF_DATE"]).ToString("dd/MM/yyyy");
                                reference.PLANT = drvRef["PLANT"].ToString();
                                reference.CR_DAYS = drvRef["CR_DAYS"].ToString();
                                reference.DOCREF_NO = drvRef["DOCREF_NO"].ToString();
                                reference.ORD_PKEY = drvRef["ORD_PKEY"].ToString();
                                reference.FREXCH_RATE = drvRef["FREXCH_RATE"].ToString();
                                reference.SER_PREFIX = drvRef["SER_PREFIX"].ToString();
                                reference.PROJ_CODE = drvRef["PROJ_CODE"].ToString();
                                mid.REFERENCE.Add(reference);
                            }
                            voucher.TOP.MID.Add(mid);
                        }
                        voucherList.Add(voucher);
                    }
                    DOC_DETAIL.VoucherDetail = voucherList;
                    d.DOC_DETAIL.Add(DOC_DETAIL);
                }
                var json = JsonConvert.SerializeObject(d, Newtonsoft.Json.Formatting.Indented);
                return Content(json, "application/json");
            }
            return Content("");
        }
        public string getconnStr()
        {
            var path = HttpContext.Server.MapPath("/mytns.txt");
            int plantid = 0;

            string str = "", srv = "", PWD = "", constr = "", IP = "", userid = "", DBNAME = "";
            try
            {
                if (System.IO.File.Exists(path))
                {
                    StreamReader sr = new StreamReader(path);
                    str = sr.ReadToEnd().Trim();
                    if (str.Contains("\r")) str = str.Replace("\r", "#");
                    if (str.Contains("\n")) str = str.Replace("\n", "#");
                    str = str.Replace(",,", ",");
                    DBNAME = str.Split(new string[] { "##" }, StringSplitOptions.None)[0].Split(new string[] { "," }, StringSplitOptions.None)[plantid];                  
                    var parts = str.Split(new string[] { "##" }, StringSplitOptions.None);
                    IP = parts[1].Split(',')[plantid];
                    userid = parts[2].Split(',')[plantid];
                    PWD = parts[3].Split(',')[plantid];
                    sr.Close();
                    constr = "Data Source=" + IP.Trim() + ";Initial Catalog=" + DBNAME.Trim() + ";" +
                        "Persist Security Info=True;User ID=" + userid.Trim() + ";Password=" + PWD.Trim() + ";" +
                        "TrustServerCertificate=True;";
                }
            }
            catch (Exception err) { }
            return constr;
            
        }
        public DataTable GetData(string query)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(getconnStr()))
            {
                try
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            return dataTable;
        }

        [HttpPost]
        public async Task<ContentResult> TallyResponse()
        {
            string requestBody = "ram";
            using (var reader = new StreamReader(HttpContext.Request.InputStream, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var orderDetail = ReadDataFromJson(requestBody);
            SaveOrderDetails(orderDetail.Tables["tallyresponse"]);
            var json = JsonConvert.DeserializeObject(requestBody);
            return Content("");
        }
        private void SaveOrderDetails(DataTable tallyresponse)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(getconnStr()))
                {
                    conn.Open();
                    InsertDataIntoTECOMOD(tallyresponse, conn);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while saving order details: {ex.Message}");
            }
        }
        private void InsertDataIntoTECOMOD(DataTable table, SqlConnection conn)
        {
            foreach (DataRow row in table.Rows)
            {
                StringBuilder columns = new StringBuilder();
                StringBuilder values = new StringBuilder();
                if (table.Columns.Contains("tallyappid")) { table.Columns["tallyappid"].ColumnName = "TALLY_ID"; }
                if (table.Columns.Contains("bill_pkey")) { table.Columns["bill_pkey"].ColumnName = "BILL_PKEY"; }
                foreach (DataColumn column in table.Columns)
                {
                    if (row[column.ColumnName] != DBNull.Value && !string.IsNullOrEmpty(row[column.ColumnName].ToString()))
                    {
                        if (columns.Length > 0)
                        {
                            columns.Append(", ");
                            values.Append(", ");
                        }

                        columns.Append(column.ColumnName);
                        values.Append("@").Append(column.ColumnName);
                    }
                }
                string query = "UPDATE tbilltop SET TALLY_ID = @TALLY_ID ,TALLY_SYNC_YN='N' WHERE BILL_PKEY = @BILL_PKEY";
                int count = 0;
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        string parameterName = $"@{column.ColumnName}";
                        if (query.Contains(parameterName))
                        {
                            object value = row[column.ColumnName] ?? DBNull.Value;
                            command.Parameters.AddWithValue(parameterName, value);
                        }
                    }
                     count = command.ExecuteNonQuery();
                }

                /*break*/;
            }
        }

        public DataSet ReadDataFromJson(string jsonString, XmlReadMode mode = XmlReadMode.Auto)
        {
            var originaljson = jsonString;
            jsonString = $"{{ \"rootNode\": {{{jsonString.Trim().TrimStart('{').TrimEnd('}')}}} }}";
            XmlDocument xd;
            try
            {
                xd = JsonConvert.DeserializeXmlNode(jsonString);
            }
            catch (Exception eee)
            {
                jsonString = $"{{ \"rootNode\": {{{originaljson.Trim().TrimStart('{').TrimEnd('}')}}} }}" + "}";
                xd = JsonConvert.DeserializeXmlNode(jsonString);

            }
            var result = new DataSet();
            result.ReadXml(new XmlNodeReader(xd), mode);
            return result;
        }



        public class DOC_DETAIL
        {
            public string BILL_PKEY { get; set; }
            public string TALLY_ID { get; set; }
            public string TALLYVCH_NO { get; set; }
            public string TALLYDOCU_TYPE { get; set; }
            public string TYPE { get; set; }
            public string VCH_SER { get; set; }
            public object VCH_NO { get; set; }
            public object VCH_DATE { get; set; }
            public string PREP_TIME { get; set; }
            public object REM_DATE { get; set; }
            public string REM_TIME { get; set; }
            public string CFLAG { get; set; }
            public string PLANT { get; set; }
            public string SER_PREFIX { get; set; }
            public string CATEGORY { get; set; }
            public string CASH_CREDIT { get; set; }
            public string SALE_TYPE { get; set; }
            public string SERCONTROL { get; set; }
            public string GS_FLAG { get; set; }
            public string LCO_FLAG { get; set; }
            public string BANK_CODE { get; set; }
            public string USRNAME { get; set; }
            public string VCODE { get; set; }
            public string AC_CODE { get; set; }
            public string AC_NAME { get; set; }
            public string ADDRESS { get; set; }
            public string CITY { get; set; }
            public string COUNTRY { get; set; }
            public string PIN { get; set; }
            public string STATE { get; set; }
            public string GSTIN_NO { get; set; }
            public object CR_DAYS { get; set; }
            public string CONS_CODE { get; set; }
            public string CONS_NAME { get; set; }
            public string CONS_ADDR { get; set; }
            public string CONS_CITY { get; set; }
            public string CONS_COUNTRY { get; set; }
            public string CONS_PIN { get; set; }
            public string CONS_STATE { get; set; }
            public string CONS_GSTIN_NO { get; set; }
            public string CLRNC_TYPE { get; set; }
            public string GST_CLRNC_TYPE { get; set; }
            public string GST_REGN_TYPE { get; set; }
            public string FAC_CODE { get; set; }
            public string DAC_CODE { get; set; }
            public string CPO_NO { get; set; }
            public object CPO_DATE { get; set; }
            public string SE_CODE { get; set; }
            public string KIND_ATTN { get; set; }
            public string SPL_INS { get; set; }
            public string NARRATION { get; set; }
            public object WBGR_WT { get; set; }
            public string WBSLIP_NO { get; set; }
            public string FRT_TYPE { get; set; }
            public string TPT_MODE { get; set; }
            public string DRIVER_NAME { get; set; }
            public string CARRIER_NAME { get; set; }
            public string VEH_NO { get; set; }
            public string RRGR_NO { get; set; }
            public object RRGR_DATE { get; set; }
            public string DOCU_THRU { get; set; }
            public object DISTANCE { get; set; }
            public string EINV_TYPE { get; set; }
            public string PVT_MARKS { get; set; }
            public string DESTINATION { get; set; }
            public string PRE_C_BY { get; set; }
            public string POR_BY_PC { get; set; }
            public string PO_DELIVERY { get; set; }
            public string NOTIFY { get; set; }
            public string POL { get; set; }
            public string POD { get; set; }
            public string COFD { get; set; }
            public string TERMS { get; set; }
            public object EXCH_RATE { get; set; }
            public object TQNTY { get; set; }
            public object TCASES { get; set; }
            public object AMT1 { get; set; }
            public object AMT2 { get; set; }
            public object AMT3 { get; set; }
            public object AMT13 { get; set; }
            public object PER1 { get; set; }
            public object PER2 { get; set; }
            public object PER3 { get; set; }
            public object PER13 { get; set; }
            public string ADESC1 { get; set; }
            public string ADESC2 { get; set; }
            public string ADESC3 { get; set; }
            public object FRTSGST_RATE { get; set; }
            public object FRTSGST_AMT { get; set; }
            public object FRTCGST_RATE { get; set; }
            public object FRTCGST_AMT { get; set; }
            public object FRTIGST_RATE { get; set; }
            public object FRTIGST_AMT { get; set; }
            public object PKGSGST_RATE { get; set; }
            public object PKGSGST_AMT { get; set; }
            public object PKGCGST_RATE { get; set; }
            public object PKGCGST_AMT { get; set; }
            public object PKGIGST_RATE { get; set; }
            public object PKGIGST_AMT { get; set; }
            public object INSSGST_RATE { get; set; }
            public object INSSGST_AMT { get; set; }
            public object INSCGST_RATE { get; set; }
            public object INSCGST_AMT { get; set; }
            public object INSIGST_RATE { get; set; }
            public object INSIGST_AMT { get; set; }
            public object RCSGST_AMT { get; set; }
            public object RCCGST_AMT { get; set; }
            public object RCIGST_AMT { get; set; }
            public object FRTCESS_RATE { get; set; }
            public object FRTCESS_AMT { get; set; }
            public object PKGCESS_RATE { get; set; }
            public object PKGCESS_AMT { get; set; }
            public object INSCESS_RATE { get; set; }
            public object INSCESS_AMT { get; set; }
            public object TTCS_RATE { get; set; }
            public object TBASIC { get; set; }
            public object TPKG_AMT { get; set; }
            public object TFRT_AMT { get; set; }
            public object TINS_AMT { get; set; }
            public object TCS_BASIC_AMT { get; set; }
            public object TTCS_AMT { get; set; }
            public object NTCS_AMT { get; set; }
            public object TTOOLING_AMT { get; set; }
            public object TFOC_AMT { get; set; }
            public object TASS_AMT { get; set; }
            public object TBED_AMT { get; set; }
            public object TAED_AMT { get; set; }
            public object TAED_AMT2 { get; set; }
            public object TTAXABLE_AMT { get; set; }
            public object TRND_AMT { get; set; }
            public object TBILL_AMT { get; set; }
            public string TASN_NO { get; set; }
            public object TWT { get; set; }
            public object TNET_WT { get; set; }
            public string TPACKING { get; set; }
            public string OBILL_PKEY { get; set; }
            public string PORT_CODE { get; set; }
            public object EINR_AMT { get; set; }
            public object EASS_AMT { get; set; }
            public string SHIPPINGBILL_NO { get; set; }
            public object SHIPPINGBILL_DATE { get; set; }
            public string EB_NO { get; set; }
            public object EB_DATE { get; set; }
            public string EB_TIME { get; set; }
            public object EB_VALID_TILL_DATE { get; set; }
            public string EB_VALID_TILL_TIME { get; set; }
            public object EBCANCEL_DATE { get; set; }
            public string EBCANCEL_TIME { get; set; }
            public string QR_CODE { get; set; }
            public object BC417_IMAGE { get; set; }
            public object BC417_IMAGE_EINVOICE { get; set; }
            public string IRN { get; set; }
            public string ACK_NO { get; set; }
            public object ACK_DATE { get; set; }
            public string ACK_TIME { get; set; }
            public object CANCEL_DATE { get; set; }
            public string CANCEL_TIME { get; set; }
            public List<ITEM> ITEMS { get; set; }
            public List<VoucherDetail> VoucherDetail { get; set; }
        }
        public class ITEM
        {
            public string BILL_PKEY { get; set; }
            public object ENT_NO { get; set; }
            public string ITEM_CODE { get; set; }
            public string ITEM_NAME { get; set; }
            public string ITEM_DESC { get; set; }
            public object ITEM_NO { get; set; }
            public string MODEL { get; set; }
            public string CPROD_CODE { get; set; }
            public string MCPROD_NAME { get; set; }
            public string CO_FLAG { get; set; }
            public string OSC_FLAG { get; set; }
            public string PROJ_CODE { get; set; }
            public string UNIT { get; set; }
            public object QNTY { get; set; }
            public string SUNIT { get; set; }
            public object SQNTY { get; set; }
            public string WQ_FLAG { get; set; }
            public object MRP_RATE { get; set; }
            public object RATE { get; set; }
            public object DIS_RATE1 { get; set; }
            public object DIS_AMT1 { get; set; }
            public object DIS_RATE2 { get; set; }
            public object DIS_AMT2 { get; set; }
            public object DIS_RATE3 { get; set; }
            public object DIS_AMT3 { get; set; }
            public object BED_RATE { get; set; }
            public object AED_RATE { get; set; }
            public object AED_RATE2 { get; set; }
            public object BED_AMT { get; set; }
            public object AED_AMT { get; set; }
            public object AED_AMT2 { get; set; }
            public object TOOLING_RATE { get; set; }
            public object TOOLING_AMT { get; set; }
            public object FOC_RATE { get; set; }
            public object FOC_AMT { get; set; }
            public object TCS_RATE { get; set; }
            public object TCS_AMT { get; set; }
            public object PKG_AMT { get; set; }
            public object FRT_AMT { get; set; }
            public object INS_AMT { get; set; }
            public object PKGSGST_AMT { get; set; }
            public object PKGCGST_AMT { get; set; }
            public object PKGIGST_AMT { get; set; }
            public object FRTSGST_AMT { get; set; }
            public object FRTCGST_AMT { get; set; }
            public object FRTIGST_AMT { get; set; }
            public object INSSGST_AMT { get; set; }
            public object INSCGST_AMT { get; set; }
            public object INSIGST_AMT { get; set; }
            public object MISC_AMT { get; set; }
            public object AMOUNT { get; set; }
            public object ASS_AMT { get; set; }
            public object TAXABLE_AMT { get; set; }
            public object BILL_AMT { get; set; }
            public object MILL_COST { get; set; }
            public object BI_RATE { get; set; }
            public string REMARKS { get; set; }
            public string SO_PKEY { get; set; }
            public object SOA_NO { get; set; }
            public string SSCH_PKEY { get; set; }
            public string CPO_NO { get; set; }
            public object CPO_DATE { get; set; }
            public string CAO_NO { get; set; }
            public object CAO_DATE { get; set; }
            public object DN_NO { get; set; }
            public string BATCH_NO { get; set; }
            public object BATCH_DATE { get; set; }
            public string SSO_PKEY { get; set; }
            public string SSA_PKEY { get; set; }
            public string OBILL_PKEY { get; set; }
            public string TAR_NO { get; set; }
            public Service Service { get; set; }
            public string OPKG_TYPE { get; set; }
            public object PKG_QNTY1 { get; set; }

            [JsonProperty("BOXES1	")]
            public object BOXES1 { get; set; }
            public object PKG_QNTY2 { get; set; }
            public object BOXES2 { get; set; }
            public string IPKG_TYPE { get; set; }
            public object IPKG_QNTY1 { get; set; }
            public object IBOXES1 { get; set; }
            public object IPKG_QNTY2 { get; set; }
            public object IBOXES2 { get; set; }
            public object GR_WT { get; set; }
            public object NET_WT { get; set; }
            public object WEIGHT { get; set; }
            public string UNL_LOC { get; set; }
            public string USG_LOC { get; set; }
            public string SRV_NO { get; set; }
            public object SRV_DATE { get; set; }
            public object CASENO_FROM { get; set; }
            public object CASENO_TO { get; set; }
            public object PALLET_NO { get; set; }
            public object CASE_NO { get; set; }
        }
        public class TOP
        {
            public string VT_PKEY { get; set; }
            public string VCH_SER { get; set; }
            public object VCH_NO { get; set; }
            public object VCH_DATE { get; set; }
            public object CHEQUE_AMT { get; set; }
            public string SERCONTROL { get; set; }
            public List<MID> MID { get; set; } = new List<MID>();

        }
        public class MID
        {
            public string VT_PKEY { get; set; }
            public string DOCU_TYPE { get; set; }
            public object VCH_DATE { get; set; }
            public object ENT_NO { get; set; }
            public string BOOK_CODE { get; set; }
            public string AC_CODE { get; set; }
            public string LEDGERNAME { get; set; }
            public string GROUP { get; set; }
            public string ADDRESS { get; set; }
            public string CITY { get; set; }
            public string COUNTRY { get; set; }
            public string STATE { get; set; }
            public string PIN { get; set; }
            public string PAN_NO { get; set; }
            public string GSTIN_NO { get; set; }
            public string AC_TYPE { get; set; }
            public string BILL_NO { get; set; }
            public object BILL_DATE { get; set; }
            public string NARRATION { get; set; }
            public object AMT { get; set; }
            public object QNTY { get; set; }
            public string OAC_CODE { get; set; }
            public string CD_FLAG { get; set; }
            public object VEXCH_RATE { get; set; }
            public string VT_SER_PREFIX { get; set; }
            public string VT_SER_SUFFIX { get; set; }

            public string PLANT { get; set; }
            public List<Reference> REFERENCE { get; set; } = new List<Reference>();
        }
        public class Reference
        {
            public string REF_NO { get; set; }
            public string DOC_PKEY { get; set; }
            public string AC_CODE { get; set; }
            public string ENT_NO { get; set; }
            public string DOCU_TYPE { get; set; }
            public object AMT { get; set; }
            public string DC_FLAG { get; set; }
            public object DUE_DATE { get; set; }
            public object ADJ_AMT { get; set; }
            public string AN_FLAG { get; set; }
            public object REF_DATE { get; set; }
            public string PLANT { get; set; }
            public object CR_DAYS { get; set; }
            public string DOCREF_NO { get; set; }
            public string ORD_PKEY { get; set; }
            public object FREXCH_RATE { get; set; }
            public string SER_PREFIX { get; set; }
            public string PROJ_CODE { get; set; }
        }
        public class Root
        {
            public List<DOC_DETAIL> DOC_DETAIL { get; set; }
        }
        public class Service
        {
            public string STAR_NO { get; set; }
            public string SERVICE_CODE { get; set; }
            public string SERVICE_NAME { get; set; }
        }
        public class data
        {

            public List<DOC_DETAIL> DOC_DETAIL { get; set; }



        }
        public class VoucherDetail
        {
            public TOP TOP { get; set; }
        }
    }
}