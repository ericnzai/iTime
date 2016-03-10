using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using iTimeService.Concrete;
using iTimeService.Entities;
using System.Net.Configuration;
using System.Net.NetworkInformation;
using System.Globalization;
using zkemkeeper;
using BioBridgeSDK;
using BioBridgeSDKLib;
using System.Text.RegularExpressions;
using System.Threading.Tasks.Schedulers;
using iTimeService.Common;

namespace iTimeService.Services
{
    public class DownloadDeviceDataService
    {
        //Calendar instance ;
        public static DateTimeFormatInfo dateInfo = CultureInfo.CurrentCulture.DateTimeFormat;
        Calendar instance = dateInfo.Calendar;
        //IBioBridgeSDKX bioBridge = new BioBridgeSDKX();

        //BioBridgeSDKLib.BioBridgeSDK bioBridge = new BioBridgeSDKLib.BioBridgeSDK();
        
        zkemkeeper.CZKEM czKEM = new CZKEM();
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private string[] devName;
        private string[] ipAdd;
        private string[] devModel;
        private string[] sdkName;
        private int[] devId;
        private string[] comKey;
        private int[] portNo;
        private int[] baudRate;
        private int[] devDelay;
        private bool[] ipComm;
       
        public DownloadDeviceDataService()
        {
           
        }
        public void StartDownload()
        {
            try
            {
                IEnumerable<Device> devices;
                string strDeviceModel = "";
                //get valid devices
                using (var db = new iTimeServiceContext())
                {
                    devices = db.Set<Device>()
                                .Where(x => (x.isdeleted == false || x.isdeleted == null))
                                .Where(x => x.isactive == true)
                                .ToList();
                }
                if (devices.Count() > 0)
                {
                    foreach (var dev in devices)
                    {
                        string ipAdd = tripIpAdd(dev.ip);
                        if (dev.model == enDeviceModel.Black_White)
                        {
                            strDeviceModel = "Black n White";
                        }
                        else
                        {
                            strDeviceModel = "MultiMedia";
                        }
                        Ping ping = new Ping();
                        PingReply reply;

                        try
                        {
                            reply = ping.Send(ipAdd);
                            if (reply.Status == IPStatus.Success)
                            {
                                if (dev.sdkname == enSDK_Name.ZKemKeeper)
                                {
                                    string idwEnrollNumber;
                                    int  enrollNo,  idwVerifyMode;
                                    int idwInOutMode, idwYear, idwMonth, idwDay, idwHour, idwMinute, idwErrorCode;
                                    int idwSecond, idwWorkCode, idwReserved;
                                    DateTime punchTime;
                                    var glCount = 0;
                                    idwWorkCode = 0;
                                    idwErrorCode = 0;
                                    idwReserved = 0;
                                    bool isConnected;
                                    //ipAdd = "192.168.1.206";
                                    isConnected = czKEM.Connect_Net(ipAdd, dev.port);
                                    if (isConnected)
                                    {
                                        if (czKEM.ReadGeneralLogData(dev.devno))
                                        {
                                            if (dev.model == enDeviceModel.MultiMedia)
                                            {
                                                while (this.czKEM.SSR_GetGeneralLogData(dev.devno, out idwEnrollNumber, out idwVerifyMode, out idwInOutMode, out idwYear, out idwMonth, out idwDay, out idwHour, out idwMinute, out idwSecond, idwWorkCode) == true)
                                                {
                                                    punchTime = instance.ToDateTime(idwYear, idwMonth, idwDay, idwHour, idwMinute, idwSecond, 0);

                                                    bool rawDataExists = false;
                                                    using (var db = new iTimeServiceContext())
                                                    {
                                                        enrollNo = Int32.Parse(idwEnrollNumber.ToString());
                                                        rawDataExists = db.Set<RawData>()
                                                                        .Where(x => x.ENROLL_NO == enrollNo && x.DT_YR == idwYear)
                                                                        .Where(x => x.DT_MNTH == idwMonth && x.DT_DAY == idwDay)
                                                                        .Where(x => x.DT_HR == idwHour && x.DT_MIN == idwMinute && x.DT_SEC == idwSecond)
                                                                        .Where(x => x.DEV_ID == dev.devno)
                                                                        .Count() > 0;
                                                    }
                                                    if (!rawDataExists)
                                                    {
                                                        RawData rData = new RawData
                                                        {
                                                            ENROLL_NO = enrollNo,
                                                            DT_YR = idwYear,
                                                            DT_MNTH = idwMonth,
                                                            DT_DAY = idwDay,
                                                            DT_HR = idwHour,
                                                            DT_MIN = idwMinute,
                                                            DT_SEC = idwSecond,
                                                            VER = idwVerifyMode,
                                                            R_IO = idwInOutMode,
                                                            R_WORK = idwWorkCode,
                                                            DEV_ID = dev.devno,
                                                            R_LOG = idwReserved,
                                                            DATE_LOG = DateTime.Now,
                                                            PUNCH_TIME = punchTime
                                                        };
                                                        using (var db = new iTimeServiceContext())
                                                        {
                                                            db.RawData.Add(rData);
                                                            db.SaveChanges();
                                                        }
                                                        Common.Common._insertedOk = true;
                                                    }
                                                }
                                                czKEM.ClearGLog(dev.devno);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        czKEM.GetLastError(idwErrorCode);
                                        if (idwErrorCode != 0)
                                        {
                                            _log.Info("Device data download failed. Error Code:" + idwErrorCode);
                                        }
                                        else _log.Info("Device returns no data");
                                        Common.Common._insertedOk = false;
                                       
                                    }
                                }
                                else if (dev.sdkname == enSDK_Name.BioBridgeSDK)
                                {
                                    //DownLoadBioBridge(dev, ipAdd);
                                    frmAxBioBridge bioBrdgeContainer = new frmAxBioBridge();
                                    string strDevModel = "Black n White";
                                    int icomKey = Int32.Parse(dev.comkey.ToString());
                                    int EnrollNo = 0; int yr = 0; int mth = 0; int day_Renamed = 0; int hr = 0; int min = 0; int sec = 0; int ver = 0; int io = 0; int work = 0; int rlog = 0;
                                    DateTime punchTime;
                                    if (bioBrdgeContainer.axBioBridgeSDK1.Connect_TCPIP(strDevModel, dev.devno, ipAdd, dev.port, icomKey) == 0)
                                    {
                                        if (dev.model == enDeviceModel.Black_White)
                                        {
                                            if (bioBrdgeContainer.axBioBridgeSDK1.ReadGeneralLog(ref rlog) == 0)
                                            {
                                                //while (bioBrdgeContainer.axBioBridgeSDK1.GetGeneralLogData(dev.devno, EnrollNo, ver, io, yr, mth, day_Renamed, hr, min, sec, work) == 0)
                                                    while (bioBrdgeContainer.axBioBridgeSDK1.GetGeneralLog(ref EnrollNo, ref yr, ref mth,  ref day_Renamed,ref hr, ref min, ref sec, ref ver, ref io,ref work) == 0)
                                                    {
                                                        punchTime = instance.ToDateTime(yr, mth, day_Renamed, hr, min, sec, 0);
                                                        bool rawDataExists = false;
                                                        using (var db = new iTimeServiceContext())
                                                        {
                                                            rawDataExists = db.Set<RawData>()
                                                                            .Where(x => x.ENROLL_NO == EnrollNo && x.DT_YR == yr)
                                                                            .Where(x => x.DT_MNTH == mth && x.DT_DAY == day_Renamed)
                                                                            .Where(x => x.DT_HR == hr && x.DT_MIN == min && x.DT_SEC == sec)
                                                                            .Where(x => x.DEV_ID == dev.devno)
                                                                            .Count() > 0;
                                                        }
                                                        if (!rawDataExists)
                                                        {
                                                            RawData rData = new RawData
                                                            {
                                                                ENROLL_NO = EnrollNo,
                                                                DT_YR = yr,
                                                                DT_MNTH = mth,
                                                                DT_DAY = day_Renamed,
                                                                DT_HR = hr,
                                                                DT_MIN = min,
                                                                DT_SEC = sec,
                                                                VER = ver,
                                                                R_IO = io,
                                                                R_WORK = work,
                                                                DEV_ID = dev.devno,
                                                                R_LOG = rlog,
                                                                DATE_LOG = DateTime.Now,
                                                                PUNCH_TIME = punchTime
                                                            };
                                                            using (var db = new iTimeServiceContext())
                                                            {
                                                                db.RawData.Add(rData);
                                                                db.SaveChanges();
                                                                Common.Common._insertedOk = true;
                                                            }
                                                        }
                                                    }
                                            }
                                            bioBrdgeContainer.axBioBridgeSDK1.DeleteGeneralLog();
                                        }
                                    }
                                    else
                                    {
                                        Common.Common._insertedOk = false;
                                        _log.Info("Connection to device IP : " + ipAdd + " Failed.");
                                    }
                                    bioBrdgeContainer.Dispose();
                                }
                            }
                            else if (reply.Status == IPStatus.DestinationHostUnreachable)
                            {
                                Common.Common._insertedOk = false;
                                _log.Info("Device name " + dev.name + " with IP address: " + ipAdd + " was not reachable  at " + DateTime.Now);
                            }
                        }
                        catch (Exception ex)
                        {
                            Common.Common._insertedOk = false;
                            Common.Common._exception = ex;
                            //_log.Debug("Network connection to device :" + dev.name + " [IP: " + ipAdd + "] Failed", ex.InnerException);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Common._insertedOk = false;
                Common.Common._exception = ex;
            }
        }
        public async Task DownLoadBioBridge(Device dev, string ipAdd)
        {
            string strDevModel = "Black n White";
            int icomKey = Int32.Parse(dev.comkey);
            int EnrollNo = 0; int yr = 0; int mth = 0; int day_Renamed = 0; int hr = 0; int min = 0; int sec = 0; int ver = 0; int io = 0; int work = 0; int rlog = 0;
            DateTime punchTime;
            using (var sta = new StaTaskScheduler(1))
            {
                var taskResult = await Task.Factory.StartNew(() =>
                {
                    int result = 0;
                    var ax = new BioBridgeSDKX();
                    ////ax.CreateControl();
                    if (ax.Connect_TCPIP(strDevModel, dev.devno, ipAdd, dev.port, icomKey) == 0)
                    {
                        result = 0;
                    
                    }
                    return result;
                }, System.Threading.CancellationToken.None, TaskCreationOptions.None, sta);
               
            }
        }
        private string tripIpAdd(string ipAdd)
        {
            char[] array = new Char[1];
            char[] zero = new Char[1];
            int i = 0;
            array[0] = Char.Parse(".");
            zero[0] = Char.Parse("0");
            string str = "";

            string[] parts = ipAdd.Split(array);
            foreach (var part in parts)
            {
                string strPart = part.TrimStart(zero);
                if (strPart == "") strPart = "0" ;
                str += "" + strPart;
                if (i < 3) str += ".";
                i++;
            }
            return str;
        }
    }
}
