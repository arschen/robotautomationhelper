using RobotAutomationHelper.Scripts.Objects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects.Tests
{
    [TestClass()]
    public class RobotFileTests
    {
        RobotFile ThisFile;

        [TestMethod()]
        public void ReadVariablesFromFile()
        {
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            List<Variables> VariablesList = new List<Variables>();
            VariablesList = ThisFile.ReadVariablesFromFile(ThisFile.ReadAllLines(FileName));
            Assert.AreEqual(1, VariablesList[0].VariableNames.Count);

            FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Data\\DepositLocators.robot";
            ThisFile = new RobotFile(FileName);
            VariablesList = ThisFile.ReadVariablesFromFile(ThisFile.ReadAllLines(FileName));
            Assert.AreEqual(35, VariablesList[0].VariableNames.Count);
        }

        [TestMethod()]
        public void HasTagTest()
        {
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            Assert.AreEqual(0, ThisFile.HasTag(ThisFile.ReadAllLines(FileName), Forms.FormType.Settings));
            Assert.AreEqual(6, ThisFile.HasTag(ThisFile.ReadAllLines(FileName), Forms.FormType.Variable));
            Assert.AreEqual(9, ThisFile.HasTag(ThisFile.ReadAllLines(FileName), Forms.FormType.Keyword));
        }

        [TestMethod()]
        public void ReadKeywordsFromFileTest()
        {
            string FileName = "C:\\Development\\robot-scripts\\jennyvegas\\Resources\\RegisterForm.robot";
            ThisFile = new RobotFile(FileName);
            List<Keyword> KeywordsList = new List<Keyword>();
            KeywordsList = ThisFile.ReadKeywordsFromFile(ThisFile.ReadAllLines(FileName));
            Assert.AreEqual(29, KeywordsList.Count);
            foreach (Keyword temp in KeywordsList)
            {
                Console.WriteLine(temp.Name);
                /*if (temp.Documentation != null)
                    Console.WriteLine(temp.Documentation);
                if (temp.Keywords != null)
                    foreach (Keyword t in temp.Keywords)
                        Console.WriteLine("\t" + t.GetName());*/
            }
        }
    }
}