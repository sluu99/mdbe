using Mdbe.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mdbe.Core.Blog
{
    public class PostProvider
    {
        MetaDataProvider _metaDataProvider = null;

        public PostProvider(MetaDataProvider metaDataProvider)
        {
            Contract.NotNull(metaDataProvider, "Meta data provider must not be null");
            _metaDataProvider = metaDataProvider;
        }


        /// <summary>
        /// Get a post by slug
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The post if found, or null otherwise</returns>
        public Post Get(string slug)
        {
            Contract.NotNullOrWhiteSpace(slug, "Slug must be specified");

            slug = slug.Trim().ToLower();
            var metaData = _metaDataProvider.Get(slug);


            if (metaData != null)
            {
                return Post.FromFile(metaData.FilePath);
            }

            return null;
        }
    }
}
