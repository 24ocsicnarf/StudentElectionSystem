using Npgsql;
using Project.Library.Helpers;
using StudentElection;
using StudentElection.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for DatabaseSetupWindow.xaml
    /// </summary>
    public partial class DatabaseSetupWindow : Window
    {
        private const string MSAccessDatabaseName = "MSAccess";
        private const string PostgreSQLDatabaseName = "PostgreSQL";

        public DatabaseSetupWindow()
        {
            InitializeComponent();
        }

        private void SetRepositories(Configuration configFile, string databaseName)
        {
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IUserRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IElectionRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IPositionRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IPartyRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.ICandidateRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IVoterRepository");
            configFile.AppSettings.Settings.Remove("StudentElection.Repository.Interfaces.IBallotRepository");

            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IUserRepository",
                $"StudentElection.{ databaseName }.Repositories.UserRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IElectionRepository",
                $"StudentElection.{ databaseName }.Repositories.ElectionRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IPositionRepository",
                $"StudentElection.{ databaseName }.Repositories.PositionRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IPartyRepository",
                $"StudentElection.{ databaseName }.Repositories.PartyRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.ICandidateRepository",
                $"StudentElection.{ databaseName }.Repositories.CandidateRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IVoterRepository",
                $"StudentElection.{ databaseName }.Repositories.VoterRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
            configFile.AppSettings.Settings.Add(new KeyValueConfigurationElement(
                "StudentElection.Repository.Interfaces.IBallotRepository",
                $"StudentElection.{ databaseName }.Repositories.BallotRepository, StudentElection.{ databaseName }, Version=1.0.0.0, Culture=neutral"
            ));
        }

        private void RestartSystem()
        {
            System.Windows.Forms.Application.Restart();
            Application.Current.Shutdown();
        }

        private bool IsMdbFileExisting()
        {
            return File.Exists(Path.Combine(App.ExeDirectory, App.MdbFileName));
        }

        private void SetNetworkLocation()
        {
            if (txtNetworkFolderAddress.Text.IsBlank()
                || txtNetworkFolderName.Text.IsBlank())
            {
                txtNetworkLocation.Text = "(invalid network location)";
                return;
            }

            txtNetworkLocation.Text = $"\\\\{ Path.Combine(txtNetworkFolderAddress.Text, txtNetworkFolderName.Text) }";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), ConfigurationManager.AppSettings["DatabaseType"]);

            radMSAccess.IsChecked = databaseType == DatabaseType.File;
            radPostgreSQL.IsChecked = databaseType == DatabaseType.Server;
            
            if (databaseType == DatabaseType.Unknown)
            {
                bdrMessage.Visibility = Visibility.Visible;
            }

            if (radPostgreSQL.IsChecked == true)
            {
                bdrMessage.Visibility = Visibility.Collapsed;

                try
                {
                    using (var context = new PostgreSQL.Model.StudentElectionContext())
                    {
                        var connectionString = context.Database.Connection.ConnectionString;
                        if (connectionString.Contains("Port="))
                        {
                            txtPort.Text = connectionString.Split(';')
                                .First(s => s.Contains("Port="))
                                .Split('=')[1];
                        }
                        if (connectionString.Contains("Host="))
                        {
                            txtIPAddress.Text = connectionString.Split(';')
                                .First(s => s.Contains("Host="))
                                .Split('=')[1];
                        }
                        if (connectionString.Contains("Database="))
                        {
                            txtDatabaseName.Text = connectionString.Split(';')
                                .First(s => s.Contains("Database="))
                                .Split('=')[1];
                        }
                        if (connectionString.Contains("Username="))
                        {
                            txtUserName.Text = connectionString.Split(';')
                                .First(s => s.Contains("Username="))
                                .Split('=')[1];
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show("Unable to load the connection string", "Connection string error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                txtNetworkFolderAddress.Text = Properties.Network.Default.NetworkFolderAddress;
                txtNetworkFolderName.Text = Properties.Network.Default.NetworkFolderName;
            }
        }

        private async void BtnConnect_Click(object sender, RoutedEventArgs e)
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var userSettings = (UserSettingsGroup)configFile.GetSectionGroup("userSettings");

            if (radMSAccess.IsChecked == true)
            {
                if (!IsMdbFileExisting())
                {
                    MessageBox.Show($"'{ App.MdbFileName }' file does not exist in\n{ App.ExeDirectory }", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                configFile.AppSettings.Settings.Remove("DatabaseType");
                configFile.AppSettings.Settings.Add("DatabaseType", DatabaseType.File.ToString());

                Properties.Network.Default.NetworkFolderAddress = string.Empty;
                Properties.Network.Default.NetworkFolderName = string.Empty;

                SetRepositories(configFile, MSAccessDatabaseName);
                
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                Properties.Network.Default.Save();

                ConfigurationHelper.ProtectConfiguration(configFile);

                MessageBox.Show("Successfully connected to the MS Access database :D\n\nThe program will now restart.", "Connection success", MessageBoxButton.OK, MessageBoxImage.Information);

                RestartSystem();

                this.DialogResult = true;
            }
            else if (radPostgreSQL.IsChecked == true)
            {
                int port = 0;
                string host = string.Empty;
                string networkFolderName = txtNetworkFolderName.Text;

                if (!txtPort.Text.IsBlank() && !int.TryParse(txtPort.Text, out port))
                {
                    MessageBox.Show("Invalid port number", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtPort.Focus();
                    return;
                }

                if (!txtIPAddress.Text.IsBlank() && IsLocalIpAddress(txtIPAddress.Text))
                {
                    host = txtIPAddress.Text;
                }
                else if (IPAddress.TryParse(txtIPAddress.Text, out var ipAddress))
                {
                    host = ipAddress.ToString();
                }
                else
                {
                    MessageBox.Show("Invalid host", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtIPAddress.Focus();
                    return;
                }
                
                if (txtNetworkFolderAddress.Text.IsBlank())
                {
                    MessageBox.Show("Enter the network folder address", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtNetworkFolderAddress.Focus();
                    return;
                }

                if (networkFolderName.IsBlank())
                {
                    MessageBox.Show("Enter the network folder name", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtNetworkFolderName.Focus();
                    return;
                }

                if (!Directory.Exists($"{ txtNetworkLocation.Text }"))
                {
                    MessageBox.Show($"Network location '{ txtNetworkLocation.Text }' does not exist", "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtNetworkFolderAddress.Focus();
                    return;
                }

                string database = txtDatabaseName.Text;
                string userName = txtUserName.Text;
                string password = pwdServer.Password;

                var connectionStringBuilder = new StringBuilder();
                if (port != 0)
                {
                    connectionStringBuilder.Append($"Port={ port };");
                }
                connectionStringBuilder.Append($"Host={ host };");
                connectionStringBuilder.Append($"Database={ database };");
                connectionStringBuilder.Append($"Username={ userName };");
                connectionStringBuilder.Append($"Password={ password };");

                string connectionString = connectionStringBuilder.ToString();

                try
                {
                    G.WaitLang(this);

                    using (var context = new PostgreSQL.Model.StudentElectionContext())
                    {
                        context.Database.Connection.ConnectionString = connectionString;

                        await context.Database.Connection.OpenAsync();
                        if (context.Database.Connection.State == System.Data.ConnectionState.Open)
                        {
                            context.Database.Connection.Close();
                        }
                        else
                        {
                            throw new NpgsqlException("Cannot open database");
                        }
                    }

                    configFile.AppSettings.Settings.Remove("DatabaseType");
                    configFile.AppSettings.Settings.Add("DatabaseType", DatabaseType.Server.ToString());
                    
                    Properties.Network.Default.NetworkFolderAddress = host;
                    Properties.Network.Default.NetworkFolderName = networkFolderName;

                    configFile.ConnectionStrings
                        .ConnectionStrings["StudentElectionContext"]
                        .ConnectionString = $"metadata=res://*/Model.StudentElectionContext.csdl|res://*/Model.StudentElectionContext.ssdl|res://*/Model.StudentElectionContext.msl;" +
                        $"provider=Npgsql;" +
                        $"provider connection string=\"{ connectionString }\"";

                    SetRepositories(configFile, PostgreSQLDatabaseName);

                    configFile.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection(configFile.ConnectionStrings.SectionInformation.Name);
                    ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                    Properties.Network.Default.Save();

                    ConfigurationHelper.ProtectConfiguration(configFile);

                    G.EndWait(this);

                    MessageBox.Show("Successfully connected to a PostgreSQL database :D\n\nThe program will now restart.", "Connection success", MessageBoxButton.OK, MessageBoxImage.Information);

                    RestartSystem();

                    this.DialogResult = true;
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    G.EndWait(this);

                    MessageBox.Show(ex.GetBaseException().Message, "Connection error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Select a database to use", "No selected database", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private bool IsLocalIpAddress(string host)
        {
            try
            {
                // get host IP addresses
                IPAddress[] hostIPs = Dns.GetHostAddresses(host);

                // get local IP addresses
                IPAddress[] localIPs = Dns.GetHostAddresses(Dns.GetHostName());

                // test if any host IP equals to any local IP or to localhost
                foreach (IPAddress hostIP in hostIPs)
                {
                    // is localhost
                    if (IPAddress.IsLoopback(hostIP)) return true;

                    // is local address
                    foreach (IPAddress localIP in localIPs)
                    {
                        if (hostIP.Equals(localIP)) return true;
                    }
                }
            }
            catch { }
            return false;
        }

        private void radMSAccess_Checked(object sender, RoutedEventArgs e)
        {
            stkPostgreSQLSettings.IsEnabled = false;
            bdrMessage.Visibility = Visibility.Visible;
            
            if (IsMdbFileExisting())
            {
                tbkMessage.Text = $"'{ App.MdbFileName }' file exists in\n{ App.ExeDirectory }";
            }
            else
            {
                tbkMessage.Text = $"'{ App.MdbFileName }' file does NOT exist in\n{ App.ExeDirectory }";
            }
        }

        private void radPostgreSQL_Checked(object sender, RoutedEventArgs e)
        {
            stkPostgreSQLSettings.IsEnabled = true;
            bdrMessage.Visibility = Visibility.Hidden;
        }

        private void TxtNetworkFolderAddress_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SetNetworkLocation();
        }

        private void TxtNetworkFolderName_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            SetNetworkLocation();
        }

        private void TxtIPAddress_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            txtNetworkFolderAddress.Text = txtIPAddress.Text;
        }
    }
}
