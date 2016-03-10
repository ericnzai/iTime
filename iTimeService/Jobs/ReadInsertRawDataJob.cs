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
using System.Threading;
using System.Configuration;
using System.Reflection;
using System.Xml.XmlConfiguration;
namespace iTimeService.Jobs
{
    public class ReadInsertRawDataJob : IJob
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ReadInsertRawDataJob()
        {
        }
        public void Execute(IJobExecutionContext context)
        {
            try
            {
                log4net.Config.XmlConfigurator.Configure();
                log.Info("Starting job [ReadInsertRawDataJob] at : " + DateTime.Now);

               
                DownloadDeviceDataService _service = new DownloadDeviceDataService();
                try
                {
                    Thread thread = new Thread(_service.StartDownload);
                    thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                    thread.Join();
                }
                catch (Exception ex)
                {
                    log.Info("Error in setting single threaded apartment", ex);
                }
                if (Common.Common._insertedOk == true)
                {
                    log.Info("Raw data inserted successfully");
                }
                else
                {
                    log.Info("Error occured during raw data insert!", Common.Common._exception);
                }
            }
            catch (JobExecutionException ex)
            {
                log.Debug(ex.Message);
            }

        }
    }
}
