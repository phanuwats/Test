using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Models
{
    public class TransactionModel
    {
        public string trnID { get; set; }
        public string amount { get; set; }
        public string currencyCode { get; set; }
        public string trnDate { get; set; }
        public string status { get; set; }

        public bool isError { get; set; }
        public string errorMessage { get; set; }
    }
}
