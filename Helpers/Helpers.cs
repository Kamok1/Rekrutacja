using System.Globalization;

namespace Helpers;

public static class Helpers
{
    public static string QueryString(IDictionary<string, object> dict)
    {
        var list = new List<string>();
        foreach (var item in dict)
        {
            list.Add(item.Key + "=" + item.Value);
        }
        return string.Join("&", list);
    }

    public static DateTimeOffset DateTimeOffsetFromISO8601(this string date)
    {
        string format = "yyyyMMdd'T'HHmmss.fff'Z'";
        DateTime dateTime = DateTime.ParseExact(date, format, CultureInfo.InvariantCulture);
        return dateTime;
    }
}