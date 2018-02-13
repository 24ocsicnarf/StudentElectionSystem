using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using LNHSVoting.Main;
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

namespace LNHSVoting
{
    /// <summary>
    /// Interaction logic for VoterWindow.xaml
    /// </summary>
    public partial class VoterWindow : Window
    {
        public bool IsCanceled = true;    
        public Voter @Voter;

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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (Voter == null)
            {
                lblTitle.Content = "Create Voter";

                btnAdd.Content = "ADD";
            }
            else
            {
                lblTitle.Content = "Edit Voter";

                txtVoterID.Text = Voter.VoterID;
                txtLastName.Text = Voter.LastName;
                txtFirstName.Text = Voter.FirstName;
                txtMiddleName.Text = Voter.MiddleName;
                cmbGradeLevel.Text = Voter.GradeLevel + "";
                txtStrandSection.Text = Voter.StrandSection;
                cmbSex.Text = Voter.IsMale ? "Male" : "Female";
                dpBirthdate.Text = Voter.BirthdateString;

                btnAdd.Content = "UPDATE";
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            IsCanceled = false;

            try
            {
                G.WaitLang(this);

                txtLastName.Text = txtLastName.Text.ToProperCase();
                txtFirstName.Text = txtFirstName.Text.ToProperCase();
                txtMiddleName.Text = txtMiddleName.Text.ToProperCase();
                txtStrandSection.Text = txtStrandSection.Text.Trim();

                var voterData = Voters.Dictionary.Values.AsEnumerable();
                if (Voter != null)
                {
                    voterData = voterData.Where(x => x.ID != Voter.ID);
                }

                if (voterData.Where(x => x.VoterID == txtVoterID.Text).Count() > 0)
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

                if (dpBirthdate.Text.IsBlank())
                {
                    G.EndWait(this);
                    MessageBox.Show("Please provide the voter's birthdate", "Voter", MessageBoxButton.OK, MessageBoxImage.Error);

                    dpBirthdate.Focus();

                    return;
                }


                G.EndWait(this);

                var voter = new Voter()
                {
                    LastName = txtLastName.Text,
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    GradeLevel = (Convert.ToInt32(cmbGradeLevel.Text)),
                    StrandSection = txtStrandSection.Text,
                    IsMale = cmbSex.Text.StartsWith("M"),
                    Birthdate = Convert.ToDateTime(dpBirthdate.Text),
                    VoterID = txtVoterID.Text,
                    IsForeign = false
                };
                
                if (Voter == null)
                {
                    Voters.InsertData(voter);

                    G.EndWait(this);
                    MessageBox.Show("New voter created!", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    voter.ID = Voter.ID;
                    Voters.UpdateData(voter);

                    G.EndWait(this);
                    MessageBox.Show("Successfully updated!", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);

                }
                (Owner as MaintenanceWindow).LoadVoters();

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
