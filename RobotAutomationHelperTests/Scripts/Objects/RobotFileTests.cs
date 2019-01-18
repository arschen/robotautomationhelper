using RobotAutomationHelper.Scripts.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using RobotAutomationHelper.Scripts.Static;

namespace RobotAutomationHelper.Scripts.Objects.Tests
{
    [TestClass()]
    public class RobotFileTests
    {
        RobotFile ThisFile;

        [TestMethod()]
        public void ReadVariablesFromFile()
        {
            FilesAndFolderStructure.SetFolder("C:\\Development\\robot-scripts\\jennyvegas");
            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            List<Variables> VariablesList = new List<Variables>();
            VariablesList = ThisFile.ReadVariablesFromFile();
            Assert.AreEqual(1, VariablesList[0].VariableNames.Count);

            FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Data\\DepositLocators.robot";
            ThisFile = new RobotFile(FileName);
            VariablesList = ThisFile.ReadVariablesFromFile();
            Assert.AreEqual(35, VariablesList[0].VariableNames.Count);
        }

        [TestMethod()]
        public void HasTagTest()
        {
            FilesAndFolderStructure.SetFolder("C:\\Development\\robot-scripts\\jennyvegas");
            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            Assert.AreEqual(0, ThisFile.HasTag(Forms.FormType.Settings));
            Assert.AreEqual(6, ThisFile.HasTag(Forms.FormType.Variable));
            Assert.AreEqual(9, ThisFile.HasTag(Forms.FormType.Keyword));
        }

        [TestMethod()]
        public void ReadKeywordsFromFileTest()
        {
            FilesAndFolderStructure.SetFolder("C:\\Development\\robot-scripts\\jennyvegas");
            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            List<Keyword> KeywordsList = new List<Keyword>();
            KeywordsList = ThisFile.ReadKeywordsFromFile();
            Assert.AreEqual(29, KeywordsList.Count);
            /*
            foreach (Keyword temp in KeywordsList)
            {
                Console.WriteLine(temp.Name);
                if (temp.Documentation != null)
                    Console.WriteLine(temp.Documentation);
                if (temp.Keywords != null)
                    foreach (Keyword t in temp.Keywords)
                        Console.WriteLine("\t" + t.GetName());
            }
            */
        }
    }
}