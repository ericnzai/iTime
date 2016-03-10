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
    public class UseDeviceLogic : IAttendanceLogic
    {
        #region ClassMembers
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IUnitOfWork _unitOfWork;
        private EnrolledEmployee employee;
        private IEnumerable<RawData> rawData;
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
        public UseDeviceLogic(EnrolledEmployee emp, IEnumerable<RawData> rData,  DateTime dateToProcess)
        {
            this.employee = emp;
            this.rawData = rData.ToList();
            this.dateToProcess = dateToProcess;
            this._unitOfWork = new UnitOfWork();
        }
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

                if (objAtt.chlocked) return;
                IEnumerable<int> indevices, outdevices, brkindevices, brkoutdevices; 
                using (var db = new iTimeServiceContext())
                {
                    indevices = db.Set<Device>().Where(x => x.isdeleted == false) .Where(x => x.isactive == true && x.transmode == enDeviceTransMode.TimeIn).Select(x => x.id).ToList();
                    brkoutdevices = db.Set<Device>().Where(x => x.isdeleted == false).Where(x => x.isactive == true && x.transmode == enDeviceTransMode.BreakOut).Select(x => x.id).ToList();
                    brkindevices = db.Set<Device>().Where(x => x.isdeleted == false).Where(x => x.isactive == true && x.transmode == enDeviceTransMode.BreakIn).Select(x => x.id).ToList();
                    outdevices = db.Set<Device>().Where(x => x.isdeleted == false).Where(x => x.isactive == true && x.transmode == enDeviceTransMode.TimeOut).Select(x => x.id).ToList();
                }

                //bool found = false;
                if (objAtt.shifttype == null)
                {
                    if (shiftType == enShiftType.FixedDay) myDynamicShft = "DAY";
                    else if (shiftType == enShiftType.FixedNight) myDynamicShft = "NIGHT";
                    else if (shiftType == enShiftType.Dynamic)
                    {
                        objAtt.isDynamic = true;
                        if (!Common.Common._useRoster)
                        {
                            foreach (RawData rawD in rawDataToProcess)
                            {
                                if (indevices.Contains(rawD.DEV_ID))
                                {
                                    if (rawD.PUNCH_TIME.ToString("tt") == "AM")
                                    {
                                        myDynamicShft = "DAY";
                                        break;
                                    }
                                    else if (rawD.PUNCH_TIME.ToString("tt") == "PM")
                                    {
                                        myDynamicShft = "NIGHT";
                                        break;
                                    }
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
                    if (objAtt.timeinM == false && objAtt.timeoutM == false) // only process if client has not modified timein or timeout
                    {
                        foreach (var rawD in rawDataToProcess)
                        {
                            if (indevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _timeInStartD && rawD.PUNCH_TIME < _breakOutStartD && objAtt.timeinE == null)
                                    objAtt.timeinE = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _breakOutStartD && rawD.PUNCH_TIME < _breakInEndD && objAtt.breakin == null)
                                    objAtt.breakin = rawD.PUNCH_TIME;
                            }
                            if (brkoutdevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _breakOutStartD && rawD.PUNCH_TIME < _breakInEndD && objAtt.breakout == null)
                                    objAtt.breakout = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME > _breakInEndD && rawD.PUNCH_TIME < _timeOutEndD)
                                    objAtt.timeoutE = rawD.PUNCH_TIME;
                            }
                            if (brkindevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _breakOutStartD && rawD.PUNCH_TIME < _breakInEndD)
                                    objAtt.breakin = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _timeInStartD && rawD.PUNCH_TIME < _breakOutStartD && objAtt.timeinE == null)
                                    objAtt.timeinE = rawD.PUNCH_TIME;
                            }
                            if (outdevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME > _breakInEndD && rawD.PUNCH_TIME < _timeOutEndD)
                                    objAtt.timeoutE = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _breakOutStartD && rawD.PUNCH_TIME < _breakInEndD && objAtt.breakout == null)
                                    objAtt.breakout = rawD.PUNCH_TIME;
                            }
                        }
                        if (objAtt.breakout != null && objAtt.breakin != null && objAtt.breakin < objAtt.breakout)
                        {
                            DateTime? temp = objAtt.breakin;
                            objAtt.breakin = objAtt.breakout;
                            objAtt.breakout = temp;
                        }
                        if (objAtt.timeinE == null && objAtt.breakout != null) objAtt.timeinE = objAtt.breakout;
                        if (objAtt.timeinE > objAtt.breakout && objAtt.breakout != null) objAtt.breakout = objAtt.timeinE;
                        if (objAtt.timeoutE == null && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        if (objAtt.breakin == null && objAtt.breakout != null && objAtt.timeoutE != null) objAtt.breakin = objAtt.timeoutE;
                        if (objAtt.timeoutE < objAtt.breakin && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        
                        
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
                    objAtt.shiftOutTime = _shiftOutD;
                    objAtt.maxOTHrs = Common.Common._maxOTHrsD;
                }
                else if (myDynamicShft == "NIGHT")
                {
                    if (objAtt.timeinM == false && objAtt.timeoutM == false)
                    {
                        IEnumerable<RawData> nxtDayData = this.rawData.Where(x => x.PUNCH_TIME <= _nextDayUpto).ToList();
                        foreach (var rawD in nxtDayData)
                        {
                            if (indevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _timeInStartN && rawD.PUNCH_TIME < _dayEnd && objAtt.timeinE == null)
                                    objAtt.timeinE = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _breakOutStartN && rawD.PUNCH_TIME < _breakInEndN && objAtt.breakin == null)
                                    objAtt.breakin = rawD.PUNCH_TIME;
                            }
                            if (brkoutdevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _breakOutStartN && rawD.PUNCH_TIME < _breakInEndN && objAtt.breakout == null)
                                    objAtt.breakout = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME > _breakInEndN && rawD.PUNCH_TIME < _nextDayUpto)
                                    objAtt.timeoutE = rawD.PUNCH_TIME;
                            }
                            if (brkindevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME >= _breakOutStartD && rawD.PUNCH_TIME < _breakInEndD)
                                    objAtt.breakin = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _timeInStartD && rawD.PUNCH_TIME < _breakOutStartD && objAtt.timeinE == null)
                                    objAtt.timeinE = rawD.PUNCH_TIME;
                            }
                            if (outdevices.Contains(rawD.DEV_ID))
                            {
                                if (rawD.PUNCH_TIME > _breakInEndN && rawD.PUNCH_TIME < _nextDayUpto)
                                    objAtt.timeoutE = rawD.PUNCH_TIME;
                                if (rawD.PUNCH_TIME >= _breakOutStartN && rawD.PUNCH_TIME < _breakInEndN && objAtt.breakout == null)
                                    objAtt.breakout = rawD.PUNCH_TIME;
                            }
                        }
                        //if (objAtt.timeoutE == null && objAtt.breakout != null) objAtt.timeoutE = objAtt.breakout;
                        //if (objAtt.timeoutE < objAtt.breakin && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        //if (objAtt.timeinE == null && objAtt.breakin != null) objAtt.timeinE = objAtt.breakin;
                        //if (objAtt.timeinE > objAtt.breakout && objAtt.breakout != null) objAtt.timeoutE = objAtt.breakout;
                        if (objAtt.timeoutE == null && objAtt.breakout != null) objAtt.timeoutE = objAtt.breakout;
                        if (objAtt.timeoutE < objAtt.breakin && objAtt.breakin != null) objAtt.timeoutE = objAtt.breakin;
                        if (objAtt.timeinE == null && objAtt.breakin != null) objAtt.timeinE = objAtt.breakin;
                        if (objAtt.timeinE > objAtt.breakout && objAtt.breakout != null) objAtt.timeoutE = objAtt.breakout;
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

                    if (Common.Common._otNDCalcMode == enOTND.DeductLostHrs) objAtt.calcOT();
                    //----------------Reset values if Holiday ------------//
                    objAtt.adjustOnHoliday();
                }
                else
                {
                    objAtt.isabsent = true;
                }
                objAtt.setComment();
                //---------------Update database---------------------------------------------------
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
    }
}
