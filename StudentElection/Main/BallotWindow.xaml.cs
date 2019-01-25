//using StudentElection.Classes;
using Humanizer;
using StudentElection.Dialogs;
using StudentElection.Repository.Models;
using StudentElection.Services;
using StudentElection.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace StudentElection.Main
{
    /// <summary>
    /// Interaction logic for BallotWindow.xaml
    /// </summary>
    public partial class BallotWindow : Window
    {
        bool _isExitClicked = false;
        
        private readonly ElectionService _electionService = new ElectionService();
        private readonly PositionService _positionService = new PositionService();
        private readonly CandidateService _candidateService = new CandidateService();
        private readonly BallotService _ballotService = new BallotService();

        private ElectionModel _currentElection;
        private VoterModel _voter;
        private BallotModel _ballot;

        public BallotWindow(VoterModel voter)
        {
            _voter = voter;

            InitializeComponent();

            tbkInstruction1.ToolTip = new TextBlock
            {
                Text = tbkInstruction1.Text,
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(tbkInstruction1, int.MaxValue);

            tbkInstruction2.ToolTip = new TextBlock
            {
                Text = tbkInstruction2.Text,
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(tbkInstruction2, int.MaxValue);

            tbkInstruction3.ToolTip = new TextBlock
            {
                Text = tbkInstruction3.Text,
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(tbkInstruction3, int.MaxValue);

            G.ChangeWindowSize(this);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.ChangeWindowSize(this);
            };
        }

        private async Task SetBallotAsync(VoterModel voter)
        {
            try
            {
                _ballot = await _ballotService.GetBallotAsync(_currentElection, _voter);

                tbkBallotCode.Text = _ballot.Code;
                tbkVoterID.Text = _voter.Vin;
                tbkFullName.Text = _ballot.EnteredAt.ToString("yyyy-MM-dd hh:mm:ss tt");

                await LoadCandidatesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }

        }

        public async Task LoadCandidatesAsync()
        {
            var listPosition = new List<PositionModel>();

            try
            {
                var positions = await _positionService.GetPositionsByYearLevelAsync(_currentElection.Id, _voter.YearLevel);
                var positionRows = positions.OrderBy(p => p.Rank);

                stkCandidates.Children.Clear();
                foreach (var position in positionRows)
                {
                    var candidates = await _candidateService.GetCandidatesByPositionAsync(position.Id);
                    var candidatesPosition = candidates.OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.Alias);

                    if (candidatesPosition.Count() == 0)
                    {
                        continue;
                    }

                    listPosition.Add(position);

                    var item = new PositionItemControl();
                    item.tbkName.Text = $"0 selected";
                    item.DataContext = position;
                    item.stkVotes.Children.Clear();
                    for (int i = 0; i < position.WinnersCount; i++)
                    {
                        item.stkVotes.Children.Add(new Rectangle
                        {
                            Width = 24,
                            Height = 24,
                            RadiusX = 12,
                            RadiusY = 12,
                            Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224)),
                            StrokeThickness = 2,
                            Stroke = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176)),
                            Margin = new Thickness(2, 0, 2, 0)
                        });
                    }
                    stkCandidates.Children.Add(item);

                    item.wrpCandidate.Children.Clear();
                    foreach (var candidate in candidatesPosition)
                    {
                        item.wrpCandidate.Visibility = Visibility.Visible;

                        var control = new CandidateBallotControl();
                        control.DataContext = candidate;
                        control.IsPressed = false;

                        item.Tag = new CandidateModel
                        {
                            Position = candidate.Position
                        };

                        control.PreviewMouseLeftButtonDown += (s, ev) =>
                        {
                            control.IsPressed = true;
                        };

                        control.PreviewMouseLeftButtonUp += (s, ev) =>
                        {
                            control.IsReleased = true;
                            if (!(control.IsPressed && control.IsReleased))
                            {
                                return;
                            }

                            var selectedCandidate = (s as CandidateBallotControl).DataContext as CandidateModel;
                            var winnersCount = selectedCandidate.Position.WinnersCount;

                            if (!control.IsSelected)
                            {
                                var selectedCandidates = item.wrpCandidate.Children.Cast<CandidateBallotControl>()
                                    .Where(cbc => cbc.IsSelected).Select(cbc => cbc.DataContext as CandidateModel);
                                var selectedCount = selectedCandidates.Count();

                                if (selectedCount == winnersCount)
                                {
                                    return;
                                }

                                control.Select();
                                selectedCount++;

                                //var c = (byte)(224 - (88 * ((double)selectedCount / winnersCount)));
                                //var selectedPartyBrush = new SolidColorBrush(Color.FromRgb(c, c, c));

                                var circle = item.stkVotes.Children.Cast<Rectangle>()
                                    .FirstOrDefault(r => r.Tag == null);
                                if (circle != null)
                                {
                                    circle.Fill = candidate.Party.ColorBrush;
                                    circle.Tag = candidate.Id;
                                    circle.ToolTip = candidate.FullName;
                                }

                                if (selectedCount > 1)
                                {
                                    item.tbkName.Text = $"{ selectedCount } selected";
                                    item.Tag = selectedCandidates;
                                }
                                else
                                {
                                    item.tbkName.Text = $"1 selected";
                                    item.Tag = candidate;
                                }
                                
                                //item.bdrPartyVoted.BorderBrush = selectedPartyBrush;
                            }
                            else
                            {
                                control.Deselect();

                                var circle = item.stkVotes.Children.Cast<Rectangle>()
                                    .FirstOrDefault(r => Convert.ToInt32(r.Tag ?? 0) == candidate.Id);
                                if (circle != null)
                                {
                                    circle.Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                                    circle.Tag = null;
                                    circle.ToolTip = null;
                                }

                                var selectedCandidates = item.wrpCandidate.Children.Cast<CandidateBallotControl>()
                                    .Where(cbc => cbc.IsSelected).Select(cbc => cbc.DataContext as CandidateModel);
                                var selectedCount = selectedCandidates.Count();

                                //var c = (byte)(224 - (88 * ((double)selectedCount / winnersCount)));
                                //var selectedPartyBrush = new SolidColorBrush(Color.FromRgb(c, c, c));

                                if (selectedCount > 1)
                                {
                                    item.tbkName.Text = $"{ selectedCount } selected";

                                    item.Tag = selectedCandidates;

                                    //item.bdrPartyVoted.BorderBrush = selectedPartyBrush;
                                }
                                else if (selectedCount == 1)
                                {
                                    var sc = selectedCandidates.First();
                                    item.tbkName.Text = $"1 selected";

                                    item.Tag = sc;

                                    //item.bdrPartyVoted.BorderBrush = selectedPartyBrush;
                                }
                                else
                                {
                                    item.tbkName.Text = $"0 selected";
                                    item.Tag = new CandidateModel
                                    {
                                        Position = (item.Tag as CandidateModel).Position
                                    };
                                }
                            }

                        };
                        
                        item.wrpCandidate.Children.Add(control);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }
        }

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {

            G.WaitLang(this);

            var votedCandidates = new List<CandidateModel>();

            foreach(PositionItemControl item in stkCandidates.Children)
            {
                if (item.Tag is CandidateModel candidate)
                {
                    votedCandidates.Add(candidate);
                }
                else if (item.Tag is IEnumerable<CandidateModel> candidates)
                {
                    votedCandidates.AddRange(candidates);
                }
            }

            votedCandidates.OrderBy(x => x.Position.Rank);

            Opacity = 0.5;

            var window = new VoteConfirmationWindow();
            window.ListVotedCandidates = votedCandidates;
            window.Voter = _voter;
            window.BallotId = _ballot.Id;
            window.Owner = this;
            window.ShowDialog();
            
            Opacity = 1;

            if (window.IsCasted)
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                
                G.EndWait(this);


                _isExitClicked = true;
                
                Close();
            }

            G.EndWait(this);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void tbkFullName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeFullNameTextBlock();
        }

        private void tbkVoterID_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeVoterIDTextBlock();
        }

        private void tbkBallotCode_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeBallotCodeTextBlock();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            _isExitClicked = true;

            var window = new MainWindow();
            window.Show();

            Close();
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);
            foreach (PositionItemControl item in stkCandidates.Children)
            {
                foreach (CandidateBallotControl control in item.wrpCandidate.Children)
                {
                    control.Deselect();
                    
                    var position = (item.DataContext as PositionModel);

                    item.tbkName.Text = $"0 selected";
                    item.bdrPartyVoted.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));

                    foreach (Rectangle circle in item.stkVotes.Children)
                    {
                        circle.Fill = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
                        circle.Tag = null;
                        circle.ToolTip = null;
                    }

                    item.Tag = new CandidateModel
                    {
                        Party = null,
                        Position = position,
                        Alias = null,
                        PictureFileName = null
                    };
                }
            }

            scvCandidates.ScrollToTop();
            G.EndWait(this);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeBallotCodeTextBlock();
            ResizeVoterIDTextBlock();
            ResizeFullNameTextBlock();
        }

        private void ResizeFullNameTextBlock()
        {
            var maxWidth = grdInfo.ActualWidth - stkInstructions.ActualWidth - 12 - 108;

            if (tbkFullName.ActualWidth > maxWidth * 1.5)
            {
                tbkFullName.Width = maxWidth * 1.5;
                
                tbkFullName.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkFullName.Text
                };
                ToolTipService.SetShowDuration(tbkFullName, int.MaxValue);

                tbkFullName.TextTrimming = TextTrimming.CharacterEllipsis;
            }

            if (tbkFullName.ActualWidth > maxWidth)
            {
                vbFullName.Width = maxWidth;
                vbFullName.Stretch = Stretch.Fill;
            }
            else
            {
                vbFullName.Stretch = Stretch.Uniform;
                tbkFullName.Width = double.NaN;
                tbkFullName.ToolTip = null;
            }
        }

        private void ResizeVoterIDTextBlock()
        {
            var maxWidth = grdInfo.ActualWidth - stkInstructions.ActualWidth - 12 - 108;

            if (tbkVoterID.ActualWidth > maxWidth * 1.5)
            {
                tbkVoterID.Width = maxWidth * 1.5;

                tbkVoterID.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkVoterID.Text
                };
                ToolTipService.SetShowDuration(tbkVoterID, int.MaxValue);

                tbkVoterID.TextTrimming = TextTrimming.CharacterEllipsis;
            }

            if (tbkVoterID.ActualWidth > maxWidth)
            {
                vbVoterID.Width = maxWidth;
                vbVoterID.Stretch = Stretch.Fill;
            }
            else
            {
                vbVoterID.Stretch = Stretch.Uniform;
                tbkVoterID.Width = double.NaN;
                tbkVoterID.ToolTip = null;
            }
        }

        private void ResizeBallotCodeTextBlock()
        {
            var maxWidth = grdInfo.ActualWidth - stkInstructions.ActualWidth - 12 - 108;

            if (tbkBallotCode.ActualWidth > maxWidth * 1.5)
            {
                tbkBallotCode.Width = maxWidth * 1.5;

                tbkBallotCode.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkBallotCode.Text
                };
                ToolTipService.SetShowDuration(tbkBallotCode, int.MaxValue);

                tbkBallotCode.TextTrimming = TextTrimming.CharacterEllipsis;
            }

            if (tbkBallotCode.ActualWidth > maxWidth)
            {
                vbBallotCode.Width = maxWidth;
                vbBallotCode.Stretch = Stretch.Fill;
            }
            else
            {
                vbBallotCode.Stretch = Stretch.Uniform;
                tbkBallotCode.Width = double.NaN;
                tbkBallotCode.ToolTip = null;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !_isExitClicked;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            await SetBallotAsync(_voter);
        }
    }
}
