using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LNHSVoting.G;

namespace LNHSVoting
{
    /// <summary>
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        public Staff @Staff;
        public int MachineID;
        public bool IsCanceled = true;

        public StaffWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Staff == null)
            {
                lblTitle.Content = "Create Staff";
                btnAddStaff.Content = "ADD";
            }
            else
            {
                lblTitle.Content = "Edit Staff";

                txtLastName.Text = Staff.LastName;
                txtFirstName.Text = Staff.FirstName;
                txtMiddleName.Text = Staff.MiddleName;
                txtUsername.Text = Staff.Username;
                pwdStaff.Password = Staff.Password;
                btnAddStaff.Content = "UPDATE";
                radAdmin.IsChecked = Staff.Type == UserType.Admin;
                radSuperAdmin.IsChecked = Staff.Type == UserType.SuperAdmin;
                //txtUsername.IsEnabled = false;

                if (Staff.Type == UserType.SuperAdmin)
                {
                    radAdmin.IsEnabled = false;
                }
                else if (Staff.Type == UserType.Admin)
                {
                    radSuperAdmin.IsEnabled = false;
                }
            }

            txtLastName.Focus();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = false;

            try
            {
                G.WaitLang(this);

                txtLastName.Text = txtLastName.Text.ToProperCase();
                txtFirstName.Text = txtFirstName.Text.ToProperCase();
                txtMiddleName.Text = txtMiddleName.Text.ToProperCase();
                txtUsername.Text = txtUsername.Text.Trim();

                var staffData = Staffs.Dictionary.Values.AsEnumerable();
                if (Staff != null)
                {
                    staffData = staffData.Where(x => x.ID == Staff.ID);
                }

                if (staffData.Where(x => x.Username == txtUsername.Text).Count() > 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Username is already in use", "Staff", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (txtLastName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a last name", "Staff", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtLastName.Focus();

                    return;
                }

                if (txtFirstName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a first name", "Staff", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtFirstName.Focus();

                    return;
                }

                if (txtUsername.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a username", "Staff", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtUsername.Focus();

                    return;
                }

                if (pwdStaff.Password.Length == 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a password", "Staff", MessageBoxButton.OK, MessageBoxImage.Error);

                    pwdStaff.Focus();

                    return;
                }

                if (pwdStaff.Password.Length < 6)
                {
                    G.EndWait(this);
                    lblPasswordInstruction.Foreground = Brushes.Red;
                    lblPasswordInstruction.Visibility = Visibility.Visible;
                    pwdStaff.Focus();

                    return;
                }

                var staff = new Staff()
                {
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    Type = (UserType)(radAdmin.IsChecked.Value ? 1 : 2),
                    Username = txtUsername.Text,
                    Password = pwdStaff.Password
                };

                if (Staff == null)
                {
                    Staffs.InsertData(staff);
                    
                    EndWait(this);
                    MessageBox.Show("New staff created!", "Staff", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    staff.ID = Staff.ID;
                    Staffs.UpdateData(staff);

                    G.EndWait(this);
                    MessageBox.Show("Successfully updated!", "Staff", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                G.EndWait(this);
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnPeek_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            txtPassword.Text = pwdStaff.Password;

            pwdStaff.Visibility = Visibility.Collapsed;
            txtPassword.Visibility = Visibility.Visible;
        }

        private void btnPeek_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            pwdStaff.Password = txtPassword.Text;

            pwdStaff.Visibility = Visibility.Visible;
            txtPassword.Visibility = Visibility.Collapsed;

            SetSelection(pwdStaff, pwdStaff.Password.Length, 0);

            pwdStaff.Focus();
        }

        private void pwdStaff_GotFocus(object sender, RoutedEventArgs e)
        {
            lblPasswordInstruction.Visibility = Visibility.Visible;
        }

        private void pwdStaff_LostFocus(object sender, RoutedEventArgs e)
        {
            lblPasswordInstruction.Foreground = Brushes.Black;
            lblPasswordInstruction.Visibility = Visibility.Hidden;
        }

        private void SetSelection(PasswordBox passwordBox, int start, int length)
        {
            passwordBox.GetType().GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(passwordBox, new object[] { start, length });
        }

        private void pwdStaff_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            lblPasswordInstruction.Foreground = Brushes.Black;
            
            e.Handled = e.Key == Key.Space;
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }
    }
}
