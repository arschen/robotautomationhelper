﻿using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Static
{
    public static class StringAndListOperations
    {

        public static List<string> ReturnListOfArgs(string arguments)
        {
            var args = new List<string>();
            if (arguments != null)
                args.AddRange(arguments.Replace("[Arguments]", "").Trim().Split(new[] { "  " },StringSplitOptions.None));

            for (var i = 0; i < args.Count; i++)
                if (args[i].Equals(""))
                    args.RemoveAt(i);
            return args;
        }

        public static bool StartsWithVariable(string name)
        {
            return name == null ? false : name.StartsWith("${")
                   || name.StartsWith("@{")
                   || name.StartsWith("&{");
        }
    }
}
