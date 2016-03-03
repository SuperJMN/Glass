namespace Glass.Imaging.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    public static class StringExtensions
    {
        public static string Truncate(this string str, int length)
        {
            if (str == null)
            {
                return null;
            }

            if (length == 0)
            {
                return str;
            }

            return str.Substring(0, Math.Min(length, str.Length));
        }

        public static string CollapseSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static int ToInt32(this string str)
        {
            return int.Parse(str);
        }

        public static IEnumerable<string> GetChunks(this string str)
        {
            var newLine = ";".ToArray();
            var lines = str.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
            return lines.Select(l => l.Trim());
        }

        public static int? TryParseToInt32(this string str)
        {
            int result;
            var success = int.TryParse(str, out result);
            if (success)
            {
                return result;
            }

            return null;
        }

        public static string RemoveDiacritics(this string str)
        {
            if (str == null) return null;

            str = str.Replace('ñ', (char)1);
            str = str.Replace('Ñ', (char)2);

            var normalize = str.Normalize(NormalizationForm.FormD);
            var charArray = normalize.ToCharArray();

            var chars =
                from c in charArray
                let uc = CharUnicodeInfo.GetUnicodeCategory(c)
                where uc != UnicodeCategory.NonSpacingMark
                select c;

            var array = chars.ToArray();
            var cleanStr = new string(array).Normalize(NormalizationForm.FormC);

            cleanStr = cleanStr.Replace((char)1, 'ñ');
            cleanStr = cleanStr.Replace((char)2, 'Ñ');

            return cleanStr;
        }
    }
}