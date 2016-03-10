using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Entities;
using iTimeService.Concrete;
using iTimeService.AttendanceLogic;
using log4net;
using System.Reflection;
using iTimeService.Common;
using System.Runtime.Serialization;
using System.ServiceModel;
using iTimeService.Contracts;
using System.Data.Entity;

using Quartz;
using Quartz.Listener;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System.Data;
namespace iTimeService.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = false,
        AddressFilterMode = AddressFilterMode.Any,
        InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AttendanceUpdateService : IAttendanceUpdateService
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
       
        IAttendanceLogic logic = null;
        IUnitOfWork unitOfWork;
        public AttendanceUpdateService()
        {
            unitOfWork = new UnitOfWork();
        }
        

        public ProcessAttendanceResult ProcessAtt(string dtFrom, string dtTo, string strEmpIds, string machineName, string ipAddress)
        {
            log4net.Config.XmlConfigurator.Configure();


          
            var dtF = dtFrom.ToDate();
            if (!dtF.HasValue)
            {
                _log.Info("Client Machine Attendance Processing Request Failed: Conversion Error");
                return CouldNotConvertToDateException(dtFrom);
            }
            var dtT = dtTo.ToDate();
            if (!dtT.HasValue)
            {
                _log.Info("Client Machine Attendance Processing Request Failed: Conversion Error");
                return CouldNotConvertToDateException(dtTo);

            }
            string strMachineIP = "[" + machineName + " - " + ipAddress + "]";
            _log.Info(strMachineIP + " Attendance Processing Request Started at " + DateTime.Now +" for Date Range : " + dtF.ToString() + " To " + dtT.ToString() );
            //------------------------------------------------------------------------------------------
            //Pause currently running scheduled attendance job to avoid db update concurrency conflicts
            //-------------------------------------------------------------------------------------------
            IScheduler sched = new StdSchedulerFactory().GetScheduler();
            //DataTable jobsTable = GetJobs(sched);
            JobKey updateJobKey = null;
            var executingJobs = sched.GetCurrentlyExecutingJobs();
            foreach (var job in executingJobs)
            {
                JobKey jobKey = job.JobDetail.Key;
                updateJobKey = jobKey;
                if (jobKey.Name == "updateAtt")
                {
                    sched.PauseJob(jobKey);
                    break;
                }
            }
            //---------------------------------------------------------------------------------------------
            // -----To do : If no job currently running check if its about to start and reschedule the job
            //---------------------------------------------------------------------------------------------
            string[] EmpIds = GetPostedEmpIds(strEmpIds);
           
            double empCount = 0;
            foreach (var empId in EmpIds)
           {
                int employeeId = Convert.ToInt16(empId);
                EnrolledEmployee emp = unitOfWork.EnrolledEmployees.All()
                                    .Where(x => x.workstatus == true)
                                    .Where(x => x.isdeleted == false)
                                    .Where(x => x.id == employeeId)
                                    .FirstOrDefault();
                if (emp != null)
                {
                    string tableName = GetCurrentAttTable(emp, DateTime.Parse(dtF.ToString()));
                    foreach (DateTime punchDate in Common.Common.GetDateRange(DateTime.Parse(dtFrom.ToString()), DateTime.Parse(dtTo.ToString())))
                    {
                        empCount++;
                        ProcessRawData(emp, punchDate, punchDate.AddDays(1),tableName);
                        if (Common.Common._processedOk == false)
                            _log.Info( "Exception encountered while processing attendance update for :" + strMachineIP + " at " + DateTime.Now, Common.Common._exception);
                       
                    }
                }
            }
            _log.Info(strMachineIP + " Attendance Processing Request Finished at " + DateTime.Now + ".  No of Attendance Records Updated : " + empCount + " From " + dtFrom.ToString() + " to " + dtTo.ToString());
            //---------Resume paused job Here..---------------------
            if (updateJobKey != null) sched.ResumeJob(updateJobKey);
         
            return new ProcessAttendanceResult
            {
                Count = empCount,
                Message = "Attendance has been processed successfully! Dates: " + dtFrom + " to  " + dtTo
            };
        }
        private  string GetCurrentAttTable(EnrolledEmployee emp, DateTime dtAttend)
        {
            try
            {
                AttendanceBase objAtt ;
                string strTableName = string.Empty;
                if (emp.type == enEmpType.Casual)
                {
                   
                     objAtt = unitOfWork.AttCasuals.All()
                                    .Where(x => x.attenddt.Month == dtAttend.Date.Month)
                                    .FirstOrDefault();
                     if (objAtt != null)
                     {
                         strTableName = "Trn_AttCasual";
                     }
                     else strTableName = "Trn_AttCasual_" + dtAttend.ToString("yyyyMM");

                }
                else if (emp.type == enEmpType.Permanent)
                {
                    strTableName = "Trn_AttPermanent";
                    objAtt = unitOfWork.AttPermanents.All()
                                    .Where(x => x.attenddt.Month == dtAttend.Date.Month)
                                    .FirstOrDefault();
                    if (objAtt != null)
                    {
                        strTableName = "Trn_AttPermanent";
                    }
                    else strTableName = "Trn_AttPermanent_" + dtAttend.ToString("yyyyMM");
                }
                else if (emp.type == enEmpType.Contract)
                {
                    objAtt = unitOfWork.AttContracts.All()
                                    .Where(x => x.attenddt.Month == dtAttend.Date.Month)
                                    .FirstOrDefault();
                    if (objAtt != null)
                    {
                        strTableName = "Trn_AttContract";
                    }
                    else strTableName = "Trn_AttContract_" + dtAttend.ToString("yyyyMM");
                }
                return strTableName;
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
                return string.Empty;
            }
        }
        private static ProcessAttendanceResult CouldNotConvertToDateException(string input)
        {
            return new ProcessAttendanceResult
            {
                Count = -1,
                Message = "Could not convert '" + input + "' to datetime datatype. Contact system admin"
            };
        }
        public void ProcessRawData(EnrolledEmployee emp, DateTime dtFrom,DateTime dtTo, string tableName = "")
        {
            try
            {
                //ShiftType shift = new ShiftType();
                enClockMode clockMode; 
                //IEnumerable<RawData> rData = this.GetRawData(false,emp.enrollno,dtFrom,dtTo).ToList();
                IEnumerable<RawData> rData;
                using (var db = new iTimeServiceContext())
                {
                    rData = db.Set<RawData>()
                                .Where(x => x.ENROLL_NO == emp.enrollno)
                                .Where(x => DbFunctions.TruncateTime(x.PUNCH_TIME) >= dtFrom.Date &&
                                    DbFunctions.TruncateTime(x.PUNCH_TIME) <= dtTo.Date)
                                .OrderBy(x => x.PUNCH_TIME)
                                .ToList();
                }
                //if (rData.Count() > 0)
                //{
                    if (emp.shift > 0 )
                    {
                        Common.Common.SetShiftParams(dtFrom, emp.ShiftType);
                        clockMode = emp.ShiftType.clockmode;
                        //foreach (DateTime dateToProcess in Common.Common.GetDateRange(dtFrom,dtTo))
                        //{
                            if (clockMode == enClockMode.Schedules)
                            {
                               logic = new UseSchedulesLogic(emp, rData, dtFrom);
                            }
                            else if (clockMode == enClockMode.StatusCode)
                            {
                                logic = new UseStatusLogic(emp, rData, dtFrom);
                            }
                            else if (clockMode == enClockMode.Device)
                            {
                                logic = new UseDeviceLogic(emp, rData,  dtFrom);
                            }
                            else if (clockMode == enClockMode.WorkCodes)
                            {
                                logic = new UseWorkCodeLogic(emp, rData, dtFrom);
                            }
                            if (logic != null)
                            {
                                ClockModeLogicProvider _provider = new ClockModeLogicProvider(logic, emp.ShiftType.type);
                                _provider.ProcessAttendance(tableName);
                            }
                        //}
                    }
                //}
                //else
                //{
                //}
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
                //_log.Debug("ProcessRawData encountered a problem : " + ex.InnerException);
            }
        }
        private static string[] GetPostedEmpIds(string input)
        {
            char[] array = new Char[1];
            array[0] = Char.Parse(",");
            string[] parts = input.Split(array);
            return parts;
        }
        private static  DataTable GetJobs(IScheduler sched)
        {
            DataTable table = new DataTable();
            table.Columns.Add("GroupName");
            table.Columns.Add("JobName");
            table.Columns.Add("JobDescription");
            table.Columns.Add("TriggerName");
            table.Columns.Add("TriggerGroupName");
            table.Columns.Add("TriggerType");
            table.Columns.Add("TriggerState");
            table.Columns.Add("NextFireTime");
            table.Columns.Add("PreviousFireTime");
            var jobGroups = sched.GetJobGroupNames();

            foreach (string group in jobGroups)
            {
                var groupMatcher = GroupMatcher<JobKey>.GroupContains(group);
                var jobKeys = sched.GetJobKeys(groupMatcher);
                foreach (var jobKey in jobKeys)
                {
                    var detail = sched.GetJobDetail(jobKey);
                    var triggers = sched.GetTriggersOfJob(jobKey);
                    foreach (ITrigger trigger in triggers)
                    {
                        DataRow row = table.NewRow();
                        row["GroupName"] = group;
                        row["JobName"] = jobKey.Name;
                        row["JobDescription"] = detail.Description;
                        row["TriggerName"] = trigger.Key.Name;
                        row["TriggerGroupName"] = trigger.Key.Group;
                        row["TriggerType"] = trigger.GetType().Name;
                        row["TriggerState"] = sched.GetTriggerState(trigger.Key);
                        DateTimeOffset? nextFireTime = trigger.GetNextFireTimeUtc();
                        if (nextFireTime.HasValue)
                        {
                            row["NextFireTime"] = TimeZone.CurrentTimeZone.ToLocalTime(nextFireTime.Value.DateTime);
                        }

                        DateTimeOffset? previousFireTime = trigger.GetPreviousFireTimeUtc();
                        if (previousFireTime.HasValue)
                        {
                            row["PreviousFireTime"] = TimeZone.CurrentTimeZone.ToLocalTime(previousFireTime.Value.DateTime);
                        }

                        table.Rows.Add(row);
                    }
                }
            }
            return table;
        }
    }
}
