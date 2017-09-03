using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using Cjwdev.WindowsApi;
using log4net;
using log4net.Config;
using Microsoft.Win32;

namespace SwitchServices
{
    public class CommHelper
    {
        private static ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public CommHelper()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"/Config/log4net.config";
            XmlConfigurator.ConfigureAndWatch(new FileInfo(path));

        }

        /// <summary>
        /// 切换进程1
        /// </summary>
        /// <param name="killAppName"></param>
        /// <param name="openAppName"></param>
        /// <param name="openAppNamePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void ToggleProcess(string killAppName,string openAppName,string openAppNamePath, string args)
        {
            //获得到进程，杀死进程
            if (!string.IsNullOrEmpty(killAppName))
            {
                Process[] processes = Process.GetProcessesByName(killAppName);
                if (processes.Length > 0)
                {
                    _log.Info(string.Format("Kill进程：{0}", killAppName));
                    foreach (Process process in processes)
                    {
                        process.Kill();
                    }
                }
            }

            //判断要开启的进程是否已经开启，已经开启则无需开启
            var startProcess = Process.GetProcessesByName(openAppName);
            if (startProcess.Length == 0)
            {
                if (!string.IsNullOrEmpty(openAppNamePath))
                {
                    //启动进程
                    if (File.Exists(openAppNamePath))
                    {
                        if (string.IsNullOrEmpty(args))
                        {
                            Process.Start(openAppNamePath);
                        }
                        else
                        {
                            Process.Start(openAppNamePath, args);
                        }
                        _log.Info(string.Format("启动进程：{0}  参数:{1}", openAppNamePath, args));
                    }
                }
            }
        }

        /// <summary>
        /// 切换进程2
        /// </summary>
        /// <param name="killAppName"></param>
        /// <param name="openAppName"></param>
        /// <param name="openAppNamePath"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static void ToggleProcess4UserSession(string killAppName, string openAppName, string openAppNamePath, string args)
        {
            //获得到进程，杀死进程
            if (!string.IsNullOrEmpty(killAppName))
            {
                Process[] processes = Process.GetProcessesByName(killAppName);
                if (processes.Length > 0)
                {
                    _log.Info(string.Format("Kill进程：{0}", killAppName));
                    foreach (Process process in processes)
                    {
                        process.Kill();
                    }
                }
            }

            //判断要开启的进程是否已经开启，已经开启则无需开启
            var startProcess = Process.GetProcessesByName(openAppName);
            if (startProcess.Length == 0)
            {
                if (!string.IsNullOrEmpty(openAppNamePath))
                {
                    //启动进程
                    if (File.Exists(openAppNamePath))
                    {
                        try
                        {
                            IntPtr userTokenHandle = IntPtr.Zero;
                            ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                            ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                            ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();

                            startInfo.cb = (uint)Marshal.SizeOf(startInfo);
                            ApiDefinitions.CreateProcessAsUser(userTokenHandle, openAppNamePath, args, IntPtr.Zero, IntPtr.Zero, false, 0, IntPtr.Zero, null, ref startInfo, out procInfo);
                            if (userTokenHandle != IntPtr.Zero)
                            {
                                ApiDefinitions.CloseHandle(userTokenHandle);
                            }

                            //int _currentAquariusProcessId = (int)procInfo.dwProcessId;

                        }
                        catch (Exception ex)
                        {
                            _log.ErrorFormat("Start Application failed, its path is {0} ,exception: {1}", openAppNamePath, ex.Message);
                        }
                    }
                }
            }
        }

        //windows服务安装后会在注册表中存储服务信息，路径是HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Services\[服务名称]
        //获取服务路径
        public static string GetServicePath(string serviceName, string machineName, out string imageName)
        {
            imageName = string.Empty;
            var ret = string.Empty;

            string registryPath = @"SYSTEM\CurrentControlSet\Services\" + serviceName;
            RegistryKey keyHKLM = Registry.LocalMachine;

            RegistryKey key;
            if (string.IsNullOrEmpty(machineName) || machineName == "localhost")
            {
                key = keyHKLM.OpenSubKey(registryPath);

            }
            else
            {
                key = RegistryKey.OpenRemoteBaseKey
                    (RegistryHive.LocalMachine, machineName).OpenSubKey(registryPath);
            }

            if (key != null)
            {
                var imagePath = key.GetValue("ImagePath").ToString();
                key.Close();
                var serviceFile = Environment.ExpandEnvironmentVariables(imagePath.Replace("\"", ""));

                if (serviceFile.IndexOf(".exe", System.StringComparison.CurrentCulture) > 0)
                {
                    var path = serviceFile.Substring(0, serviceFile.IndexOf(".exe", StringComparison.CurrentCulture) + 4);
                    var fileInfo = new FileInfo(path);
                    imageName = fileInfo.Name;
                    return new FileInfo(path).DirectoryName;
                }
            }
            return ret;
        }

        #region 服务
        //Taskkill命令是   taskkill /s 172.19.2.107 /f /t /im "[映像名称]" /U [远程机器的用户名] /P [远程机器的密码]
        /// <summary>
        /// 结束服务进程
        /// </summary>
        /// <param name="imagename"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string TaskKillService(string imagename, string user, string password, string ip)
        {

            string ret = string.Empty;
            var process = new Process();
            process.StartInfo.FileName = "taskkill.exe";
            process.StartInfo.Arguments = string.Format(" /s {0} /f /t /im \"{1}\" /U {2} /P {3}", ip, imagename, user, password);
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            //process.StartInfo.StandardOutputEncoding = Encoding.UTF8;

            //process.OutputDataReceived += (s, e) =>
            //{
            //    ret += e.Data;
            //};
            //process.ErrorDataReceived += (s, e) =>
            //{
            //    ret += e.Data;
            //};
            //process.BeginOutputReadLine();
            //process.BeginErrorReadLine();
            process.Start();

            ret = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return ret;
        }


        //获取服务状态
        public static ServiceControllerStatus GetServiceStatus(string serviceName, string ip)
        {
            try
            {
                var service = new System.ServiceProcess.ServiceController(serviceName, ip);
                return service.Status;
            }
            catch (Exception ex)
            {
                string err = ex.Message;
                return ServiceControllerStatus.Stopped;
            }
        }


        //启动服务  
        public static string StartService(string serviceName, string ip)
        {
            try
            {
                var service = new System.ServiceProcess.ServiceController(serviceName, ip);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Running)
                    return "正在运行";

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
                return "正在启动";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        //停止服务
        public static string StopService(string serviceName, string ip)
        {
            try
            {
                var service = new System.ServiceProcess.ServiceController(serviceName, ip);
                if (service.Status == System.ServiceProcess.ServiceControllerStatus.Stopped)
                    return "已经停止";
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(5));
                return "正在停止";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
    }
}
