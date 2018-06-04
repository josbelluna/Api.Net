using System;

namespace Api.Utils
{
    public static class DateUtils
    {
        public static string ToUniversalDate(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }
        public static string GetDateFormat(this string value, string sign)
        {
            var dateSplit = value.Split('|');
            string format;
            switch (sign)
            {
                case "=":
                    var dateFrom = DateTime.Parse(dateSplit[0]);
                    var dateTo = dateSplit.Length > 1 ? DateTime.Parse(dateSplit[1]) : new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, 23, 59, 59);
                    format = $"{{0}} >= DateTime({dateFrom.Year},{dateFrom.Month},{dateFrom.Day},{dateFrom.Hour},{dateFrom.Minute},{dateFrom.Second}) && {{0}} <= DateTime({dateTo.Year},{dateTo.Month},{dateTo.Day},{dateTo.Hour},{dateTo.Minute},{dateTo.Second})";

                    break;
                default:
                    dateFrom = DateTime.Parse(value);
                    format = $"{{0}}{sign} DateTime({dateFrom.Year},{dateFrom.Month},{dateFrom.Day})";
                    break;
            }

            return format;
        }
    }
}
