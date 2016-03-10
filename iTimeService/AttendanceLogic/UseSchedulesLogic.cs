using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Entities;
using iTimeService.Concrete;
using iTimeService.Common;
using System.Globalization;
using System.Data;
using System.Data.Entity;
using log4net;
using System.Reflection;

namespace iTimeService.AttendanceLogic
{
    public class UseSchedulesLogic : IAttendanceLogic
    {
        #region ClassMembers
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IUnitOfWork _unitOfWork;
        private EnrolledEmployee employee;
        private IEnumerable <RawData> rawData;
        private ShiftType shift; 
        private string myDynamicShft;
        private DateTime dateToProcess;
       
        ////company global timeinout modes
        private enGlobalTimeInOutMode _timeInMode;
        private enGlobalTimeInOutMode _timeOutMode;
        #endregion ClassMembers
        #region DynamicShiftVariables
        private decimal _graceIn;
        private DateTime _shiftGraceIn;
        private decimal _graceOut;
        private DateTime _shiftGraceOut;
        private decimal _stdHrsAllowed;
        private string myDynamicShift { get; set; }
        private DateTime _timeInStartD;
        private DateTime _timeInStartN;
        private DateTime _breakOutStartD;
        private DateTime _breakOutStartN;
        private DateTime _breakInEndD;
        private DateTime _breakInEndN;
        private DateTime _timeOutEndD;
        private DateTime _timeOutEndN;
        private DateTime _shiftInD;
        private DateTime _shiftInN;
        private DateTime _shiftOutD;
        private DateTime _shiftOutN;
        private DateTime _breakOutDefD;
        private DateTime _breakInDefD;
        private DateTime _breakOutDefN;
        private DateTime _breakInDefN;
        private DateTime _dayEnd;
        private DateTime _nextDayUpto;
        private DateTime _nextDayStart;
        private decimal _maxWorkHrsD;
        private decimal _maxWorkHrsN;
        private decimal _graceInD;
        private decimal _graceInN;
        private DateTime _shiftGraceInD;
        private DateTime _shiftGraceInN;
        private decimal _graceOutD;
        private decimal _graceOutN;
        private DateTime _shiftGraceOutD;
        private DateTime _shiftGraceOutN;
        private TimeSpan _allowedBreakD;
        private TimeSpan _allowedBreakN;
        private decimal _stdHrsAllowedD;
        private decimal _stdHrsAllowedN;
        private bool _isHoliday;
        #endregion DynamicShiftVariables
        #region ClassMethods

        public UseSchedulesLogic(EnrolledEmployee emp, IEnumerable<RawData> rData, DateTime dateToProcess)
        {
            this.employee = emp;
            this.rawData = rData.ToList();
            //this.shift = shift;
            this.dateToProcess = dateToProcess;
            _unitOfWork = new UnitOfWork();
        }
        //public void SetTimings()
        //{
        //    //IEnumerable<RawData> rawDataToProcess = this.rawData.Where(x => x.PUNCH_TIME.Date == dateToProcess.Date)
        //    //                                        .OrderBy(x => x.PUNCH_TIME)
        //    //                                        .ToList();
        //    //Common.Common.SetShiftParams(dateToProcess, employee.ShiftType);
        //    //GetShiftParams();
        //    //processAttendance(rawDataToProcess,employee.ShiftType.type);
            
        //}
       
        public void GetShiftParams()
        {
            //global common params
            _graceIn = Common.Common._graceIn;
            _shiftGraceIn = Common.Common._shiftGraceIn;
            _graceOut = Common.Common._graceOut;
            _shiftGraceOut = Common.Common._shiftGraceOut;

            _timeOutMode = Common.Common._timeOutMode;
            _timeInMode = Common.Common._timeInMode;
            _dayEnd = Common.Common._dayEnd;
            _nextDayStart = Common.Common._nextDayStart;
            _nextDayUpto = Common.Common._nextDayUpto;
            _isHoliday = Common.Common._isHoliday;
                   
            //day of week dependent params
            _timeInStartD = Common.Common._timeInStartD;
            _timeInStartN = Common.Common._timeInStartN;
            _breakOutStartD = Common.Common._breakOutStartD;
            _breakOutStartN = Common.Common._breakOutStartN;
            _breakInEndD = Common.Common._breakInEndD;
            _breakInEndN = Common.Common._breakInEndN;
            _timeOutEndD = Common.Common._timeOutEndD;
            _timeOutEndN = Common.Common._timeOutEndN;
            _shiftInD = Common.Common._shiftInD;
            _shiftInN = Common.Common._shiftInN;
            _shiftOutD = Common.Common._shiftOutD;
            _shiftOutN = Common.Common._shiftOutN;
            _breakOutDefD =  Common.Common._breakOutDefD;
            _breakOutDefN   = Common.Common._breakOutDefN;
            _breakInDefD = Common.Common._breakInDefD;
            _breakInDefN = Common.Common._breakInDefN;
            _shiftGraceInD = Common.Common._shiftGraceInD;
            _shiftGraceInN = Common.Common._shiftGraceInN;
            _shiftGraceOutD = Common.Common._shiftGraceOutD;
            _shiftGraceOutN = Common.Common._shiftGraceOutN;
            _allowedBreakD = Common.Common._allowedBreakD;
            _allowedBreakN = Common.Common._allowedBreakN;
            _maxWorkHrsD = Common.Common._maxWorkHrsD;
            _maxWorkHrsN = Common.Common._maxWorkHrsN;
            _stdHrsAllowedD = Common.Common._stdHrsNDD;
            _stdHrsAllowedN = Common.Common._stdHrsNDN;
            
        if (_isHoliday)
        {
            _stdHrsAllowedD = Common.Common._stdHrsHDD;
            _stdHrsAllowedN = Common.Common._stdHrsHDN;
            }
        }
        public void processAttendance(enShiftType shiftType, string tableName = "")
        {
            try
            {
                IEnumerable<RawData> rawDataToProcess = this.rawData.Where(x => x.PUNCH_TIME.Date == dateToProcess.Date)
                                                    .OrderBy(x => x.PUNCH_TIME)
                                                    .ToList();

                AttendanceBase objAtt = Common.Common.InitializeAttendanceObject(_unitOfWork, employee, dateToProcess,tableName);
                objAtt = Common.Common.ObjAttStatus(objAtt, _unitOfWork, employee, dateToProcess);

                // ---- Check if the attendance record for that day is locked to proceed ---- //
                if (objAtt.chlocked)
                {
                    return;
                }

                int count = rawDataToProcess.Count();
                DateTime[] Punches = new DateTime[count];
                int j = 0;
                foreach(var r in rawDataToProcess)
                {
                    Punches[j] = r.PUNCH_TIME;
                    j++;
                }
                int i = 0, index = 0;
                if (objAtt.shifttype == null)
                {
                    if (shiftType == enShiftType.FixedDay) myDynamicShft = "DAY";
                    else if (shiftType == enShiftType.FixedNight) myDynamicShft = "NIGHT";
                    else if (shiftType == enShiftType.Dynamic)
                    {
                        if (!Common.Common._useRoster)
                        {
                            bool found = false;

                            foreach (var rawD in rawDataToProcess)
                            {
                               
                                if (rawD.PUNCH_TIME >= _timeInStartN && rawD.PUNCH_TIME <= _dayEnd && !found)
                                {
                                    myDynamicShft = "NIGHT";
                                    found = true;
                                    index = i;
                                }
                                else if (rawD.PUNCH_TIME >= _timeInStartD && rawD.PUNCH_TIME <= _breakOutStartD && !found)
                                {
                                    myDynamicShft = "DAY";
                                    found = true;
                                    index = i;
                                }
                            }
                        }
                        else
                        {
                            var empRoster = _unitOfWork.EmpRoster.All()
                                            .Where(x => x.empid == employee.id
                                                && x.month == dateToProcess.Month && x.year == dateToProcess.Year)
                                            .FirstOrDefault();
                            int dayVal = dateToProcess.Day;
                            string shft = "_";
                            if (empRoster != null)
                            {
                                shft = Common.Common.GetShiftFromRoster(dayVal, empRoster);
                            }

                            if (shft == "D")
                            {
                                myDynamicShft = "DAY";
                            }
                            else if (shft == "N")
                            {
                                myDynamicShft = "NIGHT";
                            }
                            else myDynamicShft = null;
                        }
                    }
                }
                else myDynamicShft = objAtt.shifttype;
                if (myDynamicShft == "DAY")
                {
                    if (objAtt.timeinM == false && objAtt.timeoutM == false)
                    {
                        switch (count)
                        {
                            case 0:
                                objAtt.timeinE = null;
                                objAtt.breakout = null;
                                objAtt.breakin = null;
                                objAtt.timeoutE = null;
                                break;
                            case 1:
                                if (Punches[0] >= _timeInStartD && Punches[0] < _breakOutStartD)
                                {
                                    objAtt.timeinE = Punches[0]; objAtt.breakout = null;
                                    objAtt.breakin = null; objAtt.timeoutE = null;
                                }
                                else
                                {
                                    objAtt.timeinE = null; objAtt.breakout = null;
                                    objAtt.breakin = null; objAtt.timeoutE = null;
                                }
                                break;
                            case 2:
                                if (Punches[0] >= _timeInStartD && Punches[0] < _breakOutStartD) objAtt.timeinE = Punches[0];
                                else objAtt.timeinE = null;
                                if (Punches[1] >= _breakOutStartD && Punches[1] < _breakInEndD) objAtt.breakout = Punches[1];
                                else objAtt.breakout = null;
                                if (Punches[1] >= _breakInEndD && Punches[1] < _timeOutEndD) objAtt.timeoutE = Punches[1];
                                else objAtt.timeoutE = null;
                                break;
                            case 3:
                                if (Punches[0] >= _timeInStartD && Punches[0] < _breakOutStartD) objAtt.timeinE = Punches[0];
                                else objAtt.timeinE = null;
                                if (Punches[1] >= _breakOutStartD && Punches[1] < _breakInEndD) objAtt.breakout = Punches[1];
                                else objAtt.breakout = null;
                                if (Punches[2] >= _breakOutStartD && Punches[2] < _breakInEndD) objAtt.breakin = Punches[2];
                                else objAtt.breakin = null;
                                if (Punches[2] >= _breakInEndD && Punches[2] < _timeOutEndD) objAtt.timeoutE = Punches[2];
                                else objAtt.timeoutE = null;
                                break;
                            default:
                                for (i = 0; i < Punches.Length; i++)
                                {
                                    if (Punches[i] >= _timeInStartD && Punches[i] <= _breakOutStartD)
                                    {
                                        if (objAtt.timeinE == null) objAtt.timeinE = Punches[i];

                                    }
                                    if (objAtt.breakout == null)
                                    {
                                        if (Punches[i] >= _breakOutStartD && Punches[i] <= _breakInEndD) objAtt.breakout = Punches[i];
                                        if (objAtt.timeinE == objAtt.breakout) objAtt.breakout = null;
                                    }
                                    if (objAtt.breakout != null)
                                    {
                                        if ((Punches[i] >= _breakOutStartD && Punches[i] <= _breakInEndD) && Punches[i] >= objAtt.breakout) objAtt.breakin = Punches[i];
                                    }
                                    if (Punches[i] >= _breakInEndD && Punches[i] < _timeOutEndD) objAtt.timeoutE = Punches[i];

                                  
                                }
                                break;
                        }
                        if (objAtt.timeinE == null && objAtt.breakout != null) objAtt.timeinE = objAtt.breakout;
                        if (objAtt.timeoutE == null && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        if (objAtt.breakin == null && objAtt.breakout != null && objAtt.timeoutE != null) objAtt.breakin = objAtt.timeoutE;
                    //---------------UPDATE TIME IN/TIMEOUT FOR DYNAMIC DAY-------------------------------------
                    //set time in according to grace in allowed and company setting
                        if (objAtt.timeinE != null)
                        {
                            if (objAtt.timeinE > _shiftGraceInD) objAtt.timein = objAtt.timeinE;
                            else if ((objAtt.timeinE >= _shiftInD && objAtt.timeinE <= _shiftGraceInD)) objAtt.timein = _shiftInD;
                            else
                            {
                                if (_timeInMode == enGlobalTimeInOutMode.DynamicTime)
                                {
                                    objAtt.timein = objAtt.timeinE;
                                }
                                else if (_timeInMode == enGlobalTimeInOutMode.ExactShiftTime)
                                {
                                    objAtt.timein = _shiftInD;
                                }
                            }
                        }
                        //set timeout accordin to grace out allowed and company setting
                        if (objAtt.timeoutE != null)
                        {
                            if (objAtt.timeoutE < _shiftGraceOutD) objAtt.timeout = objAtt.timeoutE;
                            else if ((objAtt.timeoutE >= _shiftGraceOutD && objAtt.timeoutE <= _shiftOutD)) objAtt.timeout = _shiftOutD;
                            else
                            {
                                if (_timeOutMode == enGlobalTimeInOutMode.ExactShiftTime)
                                {
                                    objAtt.timeout = _shiftOutD;
                                }
                                else if (_timeOutMode == enGlobalTimeInOutMode.DynamicTime)
                                {
                                    objAtt.timeout = objAtt.timeoutE;
                                }
                            }
                        }
                    }
                    objAtt.allowedbreak = _allowedBreakD;
                    objAtt.maxhrsAllowed = _maxWorkHrsD;
                    objAtt.stdhrsAllowed = _stdHrsAllowedD;
                    objAtt.shiftInTime = _shiftInD;
                    objAtt.shiftOutTime= _shiftOutD;
                    objAtt.maxOTHrs = Common.Common._maxOTHrsD;
                }
                else if (myDynamicShft == "NIGHT")
                {
                if (objAtt.timeinM != true || objAtt.timeoutM != true)
                {
                    IEnumerable<RawData> nxtDayData = this.rawData.Where(x => x.PUNCH_TIME <= _nextDayUpto).ToList();
                    count = nxtDayData.Count();
                    Punches = new DateTime[count];
                    for (i = index; i < count; i++)
                    {
                        Punches[i] = nxtDayData.ElementAt(i).PUNCH_TIME;
                    }
                   
                  
                        switch (count)
                        {
                            case 0:
                                objAtt.timeinE = null;
                                objAtt.breakout = null;
                                objAtt.breakin = null;
                                objAtt.timeoutE = null;
                                break;
                            case 1:
                                if (Punches[0] >= _timeInStartN && Punches[0] < _dayEnd)
                                {
                                    objAtt.timeinE = Punches[0]; objAtt.breakout = null;
                                    objAtt.breakin = null; objAtt.timeoutE = null;
                                }
                                else
                                {
                                    objAtt.timeinE = null; objAtt.breakout = null;
                                    objAtt.breakin = null; objAtt.timeoutE = null;
                                }
                                break;
                            case 2:
                                if (Punches[0] >= _timeInStartN && Punches[0] < _dayEnd) objAtt.timeinE = Punches[0];
                                else objAtt.timeinE = null;
                                if (Punches[1] >= _breakOutStartN && Punches[1] < _breakInEndN) objAtt.breakout = Punches[1];
                                else objAtt.breakout = null;
                                if (Punches[1] >= _breakInEndN && Punches[1] < _timeOutEndN) objAtt.timeoutE = Punches[1];
                                else objAtt.timeoutE = null;
                                break;
                            case 3:
                                if (Punches[0] >= _timeInStartN && Punches[0] < _dayEnd) objAtt.timeinE = Punches[0];
                                else objAtt.timeinE = null;
                                if (Punches[1] >= _breakOutStartN && Punches[1] < _breakInEndN) objAtt.breakout = Punches[1];
                                else objAtt.breakout = null;
                                if (Punches[2] >= _breakOutStartN && Punches[2] < _breakInEndN) objAtt.breakin = Punches[2];
                                else objAtt.breakin = null;
                                if (Punches[2] >= _breakInEndN && Punches[2] < _timeOutEndN) objAtt.timeoutE = Punches[2];
                                else objAtt.timeoutE = null;
                                break;
                            default:
                                for (i = 0; i < Punches.Length; i++)
                                {
                                    if (objAtt.timeinE == null)
                                    {
                                        if (Punches[i] >= _timeInStartN && Punches[i] <= _dayEnd)
                                        {
                                            if (objAtt.timeinE == null) objAtt.timeinE = Punches[i];
                                        }
                                    }
                                    if (Punches[i] >= _breakOutStartN && Punches[i] <= _breakInEndN)
                                    {
                                        if (objAtt.breakout == null) objAtt.breakout = Punches[i];
                                    }
                                    if (objAtt.timeinE == objAtt.breakout) objAtt.breakout = null;

                                    if (objAtt.breakout != null)
                                    {
                                        if ((Punches[i] >= _breakOutStartN && Punches[i] <= _breakInEndN) && Punches[i] >= objAtt.breakout) objAtt.breakin = Punches[i];
                                    }
                                    if (Punches[i] >= _breakInEndN && Punches[i] < _timeOutEndN) objAtt.timeoutE = Punches[i];
                                }
                                break;
                        }
                        if (objAtt.timeinE == null && objAtt.breakout != null) objAtt.timeinE = objAtt.breakout;
                        if (objAtt.timeoutE == null && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        if (objAtt.breakin == null && objAtt.breakout != null && objAtt.timeoutE != null) objAtt.breakin = objAtt.timeoutE;
                        //---------------UPDATE TIME IN/TIMEOUT FOR DYNAMIC NIGHT-------------------------------------
                        //set time in according to grace in allowed and company setting
                        if (objAtt.timeinE != null)
                        {
                            if (objAtt.timeinE > _shiftGraceInN) objAtt.timein = objAtt.timeinE;
                            else if ((objAtt.timeinE >= _shiftInN && objAtt.timeinE <= _shiftGraceInN)) objAtt.timein = _shiftInN;
                            else
                            {
                                if (_timeInMode == enGlobalTimeInOutMode.DynamicTime)
                                {
                                    objAtt.timein = objAtt.timeinE;
                                }
                                else if (_timeInMode == enGlobalTimeInOutMode.ExactShiftTime)
                                {
                                    objAtt.timein = _shiftInN;
                                }
                            }
                        }
                        //set timeout accordin to grace out allowed and company setting
                        if (objAtt.timeoutE != null)
                        {
                            if (objAtt.timeoutE < _shiftGraceOutN) objAtt.timeout = objAtt.timeoutE;
                            else if ((objAtt.timeoutE >= _shiftGraceOutN && objAtt.timeoutE <= _shiftOutN)) objAtt.timeout = _shiftOutN;
                            else
                            {
                                if (_timeOutMode == enGlobalTimeInOutMode.ExactShiftTime)
                                {
                                    objAtt.timeout = _shiftOutN;
                                }
                                else if (_timeOutMode == enGlobalTimeInOutMode.DynamicTime)
                                {
                                    objAtt.timeout = objAtt.timeoutE;
                                }
                            }
                        }
                    }
                    objAtt.allowedbreak = _allowedBreakN;
                    objAtt.maxhrsAllowed = _maxWorkHrsN;
                    objAtt.stdhrsAllowed = _stdHrsAllowedN;
                    objAtt.shiftInTime = _shiftInN;
                    objAtt.shiftOutTime = _shiftOutN;
                    objAtt.maxOTHrs = Common.Common._maxOTHrsN;
                }

                objAtt.breakType = employee.breaks;
                objAtt.shiftid = employee.shift;
                objAtt.deptid = employee.department;
                objAtt.shifttype = myDynamicShft;

                if (!string.IsNullOrEmpty(objAtt.shifttype))
                {
                    objAtt.isDynamic = true;
                    objAtt.isHol = _isHoliday;
                    objAtt.calcTTHrsCount();
                    objAtt.calcTTBreaks();
                    objAtt.calcLunchLost();
                    objAtt.calcTTHrsWorked();
                    objAtt.calcNormalHrsWorked();

                    objAtt.setDayType();
                    objAtt.calcLateIn();
                    objAtt.calcEarlyGo();
                    objAtt.calcOTSeparate();
                    objAtt.calcLostSeparate();
                    
                    if (Common.Common._otNDCalcMode == enOTND.DeductLostHrs)
                    { 
                        objAtt.calcOT();
                    }
                    //----------------Reset values if Holiday ------------//
                    objAtt.adjustOnHoliday();
                }
                else
                {
                    objAtt.isabsent = true;
                }
                objAtt.setComment();
                
                //---------------Update database-----------------------//
                bool isClientCall = tableName == "" ? false : true;
                Common.Common.UpdateAttendanceData(_unitOfWork, objAtt, employee, tableName, isClientCall);
                
                Common.Common._processedOk = true;
               
            }
            catch (Exception ex)
            {
                Common.Common._processedOk = false;
                Common.Common._exception = ex;
            }
        }
       
        #endregion ClassMethods
    }
}
