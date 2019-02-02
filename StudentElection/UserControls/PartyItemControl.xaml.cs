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
            candidateForm.EditingCandidate = null;
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
                    
                    var candidateParty = await _candidateService.GetCandidateDetailsListByPartyAsync(_party.Id);

                    stkCandidate.Visibility = candidateParty.Any() ? Visibility.Visible : Visibility.Collapsed;

                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        candidateControl.DataContext = candidate;
                        stkCandidate.Children.Add(candidateControl);
                    }

                    lblCount.Content = candidateParty.Count().ToString("n0");

                    if (_hOffset != -1)
                    {
                        svrCandidate.ScrollToHorizontalOffset(_hOffset);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    G.EndWait(window);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
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
                    control.DataContext = candidate;
                }
            }
            else
            {
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

            var partyWidth = ActualWidth - 48 - btnAddCandidate.ActualWidth - btnAddCandidate.Margin.Left - btnAddCandidate.Margin.Right - btnAddCandidate.Padding.Left - btnAddCandidate.Padding.Right;

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
                Title = $"Import candidates for { _party.Title.Titleize() } ({ _party.ShortName })",
                Filter = "Excel Files|*.xls;*.xlsx;"
            };

            if (fileDialog.ShowDialog() == true)
            {
                try
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
                    _progressWindow.progressBar.IsIndeterminate = false;

                    var parentWindow = Window.GetWindow(this);
                    _maintenanceWindow = parentWindow as MaintenanceWindow;

                    G.WaitLang(_maintenanceWindow);
                    _maintenanceWindow.Opacity = 0.5;

                    _progressWindow.Closed += delegate
                    {
                        G.EndWait(_maintenanceWindow);

                        _maintenanceWindow.Opacity = 1;
                    };

                    _progressWindow.Owner = _maintenanceWindow;
                    _progressWindow.ShowDialog();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
                }
            }
        }
        
        private async void _backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _progressWindow.Close();
            
            if (e.Cancelled || e.Error is OperationCanceledException)
            {
                MessageBox.Show("Import cancelled.\n\nNo candidates imported", "Import cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else if (e.Error != null || e.Result is Exception)
            {
                var exception = e.Error ?? e.Result as Exception;
                MessageBox.Show($"{ exception.GetBaseException().Message } \n\n { "No candidates imported" }", "Import error", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            else
            {
                await _maintenanceWindow.LoadCandidatesAsync();

                var count = e.Result as Tuple<int, int>;
                var messageBuilder = new StringBuilder($"Successfully imported { "voter".ToQuantity(count.Item1) }");
                if (count.Item2 > 0)
                {
                    messageBuilder.Append($" ({ "blank row".ToQuantity(count.Item2) })");
                }

                MessageBox.Show(messageBuilder.ToString(), "Import successful", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            _backgroundWorker.Dispose();
        }

        private void _backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                var count = Convert.ToInt32(e.UserState);

                _progressWindow.btnCancel.IsEnabled = true;
                _progressWindow.progressBar.IsIndeterminate = true;
                _progressWindow.tbkMessage.Text = $"Importing { "candidate".ToQuantity(count, "n0") }...";
            }
            else
            {
                var tuple = e.UserState as Tuple<int, int, int>;

                _progressWindow.btnCancel.IsEnabled = false;
                _progressWindow.progressBar.Value = e.ProgressPercentage;
                _progressWindow.tbkMessage.Text = $"Checking { tuple.Item1.ToString("n0") } of { "candidate".ToQuantity(tuple.Item2, "n0") } ...";
                _progressWindow.tbkSubMessage.Text = $"{ "blank row".ToQuantity(tuple.Item3, "n0") }";
            }
        }

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var stream = e.Argument as System.IO.Stream;
            var importedCandidates = new List<CandidateModel>();

            int count = 0;
            int blankCount = 0;

            try
            {
                using (stream)
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
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
                            _backgroundWorker.ReportProgress((int)Math.Floor(percentage), new Tuple<int, int, int>(count, reader.RowCount, blankCount));

                            var row = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                var cellText = Convert.ToString(reader.GetValue(i) ?? string.Empty);
                                row.Add(cellText);
                            }

                            if (row.All(r => string.IsNullOrWhiteSpace(r)))
                            {
                                blankCount++;
                                continue;
                            }

                            var newCandidate = new CandidateModel();
                            newCandidate.PartyId = _party.Id;
                            newCandidate.FirstName = Convert.ToString(reader.GetValue(0) ?? string.Empty);
                            newCandidate.MiddleName = Convert.ToString(reader.GetValue(1) ?? string.Empty);
                            newCandidate.LastName = Convert.ToString(reader.GetValue(2) ?? string.Empty);
                            newCandidate.Suffix = Convert.ToString(reader.GetValue(3) ?? string.Empty);
                            if (!reader.IsDBNull(4))
                            {
                                if (DateTime.TryParse(reader.GetString(4), result: out var dateTime))
                                {
                                    newCandidate.Birthdate = dateTime;
                                }
                            }

                            var sexText = Convert.ToString(reader.GetString(5) ?? string.Empty);
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
                                e.Result = new ArgumentException($"'{ sexText }' is invalid\nMust be 'male' or 'female'\n\nRow number: { count + 1 }", nameof(newCandidate.Sex));
                                return;
                            }

                            var yearLevelText = Convert.ToString(reader.GetValue(6) ?? string.Empty);
                            if (int.TryParse(yearLevelText?.ToString(), out int yearLevel))
                            {
                                newCandidate.YearLevel = yearLevel;
                            }
                            else
                            {
                                e.Result = new ArgumentException($"Invalid year level '{ yearLevelText }'\nMust be a number and between 1 and 12\n\nRow number: { count + 1 }", nameof(newCandidate.YearLevel));
                                return;
                            }

                            newCandidate.Section = Convert.ToString(reader.GetString(7) ?? string.Empty);
                            newCandidate.Alias = Convert.ToString(reader.GetString(8) ?? string.Empty);
                            newCandidate.PictureFileName = Convert.ToString(reader.GetString(9) ?? string.Empty);

                            var positionTitle = Convert.ToString(reader.GetString(10) ?? string.Empty);
                            PositionModel position = null;
                            _positionService.GetPositionByTitleAsync(_party.ElectionId, positionTitle).ContinueWith((t) =>
                            {
                                position = t.Result;
                            }).Wait();

                            if (position == null)
                            {
                                e.Result = new ArgumentException($"Position '{ positionTitle }' does not exist\n\nRow number: { count + 1 }");
                                return;
                            }
                            else if (position.YearLevel != null && position.YearLevel != yearLevel)
                            {
                                e.Result = new ArgumentException($"Only Grade { position.YearLevel } candidates can be addded in the position '{ positionTitle }'; this candidate is in Grade { yearLevel }\n\nRow number: { count + 1 }");
                                return;
                            }
                            newCandidate.PositionId = position.Id;

                            _candidateService.ValidateAsync(_party.ElectionId, newCandidate).Wait();

                            importedCandidates.Add(newCandidate);
                        }
                    }
                }

                _backgroundWorker.ReportProgress(100, importedCandidates.Count);
                _candidateService.ImportCandidatesAsync(importedCandidates).Wait();

                e.Result = new Tuple<int, int>(importedCandidates.Count, blankCount);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                e.Result = new Exception($"{ ex.GetBaseException().Message }\n\nRow number: { count + 1 }");
                return;
            }
        }
    }
}
