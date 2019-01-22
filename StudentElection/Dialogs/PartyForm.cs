﻿//using StudentElection.Classes;
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

            //rechoose:
            var result = cdgParty.ShowDialog();
            
            if (result != DialogResult.Cancel)
            {
                //double darkness = 1 - (0.299 * cdgParty.Color.R + 0.587 * cdgParty.Color.G + 0.114 * cdgParty.Color.B) / 255;
                //if (darkness < (1 / 3d))
                //{
                //    MessageBox.Show("The chosen color is light. Make it darker.", "Make it darker", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    goto rechoose;
                //}

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
                MessageBox.Show("Please provide the name of the party.", "No Party Name", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtName.Focus();

                return;
            }
            if (txtAbbreviation.Text.IsBlank())
            {
                MessageBox.Show("Please provide an abbreviation of the party.", "No Abbreviation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAbbreviation.Focus();

                return;
            }
            if (txtName.Text.Length <= txtAbbreviation.Text.Length)
            {
                MessageBox.Show("The abbreviation of the party must be shorter than its name.", "Party", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    MessageBox.Show("Name of this party has already been used.", "Party name in us".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtName.Focus();

                    return;
                }

                rows = partyRows.Where(x => x.ShortName == txtAbbreviation.Text.Trim());
                if (rows.Count() > 0 && Party == null)
                {
                    G.EndWait(this);
                    MessageBox.Show("The short name has already been used.", "Abbreviation in Use".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                G.EndWait(this);
                MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                if (Application.MessageLoop)
                {
                    Application.Exit();
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("All candidates under this party will also be deleted.\n\nDo you want to continue?",
                Party.Title + " (" + Party.ShortName +  ")",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    await _partyService.DeletePartyAsync(Party);

                    //TODO: DELETE CANDIDATES UNDER THE DELETED PARTY
                    //Voters.Dictionary.Keys.Where(x => Candidates.Dictionary.Keys.Contains(x))?.ToList()
                    //    .ForEach(x => {
                    //        Candidates.DeleteData(x);
                    //        Voters.UpdateIsForeign(x, false);
                    //    } );

                    IsDeleted = true;

                    G.EndWait(this);
                    Close();
                }
                catch (Exception ex)
                {
                    G.EndWait(this);
                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    if (Application.MessageLoop)
                    {
                        Application.Exit();
                    }
                    else
                    {
                        Environment.Exit(1);
                    }
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
                var b = new byte[3];
                random.NextBytes(b);
                pbColor.BackColor = Color.FromArgb(255, b[0], b[1], b[2]);

                _argb = pbColor.BackColor.ToArgb();
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
    }
}