using Mdbe.Core;
using Mdbe.Core.Blog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mdbe.Web
{
    public static class Dependencies
    {
        public static Configuration Configuration { get; private set; }
        public static MetaDataProvider MetaDataProvider { get; private set; }
        public static PostProvider PostProvider { get; private set; }

        public static void ConfigDependencies()
        {
            Configuration = Mdbe.Core.Configuration.GetConfiguration();
            MetaDataProvider = new MetaDataProvider(Configuration.DataDirectory, Configuration.MetaDataCache);
            PostProvider = new PostProvider(MetaDataProvider);
        }

    }
}