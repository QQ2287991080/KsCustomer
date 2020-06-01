using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jusoft.YiFang.Api.Help
{
    public class Helper
    {
        public static DateTime DateTimeToStamp(long jsTimeStamp)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            DateTime dt = startTime.AddMilliseconds(jsTimeStamp);
            return dt;
        }
    }
}