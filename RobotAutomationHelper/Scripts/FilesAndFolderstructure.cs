using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    public static class FilesAndFolderStructure
    {
        // list of saved files for the drop down menus
        private static List<string> SavedFiles = new List<string>();
        private static string OutputFolder;

        public static bool AddFileToSavedFiles(string filePath)
        {
            if (!(filePath == null))
            {
                filePath = filePath.Replace(OutputFolder, "\\");
                if (!filePath.Trim().Equals("") && !ContainsFile(filePath))
                {
                    SavedFiles.Add(filePath);
                    return true;
                }

            }
            return false;
        }

        public static void SetFolder(string outputFolder)
        {
            OutputFolder = outputFolder;
        }

        public static string GetFolder()
        {
            return OutputFolder;
        }

        public static string ConcatFileNameToFolder(string FileName)
        {
            string outputFilePath = OutputFolder;
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

        public static List<string> GetFilesList()
        {
            return SavedFiles;
        }

        public static void AddImplementedKeywordFilesToSavedFiles(List<Keyword> Keywords, int implementedKeyword)
        {
            AddFileToSavedFiles(Keywords[implementedKeyword - 1].GetOutputFilePath());
            if (Keywords[implementedKeyword - 1].GetKeywordKeywords() != null)
                foreach (Keyword key in Keywords[implementedKeyword - 1].GetKeywordKeywords())
                    AddFilesFromKeywords(key);
        }

        public static void AddImplementedTestCasesFilesToSavedFiles(List<TestCase> TestCases, int implementedKeyword)
        {
            AddFileToSavedFiles(TestCases[implementedKeyword - 1].GetOutputFilePath());
            if (TestCases[implementedKeyword - 1].GetTestSteps() != null)
                foreach (Keyword key in TestCases[implementedKeyword - 1].GetTestSteps())
                    AddFilesFromKeywords(key);
        }

        public static void FindAllRobotFilesAndAddToStructure()
        {
            DirectoryInfo d = new DirectoryInfo(GetFolder());

            foreach (var file in d.GetFiles("*.robot", SearchOption.AllDirectories))
            {
                AddFileToSavedFiles(file.FullName.Replace(GetFolder(), "\\"));
            }
        }

        //Goes recursively through all keywords in given keyword
        public static void AddFilesFromKeywords(Keyword keyword)
        {
            AddFileToSavedFiles(keyword.GetOutputFilePath());
            if (keyword.GetKeywordKeywords() != null)
                foreach (Keyword key in keyword.GetKeywordKeywords())
                    AddFilesFromKeywords(key);
        }

    }
}
