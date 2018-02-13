using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace LNHSVoting
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += (s, args) => {
            //    var resourceName = string.Format("{0}.{1}.{2}.dll", nameof(LNHSVoting), "Dependencies", new AssemblyName(args.Name).Name);
            //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            //    {
            //        var assemblyData = new Byte[stream.Length];
            //        stream.Read(assemblyData, 0, assemblyData.Length);

            //        return Assembly.Load(assemblyData);
            //    }
            //};
            
        }
    }
}
