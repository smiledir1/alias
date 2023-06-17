using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace Common.Extensions
{
    public static class ParseStringExtensions
    {
        public static bool GetBool(this string text)
        {
            if (bool.TryParse(text, out var value))
                return value;

            throw new Exception("Can't parse: " + text + " to bool!");
        }

        public static bool GetBoolSafe(this string text, bool defaultVal = false)
        {
            return bool.TryParse(text, out var value) ? value : defaultVal;
        }

        public static int GetInt(this string text)
        {
            if (int.TryParse(text, out var value))
                return value;

            if (text == "NaN")
                return 0;

            throw new Exception("Can't parse: " + text + " to int!");
        }

        public static int GetIntSafe(this string text, int defaultVal = 0)
        {
            return int.TryParse(text, out var value) ? value : defaultVal;
        }

        public static float GetFloat(this string text)
        {
            text = text.Replace(",", ".");

            if (float.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
                return value;

            throw new Exception("Can't parse: " + text + " to float!");
        }

        public static float GetFloatSafe(this string text, float defaultVal)
        {
            text = text.Replace(",", ".");

            return float.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                ? value
                : defaultVal;
        }
        
        public static string GetMD5OfString(string text)
        {
            var inputBytes = Encoding.UTF8.GetBytes(text);

            var md5 = MD5.Create();

            var hash = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            for (var i = hash.Length - 1; i >= 0; i--)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
        
        public static T GetEnumFromString<T>(this string value) where T : Enum
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}