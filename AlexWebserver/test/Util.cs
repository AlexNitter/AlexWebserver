using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    public static class Util
    {
        public static String GetContentFromFile(String fileName)
        {
            var projectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            return File.ReadAllText(Path.Combine(projectPath, fileName));
        }
    }
}
