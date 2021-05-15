using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Test.Services;

namespace Test.Controllers
{
    public class BaseController : Controller
    {

        public Dictionary<string, string> xmlStatus = new Dictionary<string, string>();
        public Dictionary<string, string> csvStatus = new Dictionary<string, string>();

        public BaseController()
        {
            //Load the valid transaction tatus
            MasterService svc = new MasterService();
            List<dynamic> modStatus = svc.GetLookup("CSV");
            for (int i = 0; i < modStatus.Count; i++) csvStatus.Add(modStatus[i].LOOKUP_NAME.ToUpper(), modStatus[i].LOOKUP_CODE);

            modStatus = svc.GetLookup("XML");
            for (int i = 0; i < modStatus.Count; i++) xmlStatus.Add(modStatus[i].LOOKUP_NAME.ToUpper(), modStatus[i].LOOKUP_CODE);
        }



        #region Import File Helper
        public int LimitFileSize
        {
            get
            {
                try
                {
                    IConfiguration configuration = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .Build();

                    int limitFileByte = int.Parse(configuration["LimitFileByte"]);

                    return limitFileByte;
                }
                catch
                {
                    return 1048576;
                }
            }

        }

        public ImportFileType CheckFileType(string fileName)
        {
            if (Path.GetExtension(fileName.ToLower()).IndexOf(".xml") > -1)
                return ImportFileType.XML;
            else if (Path.GetExtension(fileName.ToLower()).IndexOf(".csv") > -1)
                return ImportFileType.CSV;
            else return ImportFileType.Invalid;
        }

        public string ReadFileToString(IFormFile sender)
        {
            var result = new StringBuilder();
            using (var reader = new StreamReader(sender.OpenReadStream()))
            {
                while (reader.Peek() >= 0)
                    result.AppendLine(reader.ReadLine());
            }
            return result.ToString();
        }

        public string SaveToTemp(string rootPath, IFormFile sender)
        {
            if (!rootPath.EndsWith("\\")) rootPath += "\\";
            //Path to Save
            if (!rootPath.ToLower().Contains("wwwroot"))
                rootPath += "wwwroot\\";
            rootPath += "files\\";

            //Rename file for prevent duplicate file name.
            string fileName = sender.FileName;
            string extension = Path.GetExtension(fileName);
            if (extension[0] != '.') extension = "." + extension;

            fileName = fileName.Replace(extension, "") + DateTime.Now.ToString("yyyyMMddHHmmssfff") + extension;
            using (var stream = new FileStream(rootPath+ fileName, FileMode.Create))
            {
                sender.CopyToAsync(stream);
            }

            return fileName;
        }
        #endregion
    }
}
