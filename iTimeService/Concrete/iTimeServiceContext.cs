using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using iTimeService.Entities;
using System.Data.SqlTypes;

namespace iTimeService.Concrete
{
    public class iTimeServiceContext : DbContext
    {
        public DbSet<RawData> RawData { get; set; }
        public DbSet<AttPermanent> AttPermanent { get; set; }
        public DbSet<AttCasual> AttCasual { get; set; }
        public DbSet<AttContract> AttContract { get; set; }
        public DbSet<EnrolledEmployee> EnrolledEmployee { get; set; }
        public DbSet<CompanySettings> CompanySettings { get; set; }
        public DbSet<Device> Device { get; set; }
        public DbSet<WorkCode> WorkCode { get; set; }
        public DbSet<EmpRoster> EmployeeRoster { get; set; }
        public DbSet<ServiceCustomCommand> ServiceCustomCommand { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<LeaveApplication> LeaveApplication { get; set; }

        public iTimeServiceContext() : 
            base("iTimeConn")
        {
            Database.SetInitializer<iTimeServiceContext>(null);
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RawData>().ToTable("RAW_DATA");

            modelBuilder.Entity<ShiftType>().ToTable("Mst_Shift");

            modelBuilder.Entity<AttPermanent>().ToTable("Trn_AttPermanent").HasRequired(p => p.EnrolledEmployee)
                .WithMany(p => p.AttPermanentRecords).HasForeignKey(e => e.empid);

            modelBuilder.Entity<AttCasual>().ToTable("Trn_AttCasual").HasRequired(p => p.EnrolledEmployee)
                .WithMany(p => p.AttCasualRecords).HasForeignKey(e => e.empid);

            modelBuilder.Entity<AttContract>().ToTable("Trn_AttContract").HasRequired(p => p.EnrolledEmployee)
               .WithMany(p => p.AttContractRecords).HasForeignKey(e => e.empid);

            modelBuilder.Entity<EnrolledEmployee>().ToTable("Mst_Employee").HasRequired(e => e.ShiftType)
                .WithMany().HasForeignKey(e => e.shift);

            modelBuilder.Entity<CompanySettings>().ToTable("CompanySettings");

            modelBuilder.Entity<Holiday>().ToTable("Mst_Holiday");

            modelBuilder.Entity<Device>().ToTable("Mst_Device");

            modelBuilder.Entity<WorkCode>().ToTable("Mst_Workcodes");

            modelBuilder.Entity<EmpRoster>().ToTable("Trn_EmpRoster");

            modelBuilder.Entity<ServiceCustomCommand>().ToTable("Svc_CustomCommandParams");

            modelBuilder.Entity<LeaveApplication>().ToTable("Trn_EmpLeaveApply").HasRequired(l => l.LeaveType)
                .WithMany(la => la.LeaveApplications).HasForeignKey(l => l.leaveid);

            modelBuilder.Entity<LeaveType>().ToTable("Mst_Leave");

            base.OnModelCreating(modelBuilder);
        }
        private void UpdateDates()
        {
            foreach (var change in ChangeTracker.Entries<AttendanceBase>())
            {
                var values = change.CurrentValues;
                foreach (var name in values.PropertyNames)
                {
                    var value = values[name];
                    if (value is DateTime)
                    {
                        var date = (DateTime)value;
                        if (date < SqlDateTime.MinValue.Value)
                        {
                            values[name] = SqlDateTime.MinValue.Value;
                        }
                        else if (date > SqlDateTime.MaxValue.Value)
                        {
                            values[name] = SqlDateTime.MaxValue.Value;
                        }
                    }
                }
            }
        }
        public override int SaveChanges()
        {
            //UpdateDates();
            return base.SaveChanges();
        }

    }
}
