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
            var whoCanVote = cmbWhoCanVote.SelectedIndex;
            var position = new PositionModel
            {
                Title = txtPosition.Text.Transform(To.TitleCase),
                WinnersCount = (int)nudNumberOfWinners.Value,
                YearLevel = whoCanVote == 0 ? default(int?) : whoCanVote,
                ElectionId = _currentElection.Id
            };

            if (string.IsNullOrWhiteSpace(position.Title))
            {
                MessageBox.Show("Enter a position title", "Position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPosition.Focus();

                return;
            }
            
            G.WaitLang(this);

            var editingPosition = btnCancel.Visible ? dgPositions.CurrentCell?.OwningRow.DataBoundItem as PositionModel : null;

            var isExisting = await _positionService.IsPositionTitleExistingAsync(_currentElection.Id, position.Title, editingPosition);
            if (isExisting)
            {
                G.EndWait(this);

                MessageBox.Show($"Position title '{ position.Title }' already exists", "Position", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            if (!btnCancel.Visible)
            {
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
                position.Id = editingPosition.Id;
                position.Rank = editingPosition.Rank;

                var candidatesByPosition = await _candidateService.GetCandidateDetailsListByPositionAsync(position.Id);
                var unqualifiedCandidates = candidatesByPosition.Where(c => position.YearLevel.HasValue && c.YearLevel != position.YearLevel);

                if (unqualifiedCandidates.Any())
                {
                    var count = unqualifiedCandidates.Count();

                    var candidateNamesBuilder = new StringBuilder();
                    foreach (var candidate in unqualifiedCandidates)
                    {
                        candidateNamesBuilder.AppendFormat("- {0} (Grade {1})\n", candidate.FullName, candidate.YearLevel);
                    }

                    G.EndWait(this);

                    MessageBox.Show($"Cannot update position.\n\n" +
                        $"There's a conflict with the year levels of the following { "candidate".ToQuantity(count, ShowQuantityAs.None) }:\n" +
                        $"{ candidateNamesBuilder.ToString() }",
                        "Candidate conflict", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    return;
                }

                var sIndex = dgPositions.SelectedRows[0].Index;

                await _positionService.SavePositionAsync(position);

                await LoadPositionsAsync();
                
                await _window.LoadCandidatesAsync();

                G.EndWait(this);
                
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
            
            btnAdd.Location = new Point(266, 366);
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

            btnAdd.Location = new Point(194, 366);
            btnCancel.Location = new Point(266, 366);

            txtPosition.Focus();
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var position = dgPositions.SelectedRows[0].DataBoundItem as PositionModel;

            var candidates = await _candidateService.GetCandidateDetailsListByPositionAsync(position.Id);
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

        private void cmbWhoCanVote_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbWhoCanVote.SelectedIndex == 0)
            {
                WhoCanVoteWarningLabel.Text = string.Empty;
            }
            else
            {
                WhoCanVoteWarningLabel.Text = $"Only Grade { cmbWhoCanVote.SelectedIndex } candidates are qualified in this position";
            }
        }
    }
}
 