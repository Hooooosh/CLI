using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Usings
{
    public class StringExtension
    {
        public static string Capitalize(string str)
        {
            if(str == string.Empty) { return string.Empty; }
            return string.Concat(str[0].ToString().ToUpper(), str.Length > 1? str.Substring(1) : "");
        }

        public static string FormatTokenChain(string message)
        {
            message = Regex.Replace(message, "( ){2,}", " ").Trim();
            return message;
        }
    }
}
