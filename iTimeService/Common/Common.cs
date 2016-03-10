using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Configuration;
using System.Runtime.InteropServices;
using iTimeService.Entities;
using iTimeService.Concrete;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace iTimeService.Common
{
    public static class Common
    {
        #region ServerTime
        [DllImport("coredll.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("coredll.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);


        public  struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;
        }

        #endregion ServerTime
        #region Fixed Shifts
        /*--------------------
         * shift related global properties
         * -----------------------------*/

        public static bool _processedOk = false;
        public static bool _insertedOk = false;

        public static Exception _exception;

        public static DateTime _timeInStart {get;set;}
        public static DateTime _timeIn { get; set; }
        public static DateTime _breakOutStart { get; set; }
        public static DateTime _breakOut { get; set; }
        public static DateTime _breakIn { get; set; }
        public static DateTime _breakInEnd { get; set; }
        public static DateTime _timeOut { get; set; }
        public static DateTime _timeOutEnd { get; set; }
        public static DateTime _timeInExact { get; set; }
        public static DateTime _timeOutStart { get; set; }
        public static DateTime _timeOutExact { get; set; }
        public static DateTime _shiftIn { get; set; }
        public static DateTime _shiftOut { get; set; }
        public static DateTime _breakOutDef { get; set; }
        public static DateTime _breakInDef { get; set; }
        //public readonly static DateTimeFormatInfo dateInfo { get; set { value = CultureInfo.CurrentCulture.DateTimeFormat; } }
        public static decimal _maxWorkHrs { get; set; }
        public static decimal _stdHrsND { get; set; }
        public static decimal _stdHrsHD { get; set; }
        public static decimal _graceIn { get; set; }
        public static DateTime _shiftGraceIn { get; set; }
        public static decimal _graceOut { get; set; }
        public static DateTime _shiftGraceOut { get; set; }
        public static TimeSpan _allowedBreak { get; set; }
        public static enOTND _otNDCalcMode { get; set; }
        public static int _shiftInPenalty { get; set; }
        public static int _shiftOutPenalty { get; set; }
        public static decimal _lunchPenalty { get; set; }
        public static decimal _lunchMinDuration { get; set; }
        public static decimal _minWorkHrs { get; set; }
        public static decimal _minOTHrs { get; set; }
        public static decimal _maxOTHrsN { get; set; }
        public static decimal _maxOTHrsD { get; set; }
        /*---------------------------
         * company settings
         *-----------------------------*/
        public static enGlobalTimeInOutMode _timeInMode { get; set; }
        public static enGlobalTimeInOutMode _timeOutMode { get; set; }
        public static enOTDef _otDef { get; set; }
        public static bool _isHoliday { get; set; }
        public static bool _isNextDayHol { get; set; }
        public static bool _useRoster { get; set; }
        public static int _compId { get; set; }
        #endregion Fixed Shifts
        #region DynamicShift

        public static string myDynamicShift { get; set; }
        public static DateTime _timeInStartD { get; set; }
        public static DateTime _timeInStartN { get; set; }
        public static DateTime _breakOutStartD { get; set; }
        public static DateTime _breakOutStartN { get; set; }
        public static DateTime _breakInEndD { get; set; }
        public static DateTime _breakInEndN { get; set; }
        public static DateTime _timeOutEndD { get; set; }
        public static DateTime _timeOutEndN { get; set; }
        public static DateTime _shiftInD { get; set; }
        public static DateTime _shiftInN { get; set; }
        public static DateTime _shiftOutD { get; set; }
        public static DateTime _shiftOutN { get; set; }
        public static DateTime _breakOutDefD { get; set; }
        public static DateTime _breakInDefD { get; set; }
        public static DateTime _breakOutDefN { get; set; }
        public static DateTime _breakInDefN { get; set; }

        public static decimal _maxWorkHrsD { get; set; }
        public static decimal _maxWorkHrsN { get; set; }

        public static decimal _stdHrsNDD { get; set; }
        public static decimal _stdHrsNDN { get; set; }
        public static decimal _stdHrsHDD { get; set; }
        public static decimal _stdHrsHDN { get; set; }

        public static decimal _graceInD { get; set; }
        public static decimal _graceInN { get; set; }
        public static DateTime _shiftGraceInD { get; set; }
        public static DateTime _shiftGraceInN { get; set; }
        public static decimal _graceOutD { get; set; }
        public static decimal _graceOutN { get; set; }
        public static DateTime _shiftGraceOutD { get; set; }
        public static DateTime _shiftGraceOutN { get; set; }
        public static DateTime _dayEnd { get; set; }
        public static DateTime _nextDayStart { get; set; }
        public static DateTime _nextDayUpto { get; set; }
        public static TimeSpan _allowedBreakD { get; set; }
        public static TimeSpan _allowedBreakN { get; set; }
        #endregion DynamicShift    

        public static SYSTEMTIME  GetTime()
        {
            SYSTEMTIME stime = new SYSTEMTIME();
            GetSystemTime(ref stime);
            return stime;
        }
        private static void SetTime()
        {
            // Call the native GetSystemTime method 
            // with the defined structure.ow
            SYSTEMTIME systime = new SYSTEMTIME();
            GetSystemTime(ref systime);

           
            systime.wHour = (ushort)(systime.wHour + 0 % 24);
            SetSystemTime(ref systime);
          
        }
        public static void SetCompID()
        {
            _compId = Convert.ToInt32(ConfigurationManager.AppSettings["compid"]);
        }
        public static List<DateTime> GetDateRange(DateTime StartingDate, DateTime EndingDate)
        {
            try
            {
                if (StartingDate > EndingDate)
                {
                    return null;
                }
                List<DateTime> rv = new List<DateTime>();
                DateTime tmpDate = StartingDate;
                do
                {
                    rv.Add(tmpDate);
                    tmpDate = tmpDate.AddDays(1);
                } while (tmpDate <= EndingDate);
                return rv;
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
                return null;
            }
        }
        public static bool isHolidayDate(DateTime punchDate,ShiftType shift)
        {
            try
            {
                bool isHol;
                using (var db = new iTimeServiceContext())
                {
                    isHol = db.Set<Holiday>()
                            .Where(x => DbFunctions.TruncateTime(x.holdate) == punchDate.Date)
                            .Where(x => x.compid == _compId && x.isactive == true && x.isdeleted == false)
                            .Count() > 0;
                }
                if (!isHol)
                {
                    if (punchDate.DayOfWeek == DayOfWeek.Saturday && shift.issatwkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Sunday && shift.issunwkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Monday && shift.ismonwkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Tuesday && shift.istuewkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Wednesday && shift.iswedwkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Thursday && shift.isthuwkday != true)
                    {
                        isHol = true;
                    }
                    else if (punchDate.DayOfWeek == DayOfWeek.Friday && shift.isfriwkday != true)
                    {
                        isHol = true;
                    }
                }
                return isHol;
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
                return false;
            }
        }
        public static  AttendanceBase ObjAttStatus(AttendanceBase objAtt, IUnitOfWork unitOfWork, EnrolledEmployee emp, DateTime dateToProcess)
        {
            try
            {
                LeaveApplication la = unitOfWork.LeaveApplications.All()
                                    .Where(x => dateToProcess.Date >= DbFunctions.TruncateTime(x.dtstart) 
                                        && dateToProcess.Date <= DbFunctions.TruncateTime(x.dtend))
                                    .Where(x => x.isapproved == true && x.isdeleted == false)
                                    .Where(x => x.empid == emp.id)
                                    .FirstOrDefault();

                if (la != null)
                {
                    objAtt.attstatus = la.LeaveType.attstatus;
                    if (la.LeaveType.attstatus == (int)enAttStatus.Sick)
                    {
                        if (la.ishalfday == true)
                        {
                            objAtt.attstatus = (int)enAttStatus.Sick2;
                        }
                    }
                    else if (la.LeaveType.attstatus == (int)enAttStatus.Off)
                    {
                        if (la.ishalfday == true)
                        {
                            objAtt.attstatus = (int)enAttStatus.Off2;
                        }
                    }

                }
                else objAtt.attstatus = 0;
                return objAtt;
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
                return objAtt;
            }
        }
        public static AttendanceBase InitializeAttendanceObject(IUnitOfWork unitOfWork, EnrolledEmployee emp, DateTime dateToProcess, string tableName)
        {
            try
            {
                AttendanceBase objAtt = new AttendanceBase();
                if (tableName == "")
                {
                    if (emp.type == enEmpType.Casual)
                    {
                        objAtt = unitOfWork.AttCasuals.All()
                                .Where(x => x.empid == emp.id)
                                .Where(x => DbFunctions.TruncateTime(x.attenddt) == dateToProcess.Date)
                                .FirstOrDefault();
                        if (objAtt == null) // insert new rec
                        {
                            objAtt = new AttCasual
                            {
                                empid = emp.id,
                                attenddt = dateToProcess.Date,
                                weekday = (int)dateToProcess.DayOfWeek,
                                attstatus = (int)enAttStatus.Work,
                                isNewRecord = true,
                                compid = emp.compid,
                                userid = 999
                            };
                        }
                        objAtt = (AttCasual)objAtt;
                    }
                    else if (emp.type == enEmpType.Permanent)
                    {
                        objAtt = unitOfWork.AttPermanents.All()
                                   .Where(x => x.empid == emp.id)
                                   .Where(x => DbFunctions.TruncateTime(x.attenddt) == dateToProcess.Date)
                                   .FirstOrDefault();

                        //if no record for this casual emp then initialize record and continue
                        if (objAtt == null) // insert new rec
                        {
                            objAtt = new AttPermanent
                            {
                                empid = emp.id,
                                attenddt = dateToProcess.Date,
                                weekday = (int)dateToProcess.DayOfWeek,
                                attstatus = (int)enAttStatus.Work,
                                isNewRecord = true,
                                compid = emp.compid,
                                userid = 999
                            };
                        }
                        objAtt = (AttPermanent)objAtt;
                    }
                    else if (emp.type == enEmpType.Contract)
                    {
                        objAtt = unitOfWork.AttContracts.All()
                                   .Where(x => x.empid == emp.id)
                                   .Where(x => DbFunctions.TruncateTime(x.attenddt) == dateToProcess.Date)
                                   .FirstOrDefault();


                        if (objAtt == null) // insert new rec
                        {
                            objAtt = new AttContract
                            {
                                empid = emp.id,
                                attenddt = dateToProcess.Date,
                                weekday = (int)dateToProcess.DayOfWeek,
                                attstatus = (int)enAttStatus.Work,
                                isNewRecord = true,
                                compid = emp.compid,
                                userid = 999
                            };
                        }
                        objAtt = (AttContract)objAtt;
                    }
                }
                else
                {
                    using (var db = new iTimeServiceContext())
                    {
                        object[] parameters = { tableName, emp.id, dateToProcess.Date };
                        string strFillObj = "SELECT * FROM " + tableName + "  WHERE empid =" + emp.id + " AND " +
                        " CONVERT(VARCHAR(10), attenddt,120) ='" + dateToProcess.Date.ToString("yyyy-MM-dd") + "'";

                        var obj = db.Database.SqlQuery<AttendanceBase>(strFillObj).FirstOrDefault();
                        objAtt = (AttendanceBase)obj;
                    }
                }
                return objAtt;
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
                return null;
            }
        }
        public static string GetShiftFromRoster(int dayVal, EmpRoster empRoster)
        {
            string strShift = "";

            switch (dayVal)
            {
                case 1:
                    strShift = empRoster.d1;
                    break;
                case 2:
                    strShift = empRoster.d2;
                    break;
                case 3:
                    strShift = empRoster.d3;
                    break;
                case 4:
                    strShift = empRoster.d4;
                    break;
                case 5:
                    strShift = empRoster.d5;
                    break;
                case 6:
                    strShift = empRoster.d6;
                    break;
                case 7:
                    strShift = empRoster.d7;
                    break;
                case 8:
                    strShift = empRoster.d8;
                    break;
                case 9:
                    strShift = empRoster.d9;
                    break;
                case 10:
                    strShift = empRoster.d10;
                    break;
                case 11:
                    strShift = empRoster.d11;
                    break;
                case 12:
                    strShift = empRoster.d12;
                    break;
                case 13:
                    strShift = empRoster.d13;
                    break;
                case 14:
                    strShift = empRoster.d14;
                    break;
                case 15:
                    strShift = empRoster.d15;
                    break;
                case 16:
                    strShift = empRoster.d16;
                    break;
                case 17:
                    strShift = empRoster.d17;
                    break;
                case 18:
                    strShift = empRoster.d18;
                    break;
                case 19:
                    strShift = empRoster.d19;
                    break;
                case 20:
                    strShift = empRoster.d20;
                    break;
                case 21:
                    strShift = empRoster.d21;
                    break;
                case 22:
                    strShift = empRoster.d22;
                    break;
                case 23:
                    strShift = empRoster.d23;
                    break;
                case 24:
                    strShift = empRoster.d24;
                    break;
                case 25:
                    strShift = empRoster.d25;
                    break;
                case 26:
                    strShift = empRoster.d26;
                    break;
                case 27:
                    strShift = empRoster.d27;
                    break;
                case 28:
                    strShift = empRoster.d28;
                    break;
                case 29:
                    strShift = empRoster.d29;
                    break;
                case 30:
                    strShift = empRoster.d30;
                    break;
                case 31:
                    strShift = empRoster.d31;
                    break;
            }
            return strShift;
        }
        public static void SetShiftParams(DateTime punchTime, ShiftType shift)
        {
            try
            {

                CompanySettings compSettings;
                using (var db = new iTimeServiceContext())
                {
                    compSettings = db.Set<CompanySettings>()
                                    .Where(x => x.compid == _compId)//use global 
                                    .Where(x => x.isdeleted == false)
                                    .FirstOrDefault();
                }
                _timeInMode = compSettings.timein;
                _timeOutMode = compSettings.timeout;
                _otDef = compSettings.otdefinition;
                _useRoster = compSettings.useroster;
                _graceIn = shift.gracein;
                _graceOut = shift.graceout;
                _otNDCalcMode = shift.otndtype;
                _shiftInPenalty = shift.shiftinpenalty;
                _shiftOutPenalty = shift.shiftoutpenalty;
                _lunchMinDuration = shift.lunchmin;
                _lunchPenalty = shift.lunchpenalty;
                _minWorkHrs = shift.minhrsdaily;
                //_maxOTHrs = shift.maxot;
                _minOTHrs = shift.minot;

                var timeEnd = "23:59:59";
                var timeStart = "00:00:00";
                var uptoTime = "11:59:59";

                _dayEnd = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(timeEnd.ToString()));

                DateTime nxtDay = punchTime.AddDays(1);
                _nextDayStart = DateTime.Parse(nxtDay.ToShortDateString()).Add(TimeSpan.Parse(timeStart.ToString()));
                _nextDayUpto = DateTime.Parse(nxtDay.ToShortDateString()).Add(TimeSpan.Parse(uptoTime.ToString()));

                _isHoliday = isHolidayDate(punchTime, shift);
                _isNextDayHol = isHolidayDate(nxtDay, shift);
                if (_isHoliday)
                {
                    _stdHrsHD = shift.stdhrshdd;
                    _stdHrsHDD = shift.stdhrshdd;
                    _stdHrsHDN = shift.stdhrshdn;
                }
                if (punchTime.DayOfWeek == DayOfWeek.Saturday)
                {


                    _shiftInD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinsatd.ToShortTimeString()));
                    _shiftInN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinsatn.ToShortTimeString()));
                    _shiftOutD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutsatd.ToShortTimeString()));
                    _shiftOutN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutsatn.ToShortTimeString()));
                    if (_shiftOutN.ToString("tt") == "AM") _shiftOutN.AddDays(1);
                    _timeInStartD = _shiftInD.AddMinutes(-(shift.shiftinallow));
                    _timeInStartN = _shiftInN.AddMinutes(-(shift.shiftinallow));
                    _breakOutDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutsatd.ToShortTimeString()));
                    _breakOutDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutsatn.ToShortTimeString()));
                    _breakInDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinsatd.ToShortTimeString()));
                    _breakInDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinsatn.ToShortTimeString()));
                    _breakOutStartD = _breakOutDefD.AddMinutes(-(shift.brkoutallow));
                    _breakOutStartN = _breakOutDefN.AddMinutes(-(shift.brkoutallow));
                    _breakInEndD = _breakInDefD.AddMinutes((shift.brkinallow));
                    _breakInEndN = _breakInDefN.AddMinutes((shift.brkinallow));
                    _maxWorkHrsD = shift.maxhrssatd;
                    _maxWorkHrsN = shift.maxhrssatn;
                    _timeOutEndD = _shiftOutD.AddMinutes((double)shift.shiftoutallow);
                    _timeOutEndN = _shiftOutN.AddHours((double)shift.shiftoutallow);
                    //_timeOutEndD = _shiftInD.AddHours((double)_maxWorkHrsD);
                    //_timeOutEndN = _shiftInN.AddHours((double)_maxWorkHrsN);
                    _shiftGraceInD = _shiftInD.AddMinutes((double)_graceIn);
                    _shiftGraceInN = _shiftInN.AddMinutes((double)_graceIn);
                    _shiftGraceOutD = _shiftOutD.AddMinutes(-(double)_graceOut);
                    _shiftGraceOutN = _shiftOutN.AddMinutes(-(double)_graceOut);
                    _allowedBreakD = _breakInDefD.Subtract(_breakOutDefD);
                    _allowedBreakN = _breakInDefN.Subtract(_breakOutDefN);
                    _stdHrsNDD = shift.stdhrssatd;
                    _stdHrsNDN = shift.stdhrssatn;
                    _maxOTHrsN = shift.otsatn;
                    _maxOTHrsD = shift.otsatd;

                }
                else if (punchTime.DayOfWeek == DayOfWeek.Sunday)
                {
                    _shiftInD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinsund.ToShortTimeString()));
                    _shiftInN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinsunn.ToShortTimeString()));
                    _shiftOutD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutsund.ToShortTimeString()));
                    _shiftOutN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutsunn.ToShortTimeString()));
                    if (_shiftOutN.ToString("tt") == "AM") _shiftOutN.AddDays(1);
                    _timeInStartD = _shiftInD.AddMinutes(-(shift.shiftinallow));
                    _timeInStartN = _shiftInN.AddMinutes(-(shift.shiftinallow));
                    _breakOutDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutsund.ToShortTimeString()));
                    _breakOutDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutsunn.ToShortTimeString()));
                    _breakInDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinsund.ToShortTimeString()));
                    _breakInDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinsunn.ToShortTimeString()));
                    _breakOutStartD = _breakOutDefD.AddMinutes(-(shift.brkoutallow));
                    _breakOutStartN = _breakOutDefN.AddMinutes(-(shift.brkoutallow));
                    _breakInEndD = _breakInDefD.AddMinutes((shift.brkinallow));
                    _breakInEndN = _breakInDefN.AddMinutes((shift.brkinallow));
                    _maxWorkHrsD = shift.maxhrssund;
                    _maxWorkHrsN = shift.maxhrssunn;
                    _timeOutEndD = _shiftOutD.AddMinutes((double)shift.shiftoutallow);
                    _timeOutEndN = _shiftOutN.AddHours((double)shift.shiftoutallow);
                    //_timeOutEndD = _shiftInD.AddHours((double)_maxWorkHrsD);
                    //_timeOutEndN = _shiftInN.AddHours((double)_maxWorkHrsN);
                    _shiftGraceInD = _shiftInD.AddMinutes((double)_graceIn);
                    _shiftGraceInN = _shiftInN.AddMinutes((double)_graceIn);
                    _shiftGraceOutD = _shiftOutD.AddMinutes(-(double)_graceOut);
                    _shiftGraceOutN = _shiftOutD.AddMinutes(-(double)_graceOut);
                    _allowedBreakD = _breakInDefD.Subtract(_breakOutDefD);
                    _allowedBreakN = _breakInDefN.Subtract(_breakOutDefN);
                    _stdHrsNDD = shift.stdhrssund;
                    _stdHrsNDN = shift.stdhrssunn;
                    _maxOTHrsN = shift.otsunn;
                    _maxOTHrsD = shift.otsund;
                }
                else // day is within mon to friday
                {

                    _shiftInD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinwkdayd.ToShortTimeString()));
                    _shiftInN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftinwkdayn.ToShortTimeString()));
                    _shiftOutD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutwkdayd.ToShortTimeString()));
                    _shiftOutN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.shiftoutwkdayn.ToShortTimeString()));
                    if (_shiftOutN.ToString("tt") == "AM") _shiftOutN.AddDays(1);
                    _timeInStartD = _shiftInD.AddMinutes(-(shift.shiftinallow));
                    _timeInStartN = _shiftInN.AddMinutes(-(shift.shiftinallow));
                    _breakOutDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutwkdayd.ToShortTimeString()));
                    _breakOutDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkoutwkdayn.ToShortTimeString()));
                    _breakInDefD = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinwkdayd.ToShortTimeString()));
                    _breakInDefN = DateTime.Parse(punchTime.ToShortDateString()).Add(TimeSpan.Parse(shift.brkinwkdayn.ToShortTimeString()));
                    _breakOutStartD = _breakOutDefD.AddMinutes(-(shift.brkoutallow));
                    _breakOutStartN = _breakOutDefN.AddMinutes(-(shift.brkoutallow));
                    _breakInEndD = _breakInDefD.AddMinutes((shift.brkinallow));
                    _breakInEndN = _breakInDefN.AddMinutes((shift.brkinallow));
                    _maxWorkHrsD = shift.maxhrswkdayd;
                    _maxWorkHrsN = shift.maxhrswkdayn;
                    _timeOutEndD = _shiftOutD.AddMinutes((double)shift.shiftoutallow);
                    _timeOutEndN = _shiftOutN.AddHours((double)shift.shiftoutallow);
                    //_timeOutEndD = _shiftInD.AddHours((double)_maxWorkHrsD);
                    //_timeOutEndN = _shiftInN.AddHours((double)_maxWorkHrsN);
                    _shiftGraceInD = _shiftInD.AddMinutes((double)_graceIn);
                    _shiftGraceInN = _shiftInN.AddMinutes((double)_graceIn);
                    _shiftGraceOutD = _shiftOutD.AddMinutes(-(double)_graceOut);
                    _shiftGraceOutN = _shiftOutD.AddMinutes(-(double)_graceOut);
                    _allowedBreakD = _breakInDefD.Subtract(_breakOutDefD);
                    _allowedBreakN = _breakInDefN.Subtract(_breakOutDefN);
                    _stdHrsNDD = shift.stdhrswkdayd;
                    _stdHrsNDN = shift.stdhrswkdayn;
                    _maxOTHrsN = shift.otwkdayn;
                    _maxOTHrsD = shift.otwkdayd;

                }
            }
            catch (FormatException ex)
            {
                _processedOk = false;
                _exception = ex.InnerException;
            }
        }
        public static DateTime? ToDate(this string input)
        {
            try
            {
                DateTime dtVal;
                return !DateTime.TryParse(input, out dtVal) ? (DateTime?)null : dtVal;
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
                return null;
            }
        }
        public static void UpdateAttendanceData(IUnitOfWork _unitOfWork, AttendanceBase objAtt, EnrolledEmployee employee, 
            string strTableName, bool isClientCall)
        {
            try
            {
                if (isClientCall == false)
                {
                    if (employee.type == enEmpType.Casual)
                    {
                        AttCasual objC = (AttCasual)objAtt;
                        if (objAtt.isNewRecord)
                        {
                            _unitOfWork.AttCasuals.Insert(objC);
                        }
                        else
                        {
                            _unitOfWork.AttCasuals.Update(objC);
                        }

                    }
                    else if (employee.type == enEmpType.Permanent)
                    {
                        AttPermanent objC = (AttPermanent)objAtt;
                        if (objAtt.isNewRecord)
                        {
                            _unitOfWork.AttPermanents.Insert(objC);
                        }
                        else
                        {
                            _unitOfWork.AttPermanents.Update(objC);
                        }
                    }
                    else if (employee.type == enEmpType.Contract)
                    {
                        AttContract objC = (AttContract)objAtt;
                        if (objAtt.isNewRecord)
                        {
                            _unitOfWork.AttContracts.Insert(objC);
                        }
                        else
                        {
                            _unitOfWork.AttContracts.Update(objC);
                        }
                    }
                    _unitOfWork.Commit();
                }
                else
                {
                    using (var db = new iTimeServiceContext())
                    {

                        var debug =  new SqlParameter("@debug",false);

                        var Table_Name = strTableName != null ?
                           new SqlParameter("@Table_Name", strTableName) :
                           new SqlParameter("@Table_Name", SqlDbType.VarChar);

                        var empid = objAtt.empid != null ?
                          new SqlParameter("@empid", objAtt.empid) :
                          new SqlParameter("@empid", SqlDbType.Int);

                        var id = objAtt.id != null ?
                          new SqlParameter("@id", objAtt.id) :
                          new SqlParameter("@id", SqlDbType.Int);

                        var shiftid = objAtt.shiftid != null ?
                            new SqlParameter("@shiftid", objAtt.shiftid) :
                            new SqlParameter("@shiftid", SqlDbType.Int);

                        var daytype = objAtt.daytype != null ?
                            new SqlParameter("@daytype", objAtt.daytype) :
                            new SqlParameter("@daytype", SqlDbType.Int);

                        var shifttype = objAtt.shifttype != null ?
                           new SqlParameter("@shifttype", objAtt.shifttype) :
                           new SqlParameter("@shifttype", SqlDbType.VarChar);

                        var isabsent = objAtt.isabsent != null ?
                           new SqlParameter("@isabsent", objAtt.isabsent) :
                           new SqlParameter("@isabsent", SqlDbType.Bit);

                        var attstatus = objAtt.attstatus != null ?
                            new SqlParameter("@attstatus", objAtt.attstatus) :
                            new SqlParameter("@attstatus", SqlDbType.Int);

                        var timein = objAtt.timein != null ?
                             new SqlParameter("@timein", objAtt.timein) :
                               new SqlParameter("@timein", DBNull.Value);

                        var timeout = objAtt.timeout != null ?
                             new SqlParameter("@timeout", objAtt.timeout) :
                               new SqlParameter("@timeout", DBNull.Value);

                        var timeinE = objAtt.timeinE != null ?
                          new SqlParameter("@timeinE", objAtt.timeinE) :
                            new SqlParameter("@timeinE", DBNull.Value);

                        var timeoutE = objAtt.timeoutE != null ?
                             new SqlParameter("@timeoutE", objAtt.timeoutE) :
                               new SqlParameter("@timeoutE", DBNull.Value);

                        var breakout = objAtt.breakout != null ?
                             new SqlParameter("@breakout", objAtt.breakout) :
                               new SqlParameter("@breakout", DBNull.Value);

                        var breakin = objAtt.breakin != null ?
                            new SqlParameter("@breakin", objAtt.breakin) :
                              new SqlParameter("@breakin", DBNull.Value);

                        var totalhrscount = objAtt.totalhrscount != null ?
                            new SqlParameter("@totalhrscount", objAtt.totalhrscount) :
                              new SqlParameter("@totalhrscount", SqlDbType.Float);

                        var totalhrsworked = objAtt.totalhrsworked != null ?
                            new SqlParameter("@totalhrsworked", objAtt.totalhrsworked) :
                              new SqlParameter("@totalhrsworked", SqlDbType.Float);

                        var normalhrsworked = objAtt.normalhrsworked != null ?
                            new SqlParameter("@normalhrsworked", objAtt.normalhrsworked) :
                              new SqlParameter("@normalhrsworked", SqlDbType.Float);

                        var otHD = objAtt.otHD != null ?
                            new SqlParameter("@otHD", objAtt.otHD) :
                              new SqlParameter("@otHD", SqlDbType.Float);

                        var otND = objAtt.otND != null ?
                           new SqlParameter("@otND", objAtt.otND) :
                             new SqlParameter("@otND", SqlDbType.Float);

                        var breaktm = objAtt.breaktm != null ?
                          new SqlParameter("@breaktm", objAtt.breaktm) :
                            new SqlParameter("@breaktm", SqlDbType.Float);

                        var leaveouts = objAtt.leaveouts != null ?
                          new SqlParameter("@leaveouts", objAtt.leaveouts) :
                            new SqlParameter("@leaveouts", SqlDbType.Float);

                        var losthrs = objAtt.losthrs != null ?
                          new SqlParameter("@losthrs", objAtt.losthrs) :
                            new SqlParameter("@losthrs", SqlDbType.Float);

                        var adjot = objAtt.adjot != null ?
                          new SqlParameter("@adjot", objAtt.adjot) :
                            new SqlParameter("@adjot", SqlDbType.Float);

                        var timeinM = objAtt.timeinM != null ?
                          new SqlParameter("@timeinM", objAtt.timeinM) :
                            new SqlParameter("@timeinM", SqlDbType.Bit);

                        var timeoutM = objAtt.timeoutM != null ?
                          new SqlParameter("@timeoutM", objAtt.timeoutM) :
                            new SqlParameter("@timeoutM", SqlDbType.Bit);

                        var weekday = objAtt.weekday != null ?
                          new SqlParameter("@weekday", objAtt.weekday) :
                          new SqlParameter("@weekday", SqlDbType.Int);

                        var comment = objAtt.comment != null ?
                         new SqlParameter("@comment", objAtt.comment) :
                         new SqlParameter("@comment", SqlDbType.VarChar);

                        var earlygo = objAtt.earlygo != null ?
                           new SqlParameter("@earlygo", objAtt.earlygo) :
                             new SqlParameter("@earlygo", SqlDbType.Float);

                        var latein = objAtt.latein != null ?
                            new SqlParameter("@latein", objAtt.latein) :
                              new SqlParameter("@latein", SqlDbType.Float);

                        var timeinP = objAtt.timeinP != null ?
                            new SqlParameter("@timeinP", objAtt.timeinP) :
                              new SqlParameter("@timeinP", SqlDbType.Float);

                        var timeoutP = objAtt.timeoutP != null ?
                            new SqlParameter("@timeoutP", objAtt.timeoutP) :
                              new SqlParameter("@timeoutP", SqlDbType.Float);

                        var lunchP = objAtt.lunchP != null ?
                           new SqlParameter("@lunchP", objAtt.lunchP) :
                             new SqlParameter("@lunchP", SqlDbType.Float);

                        var chlocked = objAtt.chlocked != null ?
                          new SqlParameter("@chlocked", objAtt.chlocked) :
                            new SqlParameter("@chlocked", SqlDbType.Bit);

                        var finallost = objAtt.finallost != null ?
                          new SqlParameter("@finallost", objAtt.finallost) :
                            new SqlParameter("@finallost", SqlDbType.Float);

                        var adjlost = objAtt.adjlost != null ?
                          new SqlParameter("@adjlost", objAtt.adjlost) :
                            new SqlParameter("@adjlost", SqlDbType.Float);

                        var finalot = objAtt.finalot != null ?
                          new SqlParameter("@finalot", objAtt.finalot) :
                            new SqlParameter("@finalot", SqlDbType.Float);

                        var lunchlost = objAtt.lunchlost != null ?
                          new SqlParameter("@lunchlost", objAtt.lunchlost) :
                            new SqlParameter("@lunchlost", SqlDbType.Float);

                        db.Database.ExecuteSqlCommand("spDynamicAttendanceUpdate @debug,@Table_Name,@empid,@id," +
                        "@shiftid,@daytype,@shifttype,@isabsent,@attstatus,@timein,@timeout,@timeinE,@timeoutE," +
                        "@breakout,@breakin,@totalhrscount,@totalhrsworked,@normalhrsworked,@otHD,@otND,@breaktm," +
                        "@leaveouts,@losthrs,@adjot,@timeinM,@timeoutM,@weekday,@comment,@earlygo,@latein,@timeinP," +
                        "@timeoutP,@lunchP,@chlocked,@finallost,@adjlost,@finalot,@lunchlost",
                        debug,Table_Name, empid, id, shiftid, daytype, shifttype, isabsent, attstatus, timein, timeout, timeinE, timeoutE,
                        breakout,breakin,totalhrscount,totalhrsworked,normalhrsworked,otHD,otND,breaktm,leaveouts,losthrs,adjot,
                        timeinM,timeoutM,weekday,comment,earlygo,latein,timeinP,timeoutP,lunchP,chlocked,finallost,adjlost,finalot,lunchlost);

                        _processedOk = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _processedOk = false;
                _exception = ex;
            }
        }

    }
}
