using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
//using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.XlsIO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static LNHSVoting.G;

namespace LNHSVoting.Main
{
    /// <summary>
    /// Interaction logic for MaintenanceWindow.xaml
    /// </summary>
    public partial class MaintenanceWindow : Window
    {
        public Staff @Staff;
        //public List<Candidate> CandidateList;

        private bool _isLogOut;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);

        public MaintenanceWindow()
        {
            _selectedStaff = -1;
            _selectedVoter = -1;

            _isLogOut = false;
            //CandidateList = new List<Candidate>();

            InitializeComponent();

            G.ChangeWindowSize(this);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.ChangeWindowSize(this);
            };

            chkVoted.Checked += chkVoted_CheckChanged;
            chkVoted.Unchecked += chkVoted_CheckChanged;
        }

        private void chkVoted_CheckChanged(object sender, RoutedEventArgs e)
        {
            RefreshVoterCountLabel();
        }

        private void RefreshVoterCountLabel()
        {
            _voterCount = 0;
            _foreignCount = 0;

            CollectionViewSource.GetDefaultView(lvVoter.ItemsSource).Refresh();

            if ((int)lblVoterInfo.Tag == 0)
            {
                lblVoterInfo.Content = string.Format("{0:#,##0} voter{1} & {2:#,##0} foreign candidate{3} ({4:#,##0} total)", _voterCount, _voterCount == 1 ? "" : "s", _foreignCount, _foreignCount == 1 ? "" : "s", _voterCount + _foreignCount);
            }
            else
            {
                lblVoterInfo.Content = string.Format("{0:#,##0} foreign candidate{1}", _foreignCount, _foreignCount == 1 ? "" : "s");
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);

            LoadMachine();
            LoadStaff();
            LoadVoters();
            LoadCandidates();
            LoadResults();

            lblPosition.Text = string.Format("Positions ({0})", Positions.Dictionary.Count);
            lblUsername.Content = @Staff.Username;

            if (Staff.Type == UserType.Admin)
            {
                rdfStaff.Height = new GridLength(0, GridUnitType.Star);
            }
            
            G.EndWait(this);
        }

        private void LoadMachine()
        {
            try
            {
                if (Machine.Tag.IsBlank())
                {
                    lblTag.Content = "(No Tag)";
                    txtTag.Text = "";
                }
                else
                {
                    lblTag.Content = Machine.Tag;
                    txtTag.Text = Machine.Tag;
                }
                
                if (Machine.AreCandidatesFinalized && !Machine.AreResultsViewed)
                {
                    bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(96, 224, 176));
                    bdrMaintenance.BorderThickness = new Thickness(4);
                }
                else if (Machine.AreResultsViewed)
                {
                    bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(96, 176, 224));
                    bdrMaintenance.BorderThickness = new Thickness(4);
                    
                    btnAddVoter.Visibility = Visibility.Collapsed;
                    btnEditVoter.Visibility = Visibility.Collapsed;
                    btnDeleteVoter.Visibility = Visibility.Collapsed;
                }
                else
                {
                    bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(176, 176, 176));
                    bdrMaintenance.BorderThickness = new Thickness(4);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        public void CheckCandidates()
        {
            if (!Machine.AreCandidatesFinalized)
            {
                try
                {
                    if (Candidates.Dictionary.Count > 0)
                    {
                        btnFinalize.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        btnFinalize.Visibility = Visibility.Collapsed;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
                chkVoted.Visibility = Visibility.Collapsed;

                lblTag.Cursor = Cursors.IBeam;
            }
            else
            {
                dckCandidateButtons.Visibility = Visibility.Collapsed;
                chkVoted.Visibility = Visibility.Visible;

                foreach (PartyItemControl item in stkCandidates.Children)
                {
                    item.AreCandidatesFinalized = true;
                    item.btnAddCandidate.Visibility = Visibility.Collapsed;
                    item.tbkParty.Cursor = Cursors.Arrow;
                }

                lblTag.Cursor = Cursors.Arrow;
            }
        }

        private void CheckResults()
        {
            if (!Machine.AreResultsViewed)
            {
                try
                {
                    btnVoterButtons.Visibility = Visibility.Visible;

                    if (Ballots.Dictionary.Count > 0)
                    {
                        lblResultsAvailable.Content = string.Format("Results are available ({0:n0} of {1:n0} voters voted)", Ballots.Dictionary.Count, Voters.Dictionary.Count);
                        grdViewResult.Visibility = Visibility.Visible;
                        lblNoResults.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        grdViewResult.Visibility = Visibility.Collapsed;
                        lblNoResults.Visibility = Visibility.Visible;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
            }
            else
            {
                grdViewResult.Visibility = Visibility.Collapsed;
                btnExportPrint.Visibility = Visibility.Visible;

                btnAddVoter.Visibility = Visibility.Collapsed;
                btnEditVoter.Visibility = Visibility.Collapsed;
                btnDeleteVoter.Visibility = Visibility.Collapsed;
            }
        }

        #region Staff
        
        private int _selectedStaff;

        private CollectionView cvStaff;

        private bool StaffFilter(object obj)
        {
            if (string.IsNullOrEmpty(txtStaffFilter.Text))
                return true;
            else
            {
                return ((obj as Staff).LastName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                    ((obj as Staff).FirstName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                    ((obj as Staff).MiddleName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        private void LoadStaff()
        {
            try
            {
                lvStaff.ItemsSource = Staffs.Dictionary.Values;

                cvStaff = (CollectionView)CollectionViewSource.GetDefaultView(lvStaff.ItemsSource);
                cvStaff.Filter = StaffFilter;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void lvStaff_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;

            if (lv.SelectedItems.Count == 1)
            {
                btnEditStaff.Visibility = Visibility.Visible;
                btnDeleteStaff.Visibility = Visibility.Visible;

                btnDeleteStaff.Visibility = (Staff.ID != (lvStaff.SelectedValue as Staff).ID) ? Visibility.Visible : Visibility.Collapsed;

                _selectedStaff = lv.SelectedIndex;
            }
            else
            {
                btnEditStaff.Visibility = Visibility.Collapsed;
                btnDeleteStaff.Visibility = Visibility.Collapsed;
            }
        }

        private void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            if (stkCandidates.Children.Count == 10)
            {
                MessageBox.Show("Up to 10 users only.", "Position", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;
            
            G.WaitLang(this);

            var staffWindow = new StaffWindow();
            staffWindow.MachineID = Machine.ID;
            staffWindow.Owner = this;
            
            G.EndWait(this);

            staffWindow.ShowDialog();

            G.WaitLang(this);

            Opacity = 1;

            if (!staffWindow.IsCanceled)
                LoadStaff();
            
            lvStaff.ScrollIntoView(lvStaff.Items[lvStaff.Items.Count - 1]);

            G.EndWait(this);
        }

        private void btnEditStaff_Click(object sender, RoutedEventArgs e)
        {
            G.EndWait(this);

            Opacity = 0.5;

            var staff = lvStaff.SelectedValue as Staff;
            
            var staffWindow = new StaffWindow();
            staffWindow.Staff = staff;
            staffWindow.Owner = this;

            G.EndWait(this);

            staffWindow.ShowDialog();

            G.WaitLang(this);

            Opacity = 1;
            if (!staffWindow.IsCanceled)
                LoadStaff();

            lvStaff.ScrollIntoView(lvStaff.Items[_selectedStaff]);
            lvStaff.SelectedIndex = _selectedStaff;

            G.EndWait(this);
        }

        private void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (lvStaff.SelectedItems.Count == 1)
            {
                var staff = (lvStaff.SelectedValue as Staff);
                var result = MessageBox.Show(staff.FullName + " will not be able to log in on this system. Do you want to continue?", "Deleting '" + staff.Username + "'", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                var selectedStaff = _selectedStaff == lvStaff.Items.Count - 1 ? _selectedStaff - 1 : _selectedStaff;

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        G.WaitLang(this);

                        Staffs.DeleteData(staff.ID);

                        G.EndWait(this);
                        MessageBox.Show("The user is deleted.", "Staff", MessageBoxButton.OK, MessageBoxImage.Information);

                        G.WaitLang(this);
                        LoadStaff();

                        lvStaff.ScrollIntoView(lvStaff.Items[selectedStaff]);
                        G.EndWait(this);

                    }
                    catch (Exception ex)
                    {
                        G.EndWait(this);
                        MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                        Application.Current?.Shutdown();
                    }
                }
            }
        }

        private void txtStaffFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvStaff.ItemsSource).Refresh();
        }

        #endregion


        #region Voter
        
        private int _selectedVoter;
        
        private CollectionView _cvVoter;
        private GridViewColumn _columnClicked = null;
        private bool _isAscendingVoter;
        private int _voterCount, _foreignCount;

        private bool FilterVoter(object obj)
        {
            var voter = obj as Voter;

            bool isForeign = voter.IsForeign;
            bool containsText = ((voter.LastName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.FirstName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.MiddleName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.GradeLevel.ToString().IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.StrandSection.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Sex.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.BirthdateString.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.VoterID.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0));
            bool isVoted = voter.IsVoted;
            
            if ((isForeign && (isVoted || !chkVoted.IsChecked.Value)) && containsText)
            {
                _foreignCount++;
            }
            else if (((isVoted && chkVoted.IsChecked.Value) || (!chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0)) && containsText)
            {
                _voterCount++;
            }

            return ((isForeign && !chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 1) || (isVoted && chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0) || (!chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0)) && containsText;
        }

        private void SortVoter()
        {
            if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[0].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()) || _columnClicked.Header == null)
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[1].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[2].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.FirstName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.FirstName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[3].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.MiddleName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.MiddleName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[4].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.GradeLevel).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.GradeLevel).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[5].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.StrandSection).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.StrandSection).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[6].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.SexType).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.SexType).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[7].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.Birthdate).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.Birthdate).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            //END SORTING

        }
        
        public void LoadVoters()
        {
            //_listVoter = new List<Voter>();

            if (_columnClicked == null)
            {
                _columnClicked = new GridViewColumn();
                _columnClicked.Header = gvVoter.Columns[0].Header;
                _isAscendingVoter = true;
            }

            try
            {
                SortVoter();
                
                lblVoterInfo.Tag = 0;

                _cvVoter = (CollectionView)CollectionViewSource.GetDefaultView(lvVoter.ItemsSource);
                _cvVoter.Filter = FilterVoter;

                RefreshVoterCountLabel();

                if (Machine.AreCandidatesFinalized && !Machine.AreResultsViewed)
                {
                    lblResultsAvailable.Content = string.Format("Results are available ({0:#,##0} of {1:#,##0} voters voted)", Voters.Dictionary.Count, Ballots.Dictionary.Count);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void btnAddVoter_Click(object sender, RoutedEventArgs e)
        {
            var searched = txtVoterFilter.Text;

            Opacity = 0.5;
            
            G.WaitLang(this);
            var voterWindow = new VoterWindow();
            voterWindow.Voter = null;
            voterWindow.Owner = this;

            G.EndWait(this);

            voterWindow.ShowDialog();

            G.WaitLang(this);
            Opacity = 1;
            
            if (!voterWindow.IsCanceled)
            {
                LoadVoters();
                if (voterWindow.Voter != null)
                {
                    var newItem = lvVoter.ItemsSource.Cast<Voter>().Where(x => x.ID == voterWindow.Voter.ID).First();
                    lvVoter.ScrollIntoView(newItem);
                    lvVoter.SelectedItem = newItem;

                    txtVoterFilter.Text = searched;
                }
                else
                {
                    var newItem = Voters.Dictionary.Values.OrderBy(x => x.ID).Last();
                    lvVoter.ScrollIntoView(newItem);
                    lvVoter.SelectedItem = newItem;
                }
            }

            G.EndWait(this);
        }

        private void lvVoter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;

            if (lv.SelectedItems.Count == 1)
            {
                if (Machine.AreResultsViewed) return;

                btnEditVoter.Visibility = Visibility.Visible;
                btnDeleteVoter.Visibility = Visibility.Visible;

                _selectedVoter = lv.SelectedIndex;
            }
            else
            {
                btnEditVoter.Visibility = Visibility.Collapsed;
                btnDeleteVoter.Visibility = Visibility.Collapsed;
            }
        }

        private void txtVoterFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            RefreshVoterCountLabel();
        }

        private void btnEditVoter_Click(object sender, RoutedEventArgs e)
        {
            if (lvVoter.SelectedItems.Count == 1)
            {
                var voter = lvVoter.SelectedValue as Voter;
                var selectedVoter = lvVoter.SelectedIndex;

                if (Machine.AreCandidatesFinalized)
                {
                    try
                    {
                        G.WaitLang(this);

                        if (Ballots.Dictionary.Keys.Contains(voter.ID))
                        {
                            G.EndWait(this);
                            MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Voted", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        var candidate = Candidates.Dictionary.ContainsKey(voter.ID) ? Candidates.Dictionary[voter.ID] : null;
                        if (candidate != null)
                        {
                            G.EndWait(this);
                            MessageBox.Show(voter.FullName + " is a " + (candidate.IsForeign ? "foreign " : "") + "candidate" + (candidate.IsForeign ? " on this machine" : "") + ". You cannot allow to edit or delete their info.", string.Format("Voter is a {0}candidate", candidate.IsForeign ? "foreign " : "").ToTitleCase(), MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }

                        G.EndWait(this);
                    }
                    catch (Exception ex)
                    {
                        G.EndWait(this);
                        MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                        Application.Current?.Shutdown();
                    }
                }

                Opacity = 0.5;

                var voterWindow = new VoterWindow();
                voterWindow.Voter = voter;
                voterWindow.Owner = this;
                G.EndWait(this);
                voterWindow.ShowDialog();

                G.WaitLang(this);
                Opacity = 1;

                if (!voterWindow.IsCanceled)
                {
                    LoadVoters();
                    LoadCandidates();

                    if (lvVoter.Items.Count != 0 && voterWindow.Voter != null)
                    {
                        var v = lvVoter.ItemsSource.Cast<Voter>().Where(x => x.ID == voterWindow.Voter.ID).First();
                        lvVoter.ScrollIntoView(v);
                        lvVoter.SelectedIndex = lvVoter.Items.IndexOf(v);
                    }
                }
                else
                {
                    if (lvVoter.Items.Count != 0)
                    {
                        lvVoter.ScrollIntoView(lvVoter.SelectedItem);
                    }
                }

                G.EndWait(this);
            }
        }
        
        private void btnDeleteVoter_Click(object sender, RoutedEventArgs e)
        {
            if (lvVoter.SelectedItems.Count == 1)
            {
                G.WaitLang(this);
                var voter = (lvVoter.SelectedValue as Voter);
                var selectedVoter = (lvVoter.SelectedIndex);

                selectedVoter = selectedVoter == lvVoter.Items.Count - 1 ? selectedVoter - 1 : selectedVoter;

                try
                {
                    if (Ballots.Dictionary.Keys.Contains(voter.ID))
                    {
                        G.EndWait(this);
                        MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Voted", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (Voters.Dictionary[voter.ID].IsACandidate)
                    {
                        if (Machine.AreCandidatesFinalized)
                        {
                            G.EndWait(this);
                            MessageBox.Show(voter.FullName + " is a " + (voter.IsForeign ? "foreign " : "") + "candidate" + (voter.IsForeign ? " on this machine" : "") + ". You cannot allow to edit or delete their info.", string.Format("Voter is a {0}candidate", voter.IsForeign ? "foreign " : "").ToTitleCase(), MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            lvVoter.ScrollIntoView(voter);

                            G.EndWait(this);
                            var result = MessageBox.Show(voter.FullName + " is a " + (voter.IsForeign ? "foreign " : "") + "candidate" + (voter.IsForeign ? " on this machine" : "") + ". Do you want to continue deleting this candidate voter?", "Deleting " + voter.VoterID, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                            if (result == MessageBoxResult.Yes)
                            {
                                G.WaitLang(this);

                                Voters.DeleteData(voter.ID);

                                G.EndWait(this);

                                G.WaitLang(this);

                                LoadVoters();
                                LoadCandidates();

                                MessageBox.Show("The candidate voter is deleted.", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);

                                if (selectedVoter >= 0 && selectedVoter < lvVoter.Items.Count)
                                {
                                    lvVoter.ScrollIntoView(lvVoter.Items[selectedVoter]);
                                }
                                G.EndWait(this);
                            }
                        }
                    }
                    else
                    {
                        lvVoter.ScrollIntoView(voter);

                        G.EndWait(this);
                        var result = MessageBox.Show("Delete " + voter.FullName + "?", "Deleting " + voter.VoterID, MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                        if (result == MessageBoxResult.Yes)
                        {
                            G.WaitLang(this);

                            Users.DeleteData(voter.ID);

                            G.EndWait(this);

                            G.WaitLang(this);
                            LoadVoters();

                            if (selectedVoter >= 0 && selectedVoter < lvVoter.Items.Count)
                            {
                                lvVoter.ScrollIntoView(lvVoter.Items[selectedVoter]);
                            }
                            MessageBox.Show("The voter is deleted.", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);

                            G.EndWait(this);
                        }
                    }
                }
                catch (Exception ex)
                {
                    G.EndWait(this);
                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
            }
        }

        #endregion


        #region Maintenance

        private void lblUsername_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Opacity = 0.5;
            
            var staffWindow = new StaffWindow();
            staffWindow.Staff = Staff;
            staffWindow.Owner = this;
            staffWindow.ShowDialog();

            Opacity = 1;

            if (staffWindow.IsCanceled) return;

            LoadStaff();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isLogOut)
            {
                e.Cancel = true;
                return;
            }

            var window = new MainWindow();
            window.Show();
        }

        private void lblLogOut_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isLogOut = true;

            Close();
        }

        #endregion


        #region Candidates

        private void btnAddParty_Click(object sender, RoutedEventArgs e)
        {
            if (stkCandidates.Children.Count == 20)
            {
                MessageBox.Show("Up to 20 parties only.", "Party", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;
            
            G.WaitLang(this);

            var form = new PartyForm();
            form.Party = null;

            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(this);

            form.ShowDialog();
            
            Opacity = 1;

            if (!form.IsCanceled)
            {
                G.WaitLang(this);

                LoadVoters();
                LoadCandidates();

                svrCandidates.ScrollToEnd();

                G.EndWait(this);
            }
        }


        public void LoadCandidates()
        {
            var currScroll = svrCandidates.VerticalOffset;
            foreach (PartyItemControl control in stkCandidates.Children)
            {
                CandidateHOffsets.Add(control.svrCandidate.HorizontalOffset);
            }
            
            try
            {
                if (Parties.Dictionary.Count == 0)
                    lblNoCandidates.Visibility = Visibility.Visible;
                else
                    lblNoCandidates.Visibility = Visibility.Hidden;

                stkCandidates.Children.Clear();

                var partyIndex = 0;
                foreach (var party in Parties.Dictionary.Values)
                {
                    var item = new PartyItemControl();
                    item.DataContext = party;
                    
                    stkCandidates.Children.Add(item);

                    var candidateParty = Candidates.Dictionary.Values.Where(x => x.Party.ID == party.ID).OrderBy(x => x.Position.Order).ToList();

                    if (candidateParty.Count != 0)
                        item.stkCandidate.Visibility = Visibility.Visible;
                    else
                        item.stkCandidate.Visibility = Visibility.Collapsed;

                    item.stkCandidate.Children.Clear();
                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        candidateControl.DataContext = candidate;
                        item.stkCandidate.Children.Add(candidateControl);
                    }

                    item.lblCount.Content = item.stkCandidate.Children.Count + "";

                    if (CandidateHOffsets.Count > partyIndex)
                        item.svrCandidate.ScrollToHorizontalOffset(CandidateHOffsets[partyIndex]);
                    
                    partyIndex++;
                }

                CheckCandidates();

                svrCandidates.ScrollToVerticalOffset(currScroll);
                G.CandidateHOffsets = new List<double>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }
        
        private void btnFinalize_Click(object sender, RoutedEventArgs e)
        {
            if (Machine.Tag.IsBlank())
            {
                MessageBox.Show("Please provide a tag on this machine. Make sure that the tag is unique among other machines.", "Provide A Tag", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;

            var fcv = Candidates.Dictionary.Values.Where(x => x.IsForeign).Count();
            var result = MessageBox.Show(string.Format("Once finalized, you WILL NO LONGER EDIT or DELETE ALL CANDIDATES' INFORMATION, but you may still create, edit or delete voters who did not vote yet. Also, the MACHINE TAG will be finalized and uneditable.\n\nThere are {0} {1} ({2} foreign candidate{3}).\n\n\nDo you want to finalize the candidates?",
                Candidates.Dictionary.Values.Count, (Candidates.Dictionary.Values.Count == 1 ? "candidate" : "candidates"), fcv == 0 ? "No" : fcv + "", fcv == 1 ? "" : "s"), "Finalizing Candidates", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    Machine.FinalizeCandidates();

                    G.EndWait(this);

                    MessageBox.Show("The candidates have been finalized.\n\nYou will NO LONGER EDIT or DELETE ALL CANDIDATES' INFORMATION and CHANGE the MACHINE TAG.", "Candidates Finalized", MessageBoxButton.OK, MessageBoxImage.Information);

                    G.WaitLang(this);

                    LoadMachine();
                    LoadVoters();
                    LoadCandidates();

                    G.EndWait(this);
                }
                catch (Exception ex)
                {
                    G.EndWait(this);
                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
            }

            Opacity = 1;
        }

        #endregion


        #region Results
        
        public void LoadResults()
        {
            CheckResults();

            if (!Machine.AreResultsViewed)
            {
                return;
            }

            List<Position> listPosition = new List<Position>();
            
            try
            {
                var positionRows = Positions.Dictionary.Values;
                dgResults.ItemsSource = Ballots.Results.Select(r => new VoteResult
                {
                    PictureSource = r.PictureSource,
                    Name = r.FullName,
                    Alias = r.Alias,
                    PositionName = r.Position.Title,
                    PositionOrder = r.Position.Order,
                    GradeLevel = r.GradeLevel,
                    GradeStrand = r.GradeStrand,
                    PartyTitle = r.Party.Title,
                    PartyColorBrush = r.Party.ColorBrush,
                    VoteCount = r.VoteCount
                }).OrderBy(vr => vr.PositionOrder);


                stkResults.Children.Clear();

                cmbPositions.Items.Clear();
                cmbPositions.Items.Add(new Position()
                {
                    ID = 0,
                    Title = "All"
                });

                foreach (var position in positionRows)
                {
                    var candidatesPosition = Ballots.Results.Where(x => x.Position.ID == position.ID).OrderByDescending(r => r.VoteCount).ToList();

                    if (!candidatesPosition.Any())
                    {
                        continue;
                    }

                    cmbPositions.Items.Add(position);

                    var item = new PositionItemControl();
                    item.tbkName.Visibility = Visibility.Collapsed;
                    item.recParty.Visibility = Visibility.Collapsed;
                    item.cdfCandidate.Width = new GridLength(0);
                    item.DataContext = position;

                    stkResults.Children.Add(item);

                    lblNoResults.Visibility = Visibility.Hidden;

                    item.wrpCandidate.Children.Clear();

                    int rank = 0;
                    for (int i = 0; i < candidatesPosition.Count; i++)
                    {
                        var candidate = candidatesPosition[i];
                        item.wrpCandidate.Visibility = Visibility.Visible;
                        var control = new CandidateResultControl();

                        var previousCandidate = i == 0 ? null : candidatesPosition.ElementAtOrDefault(i - 1);

                        if (previousCandidate == null || previousCandidate.VoteCount != candidate.VoteCount)
                        {
                            rank++;
                        }

                        control.tbkRank.Text = string.Format("#{0:n0}", rank);

                        control.SizeChanged += (s, ev) =>
                        {
                            var actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;
                            if (control.tbkName.ActualWidth >= actualWidth * 1.5)
                            {
                                control.vbName.Stretch = Stretch.Fill;

                                control.tbkName.Width = actualWidth * 1.5;
                                control.tbkName.TextTrimming = TextTrimming.CharacterEllipsis;


                                ToolTipService.SetShowDuration(control.tbkName, int.MaxValue);
                            }

                            actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;
                            if (control.tbkAlias.ActualWidth >= actualWidth * 1.5)
                            {
                                control.vbAlias.Stretch = Stretch.Fill;

                                control.tbkAlias.Width = actualWidth * 1.5;
                                control.tbkAlias.TextTrimming = TextTrimming.CharacterEllipsis;

                                ToolTipService.SetShowDuration(control.tbkAlias, int.MaxValue);
                            }


                            actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;
                            if (control.tbkParty.ActualWidth >= actualWidth * 1.5)
                            {
                                control.vbParty.Stretch = Stretch.Fill;

                                control.tbkParty.Width = actualWidth * 1.5;
                                control.tbkParty.TextTrimming = TextTrimming.CharacterEllipsis;

                                ToolTipService.SetShowDuration(control.tbkParty, int.MaxValue);
                            }
                        };
                        control.tbkName.SizeChanged += (s, ev) =>
                        {
                            var actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;

                            if (control.tbkName.ActualWidth >= actualWidth)
                            {
                                control.vbName.Width = actualWidth - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right; ;
                                control.vbName.Stretch = Stretch.Fill;
                            }
                            else
                            {
                                control.vbName.Stretch = Stretch.Uniform;
                            }
                        };

                        control.tbkAlias.SizeChanged += (s, ev) =>
                        {
                            var actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;

                            if (control.tbkAlias.ActualWidth >= actualWidth)
                            {
                                control.vbAlias.Width = actualWidth - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right; ;
                                control.vbAlias.Stretch = Stretch.Fill;
                            }
                            else
                            {
                                control.vbAlias.Stretch = Stretch.Uniform;
                            }
                        };

                        control.tbkParty.SizeChanged += (s, ev) =>
                        {
                            var actualWidth = control.ActualWidth - control.bdrImageInfo.ActualWidth - control.bdrImageInfo.Margin.Left - control.bdrImageInfo.Margin.Right - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right;

                            if (control.tbkParty.ActualWidth >= actualWidth)
                            {
                                control.vbParty.Width = actualWidth - control.dckInfo.Margin.Left - control.dckInfo.Margin.Right; ;
                                control.vbParty.Stretch = Stretch.Fill;
                            }
                            else
                            {
                                control.vbParty.Stretch = Stretch.Uniform;
                            }
                        };

                        item.SizeChanged += (s, ev) =>
                        {
                            item.wrpCandidate.ItemWidth = item.ActualWidth;
                        };
                        control.DataContext = candidate;

                        double quotient = candidate.VoteCount / (double)candidate.PositionVoteCount;

                        if (!double.IsNaN(quotient))
                        {
                            control.recCandidate.Height = 120 * quotient;
                        }

                        item.wrpCandidate.Children.Add(control);
                    }
                }

                cmbPositions.SelectedIndex = 0;
                
                cmbPositions.Focusable = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void cmbPositions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach(PositionItemControl item in stkResults.Children)
            {
                if ((item.DataContext as Position).ID == (cmbPositions.SelectedItem as Position).ID || cmbPositions.SelectedIndex == 0)
                {
                    item.Visibility = Visibility.Visible;
                }
                else
                {
                    item.Visibility = Visibility.Collapsed;
                }
            }
            cmbPositions.Focusable = true;
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void btnExportReport_Click(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);

            var dateNow = DateTime.Now;
            var options = new ExcelExportingOptions();

            var saveFileDialog = new SaveFileDialog
            {
                FileName = string.Format("LNHS Voting Result {1:yyyy-MM-dd-HHmmss} ({0}).xls", Machine.Tag, dateNow)
            };
            saveFileDialog.Filter = "Excel file (*.xls)|*.xls";

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    G.WaitLang(this);

                    using (var excelEngine = dgResults.ExportToExcel(dgResults.View, options))
                    {
                        var workBook = excelEngine.Excel.Workbooks[0];
                        var resultWorksheet = workBook.Worksheets[0];
                        resultWorksheet.Name = "VOTING RESULTS";

                        resultWorksheet[1, 1, 1, dgResults.Columns.Count].CellStyle.Font.Bold = true;

                        resultWorksheet.Columns[1].Group(ExcelGroupBy.ByColumns);

                        resultWorksheet.UsedRange.AutofitColumns();
                        resultWorksheet.UsedRange.AutofitRows();

                        var infoWorksheet = excelEngine.Excel.Worksheets.AddCopyBefore(resultWorksheet);
                        infoWorksheet.Clear();

                        infoWorksheet.Name = "REPORT INFO";

                        infoWorksheet.Range[1, 1].Text = "MACHINE TAG";
                        infoWorksheet.Range[1, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[1, 2].Text = Machine.Tag;
                        
                        infoWorksheet.Range[2, 1].Text = "MACHINE NAME";
                        infoWorksheet.Range[2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[2, 2].Text = Environment.MachineName;

                        infoWorksheet.Range[3, 1].Text = "GENERATED BY";
                        infoWorksheet.Range[3, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[3, 2].Text = Staff.Username;
                        infoWorksheet.Range[4, 2].Text = Staff.FullName;

                        infoWorksheet.Range[5, 1].Text = "GENERATED AT";
                        infoWorksheet.Range[5, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[5, 2].Text = string.Format("{0:MMMM d, yyyy h:mm:ss tt}", dateNow);

                        infoWorksheet.Range[1, 1, 5, 1].CellStyle.Font.Bold = true;

                        var range = infoWorksheet.Range[1, 1, 5, 2];
                        range.CellStyle.Font.FontName = "Tahoma";
                        range.AutofitColumns();

                        workBook.ActiveSheetIndex = 1;
                        workBook.Worksheets.Remove(workBook.Worksheets[2]);
                        workBook.Worksheets.Remove(workBook.Worksheets[2]);

                        resultWorksheet.Protect(Properties.Settings.Default.ReportWorksheetPW, ExcelSheetProtection.All);
                        infoWorksheet.Protect(Properties.Settings.Default.ReportWorksheetPW, ExcelSheetProtection.All);
                        workBook.Protect(true, true, Properties.Settings.Default.WorkbookPW);
                        workBook.SaveAs(saveFileDialog.FileName);
                    }
                    
                    System.Windows.Forms.MessageBox.Show("Excel file created!", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                    Process.Start(System.IO.Path.GetDirectoryName(saveFileDialog.FileName));
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("Unable to create Excel file.\n" + ex.GetBaseException().Message, "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
            }
            
            G.EndWait(this);


            //var dateNow = DateTime.Now;

            //Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            //Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            //Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetReport = null;
            //Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetVotes = null;

            //try
            //{
            //    if (xlApp == null)
            //    {
            //        G.EndWait(this);

            //        MessageBox.Show("Excel is not properly installed!", "No MS Excel", MessageBoxButton.OK, MessageBoxImage.Error);
            //        return;
            //    }
            //    object misValue = System.Reflection.Missing.Value;

            //    //REPORT
            //    xlWorkBook = xlApp.Workbooks.Add(misValue);
            //    xlWorkSheetReport = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[1];
            //    xlWorkSheetReport.Name = "REPORT INFO";

            //    xlWorkSheetReport.Cells[1, 1] = "MACHINE TAG/NAME";
            //    xlWorkSheetReport.Cells[1, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            //    xlWorkSheetReport.Cells[1, 2] = Machine.Tag + "@" + Environment.MachineName;

            //    xlWorkSheetReport.Cells[2, 1] = "LOGGED USERNAME";
            //    xlWorkSheetReport.Cells[2, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            //    xlWorkSheetReport.Cells[2, 2] = Staff.Username;
            //    xlWorkSheetReport.Cells[3, 2] = Staff.FullName;

            //    xlWorkSheetReport.Cells[4, 1] = "GENERATED DATE";
            //    xlWorkSheetReport.Cells[4, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
            //    xlWorkSheetReport.Cells[4, 2] = string.Format("{0:MMMM d, yyyy • h:mm:ss tt}", dateNow);

            //    xlWorkSheetReport.Range[xlWorkSheetReport.Cells[1, 1], xlWorkSheetReport.Cells[4, 1]].Font.Bold = true;

            //    var range = xlWorkSheetReport.Range[xlWorkSheetReport.Cells[1, 1], xlWorkSheetReport.Cells[4, 2]];

            //    range.Font.Name = "Tahoma";
            //    range.Columns.AutoFit();
            //    xlWorkSheetReport.Protect(Properties.Settings.Default.ReportWorksheetPW, misValue, misValue, misValue, true, false, false, false, false, false, false, false, false, false, false, false);


            //    //VOTING
            //    xlWorkSheetVotes = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);
            //    xlWorkSheetVotes.Name = "VOTING RESULTS";

            //    int r = 0;
            //    foreach (Position position in Positions.Dictionary.Values)
            //    {
            //        var listCandidates = Ballots.Results.Where(x => x.Position.ID == position.ID).OrderBy(x => x.FullName);

            //        if (listCandidates.Count() != 0)
            //        {
            //            r++;

            //            xlWorkSheetVotes.Cells[r, 1] = position.Title.ToUpper();
            //            xlWorkSheetVotes.Cells[r, 1].Font.Bold = true;

            //            xlWorkSheetVotes.Cells[r, 2] = "TOTAL VOTES";
            //            xlWorkSheetVotes.Cells[r, 2].Font.Bold = true;

            //            xlWorkSheetVotes.Range[xlWorkSheetVotes.Cells[r, 1], xlWorkSheetVotes.Cells[r, 2]].Font.Name = "Tahoma";

            //            foreach (Candidate candidate in listCandidates)
            //            {
            //                r++;

            //                xlWorkSheetVotes.Cells[r, 1] = candidate.FullName + string.Format(" ({0})", candidate.Party.Abbreviation);
            //                xlWorkSheetVotes.Cells[r, 2] = candidate.VoteCount;
            //                xlWorkSheetVotes.Cells[r, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;

            //                xlWorkSheetVotes.Range[xlWorkSheetVotes.Cells[r, 1], xlWorkSheetVotes.Cells[r, 2]].Font.Name = "Tahoma";
            //             }
            //        }

            //        r++;
            //    }

            //    range = xlWorkSheetVotes.Range[xlWorkSheetVotes.Cells[1, 1], xlWorkSheetVotes.Cells[r, 2]];
            //    range.Columns.AutoFit();

            //    xlWorkSheetVotes.Protect(Properties.Settings.Default.ReportWorksheetPW, misValue, misValue, misValue, true, false, false, false, false, false, false, false, false, false, false, false);
            //    xlWorkBook.Protect(Properties.Settings.Default.WorkbookPW);

            //    string dateString = string.Format("{0:00}{1:00}{2:0000} {3:00}{4:00}{5:00}", dateNow.Month, dateNow.Day, dateNow.Year, dateNow.Hour, dateNow.Minute, dateNow.Second);
            //    string filename = string.Format("LNHS Voting Result ({0}).xls", Machine.Tag);

            //    xlWorkBook.SaveAs(string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, misValue, misValue, misValue, misValue, misValue);

            //    xlWorkBook.Close(false, misValue, misValue);
            //    xlApp.Quit();


            //    G.EndWait(this);

            //    System.Windows.Forms.MessageBox.Show("Excel file created!", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            //    Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            //}
            //catch (Exception)
            //{
            //    G.EndWait(this);

            //    System.Windows.Forms.MessageBox.Show("Unable to create Excel file.", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //}

            //Process.GetProcesses().Where(p => p.ProcessName == "EXCEL" && p.MainWindowHandle.ToInt32() == 0).ToList().ForEach(process => process.Kill());

            //ReleaseObject(xlWorkSheetVotes);
            //ReleaseObject(xlWorkSheetReport);
            //ReleaseObject(xlWorkBook);
            //ReleaseObject(xlApp);

            //Topmost = true;
            //Topmost = false;
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                if (obj == null) return;

                System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                obj = null;
            }
            catch (Exception ex)
            {
                obj = null;
                MessageBox.Show("Exception occured while releasing object " + ex.ToString(), "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Error);

                Application.Current?.Shutdown();
            }
        }

        private void btnPrintReport_Click(object sender, RoutedEventArgs e)
        {
            //G.WaitLang(this);
            
            var form = new PrintForm();
            //form.Opacity = 0;
            //form.ShowInTaskbar = false;
            form.ShowDialog();
            //form.Hide();
            //WindowInteropHelper helper = new WindowInteropHelper(this);
            //SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            //form.Print();
            //form.reportViewer1.RenderingComplete += (s, ev) =>
            //{
            //    var rv = (s as ReportViewer);
            //    //MessageBox.Show(rv.Parent.ToString());
            //    //rv.Location = new System.Drawing.Point((int)Left, (int)Top);
            //    G.EndWait(this);

            //    rv.PrintDialog();

            //    //btnPrintReport.IsEnabled = true;
            //    //btnExportReport.IsEnabled = true;
            //    //lblLogOut.IsEnabled = true;
            //    //lblUsername.IsEnabled = true;
            //    //tbcMaintenance.IsEnabled = true;

            //    form.Close();

            //    Topmost = true;
            //    Topmost = false;
            //};

            //G.EndWait(this);
        }

        #endregion
        
        private void btnCancelTag_Click(object sender, RoutedEventArgs e)
        {
            lblTag.Visibility = Visibility.Visible;
            txtTag.Visibility = Visibility.Collapsed;
            stkTagButtons.Visibility = Visibility.Collapsed;

            txtTag.Clear();

            bdrMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            tbcMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
            dckTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
            bdrMaintenance.BorderBrush = new SolidColorBrush(Colors.Gray);
            lblMachineTag.Foreground = new SolidColorBrush(Colors.Black);

            stkUsername.IsEnabled = true;
            tbcMaintenance.IsEnabled = true;
            tbcMaintenance.Opacity = 1;
        }

        private void txtTag_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.IsBlank())
            {
                return;
            }

            if (!char.IsLetterOrDigit(e.Text.Last()) || (e.Text.Last().ToString().IsBlank()))
            {
                e.Handled = true;
            }
        }

        private void btnUpdateTag_Click(object sender, RoutedEventArgs e)
        {
            if (txtTag.Text.IsBlank())
            {
                MessageBox.Show("Please provide a tag for this machine.", "No Tag", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTag.Focus();

                return;
            }

            
            try
            {
                G.WaitLang(this);

                Machine.UpdateTag(txtTag.Text.Trim());

                lblTag.Visibility = Visibility.Visible;
                txtTag.Visibility = Visibility.Collapsed;
                stkTagButtons.Visibility = Visibility.Collapsed;

                LoadMachine();

                G.EndWait(this);

                txtTag.Clear();

                stkUsername.IsEnabled = true;
                tbcMaintenance.IsEnabled = true;
                tbcMaintenance.Opacity = 1;

                bdrMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                tbcMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
                dckTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
                bdrMaintenance.BorderBrush = new SolidColorBrush(Colors.Gray);
                lblMachineTag.Foreground = new SolidColorBrush(Colors.Black);

                txtTag.Focus();
            }
            catch (Exception ex)
            {

                G.EndWait(this);
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void lblTag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (Machine.AreCandidatesFinalized)
            {
                return;
            }

            lblTag.Visibility = Visibility.Collapsed;
            txtTag.Visibility = Visibility.Visible;
            stkTagButtons.Visibility = Visibility.Visible;

            stkUsername.IsEnabled = false;
            tbcMaintenance.IsEnabled = false;
            tbcMaintenance.Opacity = 0.5;

            bdrMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            tbcMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            dckTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 32, 32, 32));
            txtTag.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 64, 64, 64));
            txtTag.Foreground = new SolidColorBrush(Colors.White);
            txtTag.SelectionBrush = new SolidColorBrush(Colors.LightGray);
            txtTag.CaretBrush = new SolidColorBrush(Colors.WhiteSmoke);
            txtTag.BorderThickness = new Thickness(0);
            txtTag.BorderThickness = new Thickness(0);
            lblMachineTag.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            btnUpdateTag.Foreground = new SolidColorBrush(Colors.WhiteSmoke);
            btnCancelTag.Foreground = new SolidColorBrush(Colors.WhiteSmoke);

            txtTag.Text = Machine.Tag;
            txtTag.SelectAll();
            txtTag.Focus();
        }

        private void btnViewResult_Click(object sender, RoutedEventArgs e)
        {
            Opacity = 0.5;

            try
            {
                var votedCount = Voters.Dictionary.Values.Where(x => Ballots.Dictionary.Keys.Contains(x.ID)).Count();

                var result = MessageBox.Show(string.Format("Once viewed, ALL {0} VOTER{1}, who still not voted yet, will NOT allowed to vote anymore.\n\nAlso, you CANNOT ALLOW to CREATE, EDIT or DELETE voters.\n\n\nDo you want to view the results to this machine?", Voters.Dictionary.Count - votedCount, (Voters.Dictionary.Count - votedCount) == 1 ? "S" : ""), "Viewing Results", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    G.WaitLang(this);

                    Machine.Viewed();

                    LoadMachine();
                    LoadResults();
                    
                    G.EndWait(this);
                }

                Opacity = 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }
        

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab && !cmbPositions.Focusable)
            {
                cmbPositions.Focusable = true;
                cmbPositions.Focus();
            }
        }
        
        private void lblPosition_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Cursor = Cursors.Wait;
            lblPosition.Cursor = Cursor;
            Opacity = 0.5;

            G.WaitLang(this);
            var positionForm = new PositionForm();
            positionForm.Tag = this;
            positionForm.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            WindowInteropHelper helper = new WindowInteropHelper(this);
            SetWindowLong(new HandleRef(positionForm, positionForm.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(this);

            Cursor = Cursors.Arrow;
            lblPosition.Cursor = Cursors.Hand;

            positionForm.ShowDialog();
            
            Opacity = 1;
        }


        System.Windows.Point _posPressed;
        System.Windows.Forms.Cursor _previousCursor;
        bool _wasMoved = false;
        private void lvVoter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && _previousCursor == System.Windows.Forms.Cursors.Arrow)
            {
                var posRelease = e.GetPosition(this);
                if (Math.Abs(_posPressed.X - posRelease.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(_posPressed.Y - posRelease.Y) > SystemParameters.MinimumHorizontalDragDistance || _wasMoved) return;
                _wasMoved = false;

                var dep = (DependencyObject)e.OriginalSource;

                while ((dep != null) && !(dep is GridViewColumnHeader))
                    dep = VisualTreeHelper.GetParent(dep);

                if (dep == null)
                    return;

                var previousClicked = _columnClicked;

                _columnClicked = new GridViewColumn()
                {
                    Header = (dep as GridViewColumnHeader).Content
                };
                var columns = gvVoter.Columns.Select(x => x.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()).ToList();

                var oldIndex = columns.IndexOf(previousClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart());
                var newIndex = columns.ToList().IndexOf(_columnClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart());

                if (oldIndex == newIndex)
                {
                    _isAscendingVoter = !_isAscendingVoter;
                    if (_isAscendingVoter)
                        _columnClicked.Header = _columnClicked.Header.ToString().Replace("▼", "▲");
                    else
                        _columnClicked.Header = _columnClicked.Header.ToString().Replace("▲", "▼");
                }
                else
                {
                    _isAscendingVoter = true;
                    _columnClicked.Header = !_columnClicked.Header.ToString().Contains("▼") ? "▲ " + _columnClicked.Header : "▼ " + _columnClicked.Header;
                    
                    gvVoter.Columns[oldIndex].Header = previousClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart();
                }
                gvVoter.Columns[newIndex].Header = _columnClicked.Header;

                SortVoter();

                _foreignCount = 0;
                _voterCount = 0;

                _cvVoter = (CollectionView)CollectionViewSource.GetDefaultView(lvVoter.ItemsSource);
                _cvVoter.Filter = FilterVoter;
                
                if (lvVoter.SelectedIndex >= 0)
                {
                    lvVoter.ScrollIntoView(lvVoter.SelectedItem);
                }
                else
                {
                    if (lvVoter.Items.Count > 0) lvVoter.ScrollIntoView(lvVoter.Items[0]);
                }
            }
        }

        private void lvVoter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _previousCursor = System.Windows.Forms.Cursor.Current;
            _posPressed = e.GetPosition(this);
        }

        private void lblVoterInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((int)lblVoterInfo.Tag == 0)
                lblVoterInfo.Tag = 1;
            else
                lblVoterInfo.Tag = 0;

            RefreshVoterCountLabel();

            if (lvVoter.SelectedIndex >= 0)
            {
                lvVoter.ScrollIntoView(lvVoter.SelectedItem);
            }
        }

        private void lvVoter_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                lvVoter.SelectedIndex = -1;
            }
        }
        
        private void border_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.Source as Border) != null || (e.Source as DockPanel) != null ||
                ((e.Source as Label) != null && ((e.Source as Label).Cursor == Cursors.Arrow)))
                DragMove();
        }

        private void txtTag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private void dgResults_SortColumnsChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSortColumnsChangedEventArgs e)
        {
        }

        private void dgResults_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
        {
            var dg = (Syncfusion.UI.Xaml.Grid.SfDataGrid)sender;

            if (e.RowIndex != 0)
            {
                if (dg.GridColumnSizer.GetAutoRowHeight(e.RowIndex, new Syncfusion.UI.Xaml.Grid.GridRowSizingOptions { AutoFitMode = Syncfusion.UI.Xaml.Grid.AutoFitMode.SmartFit }, out double newHeight))
                {
                    if (newHeight > 24)
                    {
                        e.Height = newHeight;
                        e.Handled = true;
                    }
                }
            }
        }

        private void lvVoter_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _wasMoved = e.LeftButton == MouseButtonState.Released;
        }
    }

    public class VoteResult
    {
        public ImageSource PictureSource { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string PositionName { get; set; }
        public int PositionOrder { get; set; }
        public int GradeLevel { get; set; }
        public string GradeStrand { get; set; }
        public string PartyTitle { get; set; }
        public SolidColorBrush PartyColorBrush { get; set; }
        public int VoteCount { get; set; }
    }

    public class CustomSorting : IComparer<Object>, ISortDirection
    {
        private ListSortDirection _SortDirection;
        public ListSortDirection SortDirection
        {
            get { return _SortDirection; }
            set { _SortDirection = value; }
        }

        public int Compare(object x, object y)
        {
            int namX;
            int namY;
            if (x.GetType() == typeof(VoteResult))
            {
                namX = ((VoteResult)x).PositionOrder;
                namY = ((VoteResult)y).PositionOrder;
            }
            else if (x.GetType() == typeof(Group))
            {
                namX = ((Group)x).Key.ToString().Length;
                namY = ((Group)y).Key.ToString().Length;
            }
            else
            {
                namX = x.ToString().Length;
                namY = y.ToString().Length;
            }

            if (namX.CompareTo(namY) > 0)
                return SortDirection == ListSortDirection.Ascending ? 1 : -1;
            else if (namX.CompareTo(namY) == -1)
                return SortDirection == ListSortDirection.Ascending ? -1 : 1;
            else
                return 0;
        }
    }
}