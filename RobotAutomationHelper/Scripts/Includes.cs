using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts
{
    class Includes : IEquatable<Includes>
    {
        private string FileName;
        private List<string> FilesToInclude = new List<string>();

        internal Includes(string fileName)
        {
            FileName = fileName;
        }

        internal void AddToList(string fileName)
        {
            if (!FileName.Equals(fileName))
                if (!FilesToInclude.Contains(fileName))
                    FilesToInclude.Add(fileName);
        }

        internal List<string> GetFilesToInclude()
        {
            return FilesToInclude;
        }

        internal string GetFileName()
        {
            return FileName;
        }

        public bool Equals(Includes other)
        {
            if (other.GetFileName().Equals(FileName))
                return true;
            return false;
        }
    }
}
