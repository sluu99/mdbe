using Mdbe.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

            foreach (var fileName in Directory.GetFiles(_directory, "*.md"))
            {
                var metaData = new MetaData(fileName);
                var key = metaData.Slug.Trim().ToLower();
                _metaData.Add(key, metaData);
            }

            _metaDataTime = DateTime.UtcNow;
        }
    }
}
