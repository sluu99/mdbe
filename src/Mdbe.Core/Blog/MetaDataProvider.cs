using Mdbe.Core.Exceptions;
using Mdbe.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Mdbe.Core.Blog
{
    public class MetaDataProvider
    {
        private string _directory = null;
        private string _postsDir = null;
        private DateTime _metaDataTime; // the last time meta data was fetched (UTC)
        private Dictionary<string, MetaData> _metaData = null;
        private Mutex _mutex = new Mutex();
        private int _cacheMins = 0;

        /// <summary>
        /// Create a new meta data provide that works with the specified directory
        /// </summary>
        /// <param name="directory">Data directory (e.g. App_Data)</param>
        /// <param name="cacheMins">Number of minutes to cache the meta data</param>
        public MetaDataProvider(string directory, int cacheMins)
        {
            Contract.NotNullOrWhiteSpace(directory, "Directory must be specified.");
            Contract.DirectoryExists(directory, string.Format("{0} does not exist", directory));

            _directory = directory;
            _postsDir = Path.Combine(directory, "Posts");

            Contract.DirectoryExists(_postsDir, string.Format("Cannot find 'Posts' directory in {0}", _directory));

            _cacheMins = cacheMins;
        }

        /// <summary>
        /// Get all the meta data for the posts that are directly inside the specified folder
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MetaData> GetAll()
        {
            if (_metaData == null)
            {
                _mutex.WaitOne();

                if (_metaData == null) // someone else could have fetched the meta data while we're waiting
                {
                    DoGetAllMetaData();
                }

                _mutex.ReleaseMutex();
            }
            else if ((DateTime.UtcNow - _metaDataTime).TotalMinutes > _cacheMins)
            {
                _mutex.WaitOne();

                if ((DateTime.UtcNow - _metaDataTime).TotalMinutes > _cacheMins)
                {
                    DoGetAllMetaData();
                }

                _mutex.ReleaseMutex();
            }

            return _metaData.Values.ToArray();
        }

        /// <summary>
        /// Get a post meta data by the slug
        /// </summary>
        /// <param name="slug">The slug string</param>
        /// <returns>The meta data if found; null otherwise</returns>
        public MetaData Get(string slug)
        {
            Contract.NotNullOrWhiteSpace(slug, "Slug must be specified");
            slug = slug.Trim().ToLower();

            GetAll();

            MetaData md = null;

            _mutex.WaitOne();

            if (_metaData.ContainsKey(slug))
            {
                md = _metaData[slug];
            }

            _mutex.ReleaseMutex();

            return md;
        }

        /// <summary>
        /// Fetch the meta data from files in the specified folder
        /// </summary>
        private void DoGetAllMetaData()
        {
            if (_metaData == null)
            {
                _metaData = new Dictionary<string, MetaData>();
            }
            else
            {
                _metaData.Clear();
            }

            foreach (var fileName in Directory.GetFiles(_postsDir, "*.md"))
            {
                var metaData = new MetaData();
                metaData.FilePath = fileName;
                using (var stream = new StreamReader(fileName))
                {
                    ParseStream(stream, metaData);
                }
                var key = metaData.Slug.Trim().ToLower();
                _metaData.Add(key, metaData);
            }

            _metaDataTime = DateTime.UtcNow;
        }

        /// <summary>
        /// Parse the meta data from a stream and set it to the meta data instance
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="meta">The MetaData instance</param>
        public static void ParseStream(StreamReader stream, MetaData meta)
        {
            Contract.NotNull(stream, "File stream cannot be null");
            Contract.NotNull(meta, "MetaData instance cannot be null");

            string line = null;
            while ((line = stream.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    // a blank line means the meta data ends and content starts
                    break;
                }
                ParseLine(line, meta);
            }

            Infer(meta);
        }

        /// <summary>
        /// Automatically infer information inside a MetaData instance, a.k.a fill in the missing information
        /// based on what's already there (e.g. use file name as slug)
        /// </summary>
        /// <param name="meta"></param>
        /// <exception cref="MissingDataException">Thrown when a piece of data needs to be inferred and the necessary piece is missing</exception>
        private static void Infer(MetaData meta)
        {
            Contract.NotNull(meta, "MetaData instance must not be null");
                       

            if (string.IsNullOrWhiteSpace(meta.Title))
            {
                if (string.IsNullOrWhiteSpace(meta.FilePath))
                    throw new MissingDataException("Cannot infer Title without FilePath");
                meta.Title = Path.GetFileNameWithoutExtension(meta.FilePath);
            }

            if (string.IsNullOrWhiteSpace(meta.Slug))
            {
                if (string.IsNullOrWhiteSpace(meta.FilePath))
                    throw new MissingDataException("Cannot infer Slug without FilePath");
                meta.Slug = Path.GetFileNameWithoutExtension(meta.FilePath);
            }

            if (!meta.HasDate)
            {
                if (string.IsNullOrWhiteSpace(meta.FilePath))
                    throw new MissingDataException("Cannot infer Date without FilePath");

                meta.Date = new FileInfo(meta.FilePath).CreationTime;
                meta.HasDate = true;
            }
        }

        /// <summary>
        /// Parse a line and set the data to an appropriate field
        /// </summary>
        /// <param name="line"></param>
        /// <param name="meta"></param>
        private static void ParseLine(string line, MetaData meta)
        {
            Contract.NotNullOrWhiteSpace(line, "Line must not be null or white space");
            Contract.NotNull(meta, "MetaData instance cannot be null");

            int pos = line.IndexOf(':');
            if (pos != -1)
            {
                var key = line.Substring(0, pos).Trim().ToLower();
                var value = line.Substring(pos + 1);

                if ("title".Equals(key))
                {
                    meta.Title = value.Trim();
                }
                else if ("slug".Equals(key))
                {
                    meta.Slug = value.Trim().ToLower();
                }
                else if ("date".Equals(key))
                {
                    meta.HasDate = true;
                    meta.Date = DateTime.Parse(value.Trim());
                }
            }
        }
    }
}
