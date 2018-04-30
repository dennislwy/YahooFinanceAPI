using System;

namespace YahooFinanceAPI.Utils
{
    internal class DateTimeConverter
    {
        //credits to ScottCher
        //reference http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
        public static DateTime ToDateTime(double unixTimeStamp)
        {
            //Unix timestamp Is seconds past epoch
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToLocalTime();
        }

        //credits to Dmitry Fedorkov
        //reference http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
        public static double ToUnixTimestamp(DateTime datetime)
        {
            //Unix timestamp Is seconds past epoch
            return (datetime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }
    }
}