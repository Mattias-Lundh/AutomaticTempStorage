using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutomaticTempStorage.services
{
    using System.Configuration;

    using AutomaticTempStorage.models;

    using Newtonsoft.Json;

    public class InputService
    {
        public ConfigurationModel Configuration()
        {
            return this.ParseConfigFile();
        }

        private ConfigurationModel ParseConfigFile()
        {
            var configdata = File.ReadAllText(Environment.CurrentDirectory + @"\config.json");
            var config = JsonConvert.DeserializeObject<ConfigurationModel>(configdata);
            return config;
        }
    }
}
