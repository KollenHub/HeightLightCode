using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace HeightLightCode.Views
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //加载所有的dll
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
#if !DEBUG  //以管理员身份启动
            var wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            var wp = new System.Security.Principal.WindowsPrincipal(wi);

            bool runAsAdmin = wp.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);

            if (!runAsAdmin)
            {
                var processInfo = new ProcessStartInfo(System.Reflection.Assembly.GetExecutingAssembly().CodeBase);
                processInfo.UseShellExecute = true;
                processInfo.Verb = "runas";
                try
                {
                    Process.Start(processInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("程序自动以管理员身份运行出错，请手动设置以管理员身份运行程序" + ex);
                    throw;
                }
                Environment.Exit(0);

            }
#endif
            MainWindow win = new MainWindow();
            win.ShowDialog();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var assemblyName = new AssemblyName(args.Name);
            string location = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var file = System.IO.Path.Combine(location, $"{ assemblyName.Name}.dll");

            if (System.IO.File.Exists(file))
            {
                return Assembly.LoadFrom(file);
            }

            //程序继续往下执行找别的
            return null;

        }
    }
}
