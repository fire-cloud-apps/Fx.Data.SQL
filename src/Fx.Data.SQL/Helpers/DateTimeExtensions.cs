using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fx.Data.SQL.Helpers;

public static class DateTimeExtensions
{
    public static DateTime ToDateTime(this string s,
              string format = "ddMMyyyy", string cultureString = "en-US")
    {
        try
        {
            var r = DateTime.ParseExact(
                s: s,
                format: format,
                provider: CultureInfo.GetCultureInfo(cultureString));
            return r;
        }
        catch (FormatException)
        {
            //throw;
        }
        catch (CultureNotFoundException)
        {
            //throw; // Given Culture is not supported culture
        }
        return DateTime.MinValue;
    }

    public static DateTime ToDateTime(this string s,
                string format, CultureInfo culture)
    {
        try
        {
            var r = DateTime.ParseExact(s: s, format: format,
                                    provider: culture);
            return r;
        }
        catch (FormatException)
        {
            //throw;
        }
        catch (CultureNotFoundException)
        {
            //throw; // Given Culture is not supported culture
        }
        return DateTime.MinValue;

    }

}

