using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using iTimeService.Entities;
namespace iTimeService.Concrete
{
    public interface IUnitOfWork
    {
        void Commit();

        IRepository<RawData> RawData { get; }
        IRepository<AttCasual> AttCasuals { get; }
        IRepository<AttPermanent> AttPermanents  { get; }
        IRepository<AttContract> AttContracts { get; }
        IRepository<EnrolledEmployee> EnrolledEmployees { get; }
        IRepository<ShiftType> ShiftTypes { get; }
        IRepository<Device> Devices { get; }
        IRepository<CompanySettings> CompanySettings { get; }
        IRepository<WorkCode> WorkCodes { get; }
        IRepository<EmpRoster> EmpRoster { get; }
        IRepository<ServiceCustomCommand> ServiceCustomCommands { get; }
        IRepository<LeaveApplication> LeaveApplications { get; }
        IRepository<LeaveType> LeaveTypes { get;  }
    }
}
