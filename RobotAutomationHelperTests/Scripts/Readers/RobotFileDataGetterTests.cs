using Microsoft.VisualStudio.TestTools.UnitTesting;
using RobotAutomationHelper.Scripts.Readers;
using RobotAutomationHelper.Scripts.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts.Readers.Tests
{
    [TestClass()]
    public class RobotFileDataGetterTests
    {
        RobotFileDataGetter DataGetter;

        [TestMethod()]
        public void GetTheDataFromAllTheRobotFilesTest()
        {
            SuggestionsClass.PopulateSuggestionsList();
            FilesAndFolderStructure.SetFolder("C:\\Development\\robot-scripts\\jennyvegas_v005\\jennyvegas");
            FilesAndFolderStructure.FindAllRobotFilesAndAddToStructure();
            DataGetter = new RobotFileDataGetter();
            DataGetter.GetTheDataFromAllTheRobotFiles();
        }
    }
}