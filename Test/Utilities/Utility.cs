using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public enum ImportFileType
    {
        XML,
        CSV,
        Invalid
    }

    public class Utility
    {
        public static bool IsValidAPIDate(string source)
        {
            DateTime dResult;
            return (source == null || source == "" || DateTime.TryParseExact(source, "dd/MM/yyyy", new CultureInfo("en-US"),
                    DateTimeStyles.None, out dResult));
        }

        public static string ValidateCurrencyCode(string source)
        {
            source = source.ToUpper();
            CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);

            foreach (CultureInfo ci in cultures)
            {
                RegionInfo ri = new RegionInfo(ci.LCID);
                if (ri.ISOCurrencySymbol.ToUpper() == source)
                {
                    return source;
                }
            }

            return "";
        }
    }
}
