using Microsoft.Extensions.Configuration;
using System.IO;

namespace Test.Services
{
    public class ServiceBase
    {
        protected string con;
        protected bool isSuccessLog = false;
        protected bool isFailLog = false;

        public ServiceBase()
        {
            IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

            this.con = configuration["ConnectionString:DefaultConnection"];

            this.isSuccessLog = (configuration["DBLog:LogSuccess"] == "1" );
            this.isFailLog = (configuration["DBLog:LogFail"] == "1");
        }
    }
}
