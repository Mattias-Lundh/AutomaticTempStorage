using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTempStorage.models
{
    using System.Security.AccessControl;

    public class FileInstance
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string ParentFolder { get; set; }
    }
}
