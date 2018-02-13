using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using Microsoft.Win32;
using Portable.Licensing;
using Portable.Licensing.Validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static LNHSVoting.G;

namespace LNHSVoting.Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly string publicKeyFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\license.puk";
        readonly string licenseKeyFilePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\license.lic";

        public MainWindow()
        {
            InitializeComponent();

            txtStudentId.Focus();

            try
            {
                if (Machine.AreResultsViewed)
                {
                    grdVoting.Visibility = Visibility.Collapsed;
                    grdVoteEnded.Visibility = Visibility.Visible;
                    grdFinalizeCandidates.Visibility = Visibility.Collapsed;
                }
                else
                {
                    if (Machine.AreCandidatesFinalized)
                    {
                        grdVoting.Visibility = Visibility.Visible;
                        grdFinalizeCandidates.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        grdVoting.Visibility = Visibility.Collapsed;
                        grdFinalizeCandidates.Visibility = Visibility.Visible;
                    }
                }

                Title = "Student Election Voting System • " + (Machine.Tag.IsBlank() ? "(No tag)" : Machine.Tag);

                //var fileName = "D:/Videos/New Text Document.txt";
                //FileSecurity fSecurity = File.GetAccessControl(fileName);

                //fSecurity.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.ReadData, AccessControlType.Allow));

                //File.SetAccessControl(fileName, fSecurity);
                //DB.LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Close();
            }

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private bool LicenseException(License license)
        {
            return license.Type == LicenseType.Trial;
        }

        private string ValidateLicense(License license, string publicKey, string machineId)
        {
            const string ReturnValue = "License is Valid";
            
            var invalidLicenseFileFailure = new GeneralValidationFailure
            {
                Message = "Invalid License File",
                HowToResolve = ""
            };

            var invalidPublicKeyFailure = new GeneralValidationFailure
            {
                Message = "Invalid Public Key",
                HowToResolve = ""
            };

            var validationFailures = new List<IValidationFailure>();

            if (license == null)
            {
                validationFailures.Add(invalidLicenseFileFailure);

                return validationFailures.Aggregate(string.Empty, (current, validationFailure) => current + validationFailure.HowToResolve + ": " + "\r\n" + validationFailure.Message + "\r\n");
            }

            if (publicKey == null)
            {
                validationFailures.Add(invalidPublicKeyFailure);

                return validationFailures.Aggregate(string.Empty, (current, validationFailure) => current + validationFailure.HowToResolve + ": " + "\r\n" + validationFailure.Message + "\r\n");
            }

            try
            {
                license.VerifySignature(publicKey);
            }
            catch (Exception ex)
            {
                validationFailures.Add(new InvalidSignatureValidationFailure());

                return validationFailures.Aggregate(string.Empty, (current, validationFailure) => current + validationFailure.HowToResolve + ": " + "\r\n" + validationFailure.Message + "\r\n");
            }

            var invalidMachineIdFailure = new GeneralValidationFailure
            {
                Message = "Invalid Machine ID",
                HowToResolve = ""
            };

            validationFailures =
                license.Validate()
                    .ExpirationDate()
                    .When(LicenseException)
                    .And()
                    .AssertThat(lic => lic.AdditionalAttributes.Get("Machine ID") == machineId, invalidMachineIdFailure)
                    .When(lic => lic.AdditionalAttributes.Contains("Machine ID"))
                    .And()
                    .Signature(publicKey)
                    .AssertValidLicense().ToList();

            var failures = validationFailures as List<IValidationFailure> ?? validationFailures.ToList();

            return !failures.Any() ? ReturnValue : failures.Aggregate(string.Empty, (current, validationFailure) => current + validationFailure.HowToResolve + ": " + "\r\n" + validationFailure.Message + "\r\n");
        }

        private string GetHash(string s)
        {
            var cryptoService = new MD5CryptoServiceProvider();
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bt = encoding.GetBytes(s);

            return GetHexString(cryptoService.ComputeHash(bt));
        }

        private string GetHexString(byte[] bt)
        {
            string s = string.Empty;
            for (int i = 0; i < bt.Length; i++)
            {
                byte b = bt[i];
                int n, n1, n2;
                n = (int)b;
                n1 = n & 15;
                n2 = (n >> 4) & 15;
                if (n2 > 9)
                    s += ((char)(n2 - 10 + (int)'A')).ToString();
                else
                    s += n2.ToString();
                if (n1 > 9)
                    s += ((char)(n1 - 10 + (int)'A')).ToString();
                else
                    s += n1.ToString();
                if ((i + 1) != bt.Length && (i + 1) % 2 == 0) s += "-";
            }
            return s;
        }


        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WaitLang(this);

                var staff = Staffs.LogIn(txtUsername.Text, pwdMaintenance.Password);

                if (staff != null)
                {
                    var winMaintenance = new MaintenanceWindow();
                    winMaintenance.Staff = staff;
                    winMaintenance.Show();

                    EndWait(this);

                    Hide();

                    return;
                }

                EndWait(this);
                MessageBox.Show("Invalid username or password", "Log In".ToTitleCase(), MessageBoxButton.OK, MessageBoxImage.Error);

                txtUsername.Clear();
                pwdMaintenance.Clear();

                txtUsername.Focus();
            }
            catch (Exception ex)
            {
                EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void btnVote_Click(object sender, RoutedEventArgs e)
        {
            WaitLang(this);
            try
            {
                var voterRows = Voters.Dictionary.Values;

                Voter voter = null;

                if (voterRows.Count > 0)
                {
                    var vRows = voterRows.Where(x => x.VoterID == txtStudentId.Text);

                    if (vRows.Count() > 0) voter = vRows.First();
                }

                bool isVoted = voter != null && Ballots.Dictionary.Keys.Contains(Convert.ToInt32(voter.ID));

                if (voter != null && Convert.ToString(voter.VoterID) == txtStudentId.Text && !isVoted && !Convert.ToBoolean(voter.IsForeign))
                {
                    Hide();

                    var ballotWindow = new BallotWindow(voter);

                    EndWait(this);

                    ballotWindow.ShowDialog();
                }
                else
                {
                    EndWait(this);
                    MessageBox.Show("Invalid Voter ID", "Student Voting", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtStudentId.Focus();
                }
            }
            catch (Exception ex)
            {
                EndWait(this);
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //dockMain.Visibility = Visibility.Collapsed;
            //grdLicense.Visibility = Visibility.Collapsed;

            //try
            //{
            //    var mc = new ManagementClass("win32_processor");
            //    var moc = mc.GetInstances();

            //    var cpuInfo = "";
            //    foreach (var mo in moc)
            //    {
            //        cpuInfo = mo.Properties["processorID"].Value.ToString();
            //        break;
            //    }

            //    var drives = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => d.RootDirectory.Name.Replace(":\\", ""));
            //    var drive = drives.FirstOrDefault();

            //    var disk = new ManagementObject(@"win32_logicaldisk.deviceid=""" + drive + @":""");
            //    disk.Get();
            //    var volumeSerial = disk["VolumeSerialNumber"].ToString();

            //    var rawMachineId = cpuInfo + volumeSerial;
            //    var hashedMachineId = GetHash(rawMachineId);

            //    txtMachineId.Text = hashedMachineId.ToString();

            //    string publicKey = null;
            //    License license = null;

            //    G.WaitLang(this);

            //    if (File.Exists(licenseKeyFilePath))
            //    {
            //        using (var stream = new StreamReader(licenseKeyFilePath))
            //        {
            //            if (stream.BaseStream.Length > 0)
            //            {
            //                await Task.Run(() =>
            //                {
            //                    license = License.Load(stream.BaseStream);
            //                });
            //            }
            //        }
            //    }

            //    if (File.Exists(publicKeyFilePath))
            //    {
            //        using (var stream = new StreamReader(publicKeyFilePath))
            //        {
            //            if (stream.BaseStream.Length > 0)
            //            {
            //                publicKey = await stream.ReadToEndAsync();
            //            }
            //        }
            //    }

            //    if (ValidateLicense(license, publicKey, txtMachineId.Text) == "License is Valid")
            //    {
            //        grdLicense.Visibility = Visibility.Collapsed;
            //        dockMain.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        grdLicense.Visibility = Visibility.Visible;
            //        dockMain.Visibility = Visibility.Collapsed;
            //    }

            //    G.EndWait(this);
            //}
            //catch (Exception ex)
            //{
            //    G.EndWait(this);

            //    dockMain.Visibility = Visibility.Collapsed;

            //    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            //    Environment.Exit(0);
            //}
        }

        private async void btnValidate_Click(object sender, RoutedEventArgs e)
        {
            //if (txtLicFile.Text.IsBlank())
            //{
            //    MessageBox.Show("Enter an LIC file", "", MessageBoxButton.OK, MessageBoxImage.Error);

            //    return;
            //}

            //if (txtPukFile.Text.IsBlank())
            //{
            //    MessageBox.Show("Enter a PUK file", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            //try
            //{
            //    G.WaitLang(this);

            //    License license = null;
            //    string publicKey = null;
                
            //    File.Copy(txtLicFile.Text, licenseKeyFilePath, true);
            //    if (File.Exists(licenseKeyFilePath))
            //    {
            //        using (var stream = new StreamReader(licenseKeyFilePath))
            //        {
            //            if (stream.BaseStream.Length > 0)
            //            {
            //                await Task.Run(() =>
            //                {
            //                    license = License.Load(stream.BaseStream);
            //                });
            //            }
            //        }
            //    }

            //    File.Copy(txtPukFile.Text, publicKeyFilePath, true);
            //    if (File.Exists(publicKeyFilePath))
            //    {
            //        using (var stream = new StreamReader(publicKeyFilePath))
            //        {
            //            if (stream.BaseStream.Length > 0)
            //            {
            //                publicKey = await stream.ReadToEndAsync();
            //            }
            //        }
            //    }
                
            //    G.EndWait(this);

            //    var str = ValidateLicense(license, publicKey, txtMachineId.Text);

            //    if (str == "License is Valid")
            //    {
            //        MessageBox.Show("License is valid! :)", "", MessageBoxButton.OK, MessageBoxImage.Information);

            //        grdLicense.Visibility = Visibility.Collapsed;
            //        dockMain.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Invalid license", "", MessageBoxButton.OK, MessageBoxImage.Error);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    G.EndWait(this);

            //    dockMain.Visibility = Visibility.Collapsed;

            //    MessageBox.Show("Invalid license" + "\n" + ex.GetBaseException().Message);
            //}
        }

        private void btnLicenseInfo_Click(object sender, RoutedEventArgs e)
        {
            License license = null;
            using (var licenseStream = new StreamReader(licenseKeyFilePath))
            {
                if (licenseStream.BaseStream.Length > 0)
                {
                    license = License.Load(licenseStream.BaseStream);
                }
            }

            if (license != null)
            {
                var info = new StringBuilder();
                info.AppendFormat("License ID:\t{0}\n\n", license.Id);

                info.AppendFormat("Software:\t{0}\n", license.AdditionalAttributes.Get("Software"));
                info.AppendFormat("Licensed to:\t{0} ({1})\n", license.Customer.Name, license.Customer.Email);
                info.AppendFormat("Expires on:\t{0}\n", license.Type == LicenseType.Standard ? "(No expiry)" : license.Expiration.ToString("MMMM d, yyyy"));

                MessageBox.Show(info.ToString(), "License Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnBrowseLic_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
            {
                Filter = "*.lic File (.lic)|*.lic"
            };

            if (fileOpenDialog.ShowDialog() == true)
            {
                txtLicFile.Text = fileOpenDialog.FileName;
            }
        }

        private void btnBrowsePub_Click(object sender, RoutedEventArgs e)
        {
            var fileOpenDialog = new OpenFileDialog
            {
                Filter = "*.puk File (.puk)|*.puk"
            };

            if (fileOpenDialog.ShowDialog() == true)
            {
                txtPukFile.Text = fileOpenDialog.FileName;
            }
        }

        private void btnCopyright_Click(object sender, RoutedEventArgs e)
        {
            var info = new StringBuilder();
            info.AppendLine("STUDENT ELECTION VOTING SYSTEM");
            info.AppendLine("version 1.1.0");
            info.AppendLine();
            info.AppendLine();
            info.AppendLine("© 2016 Albert Francisco");
            info.AppendLine();
            info.AppendLine();
            info.AppendLine("THIRD-PARTY SOFTWARE:");
            info.AppendLine("• Portable.Licensing");
            info.AppendLine("  https://github.com/dnauck/Portable.Licensing");

            MessageBox.Show(info.ToString(), "Software Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}