using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Entities;
namespace iTimeService.Concrete
{
    public class UnitOfWork : IUnitOfWork,IDisposable
    {
        private iTimeServiceContext DbContext { get; set; }
        public UnitOfWork()
        {
            CreateDbContext();
        }

        protected void CreateDbContext()
        {
            DbContext = new iTimeServiceContext();
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        private IRepository<RawData> _rawData;
        private IRepository<AttCasual> _attCasuals;
        private IRepository<AttPermanent> _attPermanents;
        private IRepository<AttContract> _attContracts;
        private IRepository<EnrolledEmployee> _employees;
        private IRepository<Device> _devices;
        private IRepository<ShiftType> _shiftTypes;
        private IRepository<CompanySettings> _companySettings;
        private IRepository<WorkCode> _workCodes;
        private IRepository<EmpRoster> _empRoster;
        private IRepository<ServiceCustomCommand> _svcCmds;
        private IRepository<LeaveApplication> _lvApplications;
        private IRepository<LeaveType> _lvTypes;

        public IRepository<RawData> RawData
        {
            get
            {
                if (_rawData == null)
                {
                    _rawData = new Repository<RawData>(DbContext);
                }
                return _rawData;
            }
        }
        public IRepository<EmpRoster> EmpRoster
        {
            get
            {
                if (_empRoster == null)
                {
                    _empRoster = new Repository<EmpRoster>(DbContext);
                }
                return _empRoster;
            }
        }
        public IRepository<ServiceCustomCommand> ServiceCustomCommands
        {
            get
            {
                if (_svcCmds == null)
                {
                    _svcCmds = new Repository<ServiceCustomCommand>(DbContext);
                }
                return _svcCmds;
            }
        }
        public IRepository<AttCasual> AttCasuals
        {
            get
            {
                if (_attCasuals == null)
                {
                    _attCasuals  = new Repository<AttCasual>(DbContext);
                }
                return _attCasuals;
            }
        }
        public IRepository<AttPermanent> AttPermanents
        {
            get
            {
                if (_attPermanents== null)
                {
                    _attPermanents = new Repository<AttPermanent>(DbContext);
                }
                return _attPermanents;
            }
        }
        public IRepository<AttContract> AttContracts
        {
            get
            {
                if (_attContracts == null)
                {
                    _attContracts = new Repository<AttContract>(DbContext);
                }
                return _attContracts;
            }
        }
        public IRepository<EnrolledEmployee> EnrolledEmployees
        {
            get
            {
                if (_employees== null)
                {
                    _employees = new Repository<EnrolledEmployee>(DbContext);
                }
                return _employees;
            }
        }
        public IRepository<ShiftType> ShiftTypes
        {
            get
            {
                if (_shiftTypes== null)
                {
                    _shiftTypes = new Repository<ShiftType>(DbContext);
                }
                return _shiftTypes;
            }
        }
        public IRepository<Device> Devices
        {
            get
            {
                if (_devices == null)
                {
                    _devices = new Repository<Device>(DbContext);
                }
                return _devices;
            }
        }
        public IRepository<CompanySettings> CompanySettings
        {
            get
            {
                if (_companySettings == null)
                {
                    _companySettings = new Repository<CompanySettings>(DbContext);
                }
                return _companySettings;
            }
        }
        public IRepository<WorkCode> WorkCodes
        {
            get
            {
                if (_workCodes== null)
                {
                    _workCodes = new Repository<WorkCode>(DbContext);
                }
                return _workCodes;
            }
        }
        public IRepository<LeaveApplication> LeaveApplications
        {
            get
            {
                if (_lvApplications == null)
                {
                    _lvApplications = new Repository<LeaveApplication>(DbContext);
                }
                return _lvApplications;
            }
        }
        public IRepository<LeaveType> LeaveTypes
        {
            get
            {
                if (_lvTypes== null)
                {
                    _lvTypes = new Repository<LeaveType>(DbContext);
                }
                return _lvTypes;
            }
        }
        public void Commit()
        {
            DbContext.SaveChanges();
        }
    }
}
