using System;
using System.Text.RegularExpressions;

namespace Common
{
    public class Parser
    {
        public Parser(Regex regex, Func<string, string> transform)
        {
            Regex = regex;
            Transform = transform;
        }

        public Parser(Regex regex)
        {
            Regex = regex;
        }

        public Parser(Func<string, string> transform)
        {
            Transform = transform;
        }

        public readonly Regex Regex;
        public readonly Func<string, string> Transform;
    }
}
