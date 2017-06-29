using System.Collections.Generic;
using System.IO;

namespace TSQLLINT_LIB_TESTS
{
    public class TestHelper
    {
        private readonly string WorkingDirectory;
        private readonly List<string> filesToCleanUp = new List<string>();

        public TestHelper(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        public string GetTestFile(string testFilePath)
        {
            var path = Path.GetFullPath(Path.Combine(WorkingDirectory, string.Format("..\\..\\Unit Tests\\{0}", testFilePath)));
            return File.ReadAllText(path);
        }

        public void createFileForTesting(string testFileName, string content)
        {
            var testFilePath = Path.GetFullPath(Path.Combine(WorkingDirectory, testFileName));
            using (var sw = File.CreateText(testFilePath))
            {
                sw.WriteLine(content);
            }
            filesToCleanUp.Add(testFilePath);
        }

        public void cleanup()
        {
            foreach (var file in filesToCleanUp)
            {
                File.Delete(file);
            }
        }
    }
}