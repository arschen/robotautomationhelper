using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts.Static
{
    internal static class Cache
    {
        internal static void ClearCache()
        {
            FilesAndFolderStructure.CleanUp();
            SuggestionsClass.CleanUp();
        }
    }
}
