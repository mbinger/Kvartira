using System;
using System.Globalization;
using System.Linq;
using System.Text;
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

            var cleaned = str.Replace(".", "").Replace(",", ".");
            cleaned = string.Concat(cleaned.Where(c => Char.IsDigit(c) || c == '.'));

            if (decimal.TryParse(cleaned, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
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
                var task1 = Task.Run(() => GetValue(parser.Regex, html));
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

        public static async Task<string> ParseSafeAsync(Parser[] parser, string property, string html, string description, ILog log)
        {
            if (parser == null || ! parser.Any())
            {
                return null;
            }
            
            var result = new StringBuilder();
            foreach (var p in parser)
            {
                var value = await ParseSafeAsync(p, property, html, description, log);
                result.AppendLine(value);
            }
            return result.ToString();
        }

        private static string GetValue(Regex regex, string html)
        {
            if (string.IsNullOrEmpty(html))
            {
                return null;
            }

            var match = regex.Match(html);
            if (!match.Success)
            {
                return null;
            }

            var groupNames = regex.GetGroupNames();
            if (groupNames.Contains("value"))
            {
                return match.Groups["value"].Value;
            }

            var sb = new StringBuilder();
            for (var i = 1; i < 10; i++)
            {
                var groupName = $"value{i}";
                if (!groupNames.Contains(groupName))
                {
                    break;
                }
                var vali = match.Groups[groupName].Value;
                sb.Append(vali);
            }

            return sb.ToString();
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
