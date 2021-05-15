using System.Collections.Generic;
using System.Globalization;
using Test.Models;
using Test.Services;

namespace Test
{
    public abstract class BaseImportReader
    {
        public Dictionary<string, string> status = new Dictionary<string, string>();

        TransactionService svcTrn = new TransactionService();

        public abstract List<TransactionModel> GetTransaction(string content, out string errMsg);

        protected string ValidateTransactionID(string source, out string errMsg)
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

        protected string ValidateAmount(string source, out string errMsg)
        {
            errMsg = "";
            source = source.Trim().Replace("\"", "").Replace(",", "");

            decimal result;
            if (!decimal.TryParse(source, out result))
            {
                errMsg = "Invalid Amount, ";
                return "";
            }

            return result.ToString("0.00");
        }

        protected string ValidateCurrencyCode(string source, out string errMsg)
        {
            source = source.Trim().Replace("\"", "").ToUpper();
            errMsg = "";
            source = Utility.ValidateCurrencyCode(source);
            if(source.Trim().Length == 0)
                 errMsg = "Invalid Currency Code, ";

            return source;
        }

        protected abstract string ValidateTransactionDate(string source, out string errmsg);

        protected abstract string ValidateStatus(string source, out string errmsg);

    }
}
