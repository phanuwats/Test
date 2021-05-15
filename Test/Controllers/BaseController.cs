using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Test.Services;

namespace Test.Controllers
{
    public class BaseController : Controller
    {
        /// <summary>
        /// Get The limit size of file upload
        /// </summary>
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

        public Dictionary<string, string> xmlStatus = new Dictionary<string, string>();
        public Dictionary<string, string> csvStatus = new Dictionary<string, string>();

        public BaseController()
        {
            MasterService svc = new MasterService();
            List<dynamic> modStatus = svc.GetLookup("CSV");
            for (int i = 0; i < modStatus.Count; i++) csvStatus.Add(modStatus[i].LOOKUP_NAME, modStatus[i].LOOKUP_CODE);

            modStatus = svc.GetLookup("XML");
            for (int i = 0; i < modStatus.Count; i++) xmlStatus.Add(modStatus[i].LOOKUP_NAME, modStatus[i].LOOKUP_CODE);
        }

    }
}
