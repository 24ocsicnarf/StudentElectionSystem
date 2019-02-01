using Project.Library.Helpers;
using StudentElection.Main;
using StudentElection.Repository.Models;
using StudentElection.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentElection.Dialogs
{
    public partial class ElectionForm : Form
    {
        private readonly ElectionService _electionService = new ElectionService();

        private MaintenanceWindow _window;

        public ElectionModel Election;
        public bool IsCancelled { get; private set;  }

        public ElectionForm()
        {
            InitializeComponent();

            dtpHappensOn.MinDate = DateTime.Today;
            txtElectionTitle.Focus();
        }

        private void ElectionForm_Load(object sender, EventArgs e)
        {
            _window = this.Tag as MaintenanceWindow;

            if (Election != null)
            {
                lblTitle.Text = "Edit Election";

                txtElectionTitle.Text = Election.Title;
                txtDescription.Text = Election.Description;
                dtpHappensOn.Value = Election.TookPlaceOn;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            var election = new ElectionModel
            {
                Title = txtElectionTitle.Text,
                Description = txtDescription.Text,
                TookPlaceOn = dtpHappensOn.Value,
                CandidatesFinalizedAt = Election?.CandidatesFinalizedAt,
                ClosedAt = Election?.ClosedAt,
                ServerTag = Election?.ServerTag ?? " "
            };

            if (election.Title.IsBlank())
            {
                MessageBox.Show($"Enter the election title.", "Election title required", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtElectionTitle.Focus();
                return;
            }
            

            if (Election == null)
            {
                try
                {
                    await _electionService.OpenElectionAsync(election);
                    
                    if (_window != null)
                    {
                        await _window.LoadElectionAsync();
                    }

                    MessageBox.Show($"The { election.Title } is now officially open.", "New election opened", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
            else
            {
                try
                {
                    election.Id = Election.Id;
                    await _electionService.UpdateElectionAsync(election);

                    if (_window != null)
                    {
                        await _window.LoadElectionAsync();
                    }

                    MessageBox.Show($"The election is successfully updated", "Election updated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.IsCancelled = true;

            Close();
        }
    }
}
