using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Mdbe.Core
{
    public class Configuration
    {
        /// <summary>
        /// The blog title
        /// </summary>
        public string BlogTitle { get; private set; }

        /// <summary>
        /// The directory which the data is stored
        /// </summary>
        public string DataDirectory { get; private set; }

        /// <summary>
        /// When this configuration was acquired
        /// </summary>
        public DateTime ConfigurationTime { get; private set; }

        /// <summary>
        /// Number of minutes the configuration is cached
        /// </summary>
        public int ConfigurationCache { get; private set; }

        /// <summary>
        /// Number of minutes the meta data is cached
        /// </summary>
        public int MetaDataCache { get; private set; }

        /// <summary>
        /// Number of minutes the post data is cached
        /// </summary>
        public int PostCache { get; private set; }

        /// <summary>
        /// The disqus name
        /// </summary>
        public string DisqusName { get; private set; }


        private static Mutex s_mutex = new Mutex();
        private static Configuration s_configuration = null;

        /// <summary>
        /// Get the default configuration
        /// </summary>
        /// <returns></returns>
        public static Configuration GetConfiguration()
        {
            if (s_configuration == null)
            {
                s_mutex.WaitOne();

                if (s_configuration == null || // someone else could have fetched the configuration while we're waiting
                    (DateTime.UtcNow - s_configuration.ConfigurationTime).TotalMinutes > s_configuration.ConfigurationCache)
                {
                    DoGetConfiguration();
                }

                s_mutex.ReleaseMutex();
            }
            else if ((DateTime.UtcNow - s_configuration.ConfigurationTime).TotalMinutes > s_configuration.ConfigurationCache)
            {
                s_mutex.WaitOne();
                if ((DateTime.UtcNow - s_configuration.ConfigurationTime).TotalMinutes > s_configuration.ConfigurationCache)
                {
                    DoGetConfiguration();
                }
                s_mutex.ReleaseMutex();
            }

            return s_configuration;
        }

        /// <summary>
        /// Fetch the configuration
        /// </summary>
        private static void DoGetConfiguration()
        {
            if (s_configuration == null)
            {
                s_configuration = new Configuration();
            }

            s_configuration.BlogTitle = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.BlogTitle"];
            if (string.IsNullOrWhiteSpace(s_configuration.BlogTitle))
            {
                s_configuration.BlogTitle = "Markdown Blog Engine";
            }

            s_configuration.DataDirectory = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.DataDirectory"];
            if (string.IsNullOrWhiteSpace(s_configuration.DataDirectory))
            {
                s_configuration.DataDirectory = "~/App_Data";
            }
            if (s_configuration.DataDirectory[0] == '~' && HttpContext.Current != null)
            {
                s_configuration.DataDirectory = HttpContext.Current.Server.MapPath(s_configuration.DataDirectory);
            }

            var strConfigCache = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.ConfigurationCache"];
            if (!string.IsNullOrWhiteSpace(strConfigCache))
            {
                s_configuration.ConfigurationCache = int.Parse(strConfigCache);
            }
            if (s_configuration.ConfigurationCache < 0)
            {
                s_configuration.ConfigurationCache = 0;
            }

            var strMetaDataCache = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.MetaDataCache"];
            if (!string.IsNullOrWhiteSpace(strMetaDataCache))
            {
                s_configuration.MetaDataCache = int.Parse(strMetaDataCache);
            }
            if (s_configuration.MetaDataCache < 0)
            {
                s_configuration.MetaDataCache = 0;
            }

            var strPostCache = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.PostCache"];
            if (!string.IsNullOrWhiteSpace(strPostCache))
            {
                s_configuration.PostCache = int.Parse(strPostCache);
            }
            if (s_configuration.PostCache < 0)
            {
                s_configuration.PostCache = 0;
            }

            s_configuration.DisqusName = ConfigurationManager.AppSettings["Mdbe.Core.Configuration.DisqusName"];
            if (!string.IsNullOrEmpty(s_configuration.DisqusName))
            {
                s_configuration.DisqusName = s_configuration.DisqusName.Trim();
            }

            s_configuration.ConfigurationTime = DateTime.UtcNow;
        }
    }
}
