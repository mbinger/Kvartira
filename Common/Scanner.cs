using System;
using System.Globalization;
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
            
            if (int.TryParse(str, out var result))
            {
                return result;
            }

            return null;
        }

        public static decimal? ParseDecimal(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            
            if (decimal.TryParse(str.Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        public static bool? ParseBool(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            var strLower = str.ToLower();
            if (strLower == "true" || strLower == "1")
            {
                return true;
            }

            return false;
        }

        public static DateTime? ParseDate(string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }
            
            if (DateTime.TryParseExact(str, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }

            return null;
        }

        public static async Task<string> ParseSafeAsync(Parser parser, string property, string html, string description, ILog log)
        {
            if (parser == null)
            {
                return null;
            }

            string result = html;

            if (parser.Regex != null)
            {
                var task1 = Task.Run(() => parser.Regex.Match(html).Groups["value"].Value);
                var task2 = Task.Delay(10000);
                var taskResult = await Task.WhenAny(task1, task2);
                if (taskResult == task2)
                {
                    log.Write($"TIMEOUT parsing {property} from {description}");
                    return null;
                }
                result = await task1;
            }
            if (parser.Transform != null)
            {
                result = parser.Transform(result);
            }

            return result;
        }

        public static async Task<decimal?> ParseSafeDecimalAsync(Parser parser, string property, string html, string description, ILog log)
        {
            var str = await ParseSafeAsync(parser, property, html, description, log);
            return ParseDecimal(str);
        }

        public static async Task<int?> ParseSafeIntAsync(Parser parser, string property, string html, string description, ILog log)
        {
            var str = await ParseSafeAsync(parser, property, html, description, log);
            return ParseInt(str);
        }

        public static async Task<DateTime?> ParseSafeDateAsync(Parser parser, string property, string html, string description, ILog log)
        {
            var str = await ParseSafeAsync(parser, property, html, description, log);
            return ParseDate(str);
        }

        public static async Task<bool?> ParseSafeBoolAsync(Parser parser, string property, string html, string description, ILog log)
        {
            var str = await ParseSafeAsync(parser, property, html, description, log);
            return ParseBool(str);
        }
    }
}
