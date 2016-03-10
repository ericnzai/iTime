using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Quartz;
using iTimeService.Concrete;
using iTimeService.Entities;
using System.Configuration;
using iTimeService.Common;
using System.ServiceProcess;
using log4net;
using log4net.Config;


using System.Reflection;
using System.Xml.XmlConfiguration;
namespace iTimeService.Services
{
    public class iTimeMainService : ServiceBase
    {
        private int rawId { get;  set; }
        private int devId { get; set; }
        private IUnitOfWork _unitOfWork = new UnitOfWork();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        //readonly Timer _timer;
        public iTimeMainService()
        {
            Common.Common.SetCompID();
        }
        public bool Start()
        {
            //_timer.Start();
            return true;
        }
        public bool Stop()
        {
            return true;
            //_timer.Stop();
        }
        protected override void OnCustomCommand(int command)
        {
            //log4net.Config.XmlConfigurator.Configure();

            //base.OnCustomCommand(command);
            if (command == (int)enCommandCode.GeneralUpdate)
            {
                ServiceCustomCommand svcCmd = _unitOfWork.ServiceCustomCommands.All()
                                            .Where(x => x.commandcode == command)
                                            .Where(x => x.cmdstatus == enCommandStatus.Pending)
                                            .LastOrDefault();
                if (svcCmd != null)
                {
                    svcCmd.cmdstatus = enCommandStatus.Completed;
                    _unitOfWork.ServiceCustomCommands.Update(svcCmd);
                    _unitOfWork.Commit();
                }
               
                //if (svcCmd != null || svcCmd == null)
                //{
                    //log.Info("Custom Command Executed at " + DateTime.Now);
                //}

            }
        }
        
       
    }
    
}
