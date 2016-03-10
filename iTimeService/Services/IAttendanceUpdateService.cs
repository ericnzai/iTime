using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTimeService.Entities;
using iTimeService.Contracts;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace iTimeService.Services
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IAttendanceUpdateService
    {
        //IEnumerable<RawData> GetRawData(bool isProcessed, int enrollNo,DateTime dtFrom, DateTime dtTo);
        //EnrolledEmployee GetEmployee(int enrollNo);
        //[WebGet(UriTemplate = "ProcessAtt/{n1}/{n2}/{n3}",BodyStyle=WebMessageBodyStyle.Bare)]
        [OperationContract]
        ProcessAttendanceResult ProcessAtt(string dtFrom, string dtTo,string strEmpIds,string machineName,string ipAddress);

        void ProcessRawData(EnrolledEmployee emp, DateTime dtFrom, DateTime dtTo, string tableName = "");
    }
}
