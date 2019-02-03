//using StudentElection.Classes;
using StudentElection;
using StudentElection.Main;
using System;
using System.Collections.Generic;
using System.Linq;
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
using StudentElection.Repository.Models;
using StudentElection.Services;
using Project.Library.Helpers;
using Humanizer;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for VoterWindow.xaml
    /// </summary>
    public partial class VoterWindow : Window
    {
        public bool IsCanceled = true;    
        public VoterModel Voter;

        private readonly ElectionService _electionService = new ElectionService();
        private readonly VoterService _voterService = new VoterService();
        
        private ElectionModel _currentElection;

        public VoterWindow()
        {                    
            InitializeComponent();

            txtVoterID.Focus();

            dpBirthdate.DisplayDateEnd = DateTime.Now;

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            if (Voter == null)
            {
                lblTitle.Content = "Add Voter";

                btnAdd.Content = "ADD";
            }
            else
            {
                lblTitle.Content = "Edit Voter";

                txtVoterID.Text = Voter.Vin;
                txtLastName.Text = Voter.LastName;
                txtFirstName.Text = Voter.FirstName;
                txtMiddleName.Text = Voter.MiddleName;
                cmbGradeLevel.Text = Voter.YearLevel.ToString();
                txtStrandSection.Text = Voter.Section;
                cmbSex.Text = Voter.Sex.ToString();
                dpBirthdate.SelectedDate = Voter.Birthdate;

                btnAdd.Content = "UPDATE";
            }
        }

        private async void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = false;

            try
            {
                int.TryParse(cmbGradeLevel.Text, out int yearLevel);
                var voter = new VoterModel
                {
                    LastName = txtLastName.Text.Trim(),
                    FirstName = txtFirstName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    Suffix = txtSuffix.Text.Trim(),
                    YearLevel = yearLevel,
                    Section = txtStrandSection.Text.Trim(),
                    Sex = (Sex)(cmbSex.SelectedIndex + 1),
                    Birthdate = dpBirthdate.SelectedDate.HasValue ? dpBirthdate.SelectedDate.Value.Date : default(DateTime?),
                    Vin = txtVoterID.Text.Trim(),
                    ElectionId = _currentElection.Id
                };

                G.WaitLang(this);
                
                if (string.IsNullOrWhiteSpace(voter.Vin))
                {
                    G.EndWait(this);
                    MessageBox.Show("Enter voter ID", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtVoterID.Focus();

                    return;
                }

                if (await _voterService.IsVinExistingAsync(_currentElection.Id, voter.Vin, Voter))
                {
                    G.EndWait(this);
                    MessageBox.Show("Voter ID is already in use", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtVoterID.Focus();

                    return;
                }

                if (string.IsNullOrWhiteSpace(voter.FirstName))
                {
                    G.EndWait(this);
                    MessageBox.Show("Enter voter's first name", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtFirstName.Focus();

                    return;
                }

                if (string.IsNullOrWhiteSpace(voter.LastName))
                {
                    G.EndWait(this);
                    MessageBox.Show("Enter voter's last name", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtLastName.Focus();

                    return;
                }

                if (voter.Sex == Sex.Unknown)
                {
                    G.EndWait(this);
                    MessageBox.Show("Select voter's sex", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbSex.Focus();

                    return;
                }

                if (voter.YearLevel == 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Enter voter's year level", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbGradeLevel.Focus();

                    return;
                }

                if (string.IsNullOrWhiteSpace(voter.Section))
                {
                    G.EndWait(this);
                    MessageBox.Show("Enter voter's class section", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtStrandSection.Focus();

                    return;
                }


                G.EndWait(this);
                
                if (Voter == null)
                {
                    await _voterService.SaveVoterAsync(voter);

                    G.EndWait(this);
                    MessageBox.Show("New voter added!", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    voter.Id = Voter.Id;
                    await _voterService.SaveVoterAsync(voter);

                    G.EndWait(this);
                    MessageBox.Show("Successfully updated!", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                await (Owner as MaintenanceWindow).LoadVotersAsync();

                Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void txtGradeLevel_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            
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
