using System.IO;
using System.Text;

namespace TSQLLINT_LIB
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
    }
}