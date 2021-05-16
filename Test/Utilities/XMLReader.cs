using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.Linq;
using Test.Models;
using Test.Services;

namespace Test
{
    public class XMLReader : BaseImportReader
    {
        public XMLReader()
        {
            MasterService svc = new MasterService();
            List<dynamic> modStatus = modStatus = svc.GetLookup("XML");
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



            //Transaction Date XML Valid Case
            retValue = ValidateTransactionDate("2019-01-23T13:45:10", out errMsg);
            retValue = ValidateTransactionDate("2019-01-24T16:09:15", out errMsg);

            //Transaction Date XML Invalid Case
            retValue = ValidateTransactionDate("", out errMsg);
            retValue = ValidateTransactionDate("xxx", out errMsg);
            retValue = ValidateTransactionDate("2020-01-01", out errMsg);
            retValue = ValidateTransactionDate("01:01:01", out errMsg);
            retValue = ValidateTransactionDate("2020-01-33T01:01:01", out errMsg);
            retValue = ValidateTransactionDate("99999-01-01T01:01:01", out errMsg);
            retValue = ValidateTransactionDate("-01-01T01:01:01", out errMsg);
            retValue = ValidateTransactionDate("-01-01T25:01:01", out errMsg);
            retValue = ValidateTransactionDate("-01-01 25:01:01", out errMsg);

            //Status XML Valid Case
            retValue = ValidateStatus("Approved", out errMsg);
            retValue = ValidateStatus("approved", out errMsg);
            retValue = ValidateStatus("APPROVED", out errMsg);
            retValue = ValidateStatus("Rejected", out errMsg);
            retValue = ValidateStatus("Done", out errMsg);

            //Status XML Invalid Case
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
                try
                {
                    XDocument xml = XDocument.Parse(content.ToLower());

                    int i = 1;
                    foreach (var node in xml.Descendants("transaction"))
                    {
                        string rowError = "";
                        try
                        {
                            string trnID, amount, curCode, trnDate, status;
                            string fieldErr;

                            trnID = node.Attribute("id").Value;
                            trnDate = node.Element("transactiondate").Value.ToUpper();
                            amount = node.Element("paymentdetails").Element("amount").Value;
                            curCode = node.Element("paymentdetails").Element("currencycode").Value;
                            status = node.Element("status").Value;

                            trnID = ValidateTransactionID(trnID, out fieldErr);
                            if (fieldErr.Length > 0) rowError = fieldErr;

                            amount = ValidateAmount(amount, out fieldErr);
                            if (fieldErr.Length > 0) rowError += fieldErr;

                            curCode = ValidateCurrencyCode(curCode, out fieldErr);
                            if (fieldErr.Length > 0) rowError += fieldErr;

                            trnDate = ValidateTransactionDate(trnDate, out fieldErr);
                            if (fieldErr.Length > 0) rowError += fieldErr;

                            status = ValidateStatus(status, out fieldErr);
                            if (fieldErr.Length > 0) rowError += fieldErr;

                            if (rowError.Length == 0)
                                mod.Add(new TransactionModel()
                                { trnID = trnID, amount = amount, currencyCode = curCode, trnDate = trnDate, status = status });
                        }
                        catch
                        {
                            rowError = "Invalid format.";
                        }
                        if (rowError.Length > 0)
                            sbErr.Append("Item no " + i.ToString() + " " + rowError + "\n");

                        i++;
                    }
                }
                catch
                {
                    sbErr.Append("Invalid xml format.");
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
            if (!DateTime.TryParseExact(source, "yyyy-MM-ddTHH:mm:ss", new CultureInfo("en-US"),
                    DateTimeStyles.None, out dResult))
            {
                errmsg = "Invalid Transaction Date (format yyyy-MM-ddTHH:mm:ss),";
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
