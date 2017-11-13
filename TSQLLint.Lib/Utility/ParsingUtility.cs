using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;

namespace TSQLLint.Lib.Utility
{
    public static class ParsingUtility
    {
        public static TextReader CreateTextReaderFromString(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            return new StreamReader(new MemoryStream(bytes));
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
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
