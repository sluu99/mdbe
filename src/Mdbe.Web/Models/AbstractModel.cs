using Mdbe.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mdbe.Web.Models
{
    public abstract class AbstractModel
    {
        /// <summary>
        /// Page configuration
        /// </summary>
        public Configuration Configuration { get; set; }


        public AbstractModel()
        {
            this.Configuration = Mdbe.Core.Configuration.GetConfiguration();
        }
    }
}