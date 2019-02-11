using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace AutomaticTempStorage.models
{
    using System.Linq;
    using System.Security.AccessControl;

    public class MirrorModel
    {
        public MirrorModel()
        {
            this.Daily = new List<FileInstance>();
            this.Weekly = new List<FileInstance>();
            this.Monthly = new List<FileInstance>();
        }

        public List<FileInstance> Daily { get; set; }
        public List<FileInstance> Weekly { get; set; }
        public List<FileInstance> Monthly { get; set; }

        [JsonIgnore]
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

        public override string ToString()
        {
            var result = string.Empty;
            this.All.ForEach(file => result += file.Name);
            return result;
        }

        public void AddFile(FileInstance file)
        {
            switch (file.ParentFolder)
            {
                case "temp":
                    this.Daily.Add(file);
                    break;
                case "week":
                    this.Weekly.Add(file);
                    break;
                case "month":
                    this.Monthly.Add(file);
                    break;
            }
        }

        public string GetHash()
        {
            return this.ComputeSha256Hash(this.ToString());
        }

        private string ComputeSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
