using StudentElection.Classes;
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
                lblTitle.Content = "Create Voter";

                btnAdd.Content = "ADD";
            }
            else
            {
                lblTitle.Content = "Edit Voter";

                txtVoterID.Text = Voter.Vin;
                txtLastName.Text = Voter.LastName;
                txtFirstName.Text = Voter.FirstName;
                txtMiddleName.Text = Voter.MiddleName;
                cmbGradeLevel.Text = Voter.YearLevel + "";
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
                G.WaitLang(this);

                txtLastName.Text = txtLastName.Text.ToProperCase();
                txtFirstName.Text = txtFirstName.Text.ToProperCase();
                txtMiddleName.Text = txtMiddleName.Text.ToProperCase();
                txtStrandSection.Text = txtStrandSection.Text.Trim();
                
                if (await _voterService.IsVinExistingAsync(_currentElection.Id, txtVoterID.Text, Voter))
                {
                    G.EndWait(this);
                    MessageBox.Show("Voter ID is already in use", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtVoterID.Focus();

                    return;
                }

                if (txtVoterID.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide voter's ID", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtVoterID.Focus();

                    return;
                }

                if (txtLastName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a last name", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtLastName.Focus();

                    return;
                }

                if (txtFirstName.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a first name", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtFirstName.Focus();

                    return;
                }

                if (cmbGradeLevel.SelectedIndex == -1)
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a grade level", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbGradeLevel.Focus();

                    return;
                }

                if (txtStrandSection.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide a strand section", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    txtStrandSection.Focus();

                    return;
                }

                if (cmbSex.SelectedIndex == -1)
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide the voter's sex", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    cmbSex.Focus();

                    return;
                }


                G.EndWait(this);

                var voter = new VoterModel
                {
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    Suffix = txtSuffix.Text,
                    YearLevel = (Convert.ToInt32(cmbGradeLevel.Text)),
                    Section = txtStrandSection.Text,
                    Sex = cmbSex.Text.StartsWith("M") ? Sex.Male : Sex.Female,
                    Birthdate = dpBirthdate.SelectedDate,
                    Vin = txtVoterID.Text,
                    ElectionId = _currentElection.Id
                };
                
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
                G.EndWait(this);
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
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
