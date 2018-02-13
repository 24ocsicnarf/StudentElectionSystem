using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Interop;

namespace LNHSVoting
{
    public partial class CandidateForm : Form
    {
        public Candidate @Candidate;
        public bool IsCanceled = false;
        public bool IsLoadPosition = false;

        public CandidateForm()
        {
            InitializeComponent();

            ttpCandidate.AutoPopDelay = int.MaxValue;
            ttpCandidate.BackColor = Color.FromArgb(240, 240, 240);
            ttpCandidate.ForeColor = Color.FromArgb(32, 32, 32);
            
            ofdImage.HelpRequest += (s, ev) =>
            {
                MessageBox.Show("Choose an image with a minumum dimension of 120x120 and a maximum of 1920x1920.", "Help", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
        }

        private void lblBrowse_Click(object sender, EventArgs e)
        {
            rechoose:

            if (ofdImage.ShowDialog() != DialogResult.Cancel)
            {
                var image = Image.FromFile(ofdImage.FileName);

                if (image.Width > 1920 || image.Height > 1920)
                {
                    MessageBox.Show(string.Format("The maximum image dimension is 1920x1920. The image browsed has a dimension of {0}x{1}.", image.Width, image.Height), "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    goto rechoose;
                }
                else if (image.Width < 120 || image.Height < 120)
                {
                    MessageBox.Show(string.Format("The minimum image size is 120x120. The image browsed has a size of {0}x{1}.", image.Width, image.Height), "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    goto rechoose;
                }

                Bitmap bitmap = new Bitmap(image);
                int width, height;
                double imageRatio = (bitmap.Width * 1.0) / (bitmap.Height * 1.0);
                double requiredRatio = (3.0 / 4.0);

                if (imageRatio < requiredRatio)
                {
                    width = (int)Math.Floor(bitmap.Width / 3.0) * 3;
                    height = width / 3 * 4;

                    int diffWidth = bitmap.Width - width;
                    int diffHeight = bitmap.Height - height;

                    Bitmap clone = bitmap.Clone(new Rectangle(diffWidth / 2, diffHeight / 2, width, height), bitmap.PixelFormat);
                    pbImage.Image = clone;
                }
                else if (imageRatio > requiredRatio)
                {
                    height = (int)Math.Floor(bitmap.Height / 4.0) * 4;
                    width = height / 4 * 3;
                    
                    int diffWidth = bitmap.Width - width;
                    int diffHeight = bitmap.Height - height;

                    Bitmap clone = bitmap.Clone(new Rectangle(diffWidth / 2, diffHeight / 2, width, height), bitmap.PixelFormat);
                    pbImage.Image = clone;
                }
                else
                {
                    width = (int)Math.Floor(bitmap.Width / 90.0) * 90;
                    height = width / 3 * 4;

                    int diffWidth = bitmap.Width - width;
                    int diffHeight = bitmap.Height - height;

                    Bitmap clone = bitmap.Clone(new Rectangle(diffWidth / 2, diffHeight / 2, width, height), bitmap.PixelFormat);
                    pbImage.Image = clone;
                }

                bitmap.Dispose();
            }
        }

        private void lblVoterID_Click(object sender, EventArgs e)
        {
            G.WaitLang(this);

            var window = new VotersListWindow();
            var helper = new WindowInteropHelper(window);
            helper.Owner = Handle;

            G.EndWait(this);

            window.ShowDialog();

            G.WaitLang(this);

            var voter = window.GetNewData();
            G.EndWait(this);

            if (voter == null) return;

            lblName.Visible = true;
            lblStrand.Visible = true;

            lblVoterID.Text = voter.VoterID;
            lblVoterID.Tag = voter.ID;
            lblName.Text = voter.FullName;
            lblStrand.Text = voter.GradeStrand;

            if (lblName.Width > 255)
            {
                lblName.Width = 255;
                lblName.AutoSize = false;

                ttpCandidate.SetToolTip(lblName, voter.FullName);
            }
            else
            {
                ttpCandidate.SetToolTip(lblName, null);
            }

            txtAlias.Text = voter.FirstName;
            txtAlias.Focus();
            txtAlias.SelectAll();
        }

        private void CandidateForm_Load(object sender, EventArgs e)
        {
            ttpCandidate.SetToolTip(chkForeign, "A candidate but cannot allow to vote on this machine");

            if (@Candidate == null)
            {
                lblTitle.Text = "Create Candidate";
                btnAdd.Text = "ADD";
            }
            else
            {
                SetParty(Candidate.Party);

                lblTitle.Text = "Edit Candidate";
                lblVoterID.Text = Candidate.VoterID;
                lblVoterID.Tag = Candidate.ID;

                lblName.Text = Candidate.FullName;
                lblStrand.Text = Candidate.GradeStrand;
                txtAlias.Text = Candidate.Alias;
                chkForeign.Checked = Candidate.IsForeign;

                lblName.Visible = true;
                lblStrand.Visible = true;

                if (Candidate.PictureByte != null)
                {
                    pbImage.Image = Candidate.PictureImage;
                }

                btnAdd.Text = "UPDATE";

                if (lblName.Width > 255)
                {
                    lblName.Width = 255;
                    lblName.AutoSize = false;

                    ttpCandidate.SetToolTip(lblName, Candidate.FullName);
                }
                else
                {
                    ttpCandidate.SetToolTip(lblName, null);
                }
            }

            LoadPositions();
            
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };

            Text = lblTitle.Text;
        }

        public Candidate GetNewData()
        {
            if (IsCanceled)
                return null;
            
            return Candidates.Dictionary[Candidate.ID];
        }

        private void LoadPositions()
        {
            try
            {
                var positionIds = Candidates.Dictionary.Values.Where(x => x.Party.ID == Convert.ToInt32(lblParty.Tag)).Select(x => x.Position.ID);
                var positionRows = Positions.Dictionary.Values.Where(x => !positionIds.Contains(x.ID) || (x.ID == Candidate?.Position.ID && lblTitle.Text.StartsWith("E"))).OrderBy(x => x.Order);
                
                if (positionRows.Count() == 0 && lblTitle.Text.StartsWith("C"))
                {
                    var result = MessageBox.Show("There are no positions available. Do you want to create one?", "No Position", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    Close();

                    if (result == DialogResult.Yes)
                    {
                        IsLoadPosition = true;
                    }

                    IsCanceled = true;
                }

                cmbPosition.Items.Clear();
                foreach (var position in positionRows)
                {
                    cmbPosition.Items.Add(position);
                }

                cmbPosition.Text = Candidate?.Position.Title;
            }
            catch (Exception ex)
            {
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

        public void SetParty(Party party)
        {
            lblParty.Text = party.Title;
            lblParty.ForeColor = party.Color;
            lblParty.Tag = party.ID;
            lblTitle.ForeColor = party.Color;
            pbBorder.BackColor = party.Color;

            if (lblParty.Width > 414)
            {
                lblParty.Width = 414;
                lblParty.AutoSize = false;

                ttpCandidate.SetToolTip(lblParty, party.Title);
            }
            else
            {
                ttpCandidate.SetToolTip(lblParty, null);
            }
        }

        private void btnPosition_Click(object sender, EventArgs e)
        {
            //G.WaitLang(this);
            var form = new PositionForm();

            form.ShowDialog();

            LoadPositions();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;

            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(lblVoterID.Tag) == 0)
            {
                MessageBox.Show("Please provide information of the candidate.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtAlias.Text.IsBlank())
            {
                MessageBox.Show("Please provide an alias.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtAlias.Focus();
                return;
            }
            if (cmbPosition.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a position.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                cmbPosition.Focus();
                return;
            }

            txtAlias.Text = txtAlias.Text.ToProperCase();

            var result = MessageBox.Show(string.Format("This voter is {0} candidate.\n\nDo you want to continue {1} this voter?", chkForeign.Checked ? "A FOREIGN" : "NOT a foreign", lblTitle.Text.StartsWith("C") ? "adding" : "updating"), string.Format("{0} A Foreign Candidate", chkForeign.Checked ? "" : "Not"), MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes) return;

            try
            {
                G.WaitLang(this);
                var candidates = Candidates.Dictionary.Values.AsEnumerable();
                
                if (Candidate != null)
                {
                    candidates = Candidates.Dictionary.Values.Where(x => x.ID != Candidate.ID);
                }

                if (candidates.Where(x => x.Alias == txtAlias.Text).Count() > 0)
                {
                    G.EndWait(this);
                    MessageBox.Show("The alias has been already used.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAlias.Focus();

                    return;
                }
                
                byte[] pictureArray = @Candidate == null ? null : @Candidate.PictureByte;
                if (!(ofdImage.FileName == null || ofdImage.FileName.IsBlank()) || pictureArray != null)
                {
                    var memoryStream = new MemoryStream();

                    pbImage.Image.Save(memoryStream, ImageFormat.Bmp);
                    pictureArray = new byte[memoryStream.Length];
                    memoryStream.Position = 0;
                    memoryStream.Read(pictureArray, 0, pictureArray.Length);
                }

                var candidate = new Candidate()
                {
                    ID = (int)lblVoterID.Tag,
                    Position = new Position() { ID = (cmbPosition.SelectedItem as Position).ID },
                    Party = new Party() { ID = (int)lblParty.Tag },
                    Alias = txtAlias.Text,
                    PictureByte = pictureArray
                };

                if (@Candidate == null)
                {
                    Candidates.InsertData(candidate);
                    Voters.UpdateIsForeign(candidate.ID, chkForeign.Checked);
                    
                    G.EndWait(this);
                    MessageBox.Show("New candidate added!", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    Candidates.UpdateData(@Candidate.ID, candidate);
                    Voters.UpdateIsForeign(candidate.ID, chkForeign.Checked);
                    
                    G.EndWait(this);
                    //MessageBox.Show("Successfully updated!", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

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
