using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal static class FilesAndFolderStructure
    {
        // list of saved files for the drop down menus
        private static List<string> SavedFiles = new List<string>();
        private static string OutputFolder;

        internal static List<string> GetSavedFiles(string type)
        {
            List<string> results = new List<string>();
            string pattern = GetFolder(type);
            foreach (string temp in SavedFiles)
                if (temp.Contains(pattern))
                {
                    results.Add(temp.Replace(pattern, ""));
                }
            return results;
        }

        internal static void AddFileToSavedFiles(string filePath)
        {
            if (!(filePath == null))
            {
                if (!SavedFiles.Contains(filePath))
                    SavedFiles.Add(filePath);
            }
        }

        internal static void SetFolder(string outputFolder)
        {
            OutputFolder = outputFolder;
        }

        internal static string GetFolder(string type)
        {
            switch (type)
            {
                case "Keywords": return OutputFolder + "Resources\\";
                case "Tests": return OutputFolder + "Tests\\";
                default: return OutputFolder;
            }
        }

        internal static string ConcatFileNameToFolder(string FileName, string type)
        {
            string outputFilePath = GetFolder(type);
            if (!FileName.StartsWith("\\"))
                outputFilePath = outputFilePath + FileName;
            else
                outputFilePath = outputFilePath.Trim('\\') + FileName;
            return outputFilePath;
        }

        private static bool ContainsFile(string filePath)
        {
            foreach (string path in SavedFiles)
                if (path.ToLower().Equals(filePath.ToLower()))
                    return true;
            return false;
        }

        internal static void AddImplementedKeywordFilesToSavedFiles(List<Keyword> Keywords, int implementedKeyword)
        {
            AddFileToSavedFiles(Keywords[implementedKeyword - 1].OutputFilePath);
            if (Keywords[implementedKeyword - 1].Keywords != null)
                foreach (Keyword key in Keywords[implementedKeyword - 1].Keywords)
                    AddFilesFromKeywords(key);
        }

        internal static void AddImplementedTestCasesFilesToSavedFiles(List<TestCase> TestCases, int implementedKeyword)
        {
            AddFileToSavedFiles(TestCases[implementedKeyword - 1].OutputFilePath);
            if (TestCases[implementedKeyword - 1].Steps != null)
                foreach (Keyword key in TestCases[implementedKeyword - 1].Steps)
                    AddFilesFromKeywords(key);
        }

        internal static void FindAllRobotFilesAndAddToStructure()
        {
            DirectoryInfo d = new DirectoryInfo(OutputFolder);

            foreach (var file in d.GetFiles("*.robot", SearchOption.AllDirectories))
            {
                AddFileToSavedFiles(file.FullName);
            }
        }

        //Goes recursively through all keywords in given keyword
        internal static void AddFilesFromKeywords(Keyword keyword)
        {
            AddFileToSavedFiles(keyword.OutputFilePath);
            if (keyword.Keywords != null)
                foreach (Keyword key in keyword.Keywords)
                    AddFilesFromKeywords(key);
        }

    }
}
