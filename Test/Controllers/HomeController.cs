using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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



        [Route("Home/ImportFile/")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ImportFile(IFormFile file)
        {
            PageResultModel mod = new PageResultModel();
            string strError = "";

            string fileName = file.FileName;

            List<TransactionModel> trnRead = new List<TransactionModel>();
            try
            {
                //Check Valid File
                ImportFileType fileType;
                fileType = this.CheckFileType(fileName);
                if (fileType == ImportFileType.Invalid)
                {
                    strError = "Invalid file format. (allow only csv, xml)";
                }
                else if (file.Length > this.LimitFileSize)
                {
                    strError = "File is larger than 1 mb.";
                }
                else
                {
                    //Valid file Save to Temp for check later
                    fileName = base.SaveToTemp(this.env.ContentRootPath, file);

                    string fileContent = this.ReadFileToString(file);

                    switch(fileType)
                    {
                        case ImportFileType.CSV:
                            trnRead = new CSVReader().GetTransaction(fileContent, out strError);
                            break;
                        case ImportFileType.XML:
                            trnRead = new XMLReader().GetTransaction(fileContent, out strError);
                            break;
                    }
                }


               
                if (strError.Length == 0)
                {
                    string logID = new MasterService().AddImportLog(fileName, "", "1", "System");

                    svcTrn.AddTransaction(logID, trnRead);

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

                    new MasterService().AddImportLog(fileName, strError,  "0", "System");
                }
                
            }
            catch(Exception ex)
            {
                mod.statusCode = "500";
                mod.statusMessage = "Internal Server Error";
                new MasterService().AddImportLog(fileName, ex.Message,  "0", "System");
            }
           

            return View("~/Views/Home/Index.cshtml", mod);
        }

   
    }
}
