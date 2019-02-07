namespace AutomaticTempStorage.Cleaner
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using AutomaticTempStorage.models;
    using AutomaticTempStorage.schedule;
    using AutomaticTempStorage.services;

    using Newtonsoft.Json;

    public class Cleaner : IHeartbeatSubscriber
    {
        private InputService inputService;
        public Cleaner(InputService inputService)
        {
            this.inputService = inputService;
        }
        public void OnElapsed()
        {
            var dir = this.inputService.Configuration().RootDirectory;
            var files = this.LocateFiles(dir);
            var mirror = this.CreateMirror(files);
            this.UpdateMirror(mirror);
            this.MoveFilesOnDisk();
        }

        private void MoveFilesOnDisk()
        {
            var mirror = this.LoadMirror();
            var files = this.LocateFiles(this.inputService.Configuration().RootDirectory);
            files.ForEach(
                file =>
                {
                    var mirrorFile = mirror.All.FirstOrDefault(f => f.Name == file.Name);
                    if (null != mirrorFile)
                    {
                        var content = File.ReadAllBytes(file.Path);
                        File.WriteAllBytes(mirrorFile.Path,content);
                    }
                });
        }

        private List<FileInstance> LocateFiles(string dir)
        {
           var result = new List<FileInstance>();
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
                          Name = e,
                          ParentFolder = Directory.GetParent(e).Name,
                          Path = e
                      });
                   }
               });
           return result;
        }

        private MirrorModel CreateMirror(List<FileInstance> files)
        {
            var result = new MirrorModel();
            files.ForEach(
                file =>
                {
                    switch (file.ParentFolder)
                    {
                        case "temp":
                            result.Daily.Add(file);
                            break;
                        case "week":
                            result.Weekly.Add(file);
                            break;
                        case "month":
                            result.Monthly.Add(file);
                            break;
                    }
                });
            return result;
        }

        private void SaveMirror(MirrorModel mirror)
        {
            File.WriteAllText(this.inputService.Configuration().RootDirectory + @"\..\mirror.json",JsonConvert.SerializeObject(mirror));
        }

        private MirrorModel LoadMirror()
        {
            return JsonConvert.DeserializeObject<MirrorModel>(File.ReadAllText(this.inputService.Configuration().RootDirectory + @"\..\mirror.json")) ?? new MirrorModel();
        }

        private void UpdateMirror(MirrorModel mirror)
        {
            var newMirror = this.AddNewRecords(mirror);
            newMirror = this.MoveMirrorFiles(newMirror);
            if(newMirror.GetHash() != this.LoadMirror().GetHash())
            {
                this.SaveMirror(newMirror);
                var a = "";
            }
        }

        private MirrorModel AddNewRecords(MirrorModel newMirrorData)
        {
            var oldMirrorData = this.LoadMirror();
            var updatedMirror = new MirrorModel
            {
                Daily = oldMirrorData.Daily,
                Weekly = oldMirrorData.Weekly,
                Monthly = oldMirrorData.Monthly
            };


            updatedMirror.Daily.AddRange(
                    newMirrorData.Daily.Select(file => oldMirrorData.Daily.Contains(file)
                        ? null
                        : file
                        ));

            updatedMirror.Weekly.AddRange(
                newMirrorData.Weekly.Select(file => oldMirrorData.Weekly.Contains(file)
                    ? null
                    : file
                ));

            updatedMirror.Monthly.AddRange(
                newMirrorData.Monthly.Select(file => oldMirrorData.Monthly.Contains(file)
                    ? null
                    : file
                ));

            return updatedMirror;
        }

        private MirrorModel MoveMirrorFiles(MirrorModel mirror)
        {
            var result = mirror;

            result.Daily.Where(f => f.Created > DateTime.Now.AddMilliseconds(5000)).ToList().ForEach(f =>
            {
                result.Weekly.Add(f);
                result.Daily.Remove(f);
            });
            result.Weekly.Where(f => f.Created > DateTime.Now.AddMinutes(1)).ToList().ForEach(f =>
            {
                result.Monthly.Add(f);
                result.Weekly.Remove(f);
            });
            result.Monthly.Where(f => f.Created > DateTime.Now.AddMinutes(5)).ToList().ForEach(f =>
            {
                result.Monthly.Remove(f);
            });

            return result;
        }
    }
}
