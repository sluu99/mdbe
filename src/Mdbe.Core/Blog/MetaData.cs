using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Mdbe.Core.Utils;
using System.Threading;

namespace Mdbe.Core.Blog
{
    /// <summary>
    /// An instance of this class contains meta data for a specific blog
    /// </summary>
    public class MetaData
    {
        public string Title { get; set; }

        public string Slug { get; set; }

        internal bool HasDate { get; set; }

        public DateTime Date { get; set; }

        public string FilePath { get; set; }
    }
}
