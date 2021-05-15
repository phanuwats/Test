using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using Test.Models;
using Test.Services;

namespace Test.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ILogger<HomeController> _logger;
        private IHostingEnvironment env;
        private TransactionService svcTrn;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            this.env = environment;
            svcTrn = new TransactionService();
        }

        public IActionResult Index()
        {
            //TestValidation();
            return View();
        }





        #region About File Upload
        private string tempPath = "files/";


        [Route("Home/ImportFile/")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ImportFile(IFormFile file)
        {
            StringBuilder sbError = new StringBuilder();
            ImportFileType fileType;
            PageResultModel mod = new PageResultModel();
            string fileName = file.FileName;

            try
            {
                //Check Valid File
                fileType = this.CheckFileType(fileName);
                if (fileType == ImportFileType.Invalid)
                {
                    sbError.Append("Invalid file format. (allow only csv, xml)");
                }
                else if (file.Length > this.LimitFileSize)
                {
                    sbError.Append("File is larger than 1 mb.");
                }
                else
                {
                    //Valid file Save to Temp for check later
                    fileName = base.SaveToTemp(this.env.ContentRootPath, file);

                    string fileContent = this.ReadFileToString(file);

                }


                if (sbError.Length == 0)
                {
                    mod.statusCode = "200";
                    mod.statusMessage = "Success";
                }
                else
                {
                    if (fileType == ImportFileType.Invalid)
                    {
                        mod.statusCode = "500";
                        mod.statusMessage = "Unknown format";
                    }
                    else
                    {
                        mod.statusCode = "400";
                        mod.statusMessage = "Bad Request";
                    }
                }
                new MasterService().AddImportLog(fileName, sbError.ToString(), (mod.statusCode == "200" ? "1" : "0"), "System");
            }
            catch(Exception ex)
            {
                mod.statusCode = "500";
                mod.statusMessage = "Internal Server Error";
                new MasterService().AddImportLog(fileName, ex.Message,  "0", "System");
            }
           

            return View("~/Views/Home/Index.cshtml", mod);
        }



        #region Method for Validate Field
        private void TestValidation()
        {
            string retValue = "", errMsg;
            //Transaction Valid Case
            retValue = ValidateTransactionID("Inv00001", out errMsg);
            retValue = ValidateTransactionID("INV00000000000000000000000000000000000000000000001", out errMsg);
            
            //Transaction Invalid Case
            retValue = ValidateTransactionID("", out errMsg);
            retValue = ValidateTransactionID("trn00000000000000000000000000000000000000000000000000000000000001", out errMsg);
            //---Duplicate
            retValue = ValidateTransactionID("Invtemp", out errMsg);

            //Amount Valid Case
            retValue = ValidateAmount("30", out errMsg);
            retValue = ValidateAmount("40.222", out errMsg);
            retValue = ValidateAmount("1,000.00", out errMsg);

            //Amount Invalid Case
            retValue = ValidateAmount("", out errMsg);
            retValue = ValidateAmount("XXXX", out errMsg);

            //Currency Code Valid Case
            retValue = ValidateCurrencyCode("USD", out errMsg);
            retValue = ValidateCurrencyCode("AUD", out errMsg);

            //Currency Code Invalid Case
            retValue = ValidateCurrencyCode("", out errMsg);
            retValue = ValidateCurrencyCode("BBC", out errMsg);
            retValue = ValidateCurrencyCode("USDT", out errMsg);

            //Transaction Date CSV Valid Case
            retValue = ValidateCSVTransactionDate("20/02/2019 12:33:16", out errMsg);
            retValue = ValidateCSVTransactionDate("21/02/2019 02:04:59", out errMsg);

            //Transaction Date CSV Invalid Case
            retValue = ValidateCSVTransactionDate("", out errMsg);
            retValue = ValidateCSVTransactionDate("xxx", out errMsg);
            retValue = ValidateCSVTransactionDate("2019-01-24T16:09:15", out errMsg);
            retValue = ValidateCSVTransactionDate("31/01/2020", out errMsg);
            retValue = ValidateCSVTransactionDate("31/12/", out errMsg);
            retValue = ValidateCSVTransactionDate("01:01:01", out errMsg);
            retValue = ValidateCSVTransactionDate("21/02/2019 02:04", out errMsg);
            retValue = ValidateCSVTransactionDate("33/01/2020 01:01:01", out errMsg);
            retValue = ValidateCSVTransactionDate("01/13/2020 01:01:01", out errMsg);
            retValue = ValidateCSVTransactionDate("01/13/99999 01:01:01", out errMsg);
            retValue = ValidateCSVTransactionDate("01/13/2020 25:01:01", out errMsg);

            //Transaction Date XML Valid Case
            retValue = ValidateXMLTransactionDate("2019-01-23T13:45:10", out errMsg);
            retValue = ValidateXMLTransactionDate("2019-01-24T16:09:15", out errMsg);

            //Transaction Date XML Invalid Case
            retValue = ValidateXMLTransactionDate("", out errMsg);
            retValue = ValidateXMLTransactionDate("xxx", out errMsg);
            retValue = ValidateXMLTransactionDate("2020-01-01", out errMsg);
            retValue = ValidateXMLTransactionDate("01:01:01", out errMsg);
            retValue = ValidateXMLTransactionDate("2020-01-33T01:01:01", out errMsg);
            retValue = ValidateXMLTransactionDate("99999-01-01T01:01:01", out errMsg);
            retValue = ValidateXMLTransactionDate("-01-01T01:01:01", out errMsg);
            retValue = ValidateXMLTransactionDate("-01-01T25:01:01", out errMsg);
            retValue = ValidateXMLTransactionDate("-01-01 25:01:01", out errMsg);


            //Status CSV Valid Case
            retValue = ValidateCSVStatus("Approved", out errMsg);
            retValue = ValidateCSVStatus("approved", out errMsg);
            retValue = ValidateCSVStatus("APPROVED", out errMsg);
            retValue = ValidateCSVStatus("Failed", out errMsg);
            retValue = ValidateCSVStatus("Finished", out errMsg);

            //Status CSV Invalid Case
            retValue = ValidateCSVStatus("", out errMsg);
            retValue = ValidateCSVStatus("A", out errMsg);
            retValue = ValidateCSVStatus("XXXX", out errMsg);

            //Status XML Valid Case
            retValue = ValidateXMLStatus("Approved", out errMsg);
            retValue = ValidateXMLStatus("approved", out errMsg);
            retValue = ValidateXMLStatus("APPROVED", out errMsg);
            retValue = ValidateXMLStatus("Rejected", out errMsg);
            retValue = ValidateXMLStatus("Done", out errMsg);

            //Status XML Invalid Case
            retValue = ValidateXMLStatus("", out errMsg);
            retValue = ValidateXMLStatus("A", out errMsg);
            retValue = ValidateXMLStatus("XXXX", out errMsg);
        }


        private string ValidateTransactionID(string source, out string errMsg)
        {
            errMsg = "";
            source = source.Trim().Replace("\"", "");

            if (source.Length == 0 || source.Length > 50)
            {
                errMsg = "Invalid Transaction ID, ";
                return "";
            }
            else if (svcTrn.GetTransactionCount(source) > 0)
            { 
                errMsg = "Duplicate Transaction ID, ";
                return "";
            }

            return source;
        }

        private string ValidateAmount(string source, out string errMsg)
        {
            errMsg = "";
            source = source.Trim().Replace("\"", "").Replace(",","");

            decimal result;
            if (!decimal.TryParse(source, out result))
            {
                errMsg = "Invalid Amount, ";
                return "";
            }

            return result.ToString("0.00");
        }

        private string ValidateCurrencyCode(string source, out string errMsg)
        {
            source = source.Trim().Replace("\"", "").ToUpper();
            errMsg = "";
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo ci in cultures)
            {
                RegionInfo ri = new RegionInfo(ci.LCID);
                if (ri.ISOCurrencySymbol.ToUpper() == source)
                {
                    return source;
                }
            }

            errMsg = "Invalid Currency Code, ";
            return "";
        }

        private string ValidateCSVTransactionDate(string source, out string errMsg)
        {
            errMsg = "";
            source = source.Trim().Replace("\"", "");

            DateTime dResult;
            if (!DateTime.TryParseExact(source, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"),
                    DateTimeStyles.None, out dResult))
            {
                errMsg = "Invalid Transaction Date (format dd/MM/yyyy HH:mm:ss),";
                return "";
            }
            else
                return dResult.ToString("dd/MM/yyyy HH:mm:ss");
        }

        private string ValidateXMLTransactionDate(string source, out string errMsg)
        {
            errMsg = "";
            source = source.Trim().Replace("\"", "");

            DateTime dResult;
            if (!DateTime.TryParseExact(source, "yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US"),
                    DateTimeStyles.None, out dResult))
            {
                errMsg = "Invalid Transaction Date (format yyyy-MM-ddTHH:mm:ss),";
                return "";
            }
            else
                return dResult.ToString("dd/MM/yyyy HH:mm:ss");
        }


        private string ValidateCSVStatus(string source, out string errMsg)
        {
            string retValue = source;
            errMsg = "";
            if (!base.csvStatus.TryGetValue(source.ToUpper(), out retValue))
            {
                errMsg = "Invalid Status, ";
                retValue = "";
            }

            return retValue;
        }

        private string ValidateXMLStatus(string source, out string errMsg)
        {
            string retValue = source;
            errMsg = "";
            if (!base.xmlStatus.TryGetValue(source.ToUpper(), out retValue))
            {
                errMsg = "Invalid Status, ";
                retValue = "";
            }
            return retValue;
        }

      

        #endregion

        #endregion
    }
}
