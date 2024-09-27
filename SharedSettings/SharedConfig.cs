using System;
using System.Collections.Generic;
using System.Text;

namespace SharedSettings
{
    public static class SharedConfig
    {
        public static IConfigurationRoot LoadSharedConfiguration(string evn)
        {
            IConfigurationRoot builder = new ConfigurationBuilder()
                .AddJsonFile("sharedsettings.json")
                .AddJsonFile($"sharedsettings.{evn}.json", optional: true, reloadOnChange: true)
                .Build();

            return builder;
        }
    }
}
