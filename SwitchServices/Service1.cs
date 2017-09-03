using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Cjwdev.WindowsApi;
using log4net;
using log4net.Config;

namespace SwitchServices
{
    public partial class Service1 : ServiceBase
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public Service1()
        {
            InitializeComponent();
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Config/log4net.config";
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                string status = ConfigurationManager.AppSettings["Status"];
                string appName = ConfigurationManager.AppSettings["AppName"];
                string appNamePath = ConfigurationManager.AppSettings["AppNamePath"];
                string cmdLine = ConfigurationManager.AppSettings["CmdLine"];
                string killAppName = ConfigurationManager.AppSettings["KillAppName"];
                string killAppNamePath = ConfigurationManager.AppSettings["KillAppNamePath"];
                string killCmdLine = ConfigurationManager.AppSettings["KillCmdLine"];

                //OpenApp4Service(appName, cmdLine);
                if (SysHelper.IsWindows2000()||SysHelper.IsWindows2003()||SysHelper.IsWindowsXp()||SysHelper.IsWindows98Second()||SysHelper.IsWindows98())
                {
                    _log.Info("操作系统为xp或以下版本，无需使用session0解决办法。");
                    if (status == "1")
                    {
                        CommHelper.ToggleProcess(killAppName, appName, appNamePath, cmdLine);
                    }
                    else
                    {
                        CommHelper.ToggleProcess(appName, killAppName, killAppNamePath, killCmdLine);
                    }
                }
                else
                {
                    _log.Info("操作系统为Vista、Win7或以上版本，使用session0解决办法。");
                    if (status == "1")
                    {
                        CommHelper.ToggleProcess4UserSession(killAppName, appName, appNamePath, cmdLine);
                    }
                    else
                    {
                        CommHelper.ToggleProcess4UserSession(appName, killAppName, killAppNamePath, killCmdLine);
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
            }
        }


        public enum SysType
        {
            Xp,
            Win7,
            Win8,
            Win10
        }

        private void OpenApp4Service(string appName, string cmdLine)
        {
            try
            {
                //string appStartPath = @"C:\\Deepleo.exe";
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();

                startInfo.cb = (uint)Marshal.SizeOf(startInfo);
                ApiDefinitions.CreateProcessAsUser(userTokenHandle, appName, cmdLine, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startInfo, out procInfo);
                if (userTokenHandle != IntPtr.Zero)
                {
                    ApiDefinitions.CloseHandle(userTokenHandle);
                }

                //int _currentAquariusProcessId = (int)procInfo.dwProcessId;

            }
            catch (Exception ex)
            {
                _log.ErrorFormat("Start Application failed, its path is {0} ,exception: {1}", appName, ex.Message);

            }
        }

        protected override void OnStop()
        {

        }
    }
}
