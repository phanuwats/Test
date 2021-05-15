using System.Collections.Generic;
using System.Data.SqlClient;

using Dapper;

namespace Test.Services
{
    public class MasterService : ServiceBase
    {
        public List<dynamic> GetLookup(string lookupGroup)
        {
            using (var c = new SqlConnection(this.con))
            {
                var p = new DynamicParameters();
                p.Add("@LOOKUP_GROUP", lookupGroup);

                return (List<dynamic>)c.Query<dynamic>("SP_MAS_LOOKUP_SELECT", p,
                    commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60);
            }
        }


        public string AddImportLog(string fileName, string errorMessage, string status, string createBy)
        {
            if ((status == "1" && this.isSuccessLog) || (status == "0" && this.isFailLog))
            using (var c = new SqlConnection(this.con))
            {
                var p = new DynamicParameters();
                p.Add("@FILE_NAME", fileName);
                p.Add("@ERROR_MESSAGE", errorMessage);
                p.Add("@STATUS", status);
                p.Add("@CREATE_BY", createBy);

                return c.ExecuteScalar("SP_LOG_IMPORT_INSERT", p,
                    commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60).ToString();
            }

            return "-1";
        }

    }
}
