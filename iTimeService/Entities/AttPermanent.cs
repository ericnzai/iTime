using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace iTimeService.Entities
{
    public enum enAttStatus
    {
        Work =0,
        Leave =1,
        Sick = 2,
        Off = 3,
        Off2 = 4,
        Sick2 = 5
        

    }
     public enum enGlobalTimeInOutMode
     {
        ExactShiftTime = 0,
        DynamicTime = 1
     }
     public enum enOTApplicability
     {
        APPLICABLETOALL = 0,
        NOTAPPLICABLE = 1,
        DEFINABLE = 2
     }
     
    public  class AttPermanent : AttendanceBase
    {
       
    }
    public class AttCasual : AttendanceBase
    {
      
    }
    public class AttContract : AttendanceBase
    {
      
    }
    public class CompanySettings
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int id { get; set; }
        public int compid { get; set; }
        public int firstdayofwk { get; set; }
        public enOTDef otdefinition { get; set; }
        public enGlobalTimeInOutMode timein { get; set; }
        public enGlobalTimeInOutMode timeout { get; set; }
        public enOTApplicability overtime { get; set; }
        public bool isdeleted { get; set; }
        public bool useroster { get; set; }
    }
}
