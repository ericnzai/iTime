using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Entities;

namespace iTimeService.AttendanceLogic
{
    public interface IAttendanceLogic
    {
        void GetShiftParams();
        void processAttendance(enShiftType shiftType, string tableName = "");
    }
}
