using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
    public static class Scanner
    {
        public static string DeflateHtml(string html)
        {
            html = html.Replace("\n", "").Replace("\r", "").Replace("\t", "").Replace("&nbsp;", "")
                .Replace("&auml;", "ä"); //Fläche
            return html;
        }

        public static int? ParseInt(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return int.TryParse(str, out var result) ? result : null;
        }

        public static decimal? ParseDecimal(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return decimal.TryParse(str.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
        }

        public static DateTime? ParseDate(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            return DateTime.TryParseExact(str, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result)
                ? result : null;
        }

        public static async Task<string> ParseSafeAsync(Regex regex, string property, string html, string description, Log log)
        {
            var task1 = Task.Run(() => regex.Match(html).Groups["value"].Value);
            var task2 = Task.Delay(10000);
            var result = await Task.WhenAny(task1, task2);
            if (result == task2)
            {
                await log.LogAsync($"TIMEOUT parsing {property} from {description}");
                return null;
            }

            return await task1;
        }

        public static async Task<decimal?> ParseSafeDecimalAsync(Regex regex, string property, string html, string description, Log log)
        {
            var str = await ParseSafeAsync(regex, property, html, description, log);
            return ParseDecimal(str);
        }

        public static async Task<int?> ParseSafeIntAsync(Regex regex, string property, string html, string description, Log log)
        {
            var str = await ParseSafeAsync(regex, property, html, description, log);
            return ParseInt(str);
        }

        public static async Task<DateTime?> ParseSafeDateAsync(Regex regex, string property, string html, string description, Log log)
        {
            var str = await ParseSafeAsync(regex, property, html, description, log);
            return ParseDate(str);
        }
    }
}
