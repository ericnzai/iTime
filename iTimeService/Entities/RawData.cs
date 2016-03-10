using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace iTimeService.Entities
{
    public enum enClockMode
    {
        StatusCode = 0,
        Device = 1,
        Schedules = 2,
        WorkCodes = 3
    }
    public enum enDeviceTransMode 
    {
        TimeIn = 0,
        TimeOut = 1,
        BreakOut = 2,
        BreakIn = 3,
        BothInOut = 4
    }
    public enum enStatusCodes
    {
        TimeIn = 0,
        TimeOut = 1,
        BreakOut = 2,
        BreakIn = 3
    }
    public enum enEmpType
    {
        Permanent = 0,
        Casual = 1,
        Contract = 2
    }
    public enum enDeviceModel
    {
        Black_White =0,
        MultiMedia =1
    }
    
     public enum enBaudRate 
     {
        Baud75 = 75,
        Baud110 = 110,
        Baud134 = 134,
        Baud150 = 150,
        Baud300 = 300,
        Baud600 = 600,
        Baud1200 = 1200,
        Baud1800 = 1800,
        Baud2400 = 2400,
        Baud4800 = 4800,
        Baud7200 = 7200,
        Baud9600 = 9600,
        Baud14400 = 14400,
        Baud19200 = 19200,
        Baud38400 = 38400,
        Baud57600 = 57600,
        Baud115200 = 115200,
        Baud128000 = 128000
     }
    
    public enum enSDK_Name 
    {
        BioBridgeSDK = 0,
        ZKemKeeper = 1
    }
    public enum enRateType
    {
        DailyRate = 0,
        WeeklyRate = 1,
        MonthlyRate = 2
    }
    public enum enShiftType
    {
        FixedDay = 0,
        FixedNight = 1,
        Dynamic = 2
    }
    public enum enOTND
    {
        DeductLostHrs = 0,
        SeperateLostHrs = 1
    }
    public enum enOTHD 
    {
        FullPay = 0,
        SplitPay = 1,
        DynamicFixed = 2,
        DynamicSTDHRS = 3,
        SeperateLostHrs = 4,
        PenaliseHDOT = 5
    }
    public enum enRateMode 
    {
        MonthlyDays = 0,
        FixedDays = 1,
        FixedHours = 2
    }
     public enum enFrequencyType 
     {
         Monthly = 0,
        BiWeekly = 1,
        Weekly = 2
     }
     public enum enDayType
     {
         Normal = 0,
         Holiday
     }
     public enum enOTDef
     {
          OneDayOnly = 0,
        PreNextDay = 1
     }
      public enum enBreaks
         {
            Fixed = 0,
            Dynamic = 1
         }
    public class RawData
    {
       
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public decimal RAW_ID { get; set;    }
        public int ENROLL_NO { get; set; }
        public int DT_YR { get; set; }
        public int DT_MNTH { get; set; }
        public int DT_DAY { get; set; }
        public int DT_HR { get; set; }
        public int DT_MIN { get; set; }
        public int DT_SEC { get; set; }
        public int VER { get; set; }
        public int R_IO { get; set; }
        public int R_WORK { get; set; }
        public int DEV_ID { get; set; }
        //public virtual Device Device { get; set; }
        public int R_LOG { get; set; }
        public DateTime DATE_LOG { get; set; }
        public DateTime PUNCH_TIME { get; set; }
        //public bool IS_PROCESSED { get; set; }
    }
    public class ShiftType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int id { get; set; }
        public string code { get; set; }
        public enShiftType type { get; set; }
        public decimal otndrate { get; set; }
        public decimal othdrate { get; set; }
        public enRateType? otratetype { get; set; }
        public decimal gracein { get; set; }
        public decimal graceout { get; set; }
        public DateTime shiftinwkdayd { get; set; }
        public DateTime shiftinwkdayn { get; set; }
        public DateTime shiftinsatd { get; set; }
        public DateTime shiftinsatn { get; set; }
        public DateTime shiftinsund { get; set; }
        public DateTime shiftinsunn { get; set; }
        public DateTime shiftoutwkdayd { get; set; }
        public DateTime shiftoutwkdayn { get; set; }
        public DateTime shiftoutsatd { get; set; }
        public DateTime shiftoutsatn { get; set; }
        public DateTime shiftoutsund { get; set; }
        public DateTime shiftoutsunn { get; set; }
        public DateTime brkinwkdayd { get; set; }
        public DateTime brkinwkdayn { get; set; }
        public DateTime brkinsatd { get; set; }
        public DateTime brkinsatn { get; set; }
        public DateTime brkinsund { get; set; }
        public DateTime brkinsunn { get; set; }
        public DateTime brkoutwkdayd { get; set; }
        public DateTime brkoutwkdayn { get; set; }
        public DateTime brkoutsatd { get; set; }
        public DateTime brkoutsatn { get; set; }
        public DateTime brkoutsund { get; set; }
        public DateTime brkoutsunn { get; set; }
        public decimal minot { get; set; }
        public decimal maxot { get; set; }
        public decimal stdhrsdaily { get; set; }
        public decimal stdhrswkly { get; set; }
        public decimal stdhrsmnthly { get; set; }
        public decimal maxhrsdaily { get; set; }
        public decimal minhrsdaily { get; set; }
        public int shiftinallow { get; set; }
        public int shiftoutallow { get; set; }
        public int brkinallow { get; set; }
        public int brkoutallow { get; set; }
        public enClockMode clockmode { get; set; }
        public decimal stdhrswkdayd { get; set; }
        public decimal stdhrssatd { get; set; }
        public decimal stdhrssund { get; set; }
        public decimal stdhrswkdayn { get; set; }
        public decimal stdhrssatn { get; set; }
        public decimal stdhrshdd { get; set; }
        public decimal stdhrshdn { get; set; }
        public decimal stdhrssunn { get; set; }
        public decimal maxhrswkdayd { get; set; }
        public decimal maxhrssatd { get; set; }
        public decimal maxhrssund { get; set; }
        public decimal maxhrswkdayn { get; set; }
        public decimal maxhrssatn { get; set; }
        public decimal maxhrssunn { get; set; }
        public decimal lunchpenalty { get; set; }
        public decimal lunchmin { get; set; }
        //public decimal stdhrshdd { get; set; }
        //public decimal stdhrshdn { get; set; }
        public enOTND otndtype { get; set; }
        public enOTHD othdtype { get; set; }
        public int shiftinpenalty { get; set; }
        public int shiftoutpenalty { get; set; }
        //public int stdwrkdays { get; set; }
        public enRateMode ratemode { get; set; }
        public enRateMode lvrtmode { get; set; }
        public enRateMode absrtmode { get; set; }
        public int absrtdays { get; set; }
        public int lvrtdays { get; set; }
        public int srvcrtdays { get; set; }
        public decimal otwkdayd { get; set; }
        public decimal otsatd { get; set; }
        public decimal otsund { get; set; }
        public decimal otwkdayn { get; set; }
        public decimal otsatn { get; set; }
        public decimal otsunn { get; set; }
        public bool issatwkday { get; set; }
        public bool issunwkday { get; set; }
        public bool ismonwkday { get; set; }
        public bool istuewkday { get; set; }
        public bool iswedwkday { get; set; }
        public bool isthuwkday { get; set; }
        public bool isfriwkday { get; set; }
        public bool isdeleted { get; set; }
        public bool isactive { get; set; }
        public int userid { get; set; }
        public int compid { get; set; }
        public DateTime createdate { get; set; }
    }
    public class EnrolledEmployee
    {
        public EnrolledEmployee()
        {
            AttPermanentRecords = new HashSet<AttPermanent>();
            AttCasualRecords = new HashSet<AttCasual>();
            AttContractRecords = new HashSet<AttContract>();
        }
        [Key]
        public int id { get; set; }
        public string empcode { get; set; }
        public int enrollno { get; set; }
        public enEmpType type { get; set; }
        public int department { get; set; }
        public int shift { get; set; }
        public virtual ShiftType ShiftType { get; set; }
        public enFrequencyType pmtfrequency {get;set;}
        public bool workstatus {get;set;}
        public bool iscalcot {get;set;}
        public bool islosthrs { get; set; }
        public bool isdeleted {get;set;}
        public int compid { get; set; }
        public enBreaks breaks { get; set; }
        public virtual ICollection<AttPermanent> AttPermanentRecords { get; set; }
        public virtual ICollection<AttCasual> AttCasualRecords { get; set; }
        public virtual ICollection<AttContract> AttContractRecords { get; set; }
    }
    public class Device
    {
        [Key]
        public int id { get; set; }
        public int devno { get; set; }
        public string name { get; set; }
        public string ip { get; set; }
        public enDeviceModel model { get; set; }
        public int port { get; set; }
        public string comkey { get; set; }
        public enBaudRate baudrate { get; set; }
        public int delay { get; set; }
        public bool ipcomm { get; set; }
        public bool slavenodata { get; set; }
        public enDeviceTransMode transmode { get; set; }
        public enSDK_Name sdkname { get; set; }
        public int compid { get; set; }
        public bool isdeleted { get; set; }
        public bool isactive { get; set; }
    }
    public class Holiday
    {
        [Key]
        public int id { get; set; }
        public string descr { get; set; }
        public DateTime holdate { get; set; }
        public int compid { get; set; }
        public bool isdeleted { get; set; }
        public bool isactive { get; set; }
    }
    public class WorkCode
    {
        public int id { get; set; }
        public int code { get; set; }
        public int type { get; set; }
    }
    public class EmpRoster
    {
        [Key]
        public int id { get; set; }
        public int empid { get; set; }
        public int year { get; set; }
        public int month { get; set; }
        public string d1 { get; set; }
        public string d2 { get; set; }
        public string d3 { get; set; }
        public string d4 { get; set; }
        public string d5 { get; set; }
        public string d6 { get; set; }
        public string d7 { get; set; }
        public string d8 { get; set; }
        public string d9 { get; set; }
        public string d10 { get; set; }
        public string d11 { get; set; }
        public string d12 { get; set; }
        public string d13 { get; set; }
        public string d14 { get; set; }
        public string d15 { get; set; }
        public string d16 { get; set; }
        public string d17 { get; set; }
        public string d18 { get; set; }
        public string d19 { get; set; }
        public string d20 { get; set; }
        public string d21 { get; set; }
        public string d22 { get; set; }
        public string d23 { get; set; }
        public string d24 { get; set; }
        public string d25 { get; set; }
        public string d26 { get; set; }
        public string d27 { get; set; }
        public string d28 { get; set; }
        public string d29 { get; set; }
        public string d30 { get; set; }
        public string d31 { get; set; }
       
    }
    public class LeaveApplication
    {
        [Key]
        public int id { get; set; }
        public int empid { get; set; }
        public virtual LeaveType LeaveType { get; set; }
        public int leaveid { get; set; }
        public DateTime dtstart { get; set; }
        public DateTime dtend { get; set; }
        public decimal days { get; set; }
        public bool isapproved { get; set; }
        public int approvedby { get; set; }
        public bool ishalfday { get; set; }
        public int compid { get; set; }
        public DateTime createdate { get; set; }
        public bool isdeleted { get; set; }
        public bool isactive { get; set; }
    }
    public class LeaveType
    {
        public LeaveType()
        {
            LeaveApplications = new HashSet<LeaveApplication>();
        }
        [Key]
        public int id { get; set; }
        public int attstatus { get; set; }
        public virtual ICollection<LeaveApplication> LeaveApplications { get; set; }
    }
}
    