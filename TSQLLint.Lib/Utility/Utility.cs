using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TSQLLint.Lib.Utility
{
    public static class Utility
    {
        public static TextReader CreateTextReaderFromString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return new StreamReader(new MemoryStream(bytes));
        }

        public static bool TryParseJson(string jsonString, out JToken token)
        {
            try
            {
                token = JToken.Parse(jsonString);
                return true;
            }
            catch
            {
                token = null;
                return false;
            }
        }
    }
}