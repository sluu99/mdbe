using Mdbe.Core.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace Mdbe.Web.Models
{
    public class IndexModel : AbstractModel
    {
        public IEnumerable<MetaData> MetaData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IndexModel() : base()
        {
            this.MetaData = Mdbe.Core.Blog.MetaData.GetAll().OrderByDescending(x => x.Date);
        }
    }
}