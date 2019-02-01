//using StudentElection.Classes;
using StudentElection;
using StudentElection.Main;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using StudentElection.Services;
using StudentElection.Repository.Models;
using Project.Library.Helpers;

namespace StudentElection.Dialogs
{
    public partial class CandidateViewerForm : Form
    {
        public CandidateModel Candidate;

        public bool IsUpdated = false;
        public bool IsDeleted = false;

        private MaintenanceWindow _window;
        private List<CandidateModel> _candidateParty;
        private int _index;

        private readonly ElectionService _electionService = new ElectionService();
        //private readonly PositionService _positionService = new PositionService();
        //private readonly PartyService _partyService = new PartyService();
        private readonly CandidateService _candidateService = new CandidateService();

        private ElectionModel _currentElection;

        public CandidateViewerForm()
        {
            InitializeComponent();
            
            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private async void btnEdit_Click(object sender, EventArgs e)
        {
            Opacity = 0.5;

            var editCandidate = new CandidateForm();
            editCandidate.Candidate = Candidate;
            editCandidate.Owner = this;
            editCandidate.ShowDialog();

            var candidate = await editCandidate.GetNewDataAsync();

            if (candidate != null)
            {
                IsUpdated = true;

                Candidate = candidate;
                SetCandidate();

                await _window.LoadVotersAsync();
                await _window.LoadCandidatesAsync();

                _candidateParty = (await _candidateService.GetCandidatesByPartyAsync(Candidate.PartyId)).ToList();

                _index = _candidateParty.FindIndex(c => c.Id == candidate.Id);
                lblCandidatePage.Text = (_index + 1) + " of " + (_candidateParty.Count);

                btnNext.Enabled = _index != _candidateParty.Count - 1;
                btnPrev.Enabled = _index != 0;
            }

            Opacity = 1;
        }
        
        private async void CandidateViewerForm_Load(object sender, EventArgs e)
        {
            _window = (Tag as MaintenanceWindow);

            _currentElection = await _electionService.GetCurrentElectionAsync();

            btnEdit.Visible = !_currentElection.CandidatesFinalizedAt.HasValue;
            btnDelete.Visible = !_currentElection.CandidatesFinalizedAt.HasValue;

            _candidateParty = (await _candidateService.GetCandidatesByPartyAsync(Candidate.PartyId)).ToList();

            _index = _candidateParty.Select(x => x.Id).ToList().IndexOf(Candidate.Id);

            btnNext.Enabled = _index != _candidateParty.Count - 1;
            btnPrev.Enabled = _index != 0;

            lblCandidatePage.Text = (_index + 1) + " of " + (_candidateParty.Count);

            SetCandidate();
        }

        private void GetNextCandidate()
        {
            _index++;

            btnNext.Enabled = _index != _candidateParty.Count - 1;
            btnPrev.Enabled = _index != 0;

            Candidate = _candidateParty[_index];
            SetCandidate();

            lblCandidatePage.Text = (_index + 1) + " of " + (_candidateParty.Count);
        }

        private void GetPreviousCandidate()
        {
            _index--;

            btnNext.Enabled = _index != _candidateParty.Count - 1;
            btnPrev.Enabled = _index != 0;

            Candidate = _candidateParty[_index];
            SetCandidate();

            lblCandidatePage.Text = (_index + 1) + " of " + (_candidateParty.Count);
        }

        private void SetCandidate()
        {
            lblName.Text = Candidate.FullName;
            lblStrand.Text = Candidate.Section;

            lblSex.Text = Candidate.Sex == Sex.Male ? "Male" : "Female";

            lblBirthdate.Text = Candidate.Birthdate?.ToString("yyyy-MM-dd");
            lblAlias.Text = Candidate.Alias;
            
            lblPosition.Text = Candidate.Position.Title;
            lblParty.Text = Candidate.Party.Title;
            lblAbbreviation.Text = Candidate.Party.ShortName;

            pbImage.Image = Properties.Resources.default_candidate;
            if (!Candidate.PictureFileName.IsBlank())
            {
                var filePath = System.IO.Path.Combine(App.ImageFolderPath, Candidate.PictureFileName);
                if (System.IO.File.Exists(filePath))
                {
                    using (var bmpTemp = new Bitmap(System.IO.Path.Combine(App.ImageFolderPath, Candidate.PictureFileName)))
                    {
                        pbImage.Image = new Bitmap(bmpTemp);
                    }
                }
            }

            //TODO: VOTER ID should it stay or go?????
            //lblVoterID.Text = Candidate.;

            ttpCandidate.AutoPopDelay = int.MaxValue;
            ttpCandidate.BackColor = Color.FromArgb(240, 240, 240);
            ttpCandidate.ForeColor = Color.FromArgb(32, 32, 32);

            CheckLabelWidth(lblName, Candidate.FullName);
            CheckLabelWidth(lblParty, Candidate.Party.Title);
            CheckLabelWidth(lblPosition, Candidate.Position.Title);
            CheckLabelWidth(lblAlias, Candidate.Alias);
            
            lblName.MouseEnter += (s, ev) =>
            {
                var lbl = s as Label;
                var text = CheckLabelWidth(lblName, Candidate.FullName);

                ttpCandidate.Show(text, lbl, short.MaxValue);
            };
            lblName.MouseLeave += (s, ev) =>
            {
                var lbl = s as Label;
                ttpCandidate.Hide(lbl);
            };

            lblParty.MouseEnter += (s, ev) =>
            {
                var lbl = s as Label;
                var text = CheckLabelWidth(lblParty, Candidate.Party.Title);

                ttpCandidate.Show(text, lbl, short.MaxValue);
            };
            lblParty.MouseLeave += (s, ev) =>
            {
                var lbl = s as Label;
                ttpCandidate.Hide(lbl);
            };

            lblPosition.MouseEnter += (s, ev) =>
            {
                var lbl = s as Label;
                var text = CheckLabelWidth(lblPosition, Candidate.Position.Title);

                ttpCandidate.Show(text, lbl, short.MaxValue);
            };
            lblPosition.MouseLeave += (s, ev) =>
            {
                var lbl = s as Label;
                ttpCandidate.Hide(lbl);
            };

            lblAlias.MouseEnter += (s, ev) =>
            {
                var lbl = s as Label;
                var text = CheckLabelWidth(lblAlias, Candidate.Alias);

                ttpCandidate.Show(text, lbl, short.MaxValue);
            };
            lblAlias.MouseLeave += (s, ev) =>
            {
                var lbl = s as Label;
                ttpCandidate.Hide(lbl);
            };

            pnlInfo.ForeColor = Candidate.Party.Color;
            pbBackImage.BackColor = Candidate.Party.Color;
            lblCandidatePage.ForeColor = Candidate.Party.Color;
        }
        
        private string CheckLabelWidth(Label label, string text)
        {
            var newText = "";

            if (label.Width >= 225)
            {
                label.AutoSize = false;
                label.Width = 225;

                float width = 0;
                int maxChar = 0;
                var maxSize = Screen.PrimaryScreen.Bounds.Size;
                maxSize.Width -= 80;

                var lbl = new Label();
                lbl.AutoSize = true;
                lbl.AutoEllipsis = false;

                lbl.Font = new Font(SystemFonts.MenuFont.FontFamily, 9.0f, SystemFonts.MenuFont.Style);

                while (width < maxSize.Width && maxChar < text.Length)
                {
                    lbl.Text = text.Substring(0, maxChar + 1);
                    width = lbl.PreferredWidth;
                    if (width < maxSize.Width)
                        maxChar++;
                }

                var newToolTip = new StringBuilder();
                int start = 0;
                int count = text.Length;

                while (count != 0)
                {
                    newToolTip.Append(text.Substring(start, count > maxChar ? maxChar : count));
                    if (count > maxChar)
                    {
                        count -= maxChar;
                        start += maxChar;
                        newToolTip.Append("\n");
                    }
                    else
                        count = 0;
                }

                newText = newToolTip.ToString();
            }
            else
            {
                newText = text;
                ttpCandidate.SetToolTip(label, null);
            }

            return newText;
        }
        
        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var result = System.Windows.MessageBox.Show($"Delete candidate '{ Candidate.FullName }'?", "Delete candidate", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Warning, System.Windows.MessageBoxResult.No);

            if (result == System.Windows.MessageBoxResult.Yes)
            {
                try
                {
                    string existingImageFile = null;
                    if (Candidate.PictureFileName != null)
                    {
                        existingImageFile = System.IO.Path.Combine(App.ImageFolderPath, Candidate.PictureFileName);
                    }

                    if (existingImageFile != null && System.IO.File.Exists(existingImageFile))
                    {
                        System.IO.File.Delete(existingImageFile);
                    }

                    await _candidateService.DeleteCandidateAsync(Candidate);

                    IsDeleted = true;

                    Close();
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex);

                    MessageBox.Show(ex.GetBaseException().Message, "PROGRAM ERROR: " + ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
            }
        }

        public async Task<CandidateModel> GetNewDataAsync()
        {
            return await _candidateService.GetCandidateAsync(Candidate.Id);
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

        private void btnPrev_Click(object sender, EventArgs e)
        {
            GetPreviousCandidate();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            GetNextCandidate();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
