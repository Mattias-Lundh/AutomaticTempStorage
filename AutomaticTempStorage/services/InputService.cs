using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace AutomaticTempStorage.services
{
    using System.Configuration;

    using AutomaticTempStorage.models;

    public class InputService
    {
        public MirrorModel Mirror()
        {
            return this.ParseMirrorFile();
        }

        public ConfigurationModel Configuration()
        {
            return this.ParseConfigFile();
        }

        private ConfigurationModel ParseConfigFile()
        {
            return new ConfigurationModel();
        }

        private MirrorModel ParseMirrorFile()
        {
            return new MirrorModel();
        }
    }
}
