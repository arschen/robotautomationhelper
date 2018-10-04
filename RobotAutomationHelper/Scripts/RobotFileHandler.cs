using System.Collections.Generic;
using System.IO;

namespace RobotAutomationHelper.Scripts
{
    public static class RobotFileHandler
    {
        public static int GetLineAfterLastKeyword(string fileName)
        {
            return GetLine(fileName, "keywords");
        }

        public static int GetLineAfterLastTestCase(string fileName)
        {
            return GetLine(fileName, "testcases");
        }

        private static int GetLine(string fileName, string type)
        {
            int index = -1;
            string[] arrLine;
            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                string directory = fileName.Replace(fileName.Split('\\')[fileName.Split('\\').Length - 1], "");
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);
                var myFile = File.Create(fileName);
                myFile.Close();
                List<string> temp = new List<string>();
                temp.Add("*** Settings ***");
                temp.Add("*** Variables ***");
                temp.Add("*** Test Cases ***");
                temp.Add("*** Keywords ***");
                File.WriteAllLines(fileName, temp);
                temp.Clear();
                arrLine = File.ReadAllLines(fileName);

            }
            bool start = false;
            for (int i = 0; i < arrLine.Length; i++)
            {
                if (arrLine[i].StartsWith("***"))
                {
                    if (start)
                    {
                        index = i;
                        break;
                    }
                    if (arrLine[i].ToLower().Replace(" ","").Contains(type))
                        start = true;
                }
            }
            if (!start)
                if (arrLine.Length - 1 >= 0)
                    index = arrLine.Length;
                else
                    index = 0;
            else
                if (index == -1)
                {
                    if (arrLine.Length - 1 >= 0)
                        index = arrLine.Length;
                    else
                        index = 0;
                }

            return index;
        }

        public static int HasTag(string fileName, string type)
        {
            int index = -1;

            string[] arrLine;

            if (File.Exists(fileName))
                arrLine = File.ReadAllLines(fileName);
            else
            {
                var myFile = File.Create(fileName);
                myFile.Close();
                arrLine = File.ReadAllLines(fileName);
            }

            for (int i = 0; i < arrLine.Length; i++)
                if (arrLine[i].StartsWith("***"))
                    if (arrLine[i].ToLower().Replace(" ", "").Contains(type))
                        index = i;

            return index;
        }
    }
}
