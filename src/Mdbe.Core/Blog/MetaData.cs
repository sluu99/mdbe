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
        private string _slug;
        private string _title;
        private bool _hasDate = false;
        private DateTime _date;

        /// <summary>
        /// Path to the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Title of the blog post
        /// </summary>
        public string Title
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_title))
                {
                    return this.Slug;
                }
                return _title;
            }
            set
            {
                if (_title != value)
                {
                    _title = value;
                }
            }
        }


        /// <summary>
        /// The slug path to the post
        /// </summary>
        public string Slug
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_slug))
                {
                    return Path.GetFileNameWithoutExtension(FilePath);
                }
                return _slug;
            }
            set
            {
                if (value != _slug)
                {
                    _slug = value;
                }
            }
        }

        public DateTime Date
        {
            get
            {
                if (!_hasDate) return new FileInfo(FilePath).CreationTime;
                return _date;
            }
            set { _date = value; _hasDate = true; }
        }

        public MetaData()
        {
        }
        
        /// <summary>
        /// Create a new MetaData instance from a specific file
        /// </summary>
        /// <param name="filePath"></param>
        public MetaData(string filePath)
        {
            Contract.NotNullOrWhiteSpace(filePath, "File path must be specified");
            Contract.FileExists(filePath, string.Format("{0} does not exist", filePath));

            this.FilePath = filePath;
            using (var stream = new StreamReader(filePath))
            {
                this.ParseStream(stream);
            }
        }

        /// <summary>
        /// Parse the meta data from a stream
        /// </summary>
        /// <param name="stream"></param>
        public void ParseStream(StreamReader stream)
        {
            Contract.NotNull(stream, "File stream cannot be null");

            string line = null;
            while ((line = stream.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // a blank line means the meta data ends and content starts
                    break;
                }
                this.ParseLine(line);
            }
        }

        /// <summary>
        /// Parse a line and set the data to an appropriate field
        /// </summary>
        /// <param name="line"></param>
        private void ParseLine(string line)
        {
            Contract.NotNullOrWhiteSpace(line, "Line must not be null or white space");

            int pos = line.IndexOf(':');
            if (pos != -1)
            {
                var key = line.Substring(0, pos).Trim().ToLower();
                var value = line.Substring(pos + 1);

                if ("title".Equals(key))
                {
                    this.Title = value.Trim();
                }
                else if ("slug".Equals(key))
                {
                    this.Slug = value.Trim().ToLower();
                }
                else if ("date".Equals(key))
                {
                    this.Date = DateTime.Parse(value.Trim());
                }
            }
        }
    }
}
