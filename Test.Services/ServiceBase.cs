using Microsoft.Extensions.Configuration;
using System.IO;

namespace Test.Services
{
    public class ServiceBase
    {
        protected string con;

        public ServiceBase()
        {
            IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .Build();

            this.con = configuration["ConnectionString:DefaultConnection"];
        }
    }
}
