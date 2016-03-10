using System;
using System.Collections.Generic;
using System.Linq;
using Quartz;
using iTimeService.Entities;
using iTimeService.Concrete;
using iTimeService.Services;
using log4net;
using System.Configuration;
using System.Reflection;

namespace iTimeService.Jobs
{
    public class UpdateAttendanceRecordsJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        
        public UpdateAttendanceRecordsJob()
        {
        }
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log.Info("Starting job [UpdateAttendanceRecordsJob] at : " + DateTime.Now);
                int daysToUpdateVal = int.Parse(ConfigurationManager.AppSettings.Get("daysToUpdateVal").ToString());
                //set company id....TO DO: remember to retrieve from 
                //Common.Common.SetCompID();
                /*date to update
                 * typical scenario is that we will be updating yesterdays attendance record today
                 */
                //DateTime dtFrom = Convert.ToDateTime("2015-12-27 09:22:37.000");
                //DateTime dtTo = dtFrom.AddDays(1);

                DateTime dtFrom = Convert.ToDateTime(DateTime.Now.AddDays(-(daysToUpdateVal)));
                DateTime dtTo = dtFrom.AddDays(daysToUpdateVal);
                
                //Common.Common.SYSTEMTIME dt = Common.Common.GetTime();

                //get all enrolled employees
                using (var dbContext = new iTimeServiceContext())
                {
                    //get all enrolled, active employees
                    // for each empployee call processrawdata from attendanceupdateservice
                    //process raw data should return boolean
                    try
                    {
                        IEnumerable<EnrolledEmployee> employees = dbContext.Set<EnrolledEmployee>()
                                                                    .Where(x => x.workstatus == true)
                                                                    .Where(x => x.isdeleted == false)
                                                                    //.Where(x => x.enrollno == 266)
                                                                    .ToList();
                        foreach (DateTime punchDate in Common.Common.GetDateRange(dtFrom, dtTo))
                        {
                            foreach (var emp in employees)
                            {
                                Common.Common._compId = emp.compid;
                                IAttendanceUpdateService _service = new AttendanceUpdateService();
                                _service.ProcessRawData(emp, punchDate, punchDate.AddDays(1));
                                if (Common.Common._processedOk == true)
                                {
                                    log.Info("Attendance record of date [" + punchDate.Date + "] for employee : " +  emp.empcode + " successfully updated at " + DateTime.Now);
                                }
                                else
                                {
                                    log.Info("Error encountered while processing attendance record of date [" + punchDate.Date + "] for employee : " + emp.empcode + " at " + DateTime.Now, Common.Common._exception); 
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //throw new Exception(ex.Message, ex.InnerException);
                        log.Debug("Error occured at  [UpdateAttendanceRecordsJob] at :" + DateTime.UtcNow, ex.InnerException);
                    }
                }
            }
            catch (JobExecutionException ex)
            {
                log.Debug("Error occured at  [UpdateAttendanceRecordsJob] at :" + DateTime.UtcNow, ex.InnerException);
            }
        }
    }
}
