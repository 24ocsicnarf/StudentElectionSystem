using Humanizer.Configuration;
using Humanizer.DateTimeHumanizeStrategy;
using Project.Library.Helpers;
using StudentElection;
using StudentElection.Dialogs;
using StudentElection.Helpers;
using StudentElection.Main;
using StudentElection.MSAccess.AutoMapper;
using StudentElection.PostgreSQL.AutoMapper;
using StudentElection.Repository.Models;
using StudentElection.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StudentElection
{
    public enum DatabaseType
    {
        Unknown = -1,
        File = 0,
        Server = 1
    }

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public const string MdbFileName = "StudentElection.mdb";
        public const string ImageFolderName = "Images";
        
        public static string ImageFolderDirectory { get; private set; }
        public static string ImageFolderPath => Path.Combine(ImageFolderDirectory, ImageFolderName);
        public static string ExeDirectory => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        public UserService _userService = new UserService();

        public static void SetFolderPaths()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configFile.AppSettings.Settings["DatabaseType"].Value);
            if (databaseType == DatabaseType.File)
            {
                ImageFolderDirectory = ExeDirectory;
            }
            else if (databaseType == DatabaseType.Server)
            {
                var networkAddress = StudentElection.Properties.Network.Default.NetworkFolderAddress;
                var networkFolderName = StudentElection.Properties.Network.Default.NetworkFolderName;

                ImageFolderDirectory = $"\\\\{ Path.Combine(networkAddress, networkFolderName) }";
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            SetFolderPaths();

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configFile.AppSettings.Settings["DatabaseType"].Value);

            switch (databaseType)
            {
                case DatabaseType.Unknown:
                    SetUpDatabase();
                    return;
                case DatabaseType.File:
                    var result = ProvideMSAccessPassword();
                    if (result == false)
                    {
                        Current.Shutdown();
                        return;
                    }

                    ConfigurationHelper.ProtectConfiguration(configFile);
                    break;
                case DatabaseType.Server:
                    result = CheckServerDatabase();
                    if (result == false)
                    {
                        Current.Shutdown();
                        return;
                    }
                    break;
            }

            if (!Directory.Exists(ImageFolderDirectory))
            {
                MessageBox.Show("There's no network folder. Press OK to enter the setup", "No network folder", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                var window = new DatabaseSetupWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ShowInTaskbar = true;

                var result = window.ShowDialog();

                if (result != true)
                {
                    MessageBox.Show("You cannot use this system without a network folder", "No network folder", MessageBoxButton.OK, MessageBoxImage.Stop);
                }

                Current.Shutdown();

                return;
            }

            if (!Directory.Exists(ImageFolderPath))
            {
                Directory.CreateDirectory(ImageFolderPath);
            }

            int usersCount = await _userService.CountUsersAsync();
            if (usersCount == 0)
            {
                MessageBox.Show("No users yet. Press OK to add a superuser", "No users", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                var userWindow = new StaffWindow
                {
                    PresetUserType = UserType.Superuser
                };
                userWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                userWindow.WindowStyle = WindowStyle.ToolWindow;
                userWindow.Title = "Add superuser";
                userWindow.ShowInTaskbar = true;

                userWindow.ShowDialog();

                usersCount = await _userService.CountUsersAsync();

                if (usersCount == 0)
                {
                    MessageBox.Show("You cannot use this system without a user", "No users", MessageBoxButton.OK, MessageBoxImage.Stop);

                    Current.Shutdown();

                    return;
                }

                //if (Current.MainWindow == null)
                //{
                //    var mainWindow = new MainWindow();
                //    mainWindow.ShowDialog();
                //}
            }
        }

        private void SetUpDatabase()
        {
            MessageBox.Show("There's no database to be used. Press OK to enter the setup", "No database", MessageBoxButton.OK, MessageBoxImage.Exclamation);

            var window = new DatabaseSetupWindow();
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ShowInTaskbar = true;

            var result = window.ShowDialog();

            if (result != true)
            {
                MessageBox.Show("You cannot use this system without a database", "No database", MessageBoxButton.OK, MessageBoxImage.Stop);

                Current.Shutdown();
            }
        }

        private bool CheckServerDatabase()
        {
            try
            {
                using (var context = new PostgreSQL.Model.StudentElectionContext())
                {
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show("Unable to connect with the database. Press OK to enter the setup", "Database connection problem", MessageBoxButton.OK, MessageBoxImage.Exclamation);

                var window = new DatabaseSetupWindow();
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.ShowInTaskbar = true;

                var result = window.ShowDialog();

                if (result != true)
                {
                    MessageBox.Show("You cannot use this system without a database", "No database", MessageBoxButton.OK, MessageBoxImage.Stop);

                    return false;
                }

                return true;
            }
        }

        private bool ProvideMSAccessPassword()
        {
            bool isSuccessful = false;
            string password = MSAccess.Properties.Settings.Default.StudentElectionConnectionPassword;
            do
            {
                try
                {
                    var connectionStringBuilder = new StringBuilder();
                    connectionStringBuilder.Append("Provider=Microsoft.Jet.OLEDB.4.0;");
                    connectionStringBuilder.Append($@"Data Source=|DataDirectory|\{ MdbFileName };");
                    if (!string.IsNullOrEmpty(password))
                    {
                        connectionStringBuilder.Append($@"Jet OLEDB:Database Password={ password };");
                    }

                    var connectionStringName = nameof(MSAccess.Properties.Settings.Default.StudentElectionConnectionString);
                    MSAccess.Properties.Settings.Default[connectionStringName] = connectionStringBuilder.ToString();

                    using (var manager = new MSAccess.StudentElectionDataSetTableAdapters.TableAdapterManager())
                    {
                        manager.Connection = new OleDbConnection(MSAccess.Properties.Settings.Default.StudentElectionConnectionString);
                        manager.Connection.Open();
                        manager.Connection.Close();
                    }

                    isSuccessful = true;

                    if (!MSAccess.Properties.Settings.Default.StudentElectionConnectionPassword.Equals(password))
                    {
                        MSAccess.Properties.Settings.Default.StudentElectionConnectionPassword = password;
                        MSAccess.Properties.Settings.Default.Save();
                    }

                    break;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);
                    
                    if (ex is OleDbException oleDbException && oleDbException.ErrorCode == -2147217843)
                    {
                        MessageBox.Show($"{ ex.GetBaseException().Message }.\n\nPress OK to enter the correct password", "Connection error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                        var form = new PasswordInputForm($"Type the password for '{ MdbFileName }' file:", "The password will be saved until you close the program")
                        {
                            Text = $"Password for '{ MdbFileName }' file",
                            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
                        };

                        var result = form.ShowDialog();

                        if (result == System.Windows.Forms.DialogResult.OK)
                        {
                            password = form.Password;
                            continue;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        var tryAgainResult = MessageBox.Show($"Unable to connect with '{ MdbFileName }' file.\n\nDo you want to try again?", "Connection error", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                        if (tryAgainResult == MessageBoxResult.No)
                        {
                            break;
                        }
                    }
                }
            } while (true);
            
            if (!isSuccessful)
            {
                MessageBox.Show($"Failed to access the file '{ MdbFileName }'", "Connection error", MessageBoxButton.OK, MessageBoxImage.Stop);

                var window = new MainWindow();
            }

            return isSuccessful;
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            var stackTrace = new StackTrace(e.Exception);
            Logger.LogError(e.Exception, stackTrace.GetFrame(0).ToString());

            MessageBox.Show($"{ e.Exception.GetBaseException().Message }", "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);

            e.Handled = true;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Current.Shutdown();
        }
    }
}
