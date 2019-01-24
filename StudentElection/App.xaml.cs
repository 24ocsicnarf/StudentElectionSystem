using AutoMapper;
using StudentElection;
using StudentElection.MSAccess.AutoMapper;
using StudentElection.PostgreSQL.AutoMapper;
using StudentElection.Repository.Models;
using StudentElection.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

namespace StudentElection
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string ImageFolderPath { get; private set; }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            //AppDomain.CurrentDomain.AssemblyResolve += (s, args) => {
            //    var resourceName = string.Format("{0}.{1}.{2}.dll", nameof(StudentElection), "Dependencies", new AssemblyName(args.Name).Name);
            //    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
            //    {
            //        var assemblyData = new Byte[stream.Length];
            //        stream.Read(assemblyData, 0, assemblyData.Length);

            //        return Assembly.Load(assemblyData);
            //    }
            //};

            //TODO: pano ung sa server???????????????
            ImageFolderPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Images");
            if (!Directory.Exists(ImageFolderPath))
            {
                Directory.CreateDirectory(ImageFolderPath);
            }
            
            Mapper.Initialize(c =>
            {
                c.AddProfiles(typeof(MSAccessProfile),
                    typeof(PostgreSQLProfile));
            });
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"{ e.Exception.GetBaseException().Message }", "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Stop);
        }
    }
}
