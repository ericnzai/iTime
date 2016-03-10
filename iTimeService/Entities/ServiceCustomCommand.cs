using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace iTimeService.Entities
{
    public enum  enCommandCode
    {
        LeaveApplication = 140,
        GeneralUpdate = 150
    }
    public enum enCommandStatus
    {
        Pending = 1,
        Executing = 2,
        Completed = 3,
        Failed = 4
    }
    public class ServiceCustomCommand
    {
        [Key]
        public int commmandid { get; set; }
        public int commandcode { get; set; }
        public string empid { get; set; }
        public DateTime cmdstart { get; set; }
        public DateTime cmdend { get; set; }
        public enCommandStatus cmdstatus { get; set; }
        public string extrainfo1 { get; set; }
        public string extrainfo2 { get; set; }
    }
}
