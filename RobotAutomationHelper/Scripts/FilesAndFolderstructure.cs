using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts
{
    public static class FilesAndFolderStructure
    {
        private static List<string> SavedFiles = new List<string>();
        private static string OutputFolder;

        public static bool AddFile(string filePath)
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
    }
}
