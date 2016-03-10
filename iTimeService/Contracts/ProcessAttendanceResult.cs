using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace iTimeService.Contracts
{
    [DataContract]
    public class ProcessAttendanceResult
    {
        [DataMember]
        public double Count;
        [DataMember]
        public string Message;
    }
}
