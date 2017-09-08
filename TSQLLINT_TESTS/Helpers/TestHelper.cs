using System.IO;

namespace TSQLLINT_LIB_TESTS.Helpers
{
    public class TestHelper
    {
        private readonly string _workingDirectory;

        public TestHelper(string workingDirectory)
        {
            _workingDirectory = workingDirectory;
        }

        public string GetTestFile(string testFilePath)
        {
            var path = Path.GetFullPath(Path.Combine(_workingDirectory, string.Format("..\\..\\UnitTests\\{0}", testFilePath)));
            return File.ReadAllText(path);
        }
    }
}