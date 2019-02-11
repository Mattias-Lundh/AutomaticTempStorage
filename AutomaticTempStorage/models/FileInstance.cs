using System;
using System.Collections.Generic;
using System.Text;

namespace AutomaticTempStorage.models
{
    using System.IO;
    using System.Security.AccessControl;
    using System.Text.RegularExpressions;

    public class FileInstance
    {
        public string Name { get; set; }
        public DateTime Created { get; set; }

        public TargetFolder TargetFolder
        {
            get
            {
                if (this.Created < DateTime.Now.AddMilliseconds(10000))
                {
                    return TargetFolder.Temp;
                }else if (this.Created > DateTime.Now.AddMinutes(5))
                {
                    return TargetFolder.Month;
                }
                return TargetFolder.Week;
            }
        }

        public string Path { get; set; }
        public string ParentFolder
        {
            get
            {
                var parentFolder = new Regex(@".*\\([^\\]+)\\");
                return parentFolder.Match(this.Path).Groups[1].Value;
            }
        }
        public void Move()
        {
            this.Path = (this.Path.Substring(0, this.Path.Length - (this.Name.Length + this.ParentFolder.Length + 1)) + this.TargetFolder).ToLower();
        }
    }
    public enum TargetFolder {Temp = 1, Week = 2, Month = 3}
}
