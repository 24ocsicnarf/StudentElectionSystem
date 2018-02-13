using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
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

namespace LNHSVoting.Main
{
    /// <summary>
    /// Interaction logic for BallotWindow.xaml
    /// </summary>
    public partial class BallotWindow : Window
    {
        Voter _voter;
        bool _isExitClicked = false;

        public BallotWindow(Voter voter)
        {
            _voter = voter;

            InitializeComponent();
            
            try
            {
                var ballotRows = Ballots.Dictionary.Values.OrderBy(x => Convert.ToInt32(x.Code.Substring(x.Code.IndexOf('-') + 1)));

                var id = 0;
                if (ballotRows.Count() != 0)
                {
                    var code = Convert.ToString(ballotRows.Last().Code);
                    id = Convert.ToInt32(Convert.ToInt32(code.Substring(code.IndexOf('-') + 1))) + 1;
                }
                else
                {
                    id = 1;
                }

                var bCode = string.Format("{0}-{1:00000}", Machine.Tag, id);
                tbkBallotCode.Text = bCode;
                tbkVoterID.Text = voter.VoterID;

                LoadCandidates();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                Application.Current?.Shutdown();
            }

            lblInstruction1.ToolTip = new TextBlock()
            {
                Text = lblInstruction1.Content.ToString(),
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(lblInstruction1, int.MaxValue);

            lblInstruction2.ToolTip = new TextBlock()
            {
                Text = lblInstruction2.Content.ToString(),
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(lblInstruction2, int.MaxValue);

            lblInstruction3.ToolTip = new TextBlock()
            {
                Text = lblInstruction3.Content.ToString(),
                TextWrapping = TextWrapping.Wrap
            };
            ToolTipService.SetShowDuration(lblInstruction3, int.MaxValue);

            G.ChangeWindowSize(this);
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.ChangeWindowSize(this);
            };
        }

        public void LoadCandidates()
        {
            List<Position> listPosition = new List<Position>();

            try
            {
                var positionRows = Positions.Dictionary.Values.OrderBy(x => x.Order);

                stkCandidates.Children.Clear();
                foreach (var position in positionRows)
                {
                    var candidatesPosition = Candidates.Dictionary.Values.Where(x => x.Position.ID == position.ID).OrderBy(x => x.LastName)
                        .ThenBy(x => x.FirstName).ThenBy(x => x.MiddleName).ThenBy(x => x.Alias);

                    if (candidatesPosition.Count() == 0) continue;

                    listPosition.Add(position);

                    var item = new PositionItemControl();
                    item.DataContext = position;
                    stkCandidates.Children.Add(item);

                    item.wrpCandidate.Children.Clear();
                    foreach (var candidate in candidatesPosition)
                    {
                        item.wrpCandidate.Visibility = Visibility.Visible;

                        var control = new CandidateBallotControl();
                        control.DataContext = candidate;
                        control.IsPressed = false;

                        item.Tag = new Candidate()
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
                            if (!(control.IsPressed && control.IsReleased)) return;

                            foreach (CandidateBallotControl cbc in item.wrpCandidate.Children)
                            {
                                if (cbc != control)
                                {
                                    cbc.Deselect();
                                }
                                cbc.IsPressed = false;
                            }

                            if (!control.IsSelected)
                            {
                                control.Select();

                                item.tbkName.Text = candidate.FullName;
                                item.recParty.Fill = candidate.Party.ColorBrush;
                                item.recColor.Fill = candidate.Party.ColorBrush;
                                item.bdrPartyVoted.BorderBrush = candidate.Party.ColorBrush;
                                item.Tag = candidate;
                            }
                            else
                            {
                                control.Deselect();

                                item.tbkName.Text = "Choose a candidate";
                                item.recParty.Fill = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
                                item.recColor.Fill = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
                                item.bdrPartyVoted.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
                                item.Tag = new Candidate()
                                {
                                    Position = (item.Tag as Candidate).Position
                                }; ;
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

            var votedCandidates = new List<Candidate>();

            foreach(PositionItemControl item in stkCandidates.Children)
            {
                var candidate = item.Tag as Candidate;
                votedCandidates.Add(candidate);
            }

            votedCandidates.OrderBy(x => x.Position.Order);

            Opacity = 0.5;

            var window = new VoteConfirmationWindow();
            window.ListVotedCandidates = votedCandidates;
            window.Voter = _voter;
            window.BallotCode = tbkBallotCode.Text.ToString();
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

                    item.tbkName.Text = "Choose a candidate";
                    item.recParty.Fill = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
                    item.recColor.Fill = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));
                    item.bdrPartyVoted.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 176, 176, 176));

                    item.Tag = new Candidate()
                    {
                        Party = null,
                        Position = (item.Tag as Candidate).Position,
                        Alias = null,
                        PictureByte = null
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
    }
}
