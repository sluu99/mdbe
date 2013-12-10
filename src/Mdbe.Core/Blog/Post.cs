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
    public class Post
    {
        private string _markdown;

        public MetaData MetaData { get; set; }

        /// <summary>
        /// When the post was fetched (UTC)
        /// </summary>
        public DateTime FetchedOn { get; private set; }

        /// <summary>
        /// The markdown content
        /// </summary>
        public string Markdown
        {
            get { return _markdown; }
            set
            {
                if (_markdown != value)
                {
                    _markdown = value;
                    this.Html = new MarkdownSharp.Markdown().Transform(_markdown);
                }
            }
        }

        /// <summary>
        /// The HTML content
        /// </summary>
        public string Html { get; private set; }

        /// <summary>
        /// Get a post by slug
        /// </summary>
        /// <param name="slug">The slug</param>
        /// <returns>The post if found, or null otherwise</returns>
        public static Post Get(string slug)
        {
            Contract.NotNullOrWhiteSpace(slug, "Slug must be specified");

            slug = slug.Trim().ToLower();
            var metaData = MetaData.Get(slug);
                       

            if (metaData != null)
            {
                var post = Post.FromFile(metaData.FilePath);
                if (post != null)
                {
                    post.FetchedOn = DateTime.UtcNow;
                }

                return post;
            }

            return null;
        }


        /// <summary>
        /// Create a new Post instance from a file
        /// </summary>
        /// <param name="file">The file path</param>
        /// <returns></returns>
        private static Post FromFile(string file)
        {
            Contract.NotNullOrWhiteSpace(file, "File name must be specified");
            Contract.FileExists(file, string.Format("{0} does not exist", file));

            var post = new Post();

            using (var stream = new StreamReader(file))
            {
                post.MetaData = new MetaData();
                post.MetaData.FilePath = file;
                post.MetaData.ParseStream(stream);

                // get the content
                StringBuilder sb = new StringBuilder();
                string line;
                while ((line = stream.ReadLine()) != null)
                {
                    sb.AppendLine(line);
                }
                post.Markdown = sb.ToString();
            }

            return post;
        }
    }
}
