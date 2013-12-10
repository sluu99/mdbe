using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdbe.Core.Utils
{
    public static class Contract
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="notNullObj"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentNullException">Thrown when object is null</exception>
        public static void NotNull(object notNullObj, string message)
        {
            if (notNullObj == null)
            {
                throw new ArgumentNullException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="message"></param>
        /// <exception cref="ArgumentException">Thrown when the string is null or whitespace</exception>
        public static void NotNullOrWhiteSpace(string str, string message)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                throw new ArgumentException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="message"></param>
        /// <exception cref="FileNotFoundException">Thrown when the file does not exists</exception>
        public static void FileExists(string file, string message)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException(message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="message"></param>
        /// <exception cref="DirectoryNotFound">Thrown when the directory does not exist</exception>
        public static void DirectoryExists(string directory, string message)
        {
            if (!Directory.Exists(directory))
            {
                throw new DirectoryNotFoundException(message);
            }
        }
    }
}
