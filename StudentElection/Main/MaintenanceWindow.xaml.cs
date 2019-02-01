using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using static StudentElection.G;
using StudentElection.UserControls;
using StudentElection.Dialogs;
using StudentElection.Repository.Models;
using StudentElection.Services;
using Humanizer;
using ExcelDataReader;
using Syncfusion.XlsIO;
using Syncfusion.UI.Xaml.Grid.Converter;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Project.Library.Helpers;
using System.Text;

namespace StudentElection.Main
{
    /// <summary>
    /// Interaction logic for MaintenanceWindow.xaml
    /// </summary>
    public partial class MaintenanceWindow : Window
    {
        public UserModel User;

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
            //_selectedStaff = -1;
            _selectedVoter = -1;

            _isLogOut = false;

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

        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            grdMaintenance.Visibility = Visibility.Hidden;
            bdrLoadingMaintenance.Visibility = Visibility.Hidden;
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                if (await CheckElectionAsync())
                {
                    bdrLoadingMaintenance.Visibility = Visibility.Visible;

                    prgMaintenance.Text = "Loading current election...";
                    await LoadElectionAsync();

                    prgMaintenance.Text = "Loading candidates...";
                    await LoadCandidatesAsync();

                    prgMaintenance.Text = "Loading results...";
                    await LoadResultsAsync();
                    
                    prgMaintenance.Text = "Loading voters...";
                    await LoadVotersAsync();

                    prgMaintenance.Text = "Loading users...";
                    await LoadUsersAsync();

                    dgVoters.SearchHelper.AllowFiltering = true;

                    tbkUsername.Text = User.UserName;

                    //if (User.Type == Repository.Models.UserType.Admin)
                    //{
                    //    rdfStaff.Height = new GridLength(0, GridUnitType.Star);
                    //}

                    grdMaintenance.Visibility = Visibility.Visible;
                    bdrLoadingMaintenance.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        public async Task<bool> CheckElectionAsync()
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            while (_currentElection == null)
            {
                this.Opacity = 0.5;

                var form = new ElectionForm();

                var helper = new WindowInteropHelper(this);
                SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

                form.ShowDialog();

                this.Opacity = 1;

                if (!form.IsCancelled)
                {
                    _currentElection = await _electionService.GetCurrentElectionAsync();

                    if (_currentElection != null)
                    {
                        return true;
                    }
                }
                else
                {
                    MessageBox.Show("You cannot use this system without opening an election", "No elections", MessageBoxButton.OK, MessageBoxImage.Stop);
                    Application.Current.Shutdown();

                    return false;
                }
            }

            return true;
        }

        public async Task LoadElectionAsync()
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();
            
            tbkElectionTitle.Text = _currentElection.Title;
            this.Title = _currentElection.Title;

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

            btnAddVoter.Visibility = Visibility.Visible;
            btnImportVoters.Visibility = Visibility.Visible;

            if (_currentElection.CandidatesFinalizedAt.HasValue && !_currentElection.ClosedAt.HasValue)
            {
                bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(96, 224, 176));
                bdrMaintenance.BorderBrush = Brushes.MediumSeaGreen;
                bdrMaintenance.BorderThickness = new Thickness(4);
            }
            else if (_currentElection.ClosedAt.HasValue)
            {
                bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(224, 96, 96));
                bdrMaintenance.BorderThickness = new Thickness(4);

                btnAddVoter.Visibility = Visibility.Collapsed;
                btnImportVoters.Visibility = Visibility.Collapsed;
                btnEditVoter.Visibility = Visibility.Collapsed;
                btnDeleteVoter.Visibility = Visibility.Collapsed;
            }
            else
            {
                bdrMaintenance.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromRgb(176, 176, 176));
                bdrMaintenance.BorderThickness = new Thickness(4);
            }
        }

        public async Task CheckCandidatesAsync()
        {
            if (!_currentElection.CandidatesFinalizedAt.HasValue)
            {
                dckCandidateButtons.Visibility = Visibility.Visible;

                if (await _candidateService.GetCandidatesCount(_currentElection.Id) > 0)
                {
                    btnFinalize.Visibility = Visibility.Visible;
                }
                else
                {
                    btnFinalize.Visibility = Visibility.Collapsed;
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
                    item.btnImportCandidates.Visibility = Visibility.Collapsed;
                    item.tbkParty.Cursor = Cursors.Arrow;
                }

                lblTag.Cursor = Cursors.Arrow;
            }
        }

        private async Task CheckResultsAsync()
        {
            var ballotsCount = await _ballotService.CountBallotsAsync(_currentElection.Id);
            var votersCount = await _voterService.CountVotersAsync(_currentElection.Id);

            if (!_currentElection.ClosedAt.HasValue)
            {
                btnVoterButtons.Visibility = Visibility.Visible;

                if (ballotsCount > 0)
                {
                    lblResultsAvailable.Content = string.Format("Results are available ({0:n0} of {1:n0} voted)", ballotsCount, votersCount);
                    grdViewResult.Visibility = Visibility.Visible;
                    lblNoResults.Visibility = Visibility.Collapsed;
                    btnExportPrint.Visibility = Visibility.Collapsed;
                }
                else
                {
                    grdViewResult.Visibility = Visibility.Collapsed;
                    lblNoResults.Visibility = Visibility.Visible;
                    btnExportPrint.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                txtVoteTurnout.Text = $"{ ballotsCount } out of { votersCount } ({ ((double)ballotsCount / votersCount).ToString("p2") })";

                btnVoterButtons.Visibility = Visibility.Collapsed;

                grdViewResult.Visibility = Visibility.Collapsed;
                btnExportPrint.Visibility = Visibility.Visible;
            }
        }

        #region Staff
        
        //private int _selectedStaff;

        //private CollectionView cvStaff;

        //private bool StaffFilter(object obj)
        //{
        //    if (string.IsNullOrEmpty(txtStaffFilter.Text))
        //        return true;
        //    else
        //    {
        //        return ((obj as UserModel).LastName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
        //            ((obj as UserModel).FirstName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
        //            ((obj as UserModel).MiddleName.IndexOf(txtStaffFilter.Text, StringComparison.OrdinalIgnoreCase) == 0);
        //    }
        //}

        private async Task LoadUsersAsync()
        {
            //lvStaff.ItemsSource = await _userService.GetUsersAsync();

            //cvStaff = (CollectionView)CollectionViewSource.GetDefaultView(lvStaff.ItemsSource);
            //cvStaff.Filter = StaffFilter;
        }

        //private void lvStaff_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        //{
        //    var lv = sender as ListView;

        //    if (lv.SelectedItems.Count == 1)
        //    {
        //        btnEditStaff.Visibility = Visibility.Visible;
        //        btnDeleteStaff.Visibility = Visibility.Visible;

        //        btnDeleteStaff.Visibility = (User.Id != (lvStaff.SelectedValue as UserModel).Id) ? Visibility.Visible : Visibility.Collapsed;

        //        _selectedStaff = lv.SelectedIndex;
        //    }
        //    else
        //    {
        //        btnEditStaff.Visibility = Visibility.Collapsed;
        //        btnDeleteStaff.Visibility = Visibility.Collapsed;
        //    }
        //}

        //private async void btnAddStaff_Click(object sender, RoutedEventArgs e)
        //{
        //    Opacity = 0.5;
            
        //    G.WaitLang(this);

        //    var staffWindow = new StaffWindow();
        //    staffWindow.Owner = this;
            
        //    G.EndWait(this);

        //    staffWindow.ShowDialog();

        //    G.WaitLang(this);

        //    Opacity = 1;

        //    if (!staffWindow.IsCanceled)
        //    {
        //        try
        //        {
        //            await LoadUsersAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogError(ex);

        //            MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
        //        }
        //    }

        //    lvStaff.ScrollIntoView(lvStaff.Items[lvStaff.Items.Count - 1]);

        //    G.EndWait(this);
        //}

        //private async void btnEditStaff_Click(object sender, RoutedEventArgs e)
        //{
        //    G.EndWait(this);

        //    Opacity = 0.5;

        //    var staff = lvStaff.SelectedValue as UserModel;
            
        //    var staffWindow = new StaffWindow();
        //    staffWindow.User = staff;
        //    staffWindow.Owner = this;

        //    G.EndWait(this);

        //    staffWindow.ShowDialog();

        //    G.WaitLang(this);

        //    Opacity = 1;
        //    if (!staffWindow.IsCanceled)
        //    {
        //        try
        //        {
        //            await LoadUsersAsync();
        //        }
        //        catch (Exception ex)
        //        {
        //            Logger.LogError(ex);

        //            MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
        //        }
        //    }

        //    lvStaff.ScrollIntoView(lvStaff.Items[_selectedStaff]);
        //    lvStaff.SelectedIndex = _selectedStaff;

        //    G.EndWait(this);
        //}

        //private async void btnDeleteStaff_Click(object sender, RoutedEventArgs e)
        //{
        //    if (lvStaff.SelectedItems.Count == 1)
        //    {
        //        var user = (lvStaff.SelectedValue as UserModel);
        //        var result = MessageBox.Show(user.FullName + " will not be able to log in on this system. Do you want to continue?", "Deleting '" + user.UserName + "'", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);
        //        var selectedStaff = _selectedStaff == lvStaff.Items.Count - 1 ? _selectedStaff - 1 : _selectedStaff;

        //        if (result == MessageBoxResult.Yes)
        //        {
        //            try
        //            {
        //                G.WaitLang(this);

        //                await _userService.DeleteUserAsync(user);

        //                G.EndWait(this);
        //                MessageBox.Show("The user is deleted.", "Staff", MessageBoxButton.OK, MessageBoxImage.Information);

        //                G.WaitLang(this);
        //                await LoadUsersAsync();

        //                lvStaff.ScrollIntoView(lvStaff.Items[selectedStaff]);
        //                G.EndWait(this);

        //            }
        //            catch (Exception ex)
        //            {
        //                Logger.LogError(ex);

        //                G.EndWait(this);

        //                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
        //            }
        //        }
        //    }
        //}

        //private void txtStaffFilter_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    CollectionViewSource.GetDefaultView(lvStaff.ItemsSource).Refresh();
        //}

        #endregion


        #region Voter
        
        private int _selectedVoter;
        
        private bool FilterVotedVoters(object obj)
        {
            if (obj is VoterModel item)
            {
                return item.IsVoted;
            }

            return false;
        }
        
        public async Task LoadVotersAsync()
        {
            try
            {
                var voters = await _voterService.GetVoterDetailsListAsync(_currentElection.Id);

                dgVoters.ItemsSource = voters.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                    var newItem = dgVoters.View.Records
                        .Select(r => r.Data)
                        .Cast<VoterModel>()
                        .OrderBy(v => v.Id)
                        .First(x => x.Id == voterWindow.Voter.Id);
                    dgVoters.SelectedIndex = dgVoters.ResolveToRowIndex(newItem);
                }
                else
                {
                    var newItem = dgVoters.View.Records
                        .Select(r => r.Data)
                        .Cast<VoterModel>()
                        .OrderBy(v => v.Id)
                        .Last();
                    dgVoters.SelectedIndex = dgVoters.ResolveToRowIndex(newItem);
                }

                txtVoterFilter.Text = searched;
            }

            G.EndWait(this);
        }

        private void dgVoters_SelectionChanged(object sender, GridSelectionChangedEventArgs e)
        {
            if (dgVoters.SelectedItems.Count == 1)
            {
                if (_currentElection.ClosedAt.HasValue)
                {
                    return;
                }

                btnEditVoter.Visibility = Visibility.Visible;
                btnDeleteVoter.Visibility = Visibility.Visible;

                _selectedVoter = dgVoters.SelectedIndex;
            }
            else
            {
                btnEditVoter.Visibility = Visibility.Collapsed;
                btnDeleteVoter.Visibility = Visibility.Collapsed;
            }
        }

        private void txtVoterFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (txtVoterFilter.Text.IsBlank())
            {
                dgVoters.SearchHelper.ClearSearch();
            }
            else
            {
                dgVoters.SearchHelper.Search(txtVoterFilter.Text);
            }
        }

        private async void btnEditVoter_Click(object sender, RoutedEventArgs e)
        {
            if (dgVoters.SelectedItems.Count == 1)
            {
                var voter = dgVoters.SelectedItem as VoterModel;
                var selectedVoter = dgVoters.SelectedIndex;

                if (_currentElection.CandidatesFinalizedAt.HasValue)
                {
                    if (voter.IsVoted)
                    {
                        G.EndWait(this);

                        MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Already voted", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
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
                    try
                    {
                        await LoadVotersAsync();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);

                        G.EndWait(this);

                        MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
                    }

                    if (dgVoters.View.Records.Any() && voterWindow.Voter != null)
                    {
                        var rowIndex = dgVoters.View.Records
                            .Select(r => r.Data)
                            .Cast<VoterModel>()
                            .ToList()
                            .FindIndex(r => r.Id == voterWindow.Voter.Id);
                        dgVoters.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(rowIndex + 1, 1));
                        dgVoters.SelectedIndex = rowIndex;
                    }
                }

                G.EndWait(this);
            }
        }
        
        private async void btnDeleteVoter_Click(object sender, RoutedEventArgs e)
        {
            if (dgVoters.SelectedItems.Count == 1)
            {
                G.WaitLang(this);

                var voter = dgVoters.SelectedItem as VoterModel;
                var selectedIndex = dgVoters.SelectedIndex;

                selectedIndex = selectedIndex == dgVoters.View.Records.Count - 1 ? selectedIndex - 1 : selectedIndex;

                try
                {
                    if (voter.IsVoted)
                    {
                        G.EndWait(this);
                        MessageBox.Show(voter.FullName + " is already voted. You cannot allow to edit or delete their info.", "Already voted", MessageBoxButton.OK, MessageBoxImage.Error);

                        return;
                    }

                    dgVoters.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex + 1, 1));

                    G.EndWait(this);
                    var result = MessageBox.Show("Delete " + voter.FullName + "?", "Delete voter", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                    if (result == MessageBoxResult.Yes)
                    {
                        G.WaitLang(this);

                        await _voterService.DeleteVoterAsync(voter);
                        
                        await LoadVotersAsync();

                        if (selectedIndex >= 0 && selectedIndex < dgVoters.View.Records.Count)
                        {
                            dgVoters.ScrollInView(new Syncfusion.UI.Xaml.ScrollAxis.RowColumnIndex(selectedIndex + 1, 1));
                            dgVoters.SelectedIndex = selectedIndex;
                        }

                        G.EndWait(this);

                        MessageBox.Show("The voter is deleted.", "Voter", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    G.EndWait(this);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                var result = MessageBox.Show("You will log out if you close this window.\n\nDo you want to log out?",
                    "Log out", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

                if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                    return;
                }
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
            lblPosition.Text = string.Format("Positions ({0})", await _positionService.GetPositionsCountAsync(_currentElection.Id));

            var currScroll = svrCandidates.VerticalOffset;
            foreach (PartyItemControl control in stkCandidates.Children)
            {
                CandidateHOffsets.Add(control.svrCandidate.HorizontalOffset);
            }
            
            try
            {
                lblNoCandidates.Visibility = await _partyService.GetPartiesCount(_currentElection.Id) == 0
                    ? Visibility.Visible : Visibility.Hidden;

                stkCandidates.Children.Clear();

                var partyIndex = 0;
                var parties = await _partyService.GetPartiesAsync(_currentElection.Id);
                foreach (var party in parties)
                {
                    var item = new PartyItemControl();
                    item.DataContext = party;
                    
                    stkCandidates.Children.Add(item);

                    var candidateParty = await _candidateService.GetCandidatesByPartyAsync(party.Id);

                    item.stkCandidate.Visibility = candidateParty.Any() ? Visibility.Visible : Visibility.Collapsed;

                    item.stkCandidate.Children.Clear();
                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        candidateControl.DataContext = candidate;
                        item.stkCandidate.Children.Add(candidateControl);
                    }

                    item.lblCount.Content = item.stkCandidate.Children.Count + "";

                    if (CandidateHOffsets.Count > partyIndex)
                    {
                        item.svrCandidate.ScrollToHorizontalOffset(CandidateHOffsets[partyIndex]);
                    }

                    partyIndex++;
                }

                await CheckCandidatesAsync();

                svrCandidates.ScrollToVerticalOffset(currScroll);
                G.CandidateHOffsets = new List<double>();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }
        
        private async void btnFinalize_Click(object sender, RoutedEventArgs e)
        {
            if (_currentElection.ServerTag.IsBlank())
            {
                MessageBox.Show("Please provide a tag for this machine's server. Make sure that the tag is unique among other servers.", "Provide a tag", MessageBoxButton.OK, MessageBoxImage.Error);

                return;
            }

            Opacity = 0.5;

            var candidatesCount = await _candidateService.GetCandidatesCount(_currentElection.Id);
            var result = MessageBox.Show($"Once finalized, you WILL NO LONGER...\n" +
                $"- edit current election info\n" +
                $"- create, edit or delete a party\n" +
                $"- add, edit or delete a position\n" +
                $"- add, edit or delete a candidate\n" +
                $"- edit server tag" +
                $"\n" +
                $"but you CAN STILL:\n" +
                $"- create, edit or delete voters who did not vote yet.\n\n" +
                $"There are { "candidate".ToQuantity(candidatesCount) }.\n\n" + 
                $"Do you want to finalize the candidates?", "Finalize candidates", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    await _electionService.FinalizeCandidatesAsync(_currentElection.Id);

                    G.EndWait(this);

                    MessageBox.Show("The candidates have been finalized.", "Candidates finalized", MessageBoxButton.OK, MessageBoxImage.Information);

                    G.WaitLang(this);

                    await LoadElectionAsync();
                    await LoadVotersAsync();
                    await LoadCandidatesAsync();

                    G.EndWait(this);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    G.EndWait(this);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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

                        if (voteResultsPosition.All(v => v.VoteCount == 0))
                        {
                            control.tbkRank.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            control.tbkRank.Text = string.Format("#{0:n0}", rank);
                        }

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

                        double quotient = voteResult.VoteCount / (double)voteResult.PositionVoteCount;

                        if (!double.IsNaN(quotient))
                        {
                            control.recCandidate.Height = 120 * quotient;
                        }
                        else
                        {
                            control.recCandidate.Height = 0;
                        }

                        item.wrpCandidate.Children.Add(control);
                    }
                }

                cmbPositions.SelectedIndex = 0;

                cmbPositions.Focusable = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void cmbPositions_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
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
            var voteResults = await _ballotService.GetVoteResultsAsync(_currentElection.Id);
            dgResults.ItemsSource = voteResults.ToList();

            var dateNow = DateTime.Now;

            var options = new ExcelExportingOptions
            {
                ExportMode = ExportMode.Value,
                ExcelVersion = ExcelVersion.Excel2007
            };

            var saveFileDialog = new SaveFileDialog
            {
                FileName = $"{ _currentElection.Title } Result - { _currentElection.ServerTag } - { dateNow.ToString("yyyy-MM-dd HHmmss") }.xls"
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

                        //resultWorksheet.Columns[1].Group(ExcelGroupBy.ByColumns);

                        resultWorksheet.UsedRange.AutofitColumns();
                        resultWorksheet.UsedRange.AutofitRows();

                        var infoWorksheet = excelEngine.Excel.Worksheets.AddCopyBefore(resultWorksheet);
                        infoWorksheet.Clear();

                        infoWorksheet.Name = "REPORT INFO";

                        infoWorksheet.Range[1, 1].Text = "SERVER TAG";
                        infoWorksheet.Range[1, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[1, 2].Text = _currentElection.ServerTag;

                        infoWorksheet.Range[2, 1].Text = "SERVER NAME";
                        infoWorksheet.Range[2, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[2, 2].Text = Environment.MachineName;

                        infoWorksheet.Range[3, 1].Text = "GENERATED BY";
                        infoWorksheet.Range[3, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[3, 2].Text = User.UserName;
                        infoWorksheet.Range[4, 2].Text = User.FullName;

                        infoWorksheet.Range[5, 1].Text = "GENERATED AT";
                        infoWorksheet.Range[5, 1].HorizontalAlignment = ExcelHAlign.HAlignRight;
                        infoWorksheet.Range[5, 2].Text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dateNow);

                        infoWorksheet.Range[1, 1, 5, 1].CellStyle.Font.Bold = true;

                        var range = infoWorksheet.Range[1, 1, 5, 2];
                        range.CellStyle.Font.FontName = "Tahoma";
                        range.AutofitColumns();

                        workBook.ActiveSheetIndex = 1;
                        workBook.Worksheets.Remove(workBook.Worksheets[2]);
                        workBook.Worksheets.Remove(workBook.Worksheets[2]);

                        var resultWorksheetSecretCode = CryptographyHelper.GenerateCode(true, 16);
                        var infoWorksheetSecretCode = CryptographyHelper.GenerateCode(true, 16);
                        var workBookSecretCode = CryptographyHelper.GenerateCode(true, 16);

                        resultWorksheet.Protect(resultWorksheetSecretCode, ExcelSheetProtection.All);
                        infoWorksheet.Protect(infoWorksheetSecretCode, ExcelSheetProtection.All);
                        workBook.Protect(true, true, workBookSecretCode);

                        workBook.SaveAs(saveFileDialog.FileName);
                    }

                    G.EndWait(this);

                    System.Windows.Forms.MessageBox.Show("Excel file created!", "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                    Process.Start(System.IO.Path.GetDirectoryName(saveFileDialog.FileName));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    G.EndWait(this);

                    System.Windows.Forms.MessageBox.Show("Unable to create Excel file.\n" + ex.GetBaseException().Message, "Excel Report", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                }
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
            grdTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
            bdrMaintenance.BorderBrush = new SolidColorBrush(Colors.Gray);
            lblMachineTag.Foreground = new SolidColorBrush(Colors.Black);


            tbkElectionTitle.IsEnabled = true;
            btnSettings.IsEnabled = true;
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
                MessageBox.Show("Please provide a tag for this machine's server.", "No tag", MessageBoxButton.OK, MessageBoxImage.Error);
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

                tbkElectionTitle.IsEnabled = true;
                btnSettings.IsEnabled = true;
                stkUsername.IsEnabled = true;
                tbcMaintenance.IsEnabled = true;
                tbcMaintenance.Opacity = 1;

                bdrMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                tbcMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 255, 255, 255));
                border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
                grdTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(245, 245, 245, 245));
                bdrMaintenance.BorderBrush = new SolidColorBrush(Colors.Gray);
                lblMachineTag.Foreground = new SolidColorBrush(Colors.Black);

                txtTag.Focus();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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

            tbkElectionTitle.IsEnabled = false;
            btnSettings.IsEnabled = false;
            stkUsername.IsEnabled = false;
            tbcMaintenance.IsEnabled = false;
            tbcMaintenance.Opacity = 0.5;

            bdrMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            tbcMaintenance.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            border.BorderBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
            grdTitleBar.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 96, 96, 96));
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
                if (_currentElection.TookPlaceOn > DateTime.Today)
                {
                    MessageBox.Show("Cannot close a future election", "Future election", MessageBoxButton.OK, MessageBoxImage.Error);
                    Opacity = 1;

                    return;
                }

                var votersCount = await _voterService.CountVotersAsync(_currentElection.Id);
                var votedCount = await _voterService.CountVotedVotersAsync(_currentElection.Id);
                var notVotedCount = votersCount - votedCount;

                var message = string.Empty;

                if (notVotedCount > 0)
                {
                    message = $"You can view the result from the server '{ _currentElection.ServerTag }' once the { _currentElection.Title } is closed.\n\n" +
                        $"All { "voter".ToQuantity(notVotedCount) } who have not voted yet CANNOT BE ALLOWED to vote anymore.\n\n" +
                        $"Also, you WILL NO LONGER add, edit or delete voters.\n\n" +
                        $"Do you want to close the election and view the results?";
                }
                else
                {
                    message = $"You can view the result from the server '{ _currentElection.ServerTag }' once the { _currentElection.Title } is closed.\n\n" +
                           $"All voters have casted their votes.\n\n" +
                           $"You WILL NO LONGER add, edit or delete voters once you close the election.\n\n" +
                           $"Do you want to close the election and view the results?";
                }

                var result = MessageBox.Show(message, "Close election", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No);

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
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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
        
        private void border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if ((e.Source as Grid) != null || (e.Source as DockPanel) != null ||
                ((e.Source as Label) != null && ((e.Source as Label).Cursor == Cursors.Arrow)))
                DragMove();
        }

        private void txtTag_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = e.Key == Key.Space;
        }

        private void btnImportVoters_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = $"Import voters for { _currentElection.Title }",
                Filter = "Excel Files|*.xls;*.xlsx;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                try
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
                    _progressWindow.progressBar.IsIndeterminate = false;

                    G.WaitLang(this);
                    this.Opacity = 0.5;

                    _progressWindow.Owner = this;
                    _progressWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show($"{ ex.GetBaseException().Message }\n\nNo voters imported",
                        "Import error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }

        private async void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled || e.Error is OperationCanceledException)
            {
                MessageBox.Show("Import cancelled.\n\nNo voters imported", "Import cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Error != null || e.Result is Exception)
            {
                var exception = e.Error ?? e.Result as Exception;
                MessageBox.Show($"{ exception.GetBaseException().Message } \n\n { "No voters imported" }", "Import error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                await LoadVotersAsync();
                await LoadResultsAsync();

                var tuple = e.Result as Tuple<int, int>;
                var messageBuilder = new StringBuilder($"Successfully imported { "voter".ToQuantity(tuple.Item1) }");
                if (tuple.Item2 > 0)
                {
                    messageBuilder.Append($" ({ "blank row".ToQuantity(tuple.Item2) })");
                }

                MessageBox.Show(messageBuilder.ToString(), "Import successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _progressWindow.Close();

            G.EndWait(this);
            this.Opacity = 1;

            _backgroundWorker.Dispose();
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                var count = Convert.ToInt32(e.UserState);

                _progressWindow.progressBar.IsIndeterminate = true;
                _progressWindow.btnCancel.IsEnabled = false;
                _progressWindow.tbkMessage.Text = $"Importing { "voter".ToQuantity(count, "n0") }...";
            }
            else
            {
                var tuple = e.UserState as Tuple<int, int, int>;

                _progressWindow.btnCancel.IsEnabled = true;
                _progressWindow.progressBar.Value = e.ProgressPercentage;
                _progressWindow.tbkMessage.Text = $"Checking { tuple.Item1.ToString("n0") } of { "voter".ToQuantity(tuple.Item2, "n0") } ...";
                _progressWindow.tbkSubMessage.Text = $"{ "blank row".ToQuantity(tuple.Item3, "n0") }";
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stream = e.Argument as System.IO.Stream;
            var importedVoters = new List<VoterModel>();

            int count = 0;
            int blankCount = 0;

            try
            {
                using (stream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        count = 0;
                        blankCount = 0;

                        reader.Read();
                        while (reader.Read())
                        {
                            if (_backgroundWorker.CancellationPending)
                            {
                                e.Cancel = true;
                                return;
                            }

                            var percentage = count / (reader.RowCount - 1d) * 100;

                            count++;
                            _backgroundWorker.ReportProgress((int)(Math.Floor(percentage)), new Tuple<int, int, int>(count, reader.RowCount, blankCount));

                            var row = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var cellText = Convert.ToString(reader.GetValue(i) ?? string.Empty);
                                row.Add(cellText);
                            }

                            if (row.All(r => r.IsBlank()))
                            {
                                blankCount++;
                                continue;
                            }

                            var newVoter = new VoterModel();
                            newVoter.ElectionId = _currentElection.Id;
                            newVoter.Vin = Convert.ToString(reader.GetValue(0) ?? string.Empty);
                            newVoter.FirstName = Convert.ToString(reader.GetValue(1) ?? string.Empty);
                            newVoter.MiddleName = Convert.ToString(reader.GetValue(2) ?? string.Empty);
                            newVoter.LastName = Convert.ToString(reader.GetValue(3) ?? string.Empty);
                            newVoter.Suffix = Convert.ToString(reader.GetValue(4) ?? string.Empty);
                            if (!reader.IsDBNull(5))
                            {
                                if (DateTime.TryParse(reader.GetString(5), result: out var dateTime))
                                {
                                    newVoter.Birthdate = dateTime;
                                }
                            }

                            var sexText = Convert.ToString(reader.GetValue(6));
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
                                e.Result = new ArgumentException($"'{ sexText }' is invalid\nMust be 'male' or 'female'\n\nRow number: { count + 1 }", nameof(newVoter.Sex));
                                return;
                            }

                            var yearLevelText = Convert.ToString(reader.GetValue(7) ?? string.Empty);
                            if (int.TryParse(yearLevelText, out int yearLevel))
                            {
                                newVoter.YearLevel = yearLevel;
                            }
                            else
                            {
                                e.Result = new ArgumentException($"Invalid year level '{ yearLevelText }'\nMust be a number and between 1 and 12\n\nRow number: { count + 1 }", nameof(newVoter.YearLevel));
                                return;
                            }

                            newVoter.Section = Convert.ToString(reader.GetValue(8) ?? string.Empty);

                            _voterService.ValidateAsync(_currentElection.Id, newVoter).Wait();

                            importedVoters.Add(newVoter);
                        }
                    }
                }

                _backgroundWorker.ReportProgress(100, importedVoters.Count);

                _voterService.ImportVotersAsync(importedVoters).Wait();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                e.Result = new Exception($"{ ex.GetBaseException().Message }\n\nRow number: { count + 1 }"); ;
                return;
            }

            e.Result = new Tuple<int, int>(importedVoters.Count, blankCount);
        }

        private void chkVoted_Click(object sender, RoutedEventArgs e)
        {
            CheckVoterFilters();
        }

        private void CheckVoterFilters()
        {
            if (chkVoted.IsChecked == true)
            {
                dgVoters.Columns["IsVoted"].FilterPredicates.Add(new FilterPredicate
                {
                    FilterType = FilterType.Equals,
                    FilterValue = true
                });
            }
            else
            {
                dgVoters.Columns["IsVoted"].FilterPredicates.Clear();
            }

            dgVoters.View.RefreshFilter();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            Opacity = 0.5;

            G.WaitLang(this);

            var form = new SettingsForm();

            var helper = new WindowInteropHelper(this);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(this);

            form.Tag = this;
            form.ShowDialog();

            Opacity = 1;
        }
    }
}