using System.Collections;
using System.Management;

namespace SwitchServices
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.SwitchServices = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // SwitchServices
            // 
            this.SwitchServices.Description = "SwitchServices";
            this.SwitchServices.DisplayName = "SwitchServices";
            this.SwitchServices.ServiceName = "Service1";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.SwitchServices});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller SwitchServices;


        protected override void OnAfterInstall(IDictionary savedState)
        {
            //允许服务与桌面交互
            base.OnAfterInstall(savedState);
            ManagementObject wmiService = null;
            ManagementBaseObject InParam = null;
            try
            {
                wmiService = new ManagementObject(string.Format("Win32_Service.Name='{0}'", SwitchServices.ServiceName));
                InParam = wmiService.GetMethodParameters("Change");
                InParam["DesktopInteract"] = true;
                wmiService.InvokeMethod("Change", InParam, null);
            }
            finally
            {
                if (InParam != null)
                    InParam.Dispose();
                if (wmiService != null)
                    wmiService.Dispose();
            }
        }
    }
}