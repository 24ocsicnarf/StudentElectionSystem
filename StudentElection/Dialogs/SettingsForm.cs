using Humanizer;
using Project.Library.Helpers;
using StudentElection.Main;
using StudentElection.Repository.Models;
using StudentElection.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace StudentElection.Dialogs
{
    public partial class SettingsForm : Form
    {

        private readonly ElectionService _electionService = new ElectionService();
        private readonly BallotService _ballotService = new BallotService();
        private readonly VoterService _voterService = new VoterService();

        private MaintenanceWindow _window;
        private ElectionModel _currentElection = new ElectionModel();
        private bool _canOpenNewElection;

        public bool IsCancelled { get; }

        public SettingsForm()
        {
            InitializeComponent();
        }

        private async void ElectionSettingsForm_Load(object sender, EventArgs e)
        {
            _window = this.Tag as MaintenanceWindow;

            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var databaseType = (DatabaseType)Enum.Parse(typeof(DatabaseType), configFile.AppSettings.Settings["DatabaseType"].Value);
            switch (databaseType)
            {
                case DatabaseType.Unknown:
                    lblCurrentDatabase.Text = "Unknown";
                    break;
                case DatabaseType.File:
                    lblCurrentDatabase.Text = "MS Access";
                    break;
                case DatabaseType.Server:
                    lblCurrentDatabase.Text = "PostgreSQL";
                    break;
            }

            await LoadElectionAsync();
        }

        private async Task LoadElectionAsync()
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();
            _canOpenNewElection = _currentElection == null || _currentElection.ClosedAt.HasValue;

            btnEdit.Visible = _currentElection.CandidatesFinalizedAt == null;

            if (_canOpenNewElection)
            {
                btnOpenOrCloseElection.Text = "OPEN NEW ELECTION";
            }
            else
            {
                btnOpenOrCloseElection.Text = "CLOSE ELECTION";
            }

            if (_currentElection != null)
            {
                lblCurrentElectionTitle.Text = _currentElection.Title;
                lblDescription.Text = _currentElection.Description;
                if (_currentElection.TookPlaceOn > DateTime.Today)
                {
                    lblTookPlaceOnLabel.Text = $"Will be conducted on { _currentElection.TookPlaceOn.ToString("MMMM d, yyyy") }";
                }
                else if (_currentElection.TookPlaceOn < DateTime.Today || _currentElection.ClosedAt.HasValue)
                {
                    lblTookPlaceOnLabel.Text = $"Took place on { _currentElection.TookPlaceOn.ToString("MMMM d, yyyy") }";
                }
                else
                {
                    lblTookPlaceOnLabel.Text = $"Conducting today, { _currentElection.TookPlaceOn.ToString("MMMM d, yyyy") }";
                }
                lblTookPlaceOn.Text = _currentElection.TookPlaceOn.Date.Humanize(dateToCompareAgainst: DateTime.Today);

                if (_currentElection.CandidatesFinalizedAt == null)
                {
                    lblCandidatesFinalizedAt.Text = "(not finalized yet)";
                    lblCandidatesFinalizedAt.Font = new Font(lblCandidatesFinalizedAt.Font, FontStyle.Regular);
                }
                else
                {
                    lblCandidatesFinalizedAt.Text = _currentElection.CandidatesFinalizedAt.Value.ToString("yyyy-MM-dd hh:mm:ss tt");
                    lblCandidatesFinalizedAt.Font = new Font(lblCandidatesFinalizedAt.Font, FontStyle.Bold);
                }

                if (_currentElection.ClosedAt == null)
                {
                    lblClosedAt.Text = "(not closed yet)";
                    lblClosedAt.Font = new Font(lblCandidatesFinalizedAt.Font, FontStyle.Regular);
                }
                else
                {
                    lblClosedAt.Text = _currentElection.ClosedAt.Value.ToString("yyyy-MM-dd hh:mm:ss tt");
                    lblClosedAt.Font = new Font(lblCandidatesFinalizedAt.Font, FontStyle.Bold);
                }
            }
        }

        private async void btnOpenOrCloseElection_Click(object sender, EventArgs e)
        {
            if (_canOpenNewElection)
            {
                this.Opacity = 0.5;

                var form = new ElectionForm
                {
                    Owner = this
                };
                form.Tag = this.Tag;
                form.ShowDialog();

                if (!form.IsCancelled)
                {
                    await LoadElectionAsync();

                    await _window.LoadElectionAsync();
                    await _window.LoadCandidatesAsync();
                    await _window.LoadResultsAsync();
                    await _window.LoadVotersAsync();
                }

                this.Opacity = 1;
            }
            else
            {
                if (_currentElection.TookPlaceOn > DateTime.Today)
                {
                    MessageBox.Show("Cannot close a future election", "Future election", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var votersCount = await _voterService.CountVotersAsync(_currentElection.Id);
                var votedCount = await _voterService.CountVotedVotersAsync(_currentElection.Id);
                var notVotedCount = votersCount - votedCount;

                if (votedCount == 0)
                {
                    MessageBox.Show("Cannot close the election without votes", "No votes yet", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

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
                           $"All voters have voted.\n\n" +
                           $"You WILL NO LONGER add, edit or delete voters once you close the election.\n\n" +
                           $"Do you want to close the election and view the results?";
                }
                
                var result = MessageBox.Show(message, "Close election", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

                if (result == DialogResult.Yes)
                {
                    try
                    {
                        await _electionService.CloseElectionAsync(_currentElection.Id);

                        await _window.LoadElectionAsync();
                        await _window.LoadResultsAsync();

                        MessageBox.Show($"The { _currentElection.Title } is officially closed.", "Election closed", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        Close();
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ex);

                        MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            var form = new ElectionForm
            {
                Owner = this,
                Election = _currentElection
            };
            form.Tag = this.Tag;
            form.ShowDialog();

            if (!form.IsCancelled)
            {
                await LoadElectionAsync();
            }
        }

        private void btnSetupDatabase_Click(object sender, EventArgs e)
        {
            var window = new DatabaseSetupWindow();
            var helper = new WindowInteropHelper(window);
            helper.Owner = this.Handle;
            var result = window.ShowDialog();

            if (result == true)
            {
                Application.Exit();
            }
        }
    }
}
