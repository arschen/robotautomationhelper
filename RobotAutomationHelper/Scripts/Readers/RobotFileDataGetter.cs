using RobotAutomationHelper.Scripts.Objects;
using RobotAutomationHelper.Scripts.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts.Readers
{
    public class RobotFileDataGetter
    {
        List<RobotFile> RobotFiles = new List<RobotFile>();

        // returns the index of the specific tag - keyword / test cases / settings / variables
        public void GetTheDataFromAllTheRobotFiles()
        {
            var listOfVariables = new List<Variables>();
            foreach (var fileName in FilesAndFolderStructure.GetFullSavedFiles(FolderType.Root))
                RobotFiles.Add(new RobotFile(fileName));
        }
    }
}
