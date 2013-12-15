using Mdbe.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
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
                var post = CreatePostFromFile(metaData.FilePath);
                post.MetaData = metaData;

                return post;
            }

            return null;
        }

        /// <summary>
        /// Create a new Post instance from a file
        /// </summary>
        /// <param name="file">The file path</param>
        /// <returns></returns>
        private static Post CreatePostFromFile(string file)
        {
            Contract.NotNullOrWhiteSpace(file, "File name must be specified");
            Contract.FileExists(file, string.Format("{0} does not exist", file));

            var post = new Post();

            using (var stream = new StreamReader(file))
            {
                post.MetaData = new MetaData();
                post.MetaData.FilePath = file;

                string line;

                // skip the meta data lines
                while ((line = stream.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }
                }

                // get the content
                StringBuilder sb = new StringBuilder();
                
                while ((line = stream.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                post.Markdown = sb.ToString();
                post.Html = new MarkdownSharp.Markdown().Transform(post.Markdown);
            }

            return post;            
        }
    }
}
