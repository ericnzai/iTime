using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Services;
using iTimeService.Entities;
namespace iTimeService.AttendanceLogic
{
    public class ClockModeLogicProvider
    {
        IAttendanceLogic _attLogic = null;
        enShiftType _shiftType;

        public ClockModeLogicProvider(IAttendanceLogic attLogic, 
            enShiftType shiftType)
        {
            this._attLogic = attLogic;
            this._shiftType = shiftType;
        }
        public void ProcessAttendance(string tableName)
        {
            _attLogic.GetShiftParams();

            bool useOriginalAttTable = true;
            if (tableName != "")
            {
                var tblSuffix = tableName.Substring(tableName.Length - 6, 2);
                if (tblSuffix == "20")
                {
                    useOriginalAttTable = false;
                }
                else
                {
                    useOriginalAttTable = true;
                }
            }
            if (useOriginalAttTable)
                _attLogic.processAttendance(_shiftType);
            else _attLogic.processAttendance(_shiftType, tableName);
        }
    }
}
