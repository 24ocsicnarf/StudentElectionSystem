//using StudentElection.Classes;
using StudentElection;
using StudentElection.Dialogs;
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
using StudentElection.UserControls;
using StudentElection.Repository.Models;
using StudentElection.Services;
using Humanizer;
using Humanizer.Localisation;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Project.Library.Helpers;
//using StudentElection.StudentElectionDataSetTableAdapters;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for VoteConfirmationWindow.xaml
    /// </summary>
    public partial class VoteConfirmationWindow : Window
    {
        public List<CandidateModel> ListVotedCandidates;
        public VoterModel Voter;
        public int BallotId;
        public bool IsCasted = false;
        
        private readonly BallotService _ballotService = new BallotService();

        public VoteConfirmationWindow()
        {
            InitializeComponent();

            stkButtons.Visibility = Visibility.Collapsed;
            tbkScrollDown.Visibility = Visibility.Collapsed;
            lblInstructions.Visibility = Visibility.Collapsed;
            svCandidates.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void btnCast_Click(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);
            try
            {
                var votes = new List<VoteModel>();
                foreach (var candidate in ListVotedCandidates)
                {
                    if (candidate.Id != 0)
                    {
                        votes.Add(new VoteModel
                        {
                            BallotId = BallotId,
                            CandidateId = candidate.Id
                        });
                    }
                }

                await _ballotService.CastVotesAsync(BallotId, votes);

                var ballot = await _ballotService.GetBallotAsync(BallotId);

                G.EndWait(this);

                var messageBuilder = new StringBuilder();
                messageBuilder.AppendLine("Your votes have been casted.");
                messageBuilder.AppendLine("Thank you for your participation :D");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine($"Ballot Code:\t{ ballot.Code }");
                messageBuilder.AppendLine($"Voter ID:\t\t{ Voter.Vin }");
                messageBuilder.AppendLine();
                messageBuilder.AppendLine($"Started At:\t{ ballot.EnteredAt.ToString("yyyy-MM-dd hh:mm:ss tt") }");
                messageBuilder.AppendLine($"Casted At:\t{ ballot.CastedAt?.ToString("yyyy-MM-dd hh:mm:ss tt") }");
                var duration = ballot.CastedAt.Value.Subtract(ballot.EnteredAt);
                messageBuilder.AppendLine($"Duration:\t{ TimeSpan.FromSeconds(duration.TotalSeconds).Humanize(minUnit: TimeUnit.Second) }");
                
                MessageBox.Show(messageBuilder.ToString(), "Votes Casted", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCasted = true;

                Close();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stkCandidates.Children.Clear();
            for (int i = 0; i < ListVotedCandidates.Count; i++)
            {
                var candidate = ListVotedCandidates[i];
                var votedCandidate = new VotedCandidateControl
                {
                    DataContext = candidate,
                };

                var samePosition = false;
                if (i > 0 && candidate.PositionId > 0)
                {
                    var prev = ListVotedCandidates[i - 1];
                    if (prev.PositionId == candidate.PositionId)
                    {
                        votedCandidate.tbkPosition.Text = string.Empty;
                        samePosition = true;
                    }
                }

                if (!samePosition)
                {
                    stkCandidates.Children.Add(new Rectangle
                    {
                        Height = 12,
                    });
                }

                stkCandidates.Children.Add(votedCandidate);
            }
            stkCandidates.Children.Add(new Rectangle
            {
                Height = 12,
            });
        }

        private void SvCandidates_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (stkButtons.Visibility == Visibility.Visible)
            {
                return;
            }

            var sb = e.OriginalSource as ScrollViewer;
            
            if (e.VerticalOffset > 0 && sb.ScrollableHeight == e.VerticalOffset)
            {
                stkButtons.Visibility = Visibility.Visible;
                lblInstructions.Visibility = Visibility.Visible;
                tbkScrollDown.Visibility = Visibility.Collapsed;
            }
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            tbkScrollDown.Text = string.Empty;
            tbkScrollDown.Visibility = Visibility.Visible;

            if (stkCandidates.ActualHeight <= svCandidates.ViewportHeight)
            {
                await Task.Delay(3000);

                stkButtons.Visibility = Visibility.Visible;
                lblInstructions.Visibility = Visibility.Visible;
                tbkScrollDown.Visibility = Visibility.Collapsed;

                svCandidates.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
            else
            {
                tbkScrollDown.Text = "Scroll down to the bottom of the list";
                stkButtons.Visibility = Visibility.Collapsed;
                lblInstructions.Visibility = Visibility.Collapsed;
                tbkScrollDown.Visibility = Visibility.Visible;
                svCandidates.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            }
        }
    }
}
