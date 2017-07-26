using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TSQLLINT_LIB.Utility
{
    public static class Utility
    {
        public static TextReader CreateTextReaderFromString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var memoryStream = new MemoryStream(bytes);

            return new StreamReader(memoryStream);
        }

        public static string GetFileContents(string filePath)
        {
            return File.ReadAllText(filePath);
        }

        public static bool tryParseJson(string jsonString, out JToken token)
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