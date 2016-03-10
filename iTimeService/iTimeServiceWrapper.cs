using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceProcess;
using log4net;
using System.Reflection;

namespace iTimeService
{
    public class iTimeServiceWrapper<TServiceImplementation,TServiceContract> : ServiceBase
        where TServiceImplementation:TServiceContract
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private ServiceHost _serviceHost;
        private string _serviceUri;
        public iTimeServiceWrapper(string serviceName,string serviceUri)
        {
            ServiceName = serviceName;
            _serviceUri = serviceUri;
        }
        protected override void OnStart(string[] args)
        {
            Start();
        }
        protected override void OnStop()
        {
            Stop();
        }
        public void Start()
        {
            _log.Info(ServiceName + " starting......" + DateTime.Now);
            //Console.WriteLine(ServiceName + " starting......");
            bool openSucceeded = false;
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                }
                _serviceHost = new ServiceHost(typeof(TServiceImplementation));
            }
            catch (Exception e)
            {
                _log.Info("Caught Exception while CREATING " + ServiceName, e);
                //Console.WriteLine("Caught Exception while CREATING " + ServiceName + ": Exception Msg:" + e.Message);
                return;
            }
            try
            {
                //var webHttpBinding = new WebHttpBinding(WebHttpSecurityMode.None);
                //_serviceHost.AddServiceEndpoint(typeof(TServiceContract), webHttpBinding, _serviceUri);


                //var webHttpBehaivour = new WebHttpBehavior
                //{
                //    DefaultOutgoingResponseFormat = System.ServiceModel.Web.WebMessageFormat.Json
                //};

                //var netTcpBinding = new NetTcpBinding(SecurityMode.None);
                //_serviceHost.AddServiceEndpoint(typeof(TServiceContract), netTcpBinding, _netTcpUri);

                //ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                //behavior.HttpGetEnabled = true;
                //_serviceHost.Description.Behaviors.Add(behavior);
                //_serviceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(),
                //    "mex1");
               
                //_serviceHost.Description.Endpoints[0].Behaviors.Add(webHttpBehaivour);

                _serviceHost.Open();
                openSucceeded = true;
            }
            catch (Exception e)
            {
                _log.Info("Caught Exception while STARTING " + ServiceName, e);
                //Console.WriteLine("Caught Exception while STARTING " + ServiceName + ": Exception Msg:" + e.Message);
            }
            finally
            {
                if (!openSucceeded)
                {
                    _serviceHost.Abort();
                }
            }
            if (_serviceHost.State == CommunicationState.Opened)
            {
                _log.Info(ServiceName + " started at " + DateTime.Now);
                _log.Info(ServiceName + " listening at " + _serviceUri);
                //Console.WriteLine(ServiceName + " started at " + DateTime.Now);
            }
            else
            {
                _log.Info(ServiceName + " failed to start");
                //Console.WriteLine(ServiceName + " failed to start");
                bool closeSucceeded = false;
                try
                {
                    _serviceHost.Close();
                    closeSucceeded = true;
                }
                catch (Exception e)
                {
                    _log.Info(ServiceName + " failed to close ", e);
                    //Console.WriteLine(ServiceName + " failed to close " + e.Message);
                }
                finally
                {
                    if (!closeSucceeded)
                    {
                        _serviceHost.Abort();
                    }
                }

            }
        }
        public new void Stop()
        {
            _log.Info(ServiceName + " stopping..........");
            //Console.WriteLine(ServiceName + " stopping.......");
            try
            {
                if (_serviceHost != null)
                {
                    _serviceHost.Close();
                    _serviceHost = null;
                }
            }
            catch (Exception e)
            {
                _log.Info("Caught exception when stopping " + ServiceName, e);
                //Console.WriteLine("Caught exception when stopping " + ServiceName + " Message: " + e.Message);
            }
            finally
            {
                _log.Info(ServiceName + " stopped....");
                //Console.WriteLine(ServiceName + " stopped....");
            }

        }
    }
}
