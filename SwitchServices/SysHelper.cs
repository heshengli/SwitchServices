using System;

namespace SwitchServices
{
    public static class SysHelper
    {
        /// <summary>
        /// Windows98
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows98()
        {
            return (System.Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() != "2222A");
        }
        /// <summary>
        /// Windows98第二版
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows98Second()
        {

            return (Environment.OSVersion.Platform == PlatformID.Win32Windows) && (Environment.OSVersion.Version.Minor == 10) && (Environment.OSVersion.Version.Revision.ToString() == "2222A");

        }
        /// <summary>
        /// Windows2000
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows2000()
        {

            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0);

        }
        //C#判断操作系统是否为WindowsXP
        public static bool IsWindowsXp()
        {

            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1);

        }
        /// <summary>
        /// Windows2003
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows2003()
        {

            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 2);
        }
        /// <summary>
        /// WindowsVista
        /// </summary>
        /// <returns></returns>
        public static bool IsWindowsVista()
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 0);
        }
        /// <summary>
        /// Windows7
        /// </summary>
        /// <returns></returns>
        public static bool IsWindows7()
        {

            return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1);
        }
        /// <summary>
        /// Unix
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix()
        {
            return Environment.OSVersion.Platform == PlatformID.Unix;
        }
    }
}
