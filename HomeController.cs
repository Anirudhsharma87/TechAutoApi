using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Diagnostics;
using System.Web.UI.WebControls;
using RestSharp;
using System.Data;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.Owin.BuilderProperties;
using Newtonsoft.Json.Linq;
using HtmlAgilityPack;
using MongoDB.Bson;

namespace TechAuto.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = "Data Source=localhost;Initial Catalog=TSSWU12345;Persist Security Info=True;User ID=sa;Password=sa;TrustServerCertificate=True;";

        private static readonly HttpClient client = new HttpClient();
        //string clientId = "your_client_id";
        //string clientSecret = "your_client_secret";
        //string username = "your_username";
        //string password = "your_password";

        // string clientId = "60a1137bbf3b4d21b885939acf6c5271";
        // string clientSecret = "aa95fbbc815c438e9e68fb1c0ac3eee3";
        // string username = "QWSDT0255";
        // string password = "TEAT@#trid0012";


        //
        string _BillPkey = "";
        string _ServerIP = "";
        string _DBName = "";
        string _SQlID = "";
        string _SQLPass = "";
        string _Pdf64 = "";
        string inputdata = "";
        //
        //string BillPkey, string ServerIP,
        //    string DBName, string SQlID, string SQLPass, string Pdf64

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ContentResult> PostTafeInvoice(string clientId, string clientSecret, string username, string password, string BillPkey, string ServerIP, string DBName, string SQlID, string SQLPass)
        {
            string Pdf64 = "";
            Request.InputStream.Position = 0;
            string requestBody = "ram";
            using (var reader = new StreamReader(HttpContext.Request.InputStream, Encoding.UTF8))
            {
                requestBody = await reader.ReadToEndAsync();
            }
            var json = JsonConvert.DeserializeObject<Dictionary<string, string>>(requestBody);
         
            Pdf64 = json["Pdf64"];
            _BillPkey = BillPkey;
            _ServerIP = ServerIP;
            _DBName = DBName;
            _SQlID = SQlID;
            _SQLPass = SQLPass;
            _Pdf64 = Pdf64;
            // Step 1: Authenticate and get access token
            string accessToken = await GetOAuthToken(clientId, clientSecret, username, password);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(accessToken);
           // Print_Log("complete token :" + accessToken);
            var accessTokenNode = doc.DocumentNode
                .SelectSingleNode("//td[b[text()='access_token']]/following-sibling::td");
            string accessToken1 = "";
            if (accessTokenNode != null)
            {
                accessToken1 = accessTokenNode.InnerText;
                Console.WriteLine("Access Token: " + accessToken);
            }
            else
            {
                Console.WriteLine("Access Token not found");
            }
          //  Print_Log("orginal token :" + accessToken1);

            if (string.IsNullOrEmpty(accessToken1))
            {
                Console.WriteLine("Failed to obtain access token");
                return Content("Failed to obtain access token");
            }
            //string invoiceResponse = "--";
            // Step 2: Upload an invoice
            string invoiceResponse = await UploadInvoice(accessToken1);
            Console.WriteLine("Invoice Upload Response: " + invoiceResponse);
            var inputdata = "BillPkey:" + BillPkey + "\nServerIP:" + ServerIP + "\n DBName:" + DBName + "\n SQlID:" + SQlID + "\n SQLPass:" + SQLPass + "\n" + requestBody + "\n";

            Print_Log("Complete Response :" + accessToken1+ invoiceResponse);
            return Content(" \n Bearer Token \n \n" + accessToken1 + invoiceResponse);
            // return Content(accessToken1);
        }



        private async Task<string> GetOAuthToken(string clientId, string clientSecret, string username, string password)
        {
            var client = new HttpClient();

            //test
             var request = new HttpRequestMessage(HttpMethod.Post, "https://qasupplws01.tafechannel.com/tfexVI624?client_id=60a1137bbf3b4d21b885939acf6c5271&client_secret=aa95fbbc815c438e9e68fb1c0ac3eee3");
             request.Headers.Add("Authorization", "Basic UVdTRFQwMjU1OlRFQVRAI3RyaWQwMDEy");
             request.Headers.Add("Cookie", "cookiesession1=678A8C7D0D9A0051568779A7DCDC17BD");

            ///prod
             //var request = new HttpRequestMessage(HttpMethod.Post, "https://digiwayin0001.tafechannel.com/tfexVI624?client_id=9fb06b7cc02a48d3a0b0a194cafcb3e2&client_secret=1b4fbee2366a4b9085ec3207a7e86ae4");
             //request.Headers.Add("Authorization", "Basic RFQwNDMwUFdTOkM0bkYjanQlRW1wNWRW");
             //request.Headers.Add("Cookie", "cookiesession1=678A8C862CB270A74683F58CF421EC7D");

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            Console.WriteLine(await response.Content.ReadAsStringAsync());



            var result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {

                dynamic jsonResponse = JsonConvert.SerializeObject(result);
                // return jsonResponse.accessToken.access_token;
            }
            return result;
        }
        private async Task<string> UploadInvoice(string accessToken)
        {



            //string query = "INSERT INTO Invoice (VendorCode,VendorGST,DestinationGST,DeliveryLocationCode,InvoiceNumber,InvoiceDate,PONumber,TotalInvoiceAmount,CGSTTotalAmount,SGSTTotalAmount,IGSTTotalAmount,TCS) VALUES(@VendorCode,@VendorGST,@DestinationGST,@DeliveryLocationCode,@InvoiceNumber,@InvoiceDate,@PONumber,@TotalInvoiceAmount,@CGSTTotalAmount,@SGSTTotalAmount,@IGSTTotalAmount,@TCS)";
             var url = "https://qasupplws01.tafechannel.com/tfexVI625";

            //prd
          //  var url = "https://digiwayin0001.tafechannel.com/tfexVI625";

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            DataTable dtinvoiceS = new DataTable(); // //let 5 invoice earch having 10 items with 100 columns
            string sql = "SELECT BT.TAED_AMT2,bt.PLANT ,FP.PLANT_GSTIN_NO,bm.CPROD_CODE,bt.bill_pkey,VCODE, AC_CODE,GSTIN_NO,VCH_SER,VCH_NO, CONS_GSTIN_NO,CONS_STATE,vch_date,TCS_AMT,PO_DELIVERY, substring(bt.bill_pkey,9,10) as bill_no, bt.VCH_DATE, bm.CPO_NO,bt.TBILL_AMT, bt.TAED_AMT,BT.TBED_AMT, BT.TAED_AMT2,  BM.ITEM_CODE, BM.TAR_NO, BM.QNTY, BM.UNIT, BM.RATE, BM.AMOUNT, BM.AED_AMT, BM.BED_AMT, BM.AED_AMT2, BM.PKG_AMT, BM.FRT_AMT from tbilltop bt inner join tbillmid bm on bt.BILL_PKEY=bm.BILL_PKEY inner join FPLANTMAS FP on bt.PLANT=FP.PLANT where bt.bill_pkey='" + _BillPkey + "'";
            dtinvoiceS = GetData(sql);
            if (dtinvoiceS.Rows.Count > 0) { }
            else
            {
                return "Data not found in table";
            }

            DataView dvInv = dtinvoiceS.DefaultView;
            DataTable dtInv = dvInv.ToTable(true, "Bill_PKEY");//// 5row 1 column make a table with uniq billpkey column only one

            InvoiceRequest invoiceRequest = new InvoiceRequest();
            Document invDocument = new Document();
            List<Datum> invDataS = new List<Datum>();

            try
            {
                foreach (DataRow drinv in dtInv.Rows) /// Loop for Multiple Invoices
                {
                    Datum invData = new Datum();
                    invData.File = _Pdf64;
                    List<Invoice> invoiceList = new List<Invoice>();
                    Invoice inv = new Invoice();
                    DataTable CurrInvoice = dtinvoiceS.AsEnumerable().
                        Where(w => w.Field<string>("BILL_PKEY").Trim() == drinv["BILL_PKEY"].ToString().Trim())
                        .CopyToDataTable();

                    ///10rows 100column for current invoice only

                    //top start  read values from first row only
                    inv.VendorCode = CurrInvoice.Rows[0]["VCODE"].ToString();
                    inv.VendorGST = CurrInvoice.Rows[0]["PLANT_GSTIN_NO"].ToString();
                    inv.DestinationGST = CurrInvoice.Rows[0]["CONS_GSTIN_NO"].ToString();
                    inv.DeliveryLocationCode = CurrInvoice.Rows[0]["PO_DELIVERY"].ToString();
                    //inv.InvoiceNumber = CurrInvoice.Rows[0]["bill_pkey"].ToString();
                    inv.InvoiceNumber = CurrInvoice.Rows[0]["VCH_SER"].ToString() + "-" + CurrInvoice.Rows[0]["VCH_NO"].ToString().PadLeft(6, '0');
                    inv.InvoiceDate = DateTime.Parse(CurrInvoice.Rows[0]["vch_date"].ToString()).ToString("dd.MM.yyyy");
                    inv.PONumber = CurrInvoice.Rows[0]["CPO_NO"].ToString();
                    inv.TotalInvoiceAmount = string.Format("{0:F2}", CurrInvoice.Rows[0]["TBILL_AMT"]);
                    inv.CGSTTotalAmount = string.Format("{0:F2}", CurrInvoice.Rows[0]["TBED_AMT"]);
                    inv.SGSTTotalAmount = string.Format("{0:F2}", CurrInvoice.Rows[0]["TAED_AMT"]);
                    inv.IGSTTotalAmount = string.Format("{0:F2}", CurrInvoice.Rows[0]["TAED_AMT2"]);
                    inv.TCS = string.Format("{0:F2}", CurrInvoice.Rows[0]["TCS_AMT"]);
                    //top end 
                    List<LineItem> itemList = new List<LineItem>();
                    foreach (DataRow dr in CurrInvoice.Rows)// loogin current invoice for item details
                    {
                        LineItem item = new LineItem();
                        item.MaterialCode = dr["CPROD_CODE"].ToString();
                        item.HSN_SAC = dr["TAR_NO"].ToString();
                        item.Quantity = dr["QNTY"].ToString();
                        item.UOM = dr["UNIT"].ToString();
                        item.Rate = string.Format("{0:F2}", dr["RATE"]);
                        item.BasicValue = string.Format("{0:F2}", dr["AMOUNT"]);
                        item.CGSTAmount = string.Format("{0:F2}", dr["BED_AMT"]);
                        item.SGSTAmount = string.Format("{0:F2}", dr["AED_AMT"]);
                        item.IGSTAmount = string.Format("{0:F2}", dr["TAED_AMT2"]);
                        item.PackingCharge = string.Format("{0:F2}", dr["PKG_AMT"]);
                        item.FreightCharge = string.Format("{0:F2}", dr["FRT_AMT"]);

                        itemList.Add(item);/// adding item to invoice
                    }
                    inv.LineItems = itemList; //assiging item list to inv
                    var itemList1= JsonConvert.SerializeObject(itemList);
                    Print_Log("Invoice :" + itemList1);
                    invoiceList.Add(inv);///adding inv to invoicelist
                    invData.Invoice = invoiceList;/// assignng invoicelist to invData
                    invDataS.Add(invData);//adding invoice data to invoice data list

                }
            }
            catch (Exception ex)
            {
                //var LineNumber = new StackTrace(ex, true).GetFrame(0).GetFileLineNumber();
//Print_Log(GetError(ex));

            }
            invDocument.data = invDataS;/// assiging invoice data list to inv Document
            invoiceRequest.document = invDocument;/// assigning inv document to request;


            var json = JsonConvert.SerializeObject(invoiceRequest);/// converting requist to json
            var data = new StringContent(json, Encoding.UTF8, "application/json");
           
         //   Print_Log("json :" + json);

            var response = await client.PostAsync(url, data);///posting json data to httpclient
            var result = await response.Content.ReadAsStringAsync();// reading response
                                                                    // var result1 =  "\r\nHeader:\r\nContent Type : application/json \n"+ "\n\n\n Body:---\n\n" + json  + "\n Result:\r\nResponse: 200 OK" + result  ;
            var result1 = result;
            return result1;
        }
        public string GetError(Exception exception)
        {
            int lineno = 0;
            int i = 0;
            string fName = "";
            StackFrame fram;
            try
            {
                do
                {
                    fram = new System.Diagnostics.StackTrace(exception, true).GetFrame(i);
                    lineno = fram.GetFileLineNumber();
                    i++;
                }
                while (lineno < 1);
                fName = fram.GetFileName().Split('\\').Last();
            }
            catch (Exception err)
            {
               // Print_Log(GetError(err));
                return exception.Message;
            }
            return exception.Message.ToString().Replace("'", "") + " at Line no " + lineno + " in File " + fName;
        }


        private async Task<string> CancelInvoice(string accessToken, string keyId, string vendorCode, string invoiceNumber, string invoiceDate, string poNumber, string deliveryLocationCode, string cancellationReason)
        {
            var url = "https://qasupplws01.tafechannel.com/tfexVI626";
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var cancelRequest = new
            {
                invoiceCancellationReq = new
                {
                    keyId = keyId,
                    vendorCode = vendorCode,
                    invoiceNumber = invoiceNumber,
                    invoiceDate = invoiceDate,
                    PONumber = poNumber,
                    deliveryLocationCode = deliveryLocationCode,
                    cancellationReason = cancellationReason
                }
            };

            var json = JsonConvert.SerializeObject(cancelRequest);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(url, data);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }




        public DataTable GetData(string query)
        {
            DataTable dataTable = new DataTable();

            //        string connectionString = "Data Source=localhost;Initial Catalog=A1FNCE2425;" +
            //"Persist Security Info=True;User ID=sa;Password=sa;" +
            //"TrustServerCertificate=True;";

            string connectionString = "Data Source=" + _ServerIP + ";Initial Catalog=" + _DBName + ";" +
 "Persist Security Info=True;User ID=" + _SQlID + ";Password=" + _SQLPass + ";" +
 "TrustServerCertificate=True;";

            using (SqlConnection connection = new SqlConnection(connectionString))
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
                    // Handle exceptions (log them, rethrow them, or handle them as needed)
                    Console.WriteLine("An error occurred: " + ex.Message);
                  //  Print_Log(GetError(ex));
                }
            }

            return dataTable;
        }


        private void LogApiResponse(string responseCode, string responseMessage, string error, string requestParameters)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string query = "INSERT INTO APILOG (update_date,update_time, response_code, response_desc, error, request_parameters) " +
                                   "VALUES (@update_date, @update_time, @responseCode, @responseMessage, @error, @RequestParameters)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@update_date", DateTime.Now.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@update_time", DateTime.Now.ToString("HH:mm:ss"));
                        cmd.Parameters.AddWithValue("@responseCode", responseCode);
                        cmd.Parameters.AddWithValue("@responseMessage", responseMessage ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@error", error ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@RequestParameters", requestParameters);
                        cmd.ExecuteNonQuery();
                    }
                    conn.Close();
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                throw;
            }
        }

        public async Task<ContentResult> cancelTafeInvoice(string BillPkey)
        {
            string clientId = "";
            string clientSecret = "";
            string username = "";
            string password = "";
            // Step 1: Authenticate and get access token
            string accessToken = await GetOAuthToken(clientId, clientSecret, username, password);
            if (string.IsNullOrEmpty(accessToken))
            {
                //Console.WriteLine("Failed to obtain access token");
                return Content("Failed to obtain access token");
            }



            // Step 3: Cancel an invoice (using a sample keyId, you need to replace with actual one)
            string cancelResponse = await CancelInvoice(accessToken, "RA270419$1010012704$647536525317$23102023", "RA270419", "647536525317", "23.10.2023", "1010012704", "1100", "Invoice IRN cancelled");
            Console.WriteLine("Invoice Cancellation Response: " + cancelResponse);

            return Content("Done");
        }
        public string GetSolutionBasePath()
        {
            // Start from the current directory
            string currentDirectory = HttpContext.Server.MapPath("");

            // Traverse up the directory tree to find the .sln file
            DirectoryInfo directoryInfo = new DirectoryInfo(currentDirectory);

            while (directoryInfo != null && !DirectoryContainsSolutionFile(directoryInfo))
            {
                directoryInfo = directoryInfo.Parent;
            }

            if (directoryInfo == null)
            {
                throw new FileNotFoundException("Solution file (.sln) not found in any parent directory.");
            }

            return directoryInfo.FullName;
        }
        public bool DirectoryContainsSolutionFile(DirectoryInfo directoryInfo)
        {
            // Check if the directory contains a .sln file
            return directoryInfo.GetFiles("*.sln").Length > 0;
        }
        public void Print_Log(string msg)
        {
            try
            {
                //string ppath = System.IO.Path.Combine(, "MYLOGS.txt");

                var paths = HttpContext.Server.MapPath("").Split('\\');
                var home = paths.Take(paths.Count() - 1);

                string ppath = string.Join("\\", home) + "\\Mylog.txt";             //string ppath = GetSolutionBasePath(); ;
                if (System.IO.File.Exists(ppath))
                {
                    StreamWriter w = System.IO.File.AppendText(ppath);
                    w.WriteLine(msg.ToString() + "-->" + DateTime.Now.ToString("ddMMyyyy hh:mm:ss tt"));
                    w.WriteLine("=====================================================================");
                    w.Flush();
                    w.Close();
                }
                else
                {
                    StreamWriter w = new StreamWriter(ppath, true);
                    w.WriteLine(msg.ToString() + "-->" + DateTime.Now.ToString("ddMMyyyy hh:mm:ss tt"));
                    w.WriteLine("=====================================================================");
                    w.Flush();
                    w.Close();
                }
            }
            catch { }
        }











        public class InvoiceRequest
        {
            public Document document { get; set; }
        }
        public class Document
        {
            public List<Datum> data { get; set; }
        }
        public class Datum
        {
            public string File { get; set; }
            public List<Invoice> Invoice { get; set; }
        }
        public class Invoice
        {
            [JsonProperty("Vendor Code")]
            public string VendorCode { get; set; }

            [JsonProperty("Vendor GST")]
            public string VendorGST { get; set; }

            [JsonProperty("Destination GST")]
            public string DestinationGST { get; set; }

            [JsonProperty("Delivery Location Code")]
            public string DeliveryLocationCode { get; set; }

            [JsonProperty("Invoice Number")]
            public string InvoiceNumber { get; set; }

            [JsonProperty("Invoice Date")]
            public string InvoiceDate { get; set; }

            [JsonProperty("PO Number")]
            public string PONumber { get; set; }

            [JsonProperty("Total Invoice Amount")]
            public string TotalInvoiceAmount { get; set; }

            [JsonProperty("CGST Total Amount")]
            public string CGSTTotalAmount { get; set; }

            [JsonProperty("SGST Total Amount")]
            public string SGSTTotalAmount { get; set; }

            [JsonProperty("IGST Total Amount")]
            public string IGSTTotalAmount { get; set; }

            public string TCS { get; set; }

            public List<LineItem> LineItems { get; set; }
        }
        public class LineItem
        {
            [JsonProperty("Material Code")]
            public string MaterialCode { get; set; }

            [JsonProperty("HSN/SAC")]
            public string HSN_SAC { get; set; }

            public string Quantity { get; set; }
            public string UOM { get; set; }
            public string Rate { get; set; }

            [JsonProperty("Basic Value")]
            public string BasicValue { get; set; }

            [JsonProperty("CGST Amount")]
            public string CGSTAmount { get; set; }

            [JsonProperty("SGST Amount")]
            public string SGSTAmount { get; set; }

            [JsonProperty("IGST Amount")]
            public string IGSTAmount { get; set; }

            [JsonProperty("Packing Charge")]
            public string PackingCharge { get; set; }

            [JsonProperty("Freight Charge")]
            public string FreightCharge { get; set; }
        }

    }
}
