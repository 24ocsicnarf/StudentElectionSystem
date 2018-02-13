using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LNHSVoting
{
    public partial class PartyForm : Form
    {
        public Party Party;
        public bool IsDeleted;
        public bool IsCanceled = true;

        private string _rgbString;

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
                if (cdgParty.Color.R + cdgParty.Color.G + cdgParty.Color.B > 382)
                {
                    MessageBox.Show("The chosen color is light. Make it darker.", "Light Color Chosen", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    goto rechoose;
                }

                pbColor.BackColor = cdgParty.Color;
                _rgbString = string.Format("{0},{1},{2}", pbColor.BackColor.R, pbColor.BackColor.G, pbColor.BackColor.B);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public Party GetNewData()
        {
            if (IsDeleted)
            {
                return null;
            }

            return Parties.Dictionary[Party.ID];
        }

        private void btnAdd_Click(object sender, EventArgs e)
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
                var partyRows = Parties.Dictionary.Values.AsEnumerable();

                if (Party != null)
                    partyRows = partyRows.Where(x => x.ID != Party.ID);

                var rows = partyRows.Where(x => x.Title == txtName.Text.Trim());
                if (rows.Count() > 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("Name of this party has already been  used.", "Party name in us".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtName.Focus();

                    return;
                }

                rows = partyRows.Where(x => x.Abbreviation == txtAbbreviation.Text.Trim());
                if (rows.Count() > 0 && Party == null)
                {
                    G.EndWait(this);
                    MessageBox.Show("The abbreviation has already been used.", "Abbreviation in Use".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAbbreviation.Focus();

                    return;
                }

                rows = partyRows.Where(x => x.RGB == _rgbString);
                if (rows.Count() > 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("The chosen color has already been used.", "Color in use".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblChooseColor.Focus();

                    return;
                }


                var party = new Party()
                {
                    Title = txtName.Text,
                    Abbreviation = txtAbbreviation.Text,
                    RGB = _rgbString
                };
                if (Party == null)
                {
                    Parties.InsertData(party);

                    G.EndWait(this);
                    MessageBox.Show("New party created!", "Party", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    Close();
                }
                else
                {
                    party.ID = Party.ID;
                    Parties.UpdateData(party);

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

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("All candidates under this party will also be deleted.\n\nDo you want to continue?", Party.Title + " (" + Party.Abbreviation +  ")", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                try
                {
                    G.WaitLang(this);

                    Parties.DeleteData(Party.ID);

                    Voters.Dictionary.Keys.Where(x => Candidates.Dictionary.Keys.Contains(x))?.ToList()
                        .ForEach(x => {
                            Candidates.DeleteData(x);
                            Voters.UpdateIsForeign(x, false);
                        } );

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

        private void PartyForm_Load(object sender, EventArgs e)
        {
            if (Party == null)
            {
                lblTitle.Text = "Create Party";
                btnAdd.Text = "ADD";
                btnDelete.Visible = false;

                var random = new Random();
                var red = random.Next(0, 255);
                var green = random.Next(0, 382 - red > 255 ? 255 : 382 - red);
                var blue = random.Next(0, 382 - green - red > 255 ? 255 : 382 - green - red);

                pbColor.BackColor = Color.FromArgb(red, green, blue);
                _rgbString = string.Format("{0},{1},{2}", red, green, blue);
            }
            else
            {
                lblTitle.Text = "Edit Party";
                btnAdd.Text = "UPDATE";

                txtName.Text = Party.Title;
                txtName.SelectionStart = txtName.TextLength;
                txtAbbreviation.Text = Party.Abbreviation;
                pbColor.BackColor = Party.Color;

                _rgbString = string.Format("{0},{1},{2}", pbColor.BackColor.R, pbColor.BackColor.G, pbColor.BackColor.B);

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
