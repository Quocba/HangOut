namespace HangOut.API.Common.Utils
{
    public static class DateUtil
    {
        public static string GetDurationString(DateTime start, DateTime end)
        {
            if (end < start)
                return "0 ngày";

            int months = ((end.Year - start.Year) * 12) + end.Month - start.Month;

            DateTime tempDate = start.AddMonths(months);

            if (tempDate > end)
            {
                months--;
                tempDate = start.AddMonths(months);
            }

            int days = (end - tempDate).Days;

            string result = "";
            if (months > 0)
                result += $"{months} tháng ";
            if (days > 0)
                result += $"{days} ngày";
            if (string.IsNullOrEmpty(result))
                result = "0 ngày";

            return result.Trim();
        }
    }
}
