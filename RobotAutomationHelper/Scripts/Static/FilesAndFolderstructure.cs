using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    internal static class FilesAndFolderStructure
    {
        // list of saved files for the drop down menus
        private static List<string> SavedFiles = new List<string>();
        private static string OutputFolder;

        internal static void CleanUp()
        {
            SavedFiles = new List<string>();
            OutputFolder = "";
        }

        internal static List<string> GetShortSavedFiles(FolderType folderType)
        {
            List<string> results = new List<string>();
            string pattern = GetFolder(folderType);
            foreach (string temp in SavedFiles)
                if (temp.Contains(pattern))
                {
                    results.Add(temp.Replace(pattern, ""));
                }
            return results;
        }

        internal static List<string> GetFullSavedFiles(FolderType folderType)
        {
            List<string> results = new List<string>();
            string pattern = GetFolder(folderType);
            foreach (string temp in SavedFiles)
                if (temp.ToLower().Contains(pattern.ToLower()))
                {
                    results.Add(temp);
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

        internal static string GetFolder(FolderType folderType)
        {
            switch (folderType)
            {
                case FolderType.Resources: return OutputFolder + "Resources\\";
                case FolderType.Tests: return OutputFolder + "Tests\\";
                default: return OutputFolder;
            }
        }

        internal static string ConcatFileNameToFolder(string FileName, FolderType folderType)
        {
            string outputFilePath = GetFolder(folderType);
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
                    if (!key.Recursive)
                        AddFilesFromKeywords(key);
        }

        internal static FolderType ConvertFormTypeToFolderType(FormType formType)
        {
            FolderType folderType;
            if (formType.Equals(FormType.Keyword))
                folderType = FolderType.Resources;
            else
            {
                if (formType.Equals(FormType.Test))
                    folderType = FolderType.Tests;
                else
                    folderType = FolderType.Root;
            }

            return folderType;
        }
    }

    internal enum FolderType
    {
        Resources,
        Tests,
        Root
    }
}
