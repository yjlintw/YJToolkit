using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YJToolkit.YJTOolkitCSharp.Convert
{
    public static class TimeConvert
    {
        public static DateTime UnixTimeStampToDateTime( double unixTimeStamp)
        {
            // Unix timestamp is second past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

            return dtDateTime;
        }
    }
}
