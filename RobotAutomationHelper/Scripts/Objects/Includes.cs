using System;
using System.Collections.Generic;

namespace RobotAutomationHelper.Scripts.Objects
{
    public class Includes : IEquatable<Includes>
    {
        public string FileName { get; }
        public List<string> FilesToInclude { get; }

        public Includes(string fileName)
        {
            FileName = fileName;
            FilesToInclude = new List<string>();
        }

        public void AddToList(string fileName)
        {
            if (FileName.Equals(fileName)) return;
            if (!FilesToInclude.Contains(fileName))
                FilesToInclude.Add(fileName);
        }

        public bool Equals(Includes other)
        {
            return other != null && other.FileName.Equals(FileName);
        }
    }
}
