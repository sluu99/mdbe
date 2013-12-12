using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mdbe.Core.Tests
{
    public class TestUtils
    {
        /// <summary>
        /// Get the current assembly's directory
        /// </summary>
        /// <returns></returns>
        public static string GetAssemblyDir()
        {
            return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }

    }
}
