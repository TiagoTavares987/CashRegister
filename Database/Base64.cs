using System;
using System.Text;

namespace Database
{
    internal static class Base64
    {
        public static string Encode(string textToEncode)
        {
            return string.IsNullOrEmpty(textToEncode) ? null : Convert.ToBase64String(Encoding.UTF8.GetBytes(textToEncode));
        }

        public static string Decode(string encodedText)
        {
            try { return string.IsNullOrEmpty(encodedText) ? null : Encoding.UTF8.GetString(Convert.FromBase64String(encodedText)); }
            catch { return null; }
        }
    }
}
