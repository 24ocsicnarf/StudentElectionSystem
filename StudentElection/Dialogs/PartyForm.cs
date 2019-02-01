//using StudentElection.Classes;
using Humanizer;
using Project.Library.Helpers;
using StudentElection;
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
    public partial class PartyForm : Form
    {
        public PartyModel Party;
        public bool IsDeleted;
        public bool IsCanceled = true;

        private readonly ElectionService _electionService = new ElectionService();
        private readonly PartyService _partyService = new PartyService();
        private readonly CandidateService _candidateService = new CandidateService();

        private ElectionModel _currentElection;

        private int _argb;
        //private string _rgbString;

        public PartyForm()
        {
            IsDeleted = false;
            
            InitializeComponent();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void lblChooseColor_Click(object sender, EventArgs e)
        {
            cdgParty.Color = pbColor.BackColor;

            rechoose:
            var result = cdgParty.ShowDialog();
            
            if (result != DialogResult.Cancel)
            {
                double darkness = 1 - (0.299 * cdgParty.Color.R + 0.587 * cdgParty.Color.G + 0.114 * cdgParty.Color.B) / 255;
                if (darkness < (1 / 3d))
                {
                    MessageBox.Show("The chosen color is light. Make it darker.", "Make it darker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    goto rechoose;
                }

                pbColor.BackColor = cdgParty.Color;
                _argb = cdgParty.Color.ToArgb();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public async Task<PartyModel> GetNewDataAsync()
        {
            if (IsDeleted)
            {
                return null;
            }

            return await _partyService.GetPartyAsync(Party.Id);
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            IsCanceled = false;

            if (txtName.Text.IsBlank())
            {
                MessageBox.Show("Please provide the name of the party.", "No party name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();

                return;
            }
            if (txtAbbreviation.Text.IsBlank())
            {
                MessageBox.Show("Please provide the short name of the party.", "No short name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAbbreviation.Focus();

                return;
            }
            if (txtName.Text.Length <= txtAbbreviation.Text.Length)
            {
                MessageBox.Show("The short name of the party must be shorter than its actual name.", "Party", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAbbreviation.Focus();

                return;
            }

            try
            {
                G.WaitLang(this);

                var partyRows = await _partyService.GetPartiesAsync(_currentElection.Id);

                if (Party != null)
                    partyRows = partyRows.Where(x => x.Id != Party.Id);

                var rows = partyRows.Where(x => x.Title == txtName.Text.Trim());
                if (rows.Count() > 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Name of this party has already been used", "Party name in use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtName.Focus();

                    return;
                }

                rows = partyRows.Where(x => x.ShortName == txtAbbreviation.Text.Trim());
                if (rows.Count() > 0 && Party == null)
                {
                    G.EndWait(this);
                    MessageBox.Show("The short name has already been used", "Short name in use", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAbbreviation.Focus();

                    return;
                }

                //rows = partyRows.Where(x => x.RGB == _rgbString);
                //if (rows.Count() > 0)
                //{
                //    G.EndWait(this);
                //    MessageBox.Show("The chosen color has already been used.", "Color in use".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    lblChooseColor.Focus();

                //    return;
                //}


                var party = new PartyModel
                {
                    Title = txtName.Text,
                    ShortName = txtAbbreviation.Text,
                    Argb = _argb,
                    ElectionId = _currentElection.Id
                };

                if (Party == null)
                {
                    await _partyService.InsertPartyAsync(party);

                    G.EndWait(this);
                    MessageBox.Show("New party created!", "Party", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
                else
                {
                    party.Id = Party.Id;

                    await _partyService.UpdatePartyAsync(party);

                    G.EndWait(this);
                    //MessageBox.Show("Successfully updated!", "Party", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);

                G.EndWait(this);

                MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var candidates = await _candidateService.GetCandidatesByPartyAsync(Party.Id);
            if (candidates.Any())
            {
                MessageBox.Show($"Cannot delete this party\n\nThere's { "candidate".ToQuantity(candidates.Count()) } in this party",
                    "Position", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
                return;
            }


            var result = MessageBox.Show($"Delete '{ Party.Title }'?",
                Party.Title + " (" + Party.ShortName +  ")",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    await _partyService.DeletePartyAsync(Party);
                    
                    IsDeleted = true;

                    G.EndWait(this);
                    Close();
                }
                catch (Exception ex)
                {
                    G.EndWait(this);

                    Logger.LogError(ex);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        private async void PartyForm_Load(object sender, EventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            if (Party == null)
            {
                lblTitle.Text = "Create Party";
                btnAdd.Text = "ADD";
                btnDelete.Visible = false;

                var random = new Random();
                _argb = (int)(0xFF000000 + (random.Next(0xFFFFFF) & 0x7F7F7F));

                pbColor.BackColor = Color.FromArgb(_argb);
            }
            else
            {
                lblTitle.Text = "Edit Party";
                btnAdd.Text = "UPDATE";

                txtName.Text = Party.Title;
                txtName.SelectionStart = txtName.TextLength;
                txtAbbreviation.Text = Party.ShortName;
                pbColor.BackColor = Party.Color;

                _argb = Party.Argb;

                btnDelete.Visible = true;
            }

            Text = lblTitle.Text;
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

        private void pbColor_Click(object sender, EventArgs e)
        {
            var random = new Random();
            _argb = (int)(0xFF000000 + (random.Next(0xFFFFFF) & 0x7F7F7F));

            pbColor.BackColor = Color.FromArgb(_argb);
        }
    }
}
