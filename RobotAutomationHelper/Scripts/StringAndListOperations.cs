using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RobotAutomationHelper.Scripts
{
    internal static class StringAndListOperations
    {

        public static List<string> ReturnListOfArgs(string arguments)
        {
            List<string> args = new List<string>();
            if (arguments != null)
                args.AddRange(arguments.Replace("[Arguments]", "").Trim().Split(' '));

            if (args != null)
                for (int i = 0; i < args.Count; i++)
                    if (args[i].Equals(""))
                        args.RemoveAt(i);
            return args;
        }
    }
}
