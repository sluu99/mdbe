
namespace Mdbe.Core.Blog
{
    public class Post
    {

        public MetaData MetaData { get; set; }
        
        /// <summary>
        /// The markdown content
        /// </summary>
        public string Markdown { get; set; }

        /// <summary>
        /// The HTML content
        /// </summary>
        public string Html { get; set; }

    }
}
