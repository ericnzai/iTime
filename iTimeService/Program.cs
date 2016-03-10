using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using iTimeService.Services;
using Topshelf.Quartz;
using Quartz;
using Quartz.Impl;
using iTimeService.Jobs;
using System.Configuration;
using System.Xml.XmlConfiguration;
using System.IO;
using log4net.Config;

namespace iTimeService
{
    public class Program
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Main(string[] args)
        {
            string readLogStartHr = ConfigurationManager.AppSettings.Get("readLogStartHr");
            string readLogStart = ConfigurationManager.AppSettings.Get("readLogStartMin");
            string triggerStart = ConfigurationManager.AppSettings.Get("triggerStart");
            int updateJobInterval = int.Parse(ConfigurationManager.AppSettings.Get("updateJobInterval").ToString());
            int readJobInterval = int.Parse(ConfigurationManager.AppSettings.Get("readJobInterval").ToString());
            //XmlConfigurator.ConfigureAndWatch(
            //new FileInfo(".\\Logs\\log4net.config"));
            //log4net.Config.XmlConfigurator.Configure();

            //log.Info("Application Started");
            HostFactory.New(x =>
                {
                    //x.UseLog4Net("~\\Logs\\logs.txt");
                    x.Service<iTimeMainService>(s =>
                        {
                            s.ConstructUsing(name => new iTimeMainService());
                            s.WhenStarted(its => 
                                {
                                its.Start();
                                }
                           );
                            s.WhenStopped(its => 
                                {
                                    its.Stop();
                                });
                            /*This job should handle:
                             * --------------------------------
                             * -downloading of logs from device 
                             * -inserting raw data (if not exists)
                             * -deleting of device data 
                             * ---------------------------------------------
                             */
                            s.ScheduleQuartzJob<iTimeMainService>(q =>
                               q.WithJob(() =>
                                   JobBuilder.Create<ReadInsertRawDataJob>()
                                   .WithIdentity("readLogs")
                                       ////.UsingJobData("executeDT",DateTime.UtcNow.ToString())
                                   .Build())

                                   .AddTrigger(() =>
                                       TriggerBuilder.Create()
                                       .WithDescription("Regular Reading of device logs")
                                           //.StartAt(DateTime.Parse(triggerStart))
                                       .StartNow()
                                       .WithSimpleSchedule(builder => builder
                                           .WithMisfireHandlingInstructionFireNow()
                                           .WithIntervalInMinutes(readJobInterval)
                                           .RepeatForever())
                                        .Build())
                               );
                            /*This job should handle:
                          * --------------------------------
                          * -updating of permanent/casual or contractual employee attendance records
                          * -job is NOT dependent on above job and must run whether rawdata has been 
                          * inseted  or not
                          * job should flag raw data as processed to minimize the records it has to process during 
                          * each execution
                          * ---------------------------------------------
                          */
                            s.ScheduleQuartzJob<iTimeMainService>(q =>
                               q.WithJob(() =>
                                   JobBuilder.Create<UpdateAttendanceRecordsJob>()
                                   .WithIdentity("updateAtt")
                                   
                                       ////.UsingJobData("executeDT",DateTime.UtcNow.ToString())
                                   .Build())

                                   .AddTrigger(() =>
                                       TriggerBuilder.Create()
                                       .WithDescription("Daily attendance records update")
                                           //.StartAt(DateTime.Parse(triggerStart))
                                       .StartNow()
                                       .WithSimpleSchedule(builder => builder
                                           .WithMisfireHandlingInstructionFireNow()
                                           .WithIntervalInMinutes(updateJobInterval)
                                           .RepeatForever())
                                        .Build())

                               );
                        });

                    x.RunAsLocalSystem();

                    x.SetDescription("iTime Service for reading and updating attendance data from biometric devices");
                    x.SetDisplayName("iTimeService");
                    x.SetServiceName("iTimeService");
                    x.StartAutomatically();
                    //x.EnablePauseAndContinue();
                  
                    x.EnableServiceRecovery(r =>
                    {
                        //you can have up to three of these
                        r.OnCrashOnly();
                        r.RestartService(2);
                        //the last one will act for all subsequent failures TO DO: try to call an email notifier
                        r.RunProgram(7, "ping google.com");
                    });
                    //x.DependsOnMsSql();

                }).Run();
        }
    }
}
