using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using Test.Models;
using Test.Services;

namespace Test.Controllers
{
    [ApiController]
    public class APIController : Controller
    {
        [Route("api/gettransaction")]
        [HttpPost]
        public IActionResult gettransaction()
        {

            //Basic sample security (normally using Bearer Token Authentication)
            string token = Request.Headers["access_token"];
            if(token.Trim().Length > 0)
            {

                List<TransactionResultModel> mod = new List<TransactionResultModel>();
                string currencyCode = Request.Form["curcode"];
                string from = Request.Form["fdate"];
                string to = Request.Form["tdate"];
                string status = Request.Form["status"];

                if (currencyCode != null && currencyCode.Trim().Length > 0)
                {
                    currencyCode = Utility.ValidateCurrencyCode(currencyCode);
                    if (currencyCode.Trim().Length == 0)
                    {
                        return Json("400  Invalid curcode.");
                    }
                }
                if (!Utility.IsValidAPIDate(from))
                {
                    return Json("400  Invalid fdate format");
                }
                if (!Utility.IsValidAPIDate(to))
                {
                    return Json("400  Invalid tdate format");
                }
                if(status != null && !("a,r,d".IndexOf(status.ToLower()) > -1))
                {
                    return Json("400  Invalid status");
                }
                

                mod = new TransactionService().GetTransaction(currencyCode, from, to, status);

                return Json(mod);
            }
            else
            {
                return Json("401 Unauthorized");
            }

            
        }

    }
}
