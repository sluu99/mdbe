using Mdbe.Core.Blog;
using Mdbe.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mdbe.Web.Models
{
    public class ViewPostModel : AbstractModel
    {
        public Post Post { get; private set; }

        public ViewPostModel(string slug)
            : base()
        {
            Contract.NotNullOrWhiteSpace(slug, "Must specify slug");

            this.Post = Mdbe.Core.Blog.Post.Get(slug);
        }

    }
}