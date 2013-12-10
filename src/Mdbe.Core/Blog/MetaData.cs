﻿using System;
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
        private static DateTime s_metaDataTime; // the last time meta data was fetched (UTC)
        private static Dictionary<string, MetaData> s_metaData = null;
        private static Mutex s_mutex = new Mutex();

        private string _slug;
        private string _title;

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

        public DateTime Date { get; private set; }

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

        /// <summary>
        /// Get all the meta data for the posts that are directly inside the specified folder
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<MetaData> GetAll()
        {
            var config = Configuration.GetConfiguration();
            var folder = Path.Combine(config.DataDirectory, "Posts");

            Contract.DirectoryExists(folder, "Data directory does not contain a \"Posts\" folder");

            if (s_metaData == null)
            {
                s_mutex.WaitOne();

                if (s_metaData == null || // someone else could have fetched the meta data while we're waiting 
                    (DateTime.UtcNow - s_metaDataTime).TotalMinutes > config.MetaDataCache)
                {
                    DoGetAllMetaData(folder);
                }

                s_mutex.ReleaseMutex();
            }
            else if ((DateTime.UtcNow - s_metaDataTime).TotalMinutes > config.MetaDataCache)
            {
                s_mutex.WaitOne();

                if ((DateTime.UtcNow - s_metaDataTime).TotalMinutes > config.MetaDataCache)
                {
                    DoGetAllMetaData(folder);
                }

                s_mutex.ReleaseMutex();
            }

            return s_metaData.Values.ToArray();
        }

        /// <summary>
        /// Get a post meta data by the slug
        /// </summary>
        /// <param name="slug">The slug string</param>
        /// <returns>The meta data if found; null otherwise</returns>
        public static MetaData Get(string slug)
        {
            Contract.NotNullOrWhiteSpace(slug, "Slug must be specified");
            slug = slug.Trim().ToLower();

            MetaData.GetAll();

            MetaData md = null;

            s_mutex.WaitOne();

            if (s_metaData.ContainsKey(slug))
            {
                md = s_metaData[slug];
            }

            s_mutex.ReleaseMutex();

            return md;
        }

        /// <summary>
        /// Fetch the meta data from files in the specified folder
        /// </summary>
        /// <param name="folder"></param>
        private static void DoGetAllMetaData(string folder)
        {
            if (s_metaData == null)
            {
                s_metaData = new Dictionary<string, MetaData>();
            }
            else
            {
                s_metaData.Clear();
            }

            foreach (var fileName in Directory.GetFiles(folder, "*.md"))
            {
                var metaData = new MetaData(fileName);
                var key = metaData.Slug.Trim().ToLower();
                s_metaData.Add(key, metaData);
            }

            s_metaDataTime = DateTime.UtcNow;
        }
    }
}
