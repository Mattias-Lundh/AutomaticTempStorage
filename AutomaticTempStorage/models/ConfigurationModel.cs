using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTempStorage.models
{
    public class ConfigurationModel
    {
        public string RootDirectory;
        public IEnumerable<Frequency> Shedule { get; set; }
    }
}
