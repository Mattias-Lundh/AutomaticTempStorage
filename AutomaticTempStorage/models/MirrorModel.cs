using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTempStorage.models
{
    using System.Linq;
    using System.Security.AccessControl;

    public class MirrorModel
    {
        public List<FileInstance> Daily { get; set; }
        public List<FileInstance> Weekly { get; set; }
        public List<FileInstance> Monthly { get; set; }

        public List<FileInstance> All
        {
            get
            {
                var result = new List<FileInstance>();
                    result.AddRange(this.Daily);
                    result.AddRange(this.Weekly);
                    result.AddRange(this.Monthly);
                    return result;
            }
        }
    }
}
