using System;
using System.Collections.Generic;
using System.Text;

namespace SharedSettings
{
    public static class SharedConfig
    {
        public static IConfigurationRoot LoadSharedConfiguration()
        {
            var builder = new ConfigurationBuilder()
                //.SetBasePath(basePath)
                .AddJsonFile("sharedsettings.json", optional: false, reloadOnChange: true);

            return builder.Build();
        }
    }
}
