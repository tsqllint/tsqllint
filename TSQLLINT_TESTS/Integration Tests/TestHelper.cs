using System.Collections.Generic;
using System.IO;

namespace TSQLLINT_TESTS.Integration_Tests
{
    public class TestHelper
    {
        private readonly string WorkingDirectory;
        private readonly List<string> filesToCleanUp = new List<string>();

        public TestHelper(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
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