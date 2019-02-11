namespace AutomaticTempStorage.Cleaner
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    using AutomaticTempStorage.models;
    using AutomaticTempStorage.schedule;
    using AutomaticTempStorage.services;

    using Newtonsoft.Json;

    public class Cleaner : IHeartbeatSubscriber
    {
        private InputService _inputService;
        private MirrorModel _mirror;
        public Cleaner(InputService inputService)
        {
            this._inputService = inputService;
            this._mirror = this.LoadMirror();
        }
        public void OnElapsed()
        {
            var dir = this._inputService.Configuration().RootDirectory;
            var files = this.LocateFiles(dir);
            this.UpdateMirror(files);
            this.MoveFilesOnDisk();
        }

        private void MoveFilesOnDisk()
        {

            var test = File.ReadAllText(@"C:\aaamyfiles\temp\test.txt");
            File.WriteAllText(@"C:\aaamyfiles\temp\newfolder\test.txt", test);

            //var files = this.LocateFiles(this._inputService.Configuration().RootDirectory);
            //files.ForEach(
            //    file =>
            //    {
            //        var mirrorFile = this._mirror.All.FirstOrDefault(f => f.Name == file.Name);
            //        if (null != mirrorFile)
            //        {
            //            var content = File.ReadAllBytes(file.Path);

            //            File.WriteAllBytes(mirrorFile.Path,content);
            //        }
            //    });
        }

        private List<FileInstance> LocateFiles(string dir)
        {
           var result = new List<FileInstance>();
           var filename = new Regex(@"[\w\s]+(?:\.\w+)*$");
           Directory.GetFileSystemEntries(dir).ToList().ForEach(
               e =>
               {
                   if (File.GetAttributes(e).HasFlag(FileAttributes.Directory))
                   {
                       result.AddRange(this.LocateFiles(dir));
                   }
                   else
                   { 
                      result.Add(new FileInstance
                      {
                          Created = DateTime.Now,
                          Name = filename.Match(e).Value,
                          Path = e
                      });
                   }
               });
           return result;
        }

        private void UpdateMirror(List<FileInstance> files)
        {
            // new mirror based on current file system state
            var newMirror = new MirrorModel();
            files.ForEach(
                file =>
                {
                    newMirror.AddFile(file);
                    
                }
           );

            // add newly discovered files
            newMirror.All.ForEach(
                newFile =>
                {
                    if (null == newMirror.All.FirstOrDefault(oldFile => oldFile.Path == newFile.Path))
                    {
                        this._mirror.AddFile(newFile);
                    }
                }
                );

            this._mirror = newMirror;
            // rearrange file locations in mirror
            this._mirror.All.ForEach(
                file => { file.Move(); });

            this.SaveMirror();
        }

        private void SaveMirror()
        {
            File.WriteAllText(this._inputService.Configuration().RootDirectory + @"\..\mirror.json",JsonConvert.SerializeObject(this._mirror));
        }

        private MirrorModel LoadMirror()
        {
            return JsonConvert.DeserializeObject<MirrorModel>(File.ReadAllText(this._inputService.Configuration().RootDirectory + @"\..\mirror.json")) ?? new MirrorModel();
        }
    }
}
