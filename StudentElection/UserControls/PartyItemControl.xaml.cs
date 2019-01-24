//using StudentElection.Classes;
using StudentElection.Dialogs;
using StudentElection.Main;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using StudentElection.Repository.Models;
using StudentElection.Services;
using Project.Library.Helpers;
using System.Drawing;
using ExcelDataReader;
using Humanizer;
using System.ComponentModel;

namespace StudentElection.UserControls
{
    /// <summary>
    /// Interaction logic for PositionControl.xaml
    /// </summary>
    public partial class PartyItemControl : UserControl
    {
        public bool AreCandidatesFinalized = false;

        private double _hOffset = -1;

        private readonly CandidateService _candidateService = new CandidateService();
        private readonly PositionService _positionService = new PositionService();

        private PartyModel _party;

        private BackgroundWorker _backgroundWorker;
        private ProgressWindow _progressWindow;
        private MaintenanceWindow _maintenanceWindow;

        public PartyItemControl()
        {
            InitializeComponent();

            tbkParty.DataContextChanged += (s, ev) =>
            {
                tbkParty.Width = double.NaN;
            };
        }
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        private async void btnAddCandidate_Click(object sender, RoutedEventArgs e)
        {
            btnAddCandidate.Cursor = Cursors.Wait;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            G.WaitLang(window);

            window.Opacity = 0.5;

            _hOffset = svrCandidate.HorizontalOffset;
            var candidateForm = new CandidateForm();
            candidateForm.Candidate = null;
            candidateForm.SetParty(_party);

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(candidateForm, candidateForm.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(window);

            btnAddCandidate.Cursor = Cursors.Hand;

            candidateForm.ShowDialog();
            G.WaitLang(window);

            if (candidateForm.IsLoadPosition)
            {
                G.WaitLang(window);

                var form = new PositionForm();
                form.Tag = window;

                G.EndWait(window);
                form.ShowDialog();
            }
            else if (!candidateForm.IsCanceled)
            {
                try
                {
                    stkCandidate.Children.Clear();
                    
                    var candidateParty = await _candidateService.GetCandidatesByPartyAsync(_party.Id);

                    if (candidateParty.Count() > 0)
                        stkCandidate.Visibility = Visibility.Visible;
                    else
                        stkCandidate.Visibility = Visibility.Hidden;
                    

                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        candidateControl.DataContext = candidate;
                        stkCandidate.Children.Add(candidateControl);
                    }

                    await window.LoadVotersAsync();

                    lblCount.Content = stkCandidate.Children.Count + "";

                    if (_hOffset != -1)
                    {
                        svrCandidate.ScrollToHorizontalOffset(_hOffset);
                    }
                }
                catch (Exception ex)
                {
                    G.EndWait(window);

                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
            }
            
            G.EndWait(window);
            window.Opacity = 1;
        }

        private async void tbkParty_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AreCandidatesFinalized)
            {
                return;
            }

            var tbk = sender as TextBlock;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            G.WaitLang(window);
            Cursor = Cursors.Wait;

            window.Opacity = 0.5;
            
            var form = new PartyForm();
            form.Party = _party;

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(window);
            Cursor = Cursors.Arrow;

            form.ShowDialog();

            G.WaitLang(window);

            var party = await form.GetNewDataAsync();

            if (!form.IsDeleted && party != null) 
            {
                DataContext = party;
                foreach (CandidateControl control in stkCandidate.Children)
                {
                    var candidate = control.DataContext as CandidateModel;
                    candidate.Party = party;

                    //var candidate = new CandidateModel
                    //{
                    //    Alias = temp.Alias,
                    //    Party = party,
                    //    //TODO: PictureByte = temp.PictureByte,
                    //    Position = temp.Position,
                    //    //TODO: PositionVoteCount = temp.PositionVoteCount,
                    //    //TODO: VoteCount = temp.VoteCount
                    //};
                    control.DataContext = candidate;
                }
                await window.LoadVotersAsync();
            }
            else
            {
                await window.LoadVotersAsync();
                await window.LoadCandidatesAsync();
            }
            
            window.Opacity = 1;

            G.EndWait(window);
        }

        private void svrCandidate_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void tbkParty_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void ResizePartyTextBlock()
        {
            if (tbkParty.ActualWidth == 0) return;

            var partyWidth = ActualWidth - 48 - lblCount.ActualWidth - btnAddCandidate.ActualWidth - btnAddCandidate.Margin.Left - btnAddCandidate.Margin.Right - btnAddCandidate.Padding.Left - btnAddCandidate.Padding.Right;

            if (tbkParty.ActualWidth > partyWidth * 1.5)
            {
                tbkParty.Width = partyWidth * 1.5;
                tbkParty.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkParty.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkParty.Text
                };
                ToolTipService.SetShowDuration(tbkParty, int.MaxValue);
            }

            if (tbkParty.ActualWidth > partyWidth)
            {
                vbParty.Width = partyWidth;
                vbParty.Stretch = Stretch.Fill;
            }
            else
            {
                vbParty.Stretch = Stretch.Uniform;
                tbkParty.Width = double.NaN;
                tbkParty.ToolTip = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            _party = DataContext as PartyModel;

            ResizePartyTextBlock();
        }

        private void btnImportCandidates_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Title = $"Import candidates from { _party.Title.Titleize() } ({ _party.ShortName })",
                Filter = "Excel Files|*.xls;*.xlsx;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
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

                var parentWindow = Window.GetWindow(this);
                _maintenanceWindow = parentWindow as MaintenanceWindow;
                
                G.WaitLang(_maintenanceWindow);
                _maintenanceWindow.Opacity = 0.5;

                _progressWindow.Owner = _maintenanceWindow;
                _progressWindow.ShowDialog();
            }
        }

        private async void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressWindow.Close();

            G.EndWait(_maintenanceWindow);
            _maintenanceWindow.Opacity = 1;

            if (e.Cancelled)
            {
                MessageBox.Show("Import cancelled.\n\nNo candidates imported", "Import cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Error != null)
            {
                MessageBox.Show($"{ e.Error.GetBaseException().Message } \n\n { "No candidates imported" }", "Import error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                await _maintenanceWindow.LoadCandidatesAsync();

                var count = Convert.ToInt32(e.Result);
                MessageBox.Show($"Successfully imported { "candidate".ToQuantity(count) }", "Import successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _backgroundWorker.Dispose();
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            var count = Convert.ToInt32(e.UserState);

            if (e.ProgressPercentage == 0)
            {
                _progressWindow.tbkMessage.Text = $"Checking { "candidate".ToQuantity(count) }...";
            }
            else
            {
                _progressWindow.tbkMessage.Text = $"Importing { "candidate".ToQuantity(count) }...";
            }
        }

        private async void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stream = e.Argument as System.IO.Stream;
            var importedCandidates = new List<CandidateModel>();

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

                        var newCandidate = new CandidateModel();
                        newCandidate.PartyId = _party.Id;
                        newCandidate.FirstName = reader.GetString(0);
                        newCandidate.MiddleName = reader.GetString(1);
                        newCandidate.LastName = reader.GetString(2);
                        newCandidate.Suffix = reader.GetString(3);
                        newCandidate.Birthdate = reader.IsDBNull(4) ? default(DateTime?) : reader.GetDateTime(4);

                        var sexText = reader.GetString(5);
                        if (sexText.Equals("male", StringComparison.OrdinalIgnoreCase))
                        {
                            newCandidate.Sex = Sex.Male;
                        }
                        else if (sexText.Equals("female", StringComparison.OrdinalIgnoreCase))
                        {
                            newCandidate.Sex = Sex.Female;
                        }
                        else
                        {
                            throw new ArgumentException("Must be 'male' or 'female'", nameof(newCandidate.Sex));
                        }

                        var yearLevelText = reader.GetValue(6);
                        if (int.TryParse(yearLevelText?.ToString(), out int yearLevel))
                        {
                            newCandidate.YearLevel = yearLevel;
                        }
                        else
                        {
                            throw new ArgumentException("Invalid year level", nameof(newCandidate.YearLevel));
                        }

                        newCandidate.Section = reader.GetString(7);
                        newCandidate.Alias = reader.GetString(8);
                        newCandidate.PictureFileName = reader.GetString(9);

                        var positionTitle = reader.GetString(10);
                        var position = await _positionService.GetPositionByTitleAsync(_party.ElectionId, positionTitle);
                        if (position == null)
                        {
                            throw new ArgumentException($"Position '{ positionTitle }' does not exist", nameof(newCandidate.Position));
                        }

                        newCandidate.PositionId = position.Id;

                        await _candidateService.ValidateAsync(_party.ElectionId, newCandidate);

                        importedCandidates.Add(newCandidate);
                    }
                }
            }

            _backgroundWorker.ReportProgress(100, importedCandidates.Count);
            await _candidateService.ImportCandidatesAsync(importedCandidates);

            e.Result = importedCandidates.Count;
        }
    }
}
