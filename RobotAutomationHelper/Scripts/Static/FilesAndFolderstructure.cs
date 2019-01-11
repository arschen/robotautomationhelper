using System.Collections.Generic;
using System.IO;
using RobotAutomationHelper.Forms;
using RobotAutomationHelper.Scripts.Objects;

namespace RobotAutomationHelper.Scripts.Static
{
    public static class FilesAndFolderStructure
    {
        // list of saved files for the drop down menus
        private static List<string> _savedFiles = new List<string>();
        private static string _outputRootFolder;
        public static string Resources = "Resources";
        public static string Tests = "Tests";

        public static void CleanUp()
        {
            _savedFiles = new List<string>();
            _outputRootFolder = "";
        }

        public static List<string> GetShortSavedFiles(FolderType folderType)
        {
            var results = new List<string>();
            var pattern = GetFolder(folderType);
            foreach (var temp in _savedFiles)
                if (temp.Contains(pattern))
                {
                    results.Add(temp.Replace(pattern, ""));
                }
            return results;
        }

        public static List<string> GetFullSavedFiles(FolderType folderType)
        {
            var results = new List<string>();
            var pattern = GetFolder(folderType);
            foreach (var temp in _savedFiles)
                if (temp.ToLower().Contains(pattern.ToLower()))
                {
                    results.Add(temp);
                }
            return results;
        }

        public static void AddFileToSavedFiles(string filePath)
        {
            if (filePath == null) return;
            if (!_savedFiles.Contains(filePath))
                _savedFiles.Add(filePath);
        }

        public static void SetFolder(string outputFolder)
        {
            _outputRootFolder = outputFolder;
        }

        public static string GetFolder(FolderType folderType)
        {
            switch (folderType)
            {
                case FolderType.Resources: return _outputRootFolder + Resources + "\\";
                case FolderType.Tests: return _outputRootFolder + Tests + "\\";
                case FolderType.Root: return _outputRootFolder;
                default: return _outputRootFolder;
            }
        }

        public static string ConcatFileNameToFolder(string fileName, FolderType folderType)
        {
            var outputFilePath = GetFolder(folderType);
            if (!fileName.StartsWith("\\"))
                outputFilePath = outputFilePath + fileName;
            else
                outputFilePath = outputFilePath.Trim('\\') + fileName;
            return outputFilePath;
        }

        public static void AddImplementedKeywordFilesToSavedFiles(List<Keyword> keywords, int implementedKeyword)
        {
            AddFileToSavedFiles(keywords[implementedKeyword - 1].OutputFilePath);
            if (keywords[implementedKeyword - 1].Keywords == null) return;
            foreach (var key in keywords[implementedKeyword - 1].Keywords)
                AddFilesFromKeywords(key);
        }

        public static void AddImplementedTestCasesFilesToSavedFiles(List<TestCase> testCases, int implementedKeyword)
        {
            AddFileToSavedFiles(testCases[implementedKeyword - 1].OutputFilePath);
            if (testCases[implementedKeyword - 1].Steps == null) return;
            foreach (var key in testCases[implementedKeyword - 1].Steps)
                AddFilesFromKeywords(key);
        }

        public static void FindAllRobotFilesAndAddToStructure()
        {
            var d = new DirectoryInfo(_outputRootFolder);
            foreach (var dir in d.GetDirectories())
            {
                if (dir.ToString().ToLower().Equals("tests"))
                    Tests = dir.ToString();
                if (dir.ToString().ToLower().Equals("resources"))
                    Resources = dir.ToString();
            }
            foreach (var file in d.GetFiles("*.robot", SearchOption.AllDirectories))
            {
                AddFileToSavedFiles(file.FullName);
            }
        }

        public static void DeleteAllFiles()
        {
            var d = new DirectoryInfo(_outputRootFolder);
            foreach (var file in d.GetFiles("*.robot", SearchOption.AllDirectories))
            {
                file.Delete();
            }
            foreach (var file in d.GetFiles("*.py", SearchOption.AllDirectories))
            {
                file.Delete();
            }
        }

        //Goes recursively through all keywords in given keyword
        public static void AddFilesFromKeywords(Keyword keyword)
        {
            AddFileToSavedFiles(keyword.OutputFilePath);
            if (keyword.Keywords == null) return;
            foreach (var key in keyword.Keywords)
                if (!key.Recursive)
                    AddFilesFromKeywords(key);
        }

        public static FolderType ConvertFormTypeToFolderType(FormType formType)
        {
            FolderType folderType;
            if (formType.Equals(FormType.Keyword))
                folderType = FolderType.Resources;
            else
            {
                folderType = formType.Equals(FormType.Test) ? FolderType.Tests : FolderType.Root;
            }

            return folderType;
        }
    }

    public enum FolderType
    {
        Resources,
        Tests,
        Root
    }
}
