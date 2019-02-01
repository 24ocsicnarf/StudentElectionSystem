//using StudentElection.Classes;
using StudentElection;
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
using static StudentElection.G;
using StudentElection.Repository.Models;
using StudentElection.Repository;
using StudentElection.Services;
using Project.Library.Helpers;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for StaffWindow.xaml
    /// </summary>
    public partial class StaffWindow : Window
    {
        public UserModel User;
        public int MachineID;
        public bool IsCanceled = true;

        public UserType? PresetUserType;

        public UserService _userService = new UserService();

        public StaffWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (User == null)
            {
                if (PresetUserType == UserType.Superuser)
                {
                    lblTitle.Content = "Add Superuser";
                }
                else
                {
                    lblTitle.Content = "Add User";
                }

                btnAddStaff.Content = "ADD";
            }
            else
            {
                lblTitle.Content = "Edit User";
                //TODO: CHANGE PASSWORD
                txtLastName.Text = User.LastName;
                txtFirstName.Text = User.FirstName;
                txtMiddleName.Text = User.MiddleName;
                txtSuffix.Text = User.Suffix;
                txtUsername.Text = User.UserName;
                cmbSex.SelectedIndex = (Sex)User.Sex == Sex.Male ? 0 : 1;
                //pwdStaff.Password = Staff.Password;
                btnAddStaff.Content = "UPDATE";
                radAdmin.IsChecked = User.Type == Repository.Models.UserType.Admin;
                radSuperuser.IsChecked = User.Type == Repository.Models.UserType.Superuser;
                //txtUsername.IsEnabled = false;

                if (User.Type == Repository.Models.UserType.Admin)
                {
                    radAdmin.IsEnabled = false;
                }
                else if (User.Type == Repository.Models.UserType.Superuser)
                {
                    radSuperuser.IsEnabled = false;
                }
                else if (User.Type == Repository.Models.UserType.Normal)
                {
                    radNormal.IsEnabled = false;
                }
            }

            txtFirstName.Focus();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private async void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = false;

            try
            {
                G.WaitLang(this);

                txtLastName.Text = txtLastName.Text.ToProperCase();
                txtFirstName.Text = txtFirstName.Text.ToProperCase();
                txtMiddleName.Text = txtMiddleName.Text.ToProperCase();
                txtSuffix.Text = txtSuffix.Text.ToProperCase();
                txtUsername.Text = txtUsername.Text.Trim();
                
                if (await _userService.IsUserNameExistingAsync(txtUsername.Text, User))
                {
                    G.EndWait(this);
                    MessageBox.Show("Username is already in use", "User", MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                if (txtFirstName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a first name", "User", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtFirstName.Focus();

                    return;
                }

                if (txtLastName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a last name", "User", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtLastName.Focus();

                    return;
                }

                if (cmbSex.SelectedIndex < 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Please select a sex", "User", MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbSex.Focus();

                    return;
                }

                if (txtUsername.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a username", "User", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtUsername.Focus();

                    return;
                }

                if (pwdStaff.Password.Length == 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a password", "User", MessageBoxButton.OK, MessageBoxImage.Error);

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

                var userType = UserType.Normal;
                if (radNormal.IsChecked == true)
                {
                    userType = UserType.Normal;
                }
                else if (radAdmin.IsChecked == true)
                {
                    userType = UserType.Admin;
                }
                else if (radSuperuser.IsChecked == true)
                {
                    userType = UserType.Superuser;
                }

                var staff = new UserModel
                {
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    Type = PresetUserType ?? userType,
                    UserName = txtUsername.Text
                };

                if (User == null)
                {
                    await _userService.SaveUserAsync(staff, pwdStaff.SecurePassword);
                    
                    EndWait(this);
                    MessageBox.Show("New user created!", "User", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    staff.Id = User.Id;
                    await _userService.SaveUserAsync(staff, null);

                    G.EndWait(this);
                    MessageBox.Show("Successfully updated!", "User", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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
