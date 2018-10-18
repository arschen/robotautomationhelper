using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts
{
    class Includes : IEquatable<Includes>
    {
        internal string FileName { get; }
        internal List<string> FilesToInclude { get; }

        internal Includes(string fileName)
        {
            FileName = fileName;
            FilesToInclude = new List<string>();
        }

        internal void AddToList(string fileName)
        {
            if (!FileName.Equals(fileName))
                if (!FilesToInclude.Contains(fileName))
                    FilesToInclude.Add(fileName);
        }

        public bool Equals(Includes other)
        {
            if (other.FileName.Equals(FileName))
                return true;
            return false;
        }
    }
}
