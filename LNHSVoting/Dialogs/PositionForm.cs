using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using LNHSVoting.Main;
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

namespace LNHSVoting
{
    public partial class PositionForm : Form
    {
        public bool IsDeleted = false;
        public int PartyID;

        private MaintenanceWindow _window;

        public PositionForm()
        {
            InitializeComponent();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        private void PositionForm_Load(object sender, EventArgs e)
        {
            _window = (Tag as MaintenanceWindow);

            LoadPositions();
        }

        private void LoadPositions()
        {
            try
            {
                var positionAdapter = new PositionTableAdapter();

                var positionRows = positionAdapter.GetData().OrderBy(x => x.Field<int>("Order"));

                lbPosition.Items.Clear();
                foreach (DataRow row in positionRows)
                {
                    var position = new Position()
                    {
                        ID = Convert.ToInt32(row["ID"]),
                        Title = Convert.ToString(row["Title"]),
                        Order = Convert.ToInt32(row["Order"])
                    };
                    
                    lbPosition.Items.Add(position);
                }

                _window.lblPosition.Text = string.Format("Positions ({0})", lbPosition.Items.Count);

                SetToAddSettings();
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

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (lbPosition.Items.Count == 20)
            {
                MessageBox.Show("Up to 20 positions only.", "Position", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }
            
            if (txtPosition.Text.IsBlank())
            {
                MessageBox.Show("Please provide a position title.", "no position".ToTitleCase(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPosition.Focus();

                return;
            }

            txtPosition.Text = txtPosition.Text.ToTitleCase();

            G.WaitLang(this);

            var positionRows = Positions.Dictionary.Values.AsEnumerable();

            if (btnCancel.Visible)
                positionRows = positionRows.Where(x => x.ID != ((Position)lbPosition.Items[lbPosition.SelectedIndex]).ID);


            var rows = positionRows.Where(x => x.Title == txtPosition.Text.Trim());
            if (rows.Count() > 0)
            {
                G.EndWait(this);
                MessageBox.Show("Position title has already been added.", "Position", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            if (!btnCancel.Visible)
            {
                Positions.InsertData(new Position()
                {
                    Title = txtPosition.Text,
                    Order = positionRows.Count() == 0 ? 1 : positionRows.OrderByDescending(x => x.Order).Select(x => x.Order).First() + 1
                });

                LoadPositions();

                lbPosition.SelectedIndex = lbPosition.Items.Count - 1;

                G.EndWait(this);
            }
            else
            {
                var sIndex = lbPosition.SelectedIndex;
                var prevPos = ((Position)lbPosition.SelectedItem).ToString();

                Positions.UpdateData(new Position()
                {
                    ID = ((Position)lbPosition.SelectedItem).ID,
                    Title = txtPosition.Text,
                    Order = ((Position)lbPosition.SelectedItem).Order
                });

                LoadPositions();

                _window.LoadCandidates();

                G.EndWait(this);
                MessageBox.Show("Successfully updated!", "Position", MessageBoxButtons.OK, MessageBoxIcon.Information);

                lbPosition.SelectedIndex = sIndex;
            }
            
            txtPosition.Focus();
        }

        private void lbPosition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lbPosition.SelectedIndices.Count == 0)
            {
                SetToAddSettings();
            }
            else
            {
                btnUp.Enabled = lbPosition.SelectedIndex > 0;
                btnDown.Enabled = lbPosition.SelectedIndex < lbPosition.Items.Count - 1;
                btnDelete.Enabled = true;
            }
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            try
            {
                G.WaitLang(this);

                var sIndex = lbPosition.SelectedIndex;
                var selected = lbPosition.SelectedItem;
                var next = lbPosition.Items[lbPosition.SelectedIndex - 1];
                
                lbPosition.Items[sIndex] = next;
                lbPosition.Items[sIndex - 1] = selected;

                Positions.UpdateData(new Position()
                {
                    Title = (selected as Position).Title,
                    Order = (next as Position).Order,
                    ID = (selected as Position).ID
                });
                Positions.UpdateData(new Position()
                {
                    Title = (next as Position).Title,
                    Order = (selected as Position).Order,
                    ID = (next as Position).ID
                });

                _window.LoadCandidates();
                LoadPositions();

                G.EndWait(this);

                lbPosition.SelectedIndex = sIndex - 1;
            }
            catch (Exception)
            {
                G.EndWait(this);
                SetToAddSettings();
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            var positionAdapter = new PositionTableAdapter();

            try
            {
                G.WaitLang(this);

                var sIndex = lbPosition.SelectedIndex;
                var selected = lbPosition.SelectedItem;
                var prev = lbPosition.Items[lbPosition.SelectedIndex + 1];

                lbPosition.Items[sIndex] = prev;
                lbPosition.Items[sIndex + 1] = selected;


                Positions.UpdateData(new Position()
                {
                    Title = (selected as Position).Title,
                    Order = (prev as Position).Order,
                    ID = (selected as Position).ID
                });
                Positions.UpdateData(new Position()
                {
                    Title = (prev as Position).Title,
                    Order = (selected as Position).Order,
                    ID = (prev as Position).ID
                });

                _window.LoadCandidates();
                LoadPositions();

                G.EndWait(this);

                lbPosition.SelectedIndex = sIndex + 1;
            }
            catch (Exception)
            {
                SetToAddSettings();
                G.EndWait(this);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (lbPosition.SelectedIndex >= 0)
            {
                SetToAddSettings();

                txtPosition.Focus();
            }
        }

        private void lbPosition_DoubleClick(object sender, EventArgs e)
        {
            if (lbPosition.SelectedIndices.Count == 1)
            {
                SetToUpdateSettings();

                btnUp.Enabled = false;
                btnDown.Enabled = false;
                btnDelete.Enabled = false;
            }
        }

        private void SetToAddSettings()
        {
            txtPosition.Clear();
            btnAdd.Text = "ADD";
            btnCancel.Visible = false;

            btnDelete.Enabled = lbPosition.SelectedIndex >= 0;
            btnUp.Enabled = lbPosition.SelectedIndex >= 0;
            btnDown.Enabled = lbPosition.SelectedIndex >= 0 && lbPosition.SelectedIndex + 1 < lbPosition.Items.Count;
            lbPosition.Enabled = true;

            btnAdd.Location = new Point(176, 306);
        }

        private void SetToUpdateSettings()
        {
            txtPosition.Text = lbPosition.SelectedItem.ToString();
            btnAdd.Text = "UPDATE";
            btnCancel.Visible = true;

            btnUp.Enabled = lbPosition.SelectedIndex >= 0 ;
            btnDown.Enabled = lbPosition.SelectedIndex >= 0 && lbPosition.SelectedIndex + 1 < lbPosition.Items.Count;
            btnDelete.Enabled = lbPosition.SelectedIndex >= 0;

            lbPosition.Enabled = false;

            btnAdd.Location = new Point(104, 306);
            btnCancel.Location = new Point(176, 306);

            txtPosition.Focus();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var answer = MessageBox.Show(string.Format("Deleting the {0} position also deletes its candidate/s. Do you want to continue anyway?", lbPosition.SelectedItem.ToString()), "Position", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (answer == DialogResult.Yes)
            {
                IsDeleted = true;

                G.WaitLang(this);

                Positions.DeleteData((lbPosition.SelectedItem as Position).ID);

                G.EndWait(this);

                G.WaitLang(this);
                LoadPositions();

                _window.LoadVoters();
                _window.LoadCandidates();

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
    }
}
 