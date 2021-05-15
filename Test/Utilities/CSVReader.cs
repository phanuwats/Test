using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Test.Models;
using Test.Services;

namespace Test
{
    public class CSVReader : BaseImportReader
    {
        public CSVReader()
        {
            MasterService svc = new MasterService();
            List<dynamic> modStatus = modStatus = svc.GetLookup("CSV");
            for (int i = 0; i < modStatus.Count; i++) this.status.Add(modStatus[i].LOOKUP_NAME.ToUpper(), modStatus[i].LOOKUP_CODE);
            //TestValidation();
        }

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
            retValue = ValidateTransactionDate("20/02/2019 12:33:16", out errMsg);
            retValue = ValidateTransactionDate("21/02/2019 02:04:59", out errMsg);

            //Transaction Date CSV Invalid Case
            retValue = ValidateTransactionDate("", out errMsg);
            retValue = ValidateTransactionDate("xxx", out errMsg);
            retValue = ValidateTransactionDate("2019-01-24T16:09:15", out errMsg);
            retValue = ValidateTransactionDate("31/01/2020", out errMsg);
            retValue = ValidateTransactionDate("31/12/", out errMsg);
            retValue = ValidateTransactionDate("01:01:01", out errMsg);
            retValue = ValidateTransactionDate("21/02/2019 02:04", out errMsg);
            retValue = ValidateTransactionDate("33/01/2020 01:01:01", out errMsg);
            retValue = ValidateTransactionDate("01/13/2020 01:01:01", out errMsg);
            retValue = ValidateTransactionDate("01/13/99999 01:01:01", out errMsg);
            retValue = ValidateTransactionDate("01/13/2020 25:01:01", out errMsg);

            //Status CSV Valid Case
            retValue = ValidateStatus("Approved", out errMsg);
            retValue = ValidateStatus("approved", out errMsg);
            retValue = ValidateStatus("APPROVED", out errMsg);
            retValue = ValidateStatus("Failed", out errMsg);
            retValue = ValidateStatus("Finished", out errMsg);

            //Status CSV Invalid Case
            retValue = ValidateStatus("", out errMsg);
            retValue = ValidateStatus("A", out errMsg);
            retValue = ValidateStatus("XXXX", out errMsg);
        }

        public override List<TransactionModel> GetTransaction(string content, out string errMsg)
        {
            StringBuilder sbErr = new StringBuilder();
            List<TransactionModel> mod = new List<TransactionModel>();

            if (content.Trim().Length > 0)
            {
                string[] lines = content.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] fields = lines[i].Trim().Split(new string[] { "\",\"" }, StringSplitOptions.RemoveEmptyEntries);
                    string rowError = "";
                    if (fields.Length == 5)
                    {
                        string trnID, amount, curCode, trnDate, status;
                        string fieldErr;

                        trnID = ValidateTransactionID(fields[0], out fieldErr);
                        if (fieldErr.Length > 0) rowError = fieldErr;

                        amount = ValidateAmount(fields[1], out fieldErr);
                        if (fieldErr.Length > 0) rowError += fieldErr;

                        curCode = ValidateCurrencyCode(fields[2], out fieldErr);
                        if (fieldErr.Length > 0) rowError += fieldErr;

                        trnDate = ValidateTransactionDate(fields[3], out fieldErr);
                        if (fieldErr.Length > 0) rowError += fieldErr;

                        status = ValidateStatus(fields[4], out fieldErr);
                        if (fieldErr.Length > 0) rowError += fieldErr;

                        if (rowError.Length == 0)
                            mod.Add(new TransactionModel()
                            { trnID = trnID, amount = amount, currencyCode = curCode, trnDate = trnDate, status = status });
                    }
                    else
                    {
                        rowError = "Invalid format.";
                    }

                    if (rowError.Length > 0)
                        sbErr.Append("Item no " + (i + 1).ToString() + rowError + "\n");
                }
            }
            else
                sbErr.Append("Empty content in file.");

            errMsg = sbErr.ToString();
            return mod;
        }

        protected override string ValidateTransactionDate(string source, out string errmsg)
        {
            errmsg = "";
            source = source.Trim().Replace("\"", "");

            DateTime dResult;
            if (!DateTime.TryParseExact(source, "dd/MM/yyyy HH:mm:ss", new CultureInfo("en-US"),
                    DateTimeStyles.None, out dResult))
            {
                errmsg = "Invalid Transaction Date (format dd/MM/yyyy HH:mm:ss),";
                return "";
            }
            else
                return dResult.ToString("dd/MM/yyyy HH:mm:ss");
        }

        protected override string ValidateStatus(string source, out string errmsg)
        {
            source = source.Trim().Replace("\"", "");
            string retValue = source;
            errmsg = "";
            if (!base.status.TryGetValue(source.ToUpper(), out retValue))
            {
                errmsg = "Invalid Status, ";
                retValue = "";
            }

            return retValue;
        }

       
    }
}
