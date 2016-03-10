using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using iTimeService.Entities;
using iTimeService.Concrete;
using iTimeService.Services;

using log4net;
using log4net.Config;

using System.Configuration;
using System.Reflection;
using System.Xml.XmlConfiguration;
namespace iTimeService.Jobs
{
    public class TruncateLogDataJob :IJob
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public TruncateLogDataJob()
        {
        }
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                using (var db = new iTimeServiceContext())
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE log4net");
                }
            }
            catch (Exception ex)
            {
                log.Info("Exception while truncating log data", ex);
            }
        }
    }
}
