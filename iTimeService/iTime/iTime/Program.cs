using System;
using System.Collections.Generic;
using System.Linq;
using Topshelf;
using iTimeService.Services;
using Topshelf.Quartz;
using Quartz;
using iTimeService.Jobs;
using System.Configuration;
using iTimeService;




namespace iTime
{
    public class Program
    {
        //app.config settings
        private static readonly int updateJobInterval = int.Parse(ConfigurationManager.AppSettings.Get("updateJobInterval").ToString());
        private static readonly int readJobInterval = int.Parse(ConfigurationManager.AppSettings.Get("readJobInterval").ToString());
        private static readonly int truncateLogsInterval = int.Parse(ConfigurationManager.AppSettings.Get("truncateLogsInterval").ToString());
        private static readonly string serviceUri = ConfigurationManager.AppSettings.Get("serviceUri").ToString();
        public static void Main(string[] args)
        {
            var host = HostFactory.New(c =>
              {
                  c.Service<iTimeServiceWrapper<AttendanceUpdateService, IAttendanceUpdateService>>(s =>
                      {
                          s.ConstructUsing(x => new iTimeServiceWrapper<AttendanceUpdateService, IAttendanceUpdateService>("iTimeService",serviceUri));
                          s.WhenStarted(service => service.Start());
                          s.WhenStopped(service => service.Stop());

                          s.ScheduleQuartzJob<iTimeServiceWrapper<AttendanceUpdateService, IAttendanceUpdateService>>(q =>
                            q.WithJob(() =>
                                JobBuilder.Create<ReadInsertRawDataJob>()
                                .WithIdentity("readLogs")
                                .Build())

                                .AddTrigger(() =>
                                    TriggerBuilder.Create()
                                    .WithDescription("Regular Reading of device logs")
                                    .StartNow()
                                    .WithSimpleSchedule(builder => builder
                                        .WithMisfireHandlingInstructionFireNow()
                                        .WithIntervalInMinutes(readJobInterval)
                                        .RepeatForever())
                                     .Build())
                            );

                          s.ScheduleQuartzJob<iTimeServiceWrapper<AttendanceUpdateService, IAttendanceUpdateService>>(q =>
                            q.WithJob(() =>
                                JobBuilder.Create<UpdateAttendanceRecordsJob>()
                                .WithIdentity("updateAtt")
                                .Build())
                                .AddTrigger(() =>
                                    TriggerBuilder.Create()
                                    .WithDescription("Daily attendance records update")
                                    .StartNow()
                                    .WithSimpleSchedule(builder => builder
                                        .WithMisfireHandlingInstructionFireNow()
                                        .WithIntervalInMinutes(updateJobInterval)
                                        .RepeatForever())
                                     .Build())

                            );
                          s.ScheduleQuartzJob<iTimeServiceWrapper<AttendanceUpdateService, IAttendanceUpdateService>>(q =>
                           q.WithJob(() =>
                               JobBuilder.Create<TruncateLogDataJob>()
                               .WithIdentity("truncateLogs")
                               .Build())

                               .AddTrigger(() =>
                                   TriggerBuilder.Create()
                                   .WithDescription("Truncate logs data")
                                   .StartNow()
                                   .WithSimpleSchedule(builder => builder
                                       .WithMisfireHandlingInstructionFireNow()
                                       .WithIntervalInHours(truncateLogsInterval)
                                       .RepeatForever())
                                    .Build())

                           );
                      });
                  c.RunAsLocalSystem();
                  c.SetDescription("iTime Service for reading and updating attendance data from biometric devices");
                  c.SetDisplayName("iTimeService");
                  c.SetServiceName("iTimeService");
              });
            Console.WriteLine("Hosting.......");
            host.Run();
            Console.WriteLine("Done Hosting.......");
        }
       
    }
}
