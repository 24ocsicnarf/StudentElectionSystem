using StudentElection;
using StudentElection.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentElection.Services;
using StudentElection.Repository.Models;
using Humanizer;
using Project.Library.Helpers;

namespace StudentElection.Dialogs
{
    public partial class PositionForm : Form
    {
        public bool IsDeleted = false;
        public int PartyID;

        private MaintenanceWindow _window;

        private readonly ElectionService _electionService = new ElectionService();
        private readonly PositionService _positionService = new PositionService();
        private readonly CandidateService _candidateService = new CandidateService();

        private ElectionModel _currentElection;

        public PositionForm()
        {
            InitializeComponent();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }
        
        private async void PositionForm_Load(object sender, EventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            _window = (Tag as MaintenanceWindow);

            await LoadPositionsAsync();

            nudNumberOfWinners.Value = Math.Min(2, nudNumberOfWinners.Maximum);
            nudNumberOfWinners.Value = 1;
            cmbWhoCanVote.SelectedIndex = 0;
        }

        private async Task LoadPositionsAsync()
        {
            try
            {
                //TODO:  SELECTION GLITCH AFTER UP OR DOWN
                var positionRows = await _positionService.GetPositionsAsync(_currentElection.Id);

                dgPositions.DataSource = positionRows.ToList();
                dgPositions.Refresh();

                _window.lblPosition.Text = string.Format("Positions ({0})", dgPositions.Rows.Count);
                
                SetToAddSettings();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtPosition.Text.IsBlank())
            {
                MessageBox.Show("Please provide a position title.", "No position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPosition.Focus();

                return;
            }

            txtPosition.Text = txtPosition.Text.ToTitleCase();

            G.WaitLang(this);

            var positionRows = await _positionService.GetPositionsAsync(_currentElection.Id);

            if (btnCancel.Visible)
                positionRows = positionRows.Where(x => x.Id != (dgPositions.SelectedRows[0].DataBoundItem as PositionModel).Id);


            var rows = positionRows.Where(x => x.Title == txtPosition.Text.Trim());
            if (rows.Count() > 0)
            {
                G.EndWait(this);
                MessageBox.Show("Position title has already been added.", "Position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var whoCanVote = cmbWhoCanVote.SelectedIndex;
            var position = new PositionModel
            {
                Title = txtPosition.Text,
                WinnersCount = (int)nudNumberOfWinners.Value,
                YearLevel = whoCanVote == 0 ? default(int?) : whoCanVote,
                ElectionId = _currentElection.Id
            };

            if (!btnCancel.Visible)
            {
                position.Rank = positionRows.Count() == 0 ? 1 : positionRows.OrderByDescending(x => x.Rank).Select(x => x.Rank).First() + 1;

                await _positionService.SavePositionAsync(position);

                await LoadPositionsAsync();

                dgPositions.ClearSelection();
                var index = dgPositions.Rows.Count - 1;
                dgPositions.Rows[index].Selected = true;
                dgPositions.FirstDisplayedScrollingRowIndex = index;

                G.EndWait(this);
            }
            else
            {
                var model = dgPositions.SelectedRows[0].DataBoundItem as PositionModel;
                position.Id = model.Id;
                position.Rank = model.Rank;

                var sIndex = dgPositions.SelectedRows[0].Index;

                await _positionService.SavePositionAsync(position);

                await LoadPositionsAsync();
                
                await _window.LoadCandidatesAsync();

                G.EndWait(this);

                //MessageBox.Show("Successfully updated!", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);

                dgPositions.ClearSelection();
                dgPositions.Rows[sIndex].Selected = true;
                dgPositions.FirstDisplayedScrollingRowIndex = sIndex;
            }

            txtPosition.Focus();
        }

        private void dgPositions_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private async void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                G.WaitLang(this);

                var sIndex = dgPositions.SelectedRows[0].Index;
                var selected = dgPositions.SelectedRows[0];
                var prev = dgPositions.Rows[dgPositions.SelectedRows[0].Index - 1];

                dgPositions.ClearSelection();

                var selectedModel = (selected.DataBoundItem as PositionModel);
                var prevModel = (prev.DataBoundItem as PositionModel);
                
                await _positionService.MoveRankAsync(selectedModel, prevModel);
                
                await _window.LoadCandidatesAsync();
                await LoadPositionsAsync();

                G.EndWait(this);

                dgPositions.Rows[sIndex - 1].Selected = true;
                dgPositions.FirstDisplayedScrollingRowIndex = sIndex - 1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                SetToAddSettings();
            }
        }

        private async void btnDown_Click(object sender, EventArgs e)
        {
            try
            {
                G.WaitLang(this);
                
                var sIndex = dgPositions.SelectedRows[0].Index;
                var selected = dgPositions.SelectedRows[0];
                var next = dgPositions.Rows[dgPositions.SelectedRows[0].Index + 1];

                dgPositions.ClearSelection();

                var selectedModel = (selected.DataBoundItem as PositionModel);
                var nextModel = (next.DataBoundItem as PositionModel);

                await _positionService.MoveRankAsync(selectedModel, nextModel);

                await _window.LoadCandidatesAsync();
                await LoadPositionsAsync();

                G.EndWait(this);

                dgPositions.Rows[sIndex + 1].Selected = true;
                dgPositions.FirstDisplayedScrollingRowIndex = sIndex + 1;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                SetToAddSettings();
                G.EndWait(this);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dgPositions.SelectedRows[0].Index >= 0)
            {
                SetToAddSettings();

                txtPosition.Focus();
            }
        }

        private void dgPositions_DoubleClick(object sender, EventArgs e)
        {
        }

        private void SetToAddSettings()
        {
            txtPosition.Clear();

            nudNumberOfWinners.Value = Math.Min(nudNumberOfWinners.Value + 1, nudNumberOfWinners.Maximum);
            nudNumberOfWinners.Value = 1;
            cmbWhoCanVote.SelectedIndex = 0;
            btnAdd.Text = "ADD";
            btnCancel.Visible = false;

            btnDelete.Enabled = dgPositions.SelectedRows.Count > 0 && dgPositions.SelectedRows[0].Index >= 0;
            btnUp.Enabled = dgPositions.SelectedRows.Count > 0 && dgPositions.SelectedRows[0].Index >= 0;
            btnDown.Enabled = dgPositions.SelectedRows.Count > 0 && dgPositions.SelectedRows[0].Index >= 0 && dgPositions.SelectedRows[0].Index + 1 < dgPositions.Rows.Count;
            dgPositions.Enabled = true;
            
            btnAdd.Location = new Point(226, 366);
        }

        private void SetToUpdateSettings()
        {
            var position = (dgPositions.SelectedRows[0].DataBoundItem as PositionModel);
            txtPosition.Text = position.Title;
            nudNumberOfWinners.Value = Math.Min(position.WinnersCount + 1, nudNumberOfWinners.Maximum);
            nudNumberOfWinners.Value = position.WinnersCount;
            cmbWhoCanVote.SelectedIndex = position.YearLevel ?? 0;
            btnAdd.Text = "UPDATE";
            btnCancel.Visible = true;

            btnUp.Enabled = dgPositions.SelectedRows[0].Index >= 0 ;
            btnDown.Enabled = dgPositions.SelectedRows[0].Index >= 0 && dgPositions.SelectedRows[0].Index + 1 < dgPositions.Rows.Count;
            btnDelete.Enabled = dgPositions.SelectedRows[0].Index >= 0;

            dgPositions.Enabled = false;

            btnAdd.Location = new Point(154, 366);
            btnCancel.Location = new Point(226, 366);

            txtPosition.Focus();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var position = dgPositions.SelectedRows[0].DataBoundItem as PositionModel;

            var candidates = await _candidateService.GetCandidatesByPositionAsync(position.Id);
            if (candidates.Any())
            {
                MessageBox.Show($"Cannot delete this position\n\nThere's { "candidate".ToQuantity(candidates.Count()) } in this position",
                    "Position", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                return;
            }

            var answer = MessageBox.Show(string.Format("Delete '{0}'?", position.Title), "Position", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (answer == DialogResult.Yes)
            {
                IsDeleted = true;

                G.WaitLang(this);

                await _positionService.DeletePositionAsync(position);

                G.EndWait(this);

                G.WaitLang(this);
                await LoadPositionsAsync();

                await _window.LoadVotersAsync();
                await _window.LoadCandidatesAsync();

                MessageBox.Show("Position deleted!", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);

                G.EndWait(this);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Escape:
                    Close();
                    break;
                default:
                    break;
            }

            return false;
        }

        private void dgPositions_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgPositions.SelectedRows[0].Index >= 0)
            {
                SetToUpdateSettings();

                btnUp.Enabled = false;
                btnDown.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void dgPositions_SelectionChanged(object sender, EventArgs e)
        {
            if (dgPositions.SelectedRows.Count == 0)
            {
                SetToAddSettings();
            }
            else
            {
                btnUp.Enabled = dgPositions.SelectedRows[0].Index > 0;
                btnDown.Enabled = dgPositions.SelectedRows[0].Index < dgPositions.Rows.Count - 1;
                btnDelete.Enabled = true;
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dgPositions_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var dgv = (DataGridView)sender;

            int whoCanVoteColumnIndex = 3;

            if (e.ColumnIndex == whoCanVoteColumnIndex
                && e.RowIndex >= 0)
            {
                var yearLevel = (int?)dgv[whoCanVoteColumnIndex, e.RowIndex].Value;
                if (yearLevel == null)
                {
                    e.Value = "All voters";
                    e.FormattingApplied = true;
                }
                else
                {
                    e.Value = $"Grade { yearLevel } only";
                    e.FormattingApplied = true;
                }
            }
        }
    }
}
 