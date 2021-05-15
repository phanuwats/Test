using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment environment)
        {
            _logger = logger;
            this.env = environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





        #region About File Upload
        private string tempPath = "files/";


        [Route("Home/ImportFile/")]
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ImportFile(IFormFile file)
        {
            StringBuilder sbError = new StringBuilder();
            bool isValidFormat = true;
            PageResultModel mod = new PageResultModel();

            //Check Valid File
            if(".csv,.xml".IndexOf(Path.GetExtension(file.FileName.ToLower())) == -1)
            {
                isValidFormat = false;
                sbError.Append("Invalid file format. (allow only csv, xml)");
            }
            else if(file.Length > this.LimitFileSize)
            {
                isValidFormat = false;
                sbError.Append("File is larger than 1 mb.");
            }
            else
            {

            }


            if(sbError.Length == 0)
            {
                mod.statusCode = "200";
                mod.statusMessage = "Success";
            }
            else
            {
                if (isValidFormat)
                {
                    mod.statusCode = "400";
                    mod.statusMessage = "Bad Request";
                }
                else
                {
                    mod.statusCode = "500";
                    mod.statusMessage = "Unknown format";
                }
            }
            new MasterService().AddImportLog(file.FileName, sbError.ToString(), (mod.statusCode == "200" ? "1" : "0"), "System");

            return View("~/Views/Home/Index.cshtml", mod);
        }


        #endregion
    }
}
