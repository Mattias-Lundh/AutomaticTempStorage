using System;
using Topshelf;
using AutomaticTempStorage.Cleaner;

namespace AutomaticTempStorage
{
    using AutomaticTempStorage.services;

    class Program
    {
        static void Main(string[] args)
        {
            var exitCode = HostFactory.Run(
                x =>
                {
                    x.Service<Heartbeat>(
                        s =>
                        {
                            s.ConstructUsing(heartbeat => new Heartbeat(new Cleaner.Cleaner(new InputService())));
                            s.WhenStarted(heartbeat => heartbeat.Start());
                            s.WhenStopped(heartbeat => heartbeat.Stop());
                        });

                    x.RunAsLocalSystem();

                    x.SetServiceName("AutomaticStorage");
                    x.SetDisplayName("Automatic Storage");
                    x.SetDescription("this service moves around files in the temp directory on a timetable");

                });

            int exitCodeValue = (int) Convert.ChangeType(exitCode, exitCode.GetTypeCode());
            Environment.ExitCode = exitCodeValue;
        }
    }
}
