//using StudentElection.Classes;
using StudentElection;
//using Microsoft.Reporting.WinForms;
using Microsoft.Win32;
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
using static StudentElection.G;
using StudentElection.UserControls;
using StudentElection.Dialogs;
using StudentElection.Repository.Models;
using StudentElection.Services;
using Project.Library.Helpers;
using StudentElection.Repository.Interfaces;
using Humanizer;
using ExcelDataReader;

namespace StudentElection.Main
{
    /// <summary>
    /// Interaction logic for MaintenanceWindow.xaml
    /// </summary>
    public partial class MaintenanceWindow : Window
    {
        public UserModel User;
        //public List<Candidate> CandidateList;

        private bool _isLogOut;

        private readonly UserService _userService = new UserService();
        private readonly ElectionService _electionService = new ElectionService();
        private readonly PositionService _positionService = new PositionService();
        private readonly PartyService _partyService = new PartyService();
        private readonly CandidateService _candidateService = new CandidateService();
        private readonly VoterService _voterService = new VoterService();
        private readonly BallotService _ballotService = new BallotService();

        private ElectionModel _currentElection;
        
        private BackgroundWorker _backgroundWorker;
        private ProgressWindow _progressWindow;

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
            _voterCount = lvVoter.Items.Count;
            CollectionViewSource.GetDefaultView(lvVoter.ItemsSource).Refresh();

            lblVoterInfo.Content = $"{ "voter".ToQuantity(_voterCount) }";
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);

            await LoadElectionAsync();
            await LoadUsersAsync();
            await LoadVotersAsync();
            await LoadCandidatesAsync();
            await LoadResultsAsync();

            lblPosition.Text = string.Format("Positions ({0})", await _positionService.GetPositionsCountAsync(_currentElection.Id));
            lblUsername.Content = User.UserName;

            if (User.Type == Repository.Models.UserType.Admin)
            {
                rdfStaff.Height = new GridLength(0, GridUnitType.Star);
            }
            
            G.EndWait(this);
        }

        private async Task LoadElectionAsync()
        {
            try
            {
                _currentElection = await _electionService.GetCurrentElectionAsync();

                if (_currentElection.ServerTag.IsBlank())
                {
                    lblTag.Content = "(No Tag)";
                    txtTag.Text = "";
                }
                else
                {
                    lblTag.Content = _currentElection.ServerTag;
                    txtTag.Text = _currentElection.ServerTag;
                }
                
                if (_currentElection.CandidatesFinalizedAt.HasValue && !_currentElection.ClosedAt.HasValue)
                {
                    bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(96, 224, 176));
                    bdrMaintenance.BorderThickness = new Thickness(4);
                }
                else if (_currentElection.ClosedAt.HasValue)
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

        public async Task CheckCandidatesAsync()
        {
            if (!_currentElection.CandidatesFinalizedAt.HasValue)
            {
                try
                {
                    if (await _candidateService.GetCandidatesCount(_currentElection.Id) > 0)
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
                //chkVoted.Visibility = Visibility.Collapsed;

                lblTag.Cursor = Cursors.IBeam;
            }
            else
            {
                dckCandidateButtons.Visibility = Visibility.Collapsed;
                //chkVoted.Visibility = Visibility.Visible;

                foreach (PartyItemControl item in stkCandidates.Children)
                {
                    item.AreCandidatesFinalized = true;
                    item.btnAddCandidate.Visibility = Visibility.Collapsed;
                    item.btnImportCandidates.Visibility = Visibility.Collapsed;
                    item.tbkParty.Cursor = Cursors.Arrow;
                }

                lblTag.Cursor = Cursors.Arrow;
            }
        }

        private async Task CheckResultsAsync()
        {
            if (!_currentElection.ClosedAt.HasValue)
            {
                try
                {
                    btnVoterButtons.Visibility = Visibility.Visible;

                    var ballotsCount = await _ballotService.CountBallotsAsync(_currentElection.Id);
                    var votersCount = await _voterService.CountVotersAsync(_currentElection.Id);

                    if (ballotsCount > 0)
                    {
                        lblResultsAvailable.Content = string.Format("Results are available ({0:n0} of {1:n0} voters voted)", ballotsCount, votersCount);
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
                return ((obj as UserModel).LastName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                    ((obj as UserModel).FirstName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                    ((obj as UserModel).MiddleName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0);
            }
        }

        private async Task LoadUsersAsync()
        {
            try
            {
                lvStaff.ItemsSource = await _userService.GetUsersAsync();

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

                btnDeleteStaff.Visibility = (User.Id != (lvStaff.SelectedValue as UserModel).Id) ? Visibility.Visible : Visibility.Collapsed;

                _selectedStaff = lv.SelectedIndex;
            }
            else
            {
                btnEditStaff.Visibility = Visibility.Collapsed;
                btnDeleteStaff.Visibility = Visibility.Collapsed;
            }
        }

        private async void btnAddStaff_Click(object sender, RoutedEventArgs e)
        {
            if (stkCandidates.Children.Count == 10)
            {
                MessageBox.Show("Up to 10 users only.", "Position", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;
            
            G.WaitLang(this);

            var staffWindow = new StaffWindow();
            //staffWindow.MachineID = _currentElection.Id;
            staffWindow.Owner = this;
            
            G.EndWait(this);

            staffWindow.ShowDialog();

            G.WaitLang(this);

            Opacity = 1;

            if (!staffWindow.IsCanceled)
                await LoadUsersAsync();
            
            lvStaff.ScrollIntoView(lvStaff.Items[lvStaff.Items.Count - 1]);

            G.EndWait(this);
        }

        private async void btnEditStaff_Click(object sender, RoutedEventArgs e)
        {
            G.EndWait(this);

            Opacity = 0.5;

            var staff = lvStaff.SelectedValue as UserModel;
            
            var staffWindow = new StaffWindow();
            staffWindow.User = staff;
            staffWindow.Owner = this;

            G.EndWait(this);

            staffWindow.ShowDialog();

            G.WaitLang(this);

            Opacity = 1;
            if (!staffWindow.IsCanceled)
                await LoadUsersAsync();

            lvStaff.ScrollIntoView(lvStaff.Items[_selectedStaff]);
            lvStaff.SelectedIndex = _selectedStaff;

            G.EndWait(this);
        }

        private async void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        {
            if (lvStaff.SelectedItems.Count == 1)
            {
                var user = (lvStaff.SelectedValue as UserModel);
                var result = MessageBox.Show(user.FullName + " will not be able to log in on this system. Do you want to continue?", "Deleting '" + user.UserName + "'", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
                var selectedStaff = _selectedStaff == lvStaff.Items.Count - 1 ? _selectedStaff - 1 : _selectedStaff;

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        G.WaitLang(this);

                        await _userService.DeleteUserAsync(user);

                        G.EndWait(this);
                        MessageBox.Show("The user is deleted.", "Staff", MessageBoxButton.OK, MessageBoxImage.Information);

                        G.WaitLang(this);
                        await LoadUsersAsync();

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
            var voter = obj as VoterModel;
            
            bool containsText = ((voter.FullName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       //(voter.FirstName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       //(voter.MiddleName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.YearLevel.ToString().IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Section.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Sex.ToString().IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Birthdate?.ToString("yyyy-MM-dd").IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Vin.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0));

            return containsText;
            //bool isVoted = voter.IsVoted;

            //if ((isForeign && (isVoted || !chkVoted.IsChecked.Value)) && containsText)
            //{
            //    _foreignCount++;
            //}
            //else if (((isVoted && chkVoted.IsChecked.Value) || (!chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0)) && containsText)
            //{
            //    _voterCount++;
            //}

            //return ((isForeign && !chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 1) || (isVoted && chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0) || (!chkVoted.IsChecked.Value && (int)lblVoterInfo.Tag == 0)) && containsText;
        }

        private async Task SortVoterAsync()
        {
            lvVoter.ItemsSource = await _voterService.GetVotersAsync(_currentElection.Id);

            //TODO: SORT
            //if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[0].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()) || _columnClicked.Header == null)
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[1].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[2].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.FirstName).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.FirstName).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[3].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.MiddleName).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.MiddleName).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[4].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.GradeLevel).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.GradeLevel).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[5].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.StrandSection).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.StrandSection).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[6].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.SexType).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.SexType).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[7].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            //{
            //    if (_isAscendingVoter)
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderBy(x => x.Birthdate).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //    else
            //    {
            //        lvVoter.ItemsSource = Voters.Dictionary.Values.OrderByDescending(x => x.Birthdate).
            //            ThenBy(x => x.FullName).
            //            ThenBy(x => x.VoterID);
            //    }
            //}
            //END SORTING

        }
        
        public async Task LoadVotersAsync()
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
                await SortVoterAsync();

                lblVoterInfo.Tag = 0;

                _cvVoter = (CollectionView)CollectionViewSource.GetDefaultView(lvVoter.ItemsSource);
                _cvVoter.Filter = FilterVoter;

                RefreshVoterCountLabel();

                if (_currentElection.CandidatesFinalizedAt.HasValue && !_currentElection.ClosedAt.HasValue)
                {
                    //TODO: RESULTS ARE AVAILABLE COUNT OF TOTAL VOTERS VOTED
                    //lblResultsAvailable.Content = string.Format("Results are available ({0:#,##0} of {1:#,##0} voters voted)", Voters.Dictionary.Count, Ballots.Dictionary.Count);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private async void btnAddVoter_Click(object sender, RoutedEventArgs e)
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
                await LoadVotersAsync();

                if (voterWindow.Voter != null)
                {
                    var newItem = lvVoter.ItemsSource.Cast<VoterModel>().Where(x => x.Id == voterWindow.Voter.Id).First();
                    lvVoter.ScrollIntoView(newItem);
                    lvVoter.SelectedItem = newItem;

                    txtVoterFilter.Text = searched;
                }
                else
                {
                    var newItem = lvVoter.ItemsSource.Cast<VoterModel>().OrderBy(v => v.Id).LastOrDefault();
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
                if (_currentElection.ClosedAt.HasValue) return;

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

        private async void btnEditVoter_Click(object sender, RoutedEventArgs e)
        {
            if (lvVoter.SelectedItems.Count == 1)
            {
                var voter = lvVoter.SelectedValue as VoterModel;
                var selectedVoter = lvVoter.SelectedIndex;

                if (_currentElection.CandidatesFinalizedAt.HasValue)
                {
                    //TODO: CHECK IF ALREADY VOTED
                    //try
                    //{
                    //    G.WaitLang(this);

                    //    if (Ballots.Dictionary.Keys.Contains(voter.Id))
                    //    {
                    //        G.EndWait(this);
                    //        MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Voted", MessageBoxButton.OK, MessageBoxImage.Error);
                    //        return;
                    //    }
                        
                    //    G.EndWait(this);
                    //}
                    //catch (Exception ex)
                    //{
                    //    G.EndWait(this);
                    //    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    //    Application.Current?.Shutdown();
                    //}
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
                    await LoadVotersAsync();
                    await LoadCandidatesAsync();

                    if (lvVoter.Items.Count != 0 && voterWindow.Voter != null)
                    {
                        var v = lvVoter.ItemsSource.Cast<VoterModel>().Where(x => x.Id == voterWindow.Voter.Id).First();
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
        
        private async void btnDeleteVoter_Click(object sender, RoutedEventArgs e)
        {
            if (lvVoter.SelectedItems.Count == 1)
            {
                G.WaitLang(this);
                var voter = (lvVoter.SelectedValue as VoterModel);
                var selectedVoter = (lvVoter.SelectedIndex);

                selectedVoter = selectedVoter == lvVoter.Items.Count - 1 ? selectedVoter - 1 : selectedVoter;

                try
                {
                    //TODO: DO NOT DELETE IF ALREADY VOTED
                    //if (Ballots.Dictionary.Keys.Contains(voter.Id))
                    //{
                    //    G.EndWait(this);
                    //    MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Voted", MessageBoxButton.OK, MessageBoxImage.Error);

                    //    return;
                    //}

                    lvVoter.ScrollIntoView(voter);

                    G.EndWait(this);
                    var result = MessageBox.Show("Delete " + voter.FullName + "?", "Deleting Voter", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (result == MessageBoxResult.Yes)
                    {
                        G.WaitLang(this);

                        await _voterService.DeleteVoterAsync(voter);

                        G.EndWait(this);

                        G.WaitLang(this);

                        await LoadVotersAsync();

                        if (selectedVoter >= 0 && selectedVoter < lvVoter.Items.Count)
                        {
                            lvVoter.ScrollIntoView(lvVoter.Items[selectedVoter]);
                        }
                        MessageBox.Show("The voter is deleted.", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);

                        G.EndWait(this);
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

        private async void lblUsername_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Opacity = 0.5;

            var staffWindow = new StaffWindow();
            staffWindow.User = this.User;
            staffWindow.Owner = this;
            staffWindow.ShowDialog();

            Opacity = 1;

            if (staffWindow.IsCanceled) return;

            await LoadUsersAsync();
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

        private async void btnAddParty_Click(object sender, RoutedEventArgs e)
        {
            //if (stkCandidates.Children.Count == 20)
            //{
            //    MessageBox.Show("Up to 20 parties only.", "Party", MessageBoxButton.OK, MessageBoxImage.Error);

            //    return;
            //}

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

                await LoadVotersAsync();
                await LoadCandidatesAsync();

                svrCandidates.ScrollToEnd();

                G.EndWait(this);
            }
        }


        public async Task LoadCandidatesAsync()
        {
            var currScroll = svrCandidates.VerticalOffset;
            foreach (PartyItemControl control in stkCandidates.Children)
            {
                CandidateHOffsets.Add(control.svrCandidate.HorizontalOffset);
            }
            
            try
            {
                if (await _partyService.GetPartiesCount(_currentElection.Id) == 0)
                    lblNoCandidates.Visibility = Visibility.Visible;
                else
                    lblNoCandidates.Visibility = Visibility.Hidden;

                stkCandidates.Children.Clear();

                var partyIndex = 0;
                var parties = await _partyService.GetPartiesAsync(_currentElection.Id);
                foreach (var party in parties)
                {
                    var item = new PartyItemControl();
                    item.DataContext = party;
                    
                    stkCandidates.Children.Add(item);

                    var candidateParty = await _candidateService.GetCandidatesByPartyAsync(party.Id);
                    //var candidateParty = Candidates.Dictionary.Values.Where(x => x.Party.ID == party.Id).OrderBy(x => x.Position.Order).ToList();

                    if (candidateParty.Count() > 0)
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

                await CheckCandidatesAsync();

                svrCandidates.ScrollToVerticalOffset(currScroll);
                G.CandidateHOffsets = new List<double>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }
        
        private async void btnFinalize_Click(object sender, RoutedEventArgs e)
        {
            if (_currentElection.ServerTag.IsBlank())
            {
                MessageBox.Show("Please provide a tag for this machine's server. Make sure that the tag is unique among other servers.", "Provide A Tag", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;

            var candidatesCount = await _candidateService.GetCandidatesCount(_currentElection.Id);
            //var fcv = Candidates.Dictionary.Values.Where(x => x.IsForeign).Count();
            var result = MessageBox.Show(string.Format("Once finalized, you WILL NO LONGER EDIT or DELETE ALL CANDIDATES' INFORMATION, but you may still create, edit or delete voters who did not vote yet. Also, the SERVER TAG will be finalized and uneditable.\n\nThere are {0} {1}.\n\n\nDo you want to finalize the candidates?",
                candidatesCount, (candidatesCount == 1 ? "candidate" : "candidates")), "Finalizing Candidates", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    await _electionService.FinalizeCandidatesAsync(_currentElection.Id);

                    G.EndWait(this);

                    MessageBox.Show("The candidates have been finalized.\n\nYou will NO LONGER EDIT or DELETE ALL CANDIDATES' INFORMATION and CHANGE the SERVER TAG.", "Candidates Finalized", MessageBoxButton.OK, MessageBoxImage.Information);

                    G.WaitLang(this);

                    await LoadElectionAsync();
                    await LoadVotersAsync();
                    await LoadCandidatesAsync();

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
        
        public async Task LoadResultsAsync()
        {
            await CheckResultsAsync();

            if (!_currentElection.ClosedAt.HasValue)
            {
                return;
            }

            var listPosition = new List<PositionModel>();

            try
            {
                var positionRows = await _positionService.GetPositionsAsync(_currentElection.Id);
                var voteResults = await _ballotService.GetVoteResultsAsync(_currentElection.Id);
                
                stkResults.Children.Clear();

                cmbPositions.Items.Clear();
                cmbPositions.Items.Add(new PositionModel
                {
                    Id = 0,
                    Title = "All"
                });

                foreach (var position in positionRows)
                {
                    var voteResultsPosition = voteResults.Where(x => x.PositionId == position.Id).OrderByDescending(r => r.VoteCount).ToList();

                    if (!voteResultsPosition.Any())
                    {
                        continue;
                    }

                    cmbPositions.Items.Add(position);

                    var item = new PositionItemControl();
                    item.tbkName.Visibility = Visibility.Collapsed;
                    item.stkVotes.Visibility = Visibility.Collapsed;
                    item.cdfCandidate.Width = new GridLength(0);
                    item.DataContext = position;

                    stkResults.Children.Add(item);

                    lblNoResults.Visibility = Visibility.Hidden;

                    item.wrpCandidate.Children.Clear();

                    int rank = 0;
                    int continuousRank = 0;
                    for (int i = 0; i < voteResultsPosition.Count; i++)
                    {
                        continuousRank++;

                        var voteResult = voteResultsPosition[i];
                        item.wrpCandidate.Visibility = Visibility.Visible;
                        var control = new CandidateResultControl();

                        var previousCandidate = i == 0 ? null : voteResultsPosition.ElementAtOrDefault(i - 1);

                        if (previousCandidate == null || previousCandidate.VoteCount != voteResult.VoteCount)
                        {
                            rank = continuousRank;
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
                        control.DataContext = voteResult;
                        control.recCandidate.Height = 120;

                        //double quotient = candidate.VoteCount / (double)candidate.PositionVoteCount;

                        //if (!double.IsNaN(quotient))
                        //{
                        //    control.recCandidate.Height = 120 * quotient;
                        //}

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
                if ((item.DataContext as PositionModel).Id == (cmbPositions.SelectedItem as PositionModel).Id || cmbPositions.SelectedIndex == 0)
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

        private async void btnExportReport_Click(object sender, RoutedEventArgs e)
        {
            //var options = new ExcelExportingOptions();
            //options.ExportMode = ExportMode.Text;
            //options.ExcelVersion = ExcelVersion.Excel2007;

            //var saveFileDialog = new SaveFileDialog
            //{
            //    FileName = $"{ _currentElection.Title } Result - { _currentElection.ServerTag } - { dateNow.ToString("yyyy-MM-dd HHmmss") }.xls"
            //};
            //saveFileDialog.Filter = "Excel file (*.xls)|*.xls";

            //if (saveFileDialog.ShowDialog() == true)
            //{
            //    try
            //    {
            //        G.WaitLang(this);

            //        using (var excelEngine = dgResults.ExportToExcel(dgResults.View, options))
            //        {
            //            var workBook = excelEngine.Excel.Workbooks[0];
            //            var resultWorksheet = workBook.Worksheets[0];
            //            resultWorksheet.Name = "VOTING RESULTS";

            //            resultWorksheet[1, 1, 1, dgResults.Columns.Count].CellStyle.Font.Bold = true;

            //            resultWorksheet.Columns[1].Group(ExcelGroupBy.ByColumns);

            //            resultWorksheet.UsedRange.AutofitColumns();
            //            resultWorksheet.UsedRange.AutofitRows();

            //            var infoWorksheet = excelEngine.Excel.Worksheets.AddCopyBefore(resultWorksheet);
            //            infoWorksheet.Clear();

            //            infoWorksheet.Name = "REPORT INFO";

            //            infoWorksheet.Range[1, 1].Text = "SERVER TAG";
            //            infoWorksheet.Range[1, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
            //            infoWorksheet.Range[1, 2].Text = _currentElection.ServerTag;

            //            infoWorksheet.Range[2, 1].Text = "SERVER NAME";
            //            infoWorksheet.Range[2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
            //            infoWorksheet.Range[2, 2].Text = Environment.MachineName;

            //            infoWorksheet.Range[3, 1].Text = "GENERATED BY";
            //            infoWorksheet.Range[3, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
            //            infoWorksheet.Range[3, 2].Text = User.UserName;
            //            infoWorksheet.Range[4, 2].Text = User.FullName;

            //            infoWorksheet.Range[5, 1].Text = "GENERATED AT";
            //            infoWorksheet.Range[5, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
            //            infoWorksheet.Range[5, 2].Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dateNow);

            //            infoWorksheet.Range[1, 1, 5, 1].CellStyle.Font.Bold = true;

            //            var range = infoWorksheet.Range[1, 1, 5, 2];
            //            range.CellStyle.Font.FontName = "Tahoma";
            //            range.AutofitColumns();

            //            workBook.ActiveSheetIndex = 1;
            //            workBook.Worksheets.Remove(workBook.Worksheets[2]);
            //            workBook.Worksheets.Remove(workBook.Worksheets[2]);

            //            resultWorksheet.Protect(Properties.Settings.Default.ReportWorksheetPW, ExcelSheetProtection.All);
            //            infoWorksheet.Protect(Properties.Settings.Default.ReportWorksheetPW, ExcelSheetProtection.All);
            //            workBook.Protect(true, true, Properties.Settings.Default.WorkbookPW);
            //            workBook.SaveAs(saveFileDialog.FileName);
            //        }

            //        System.Windows.Forms.MessageBox.Show("Excel file created!", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            //        Process.Start(System.IO.Path.GetDirectoryName(saveFileDialog.FileName));
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Windows.Forms.MessageBox.Show("Unable to create Excel file.\n" + ex.GetBaseException().Message, "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            //    }
            //}
            
            G.WaitLang(this);

            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook xlWorkBook = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetReport = null;
            Microsoft.Office.Interop.Excel.Worksheet xlWorkSheetVotes = null;

            try
            {
                if (xlApp == null)
                {
                    G.EndWait(this);

                    MessageBox.Show("Excel is not properly installed!", "No MS Excel", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var dateNow = DateTime.Now;
                object misValue = System.Reflection.Missing.Value;

                //REPORT
                xlWorkBook = xlApp.Workbooks.Add(misValue);
                xlWorkSheetReport = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets[1];
                xlWorkSheetReport.Name = "REPORT INFO";

                xlWorkSheetReport.Cells[2, 1] = "SERVER TAG/NAME";
                xlWorkSheetReport.Cells[2, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                xlWorkSheetReport.Cells[2, 2] = _currentElection.ServerTag + "@" + Environment.MachineName;

                xlWorkSheetReport.Cells[3, 1] = "GENERATED BY";
                xlWorkSheetReport.Cells[3, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                xlWorkSheetReport.Cells[3, 2] = User.UserName;
                xlWorkSheetReport.Cells[4, 2] = User.FullName;

                xlWorkSheetReport.Cells[5, 1] = "GENERATED AT";
                xlWorkSheetReport.Cells[5, 1].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                xlWorkSheetReport.Cells[5, 2] = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dateNow);
                xlWorkSheetReport.Cells[5, 2].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;


                xlWorkSheetReport.Range[xlWorkSheetReport.Cells[1, 1], xlWorkSheetReport.Cells[5, 1]].Font.Bold = true;

                var range = xlWorkSheetReport.Range[xlWorkSheetReport.Cells[1, 1], xlWorkSheetReport.Cells[5, 2]];
                range.Font.Name = "Tahoma";
                range.Columns.AutoFit();
                xlWorkSheetReport.Protect(Properties.Settings.Default.ReportWorksheetPW, misValue, misValue, misValue, true, false, false, false, false, false, false, false, false, false, false, false);


                //VOTING
                xlWorkSheetVotes = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkBook.Worksheets.Add(After: xlWorkBook.Sheets[xlWorkBook.Sheets.Count]);
                xlWorkSheetVotes.Name = "VOTING RESULTS";

                xlWorkSheetVotes.Cells[1, 1] = "Position";
                xlWorkSheetVotes.Cells[1, 2] = "Candidate";
                xlWorkSheetVotes.Cells[1, 3] = "YearLevel";
                xlWorkSheetVotes.Cells[1, 4] = "Section";
                xlWorkSheetVotes.Cells[1, 5] = "Alias";
                xlWorkSheetVotes.Cells[1, 6] = "Party";
                xlWorkSheetVotes.Cells[1, 7] = "Vote Count";
                xlWorkSheetVotes.Range[xlWorkSheetVotes.Cells[1, 1], xlWorkSheetVotes.Cells[1, 7]].Font.Bold = true;
                
                var voteResults = (await _ballotService.GetVoteResultsAsync(_currentElection.Id))
                    .OrderBy(vr => vr.PositionRank)
                    .ThenBy(vr => vr.LastName)
                    .ThenBy(vr => vr.FirstName)
                    .ThenBy(vr => vr.Suffix)
                    .ThenBy(vr => vr.MiddleName)
                    .ThenBy(vr => vr.Alias);

                int r = 1;
                foreach (var result in voteResults)
                {
                    r++;
                    xlWorkSheetVotes.Cells[r, 1] = result.PositionTitle;
                    xlWorkSheetVotes.Cells[r, 2] = result.FullName;
                    xlWorkSheetVotes.Cells[r, 3] = result.YearLevel;
                    xlWorkSheetVotes.Cells[r, 4] = result.Section;
                    xlWorkSheetVotes.Cells[r, 5] = result.Alias;
                    xlWorkSheetVotes.Cells[r, 6] = result.PartyTitle;

                    xlWorkSheetVotes.Cells[r, 7] = result.VoteCount;
                    xlWorkSheetVotes.Cells[r, 7].HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignGeneral;
                }

                range = xlWorkSheetVotes.Range[xlWorkSheetVotes.Cells[1, 1], xlWorkSheetVotes.Cells[r, 7]];
                range.Font.Name = "Tahoma";
                range.Columns.AutoFit();

                xlWorkSheetVotes.Protect(Properties.Settings.Default.ReportWorksheetPW, misValue, misValue, misValue, true, false, false, false, false, false, false, false, false, false, false, false);
                xlWorkBook.Protect(Properties.Settings.Default.WorkbookPW);

                string filename = $"{ _currentElection.Title } Result - { _currentElection.ServerTag } - { dateNow.ToString("yyyy-MM-dd HHmmss") }.xls";

                xlWorkBook.SaveAs(string.Format(@"{0}\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename), Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal, misValue, misValue, misValue, misValue, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared, misValue, misValue, misValue, misValue, misValue);

                //xlWorkBook.Close(misValue, misValue, misValue);
                xlApp.Quit();


                G.EndWait(this);

                System.Windows.Forms.MessageBox.Show("Excel file created!", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));
            }
            catch (Exception ex)
            {
                G.EndWait(this);

                System.Windows.Forms.MessageBox.Show("Unable to create Excel file.", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }

            Process.GetProcesses().Where(p => p.ProcessName == "EXCEL" && p.MainWindowHandle.ToInt32() == 0).ToList().ForEach(process => process.Kill());

            ReleaseObject(xlWorkSheetVotes);
            ReleaseObject(xlWorkSheetReport);
            ReleaseObject(xlWorkBook);
            ReleaseObject(xlApp);

            Topmost = true;
            Topmost = false;

            G.EndWait(this);
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
            
            //var form = new PrintForm();
            //form.Opacity = 0;
            //form.ShowInTaskbar = false;
            //form.ShowDialog();
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

        private async void btnUpdateTag_Click(object sender, RoutedEventArgs e)
        {
            if (txtTag.Text.IsBlank())
            {
                MessageBox.Show("Please provide a tag for this machine's server.", "No Server Tag", MessageBoxButton.OK, MessageBoxImage.Error);
                txtTag.Focus();

                return;
            }

            
            try
            {
                G.WaitLang(this);

                var newTag = txtTag.Text.Trim();
                await _electionService.UpdateTagAsync(_currentElection.Id, newTag);

                lblTag.Content = newTag;
                lblTag.Visibility = Visibility.Visible;
                txtTag.Visibility = Visibility.Collapsed;
                stkTagButtons.Visibility = Visibility.Collapsed;

                await LoadElectionAsync();

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
            if (_currentElection.CandidatesFinalizedAt.HasValue)
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

            txtTag.Text = _currentElection.ServerTag;
            txtTag.SelectAll();
            txtTag.Focus();
        }

        private async void btnViewResult_Click(object sender, RoutedEventArgs e)
        {
            Opacity = 0.5;

            try
            {
                var votersCount = await _voterService.CountVotersAsync(_currentElection.Id);
                var votedCount = await _voterService.CountVotedVotersAsync(_currentElection.Id);
                var notVotedCount = votersCount - votedCount;

                //TODO: CHANGE TO CLOSE MESSAGE
                var result = MessageBox.Show($"Once viewed, ALL { "VOTER".ToQuantity(notVotedCount) }, who still not voted yet, will NOT allowed to vote anymore.\n\nAlso, you CANNOT ALLOW to CREATE, EDIT or DELETE voters.\n\n\nDo you want to view the results from this server?",
                    "Viewing Results", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.Yes)
                {
                    G.WaitLang(this);

                    await _electionService.CloseElectionAsync(_currentElection.Id);

                    await LoadElectionAsync();
                    await LoadResultsAsync();

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
        private async void lvVoter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
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

                await SortVoterAsync();

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

        //private void dgResults_SortColumnsChanged(object sender, Syncfusion.UI.Xaml.Grid.GridSortColumnsChangedEventArgs e)
        //{
        //}

        //private void dgResults_QueryRowHeight(object sender, Syncfusion.UI.Xaml.Grid.QueryRowHeightEventArgs e)
        //{
        //    var dg = (Syncfusion.UI.Xaml.Grid.SfDataGrid)sender;

        //    if (e.RowIndex != 0)
        //    {
        //        if (dg.GridColumnSizer.GetAutoRowHeight(e.RowIndex, new Syncfusion.UI.Xaml.Grid.GridRowSizingOptions { AutoFitMode = Syncfusion.UI.Xaml.Grid.AutoFitMode.SmartFit }, out double newHeight))
        //        {
        //            if (newHeight > 24)
        //            {
        //                e.Height = newHeight;
        //                e.Handled = true;
        //            }
        //        }
        //    }
        //}

        private void btnImportVoters_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = $"Import voters for { _currentElection.Title }",
                Filter = "Excel Files|*.xls;*.xlsx;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true,
                };

                _backgroundWorker.DoWork += BackgroundWorker_DoWork;
                _backgroundWorker.ProgressChanged += _backgroundWorker_ProgressChanged;
                _backgroundWorker.RunWorkerCompleted += _backgroundWorker_RunWorkerCompleted;
                _backgroundWorker.RunWorkerAsync(fileDialog.OpenFile());

                _progressWindow = new ProgressWindow();
                _progressWindow.btnCancel.Click += delegate
                {
                    _backgroundWorker.CancelAsync();
                };
                _progressWindow.progressBar.IsIndeterminate = true;
                
                G.WaitLang(this);
                this.Opacity = 0.5;

                _progressWindow.Owner = this;
                _progressWindow.ShowDialog();
            }
        }

        private async void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null || e.Result is Exception)
            {
                MessageBox.Show($"{ (e.Error ?? e.Result as Exception).GetBaseException().Message }\n\nNo voters imported",
                    "Import error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else if (e.Cancelled)
            {
                MessageBox.Show("Import cancelled.\n\nNo voters imported", "Import cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                await LoadVotersAsync();

                var count = Convert.ToInt32(e.Result);
                MessageBox.Show($"Successfully imported { "voter".ToQuantity(count) }", "Import successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _progressWindow.Close();

            G.EndWait(this);
            this.Opacity = 1;

            _backgroundWorker.Dispose();
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var count = Convert.ToInt32(e.UserState);

            if (e.ProgressPercentage == 0)
            {
                _progressWindow.tbkMessage.Text = $"Checking { "voter".ToQuantity(count) }...";
            }
            else
            {
                _progressWindow.tbkMessage.Text = $"Importing { "voter".ToQuantity(count) }...";
            }
        }

        private async void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stream = e.Argument as System.IO.Stream;
            var importedVoters = new List<VoterModel>();

            try
            {
                using (stream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        int count = 0;

                        reader.Read();
                        while (reader.Read())
                        {
                            count++;
                            _backgroundWorker.ReportProgress(0, count);

                            var newVoter = new VoterModel();
                            newVoter.ElectionId = _currentElection.Id;
                            newVoter.Vin = reader.GetString(0);
                            newVoter.FirstName = reader.GetString(1);
                            newVoter.MiddleName = reader.GetString(2);
                            newVoter.LastName = reader.GetString(3);
                            newVoter.Suffix = reader.GetString(4);
                            newVoter.Birthdate = reader.IsDBNull(5) ? default(DateTime?) : reader.GetDateTime(5);

                            var sexText = reader.GetString(6);
                            if (sexText.Equals("male", StringComparison.OrdinalIgnoreCase))
                            {
                                newVoter.Sex = Sex.Male;
                            }
                            else if (sexText.Equals("female", StringComparison.OrdinalIgnoreCase))
                            {
                                newVoter.Sex = Sex.Female;
                            }
                            else
                            {
                                throw new ArgumentException("Must be 'male' or 'female'", nameof(newVoter.Sex));
                            }

                            var yearLevelText = reader.GetValue(7);
                            if (int.TryParse(yearLevelText?.ToString(), out int yearLevel))
                            {
                                newVoter.YearLevel = yearLevel;
                            }
                            else
                            {
                                throw new ArgumentException("Invalid year level", nameof(newVoter.YearLevel));
                            }

                            newVoter.Section = reader.GetString(8);

                            await _voterService.ValidateAsync(_currentElection.Id, newVoter);

                            importedVoters.Add(newVoter);
                        }
                    }
                }

                _backgroundWorker.ReportProgress(100, importedVoters.Count);
                await _voterService.ImportVotersAsync(importedVoters);

            }
            catch (Exception ex)
            {
                e.Result = ex;
                return;
            }

            e.Result = importedVoters.Count;
        }

        private void lvVoter_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _wasMoved = e.LeftButton == MouseButtonState.Released;
        }
    }

    //public class VoteResult
    //{
    //    public ImageSource PictureSource { get; set; }
    //    public string Name { get; set; }
    //    public string Alias { get; set; }
    //    public string PositionName { get; set; }
    //    public int PositionOrder { get; set; }
    //    public int GradeLevel { get; set; }
    //    public string GradeStrand { get; set; }
    //    public string PartyTitle { get; set; }
    //    public SolidColorBrush PartyColorBrush { get; set; }
    //    public int VoteCount { get; set; }
    //}

    //public class CustomSorting : IComparer<Object>, ISortDirection
    //{
    //    private ListSortDirection _SortDirection;
    //    public ListSortDirection SortDirection
    //    {
    //        get { return _SortDirection; }
    //        set { _SortDirection = value; }
    //    }

    //    public int Compare(object x, object y)
    //    {
    //        int namX;
    //        int namY;
    //        if (x.GetType() == typeof(VoteResult))
    //        {
    //            namX = ((VoteResult)x).PositionOrder;
    //            namY = ((VoteResult)y).PositionOrder;
    //        }
    //        else if (x.GetType() == typeof(Group))
    //        {
    //            namX = ((Group)x).Key.ToString().Length;
    //            namY = ((Group)y).Key.ToString().Length;
    //        }
    //        else
    //        {
    //            namX = x.ToString().Length;
    //            namY = y.ToString().Length;
    //        }

    //        if (namX.CompareTo(namY) > 0)
    //            return SortDirection == ListSortDirection.Ascending ? 1 : -1;
    //        else if (namX.CompareTo(namY) == -1)
    //            return SortDirection == ListSortDirection.Ascending ? -1 : 1;
    //        else
    //            return 0;
    //    }
    //}
}