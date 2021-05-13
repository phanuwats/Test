using System.Collections.Generic;
using System.Data.SqlClient;

using Dapper;

using Test.Models;

namespace Test.Services
{
    public class TransactionService : ServiceBase
    {
        public List<TransactionResultModel> GetTransaction(string currencyCode, string startTrnDate, string endTrnDate, string status)
        {
            using (var c = new SqlConnection(this.con))
            {
                var p = new DynamicParameters();
                if (currencyCode != null && currencyCode.Trim().Length > 0) p.Add("@CUR_CODE", currencyCode);
                if (startTrnDate != null && startTrnDate.Trim().Length > 0) p.Add("@S_TRN_DATE", startTrnDate);
                if (endTrnDate != null && endTrnDate.Trim().Length > 0) p.Add("@E_TRN_DATE", endTrnDate);
                if (status != null && status.Trim().Length > 0) p.Add("@STATUS", status);

                return (List<TransactionResultModel>)c.Query<TransactionResultModel>("SP_TRN_TRANSACTION_SELECT", p,
                    commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60);
            }
        }

        public int GetTransactionCount(string trnNo)
        {
            using (var c = new SqlConnection(this.con))
            {
                var p = new DynamicParameters();
                p.Add("@TRN_NO", trnNo);

                return c.ExecuteScalar<int>("SP_TRN_TRANSACTION_SELECT_TRNNO_COUNT", p,
                    commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60);
            }
        }

        public void AddTransaction(string trnNo, string amount, string currencyCode, string trnDate, string status, string createBy)
        {
            using (var c = new SqlConnection(this.con))
            {
                var p = new DynamicParameters();
                p.Add("@TRN_NO", trnNo);
                p.Add("@AMOUNT", amount);
                p.Add("@CUR_CODE", currencyCode);
                p.Add("@TRN_DATE", trnDate);
                p.Add("@STATUS", status);
                p.Add("@CREATE_BY", createBy);

                c.Execute("SP_TRN_TRANSACTION_INSERT", p,
                    commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60).ToString();
            }
        }


    }
}
