//using StudentElection.Classes;
using StudentElection;
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
using StudentElection.Repository.Models;
using StudentElection.Services;
using Project.Library.Helpers;

namespace StudentElection.Dialogs
{
    public partial class CandidateForm : Form
    {
        public CandidateModel Candidate;

        public bool IsCanceled = false;
        public bool IsLoadPosition = false;

        private ElectionService _electionService = new ElectionService();
        private PositionService _positionService = new PositionService();
        private PartyService _partyService = new PartyService();
        private CandidateService _candidateService = new CandidateService();

        private ElectionModel _currentElection;

        private string _mainImageFolder = $"D:\\Pictures\\assemble\\";

        public CandidateForm()
        {
            InitializeComponent();

            ttpCandidate.AutoPopDelay = int.MaxValue;
            ttpCandidate.BackColor = Color.FromArgb(240, 240, 240);
            ttpCandidate.ForeColor = Color.FromArgb(32, 32, 32);

            dtBirthdate.MaxDate = DateTime.Today;

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

        //private void lblVoterID_Click(object sender, EventArgs e)
        //{
        //    G.WaitLang(this);

        //    var window = new VotersListWindow();
        //    var helper = new WindowInteropHelper(window);
        //    helper.Owner = Handle;

        //    G.EndWait(this);

        //    window.ShowDialog();

        //    G.WaitLang(this);

        //    var voter = window.GetNewData();
        //    G.EndWait(this);

        //    if (voter == null) return;

        //    lblName.Visible = true;
        //    lblStrand.Visible = true;

        //    lblVoterID.Text = voter.VoterID;
        //    lblVoterID.Tag = voter.ID;
        //    lblName.Text = voter.FullName;
        //    lblStrand.Text = voter.GradeStrand;

        //    if (lblName.Width > 255)
        //    {
        //        lblName.Width = 255;
        //        lblName.AutoSize = false;

        //        ttpCandidate.SetToolTip(lblName, voter.FullName);
        //    }
        //    else
        //    {
        //        ttpCandidate.SetToolTip(lblName, null);
        //    }

        //    txtAlias.Text = voter.FirstName;
        //    txtAlias.Focus();
        //    txtAlias.SelectAll();
        //}

        private async void CandidateForm_Load(object sender, EventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            dtBirthdate.MaxDate = DateTime.Today;

            if (Candidate == null)
            {
                lblTitle.Text = "Add Candidate";
                btnAdd.Text = "ADD";
            }
            else
            {
                var party = await _partyService.GetPartyAsync(Candidate.PartyId);
                SetParty(party);
                
                lblTitle.Text = "Edit Candidate";

                txtFirstName.Text = Candidate.FirstName;
                txtMiddleName.Text = Candidate.MiddleName;
                txtLastName.Text = Candidate.LastName;
                txtSuffix.Text = Candidate.Suffix;
                dtBirthdate.Checked = Candidate.Birthdate.HasValue;
                if (dtBirthdate.Checked)
                {
                    dtBirthdate.Value = Candidate.Birthdate.Value;
                }
                radMale.Checked = Candidate.Sex == Sex.Male;
                radFemale.Checked = Candidate.Sex == Sex.Female;
                cmbYearLevel.Text = Candidate.YearLevel.ToString();
                txtSection.Text = Candidate.Section;
                txtAlias.Text = Candidate.Alias;
                
                if (!Candidate.PictureFileName.IsBlank())
                {
                    using (var bmpTemp = new Bitmap(Path.Combine(App.ImageFolderPath, Candidate.PictureFileName)))
                    {
                        pbImage.Image = new Bitmap(bmpTemp);
                    }
                }

                btnAdd.Text = "UPDATE";

                //if (lblName.Width > 255)
                //{
                //    lblName.Width = 255;
                //    lblName.AutoSize = false;

                //    ttpCandidate.SetToolTip(lblName, Candidate.FullName);
                //}
                //else
                //{
                //    ttpCandidate.SetToolTip(lblName, null);
                //}
            }

            await LoadPositionsAsync();
            
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };

            Text = lblTitle.Text;
        }

        public async Task<CandidateModel> GetNewDataAsync()
        {
            if (IsCanceled)
                return null;

            return await _candidateService.GetCandidateAsync(Candidate.Id);
        }

        private async Task LoadPositionsAsync()
        {
            try
            {
                //var positionIds = Candidates.Dictionary.Values.Where(x => x.Party.ID == Convert.ToInt32(lblParty.Tag)).Select(x => x.Position.ID);
                //var positionRows = Positions.Dictionary.Values.Where(x => !positionIds.Contains(x.ID) || (x.ID == Candidate?.Position.ID && lblTitle.Text.StartsWith("E"))).OrderBy(x => x.Order);

                var positionRows = await _positionService.GetPositionsAsync(_currentElection.Id);

                if (positionRows.Count() == 0 && lblTitle.Text.StartsWith("A"))
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

                if (Candidate == null)
                {
                    return;
                }

                var candidatePosition = await _positionService.GetPositionAsync(Candidate.PositionId);
                cmbPosition.Text = candidatePosition.Title;
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

        public void SetParty(PartyModel party)
        {
            if (party == null)
            {
                throw new ArgumentNullException(nameof(party));
            }

            lblParty.Text = party.Title;
            lblParty.ForeColor = party.Color;
            lblParty.Tag = party.Id;
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

        private async void btnPosition_Click(object sender, EventArgs e)
        {
            //G.WaitLang(this);
            var form = new PositionForm();

            form.ShowDialog();

            await LoadPositionsAsync();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            IsCanceled = true;

            Close();
        }

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtFirstName.Text.IsBlank())
            {
                MessageBox.Show("Enter the candidate's first name.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtLastName.Text.IsBlank())
            {
                MessageBox.Show("Enter the candidate's last name.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!radMale.Checked && !radFemale.Checked)
            {
                MessageBox.Show("Select the candidate's sex.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (cmbYearLevel.SelectedItem == null)
            {
                MessageBox.Show("Select the candidate's year level.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (txtSection.Text.IsBlank())
            {
                MessageBox.Show("Enter the candidate's section.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            try
            {
                G.WaitLang(this);

                var isAliasExisting = await _candidateService.IsAliasExistingAsync(_currentElection.Id, txtAlias.Text, Candidate);
                
                if (isAliasExisting)
                {
                    G.EndWait(this);
                    MessageBox.Show("The alias has been already used.", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    txtAlias.Focus();

                    return;
                }

                string existingImageFile = null;
                if (Candidate?.PictureFileName != null)
                {
                    existingImageFile = Path.Combine(App.ImageFolderPath, Candidate.PictureFileName);
                }

                string newFileName = Candidate?.PictureFileName;
                if (!(ofdImage.FileName == null || ofdImage.FileName.IsBlank()))
                {
                    if (existingImageFile != null && File.Exists(existingImageFile))
                    {
                        File.Delete(existingImageFile);
                    }

                    var fileExtension = Path.GetExtension(ofdImage.FileName);
                    newFileName = $"{ Guid.NewGuid().ToString() }{ fileExtension }";

                    pbImage.Image.Save($"{ Path.Combine(App.ImageFolderPath, newFileName) }");
                }

                var position = cmbPosition.SelectedItem as PositionModel;

                var candidate = new CandidateModel
                {
                    FirstName = txtFirstName.Text,
                    MiddleName = txtMiddleName.Text,
                    LastName = txtLastName.Text,
                    Suffix = txtSuffix.Text,
                    Sex = radMale.Checked ? Sex.Male : Sex.Female,
                    Birthdate = dtBirthdate.Checked ? dtBirthdate.Value.Date : default(DateTime?),
                    YearLevel = int.Parse(cmbYearLevel.Text),
                    Section = txtSection.Text,
                    PositionId = position.Id,
                    PartyId = (int)lblParty.Tag,
                    Alias = txtAlias.Text,
                    PictureFileName = newFileName
                };

                if (Candidate == null)
                {
                    await _candidateService.SaveCandidateAsync(candidate);
                    
                    G.EndWait(this);
                    MessageBox.Show("New candidate added!", "Candidate", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    candidate.Id = Candidate.Id;
                    await _candidateService.SaveCandidateAsync(candidate);

                    G.EndWait(this);
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
