using StudentElection.Classes;
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
//using StudentElection.StudentElectionDataSetTableAdapters;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for VoteConfirmationWindow.xaml
    /// </summary>
    public partial class VoteConfirmationWindow : Window
    {
        public List<Candidate> ListVotedCandidates;
        public Voter @Voter;
        public string BallotCode;
        public bool IsCasted = false;

        public VoteConfirmationWindow()
        {
            InitializeComponent();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void btnChange_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnCast_Click(object sender, RoutedEventArgs e)
        {
            G.WaitLang(this);
            try
            {
                Ballots.InsertData(new Ballot()
                {
                    Code = BallotCode,
                    UserID = Voter.ID
                });

                var bcAdapter = new BallotCandidateTableAdapter();
                foreach (Candidate candidate in ListVotedCandidates)
                {
                    if (candidate.ID != 0)
                    {
                        bcAdapter.Insert(Voter.ID, candidate.ID);
                    }
                }

                Voters.Dictionary[Voter.ID].IsVoted = true;

                G.EndWait(this);

                MessageBox.Show("Your votes have been casted. Thank you for your participation. :D", "Votes Casted", MessageBoxButton.OK, MessageBoxImage.Information);
                IsCasted = true;

                Close();
            }
            catch (Exception ex)
            {
                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            stkCandidates.Children.Clear();
            foreach (Candidate candidate in ListVotedCandidates)
            {
                var votedCandidate = new VotedCandidateControl();
                votedCandidate.DataContext = candidate;
                
                stkCandidates.Children.Add(votedCandidate);
            }
        }
    }
}
