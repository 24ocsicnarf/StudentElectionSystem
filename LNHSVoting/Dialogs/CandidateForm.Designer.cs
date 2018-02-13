namespace LNHSVoting
{
    partial class CandidateForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblBrowse = new System.Windows.Forms.Label();
            this.lblVoterID = new System.Windows.Forms.Label();
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblParty = new System.Windows.Forms.Label();
            this.cmbPosition = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ofdImage = new System.Windows.Forms.OpenFileDialog();
            this.lblName = new System.Windows.Forms.Label();
            this.lblStrand = new System.Windows.Forms.Label();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.ttpCandidate = new System.Windows.Forms.ToolTip(this.components);
            this.pbBorder = new System.Windows.Forms.PictureBox();
            this.btnPosition = new System.Windows.Forms.Button();
            this.chkForeign = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBorder)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(203, 27);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Create Candidate";
            // 
            // lblBrowse
            // 
            this.lblBrowse.AutoSize = true;
            this.lblBrowse.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblBrowse.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBrowse.Location = new System.Drawing.Point(34, 204);
            this.lblBrowse.Name = "lblBrowse";
            this.lblBrowse.Size = new System.Drawing.Size(56, 14);
            this.lblBrowse.TabIndex = 4;
            this.lblBrowse.Text = "BROWSE";
            this.lblBrowse.Click += new System.EventHandler(this.lblBrowse_Click);
            // 
            // lblVoterID
            // 
            this.lblVoterID.AutoEllipsis = true;
            this.lblVoterID.AutoSize = true;
            this.lblVoterID.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblVoterID.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoterID.Location = new System.Drawing.Point(177, 79);
            this.lblVoterID.Name = "lblVoterID";
            this.lblVoterID.Size = new System.Drawing.Size(94, 14);
            this.lblVoterID.TabIndex = 0;
            this.lblVoterID.Tag = "0";
            this.lblVoterID.Text = "Click to select...";
            this.lblVoterID.Click += new System.EventHandler(this.lblVoterID_Click);
            // 
            // txtAlias
            // 
            this.txtAlias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAlias.Location = new System.Drawing.Point(180, 160);
            this.txtAlias.MaxLength = 30;
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(248, 22);
            this.txtAlias.TabIndex = 1;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(120, 79);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "Voter ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(144, 163);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(30, 14);
            this.label6.TabIndex = 1;
            this.label6.Text = "Alias";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(125, 191);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(49, 14);
            this.label7.TabIndex = 1;
            this.label7.Text = "Position";
            // 
            // lblParty
            // 
            this.lblParty.AutoEllipsis = true;
            this.lblParty.AutoSize = true;
            this.lblParty.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParty.ForeColor = System.Drawing.Color.DimGray;
            this.lblParty.Location = new System.Drawing.Point(14, 36);
            this.lblParty.Name = "lblParty";
            this.lblParty.Size = new System.Drawing.Size(45, 17);
            this.lblParty.TabIndex = 1;
            this.lblParty.Text = "Party";
            // 
            // cmbPosition
            // 
            this.cmbPosition.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbPosition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPosition.ForeColor = System.Drawing.Color.Black;
            this.cmbPosition.FormattingEnabled = true;
            this.cmbPosition.Location = new System.Drawing.Point(180, 188);
            this.cmbPosition.Name = "cmbPosition";
            this.cmbPosition.Size = new System.Drawing.Size(248, 22);
            this.cmbPosition.TabIndex = 2;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(290, 265);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 23);
            this.btnAdd.TabIndex = 5;
            this.btnAdd.Text = "ADD";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.White;
            this.btnCancel.FlatAppearance.BorderSize = 0;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(362, 265);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(66, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ofdImage
            // 
            this.ofdImage.Filter = "Image file (*.png, *.jpg, *.bmp) | *.png; *.jpg; *.bmp";
            this.ofdImage.ShowHelp = true;
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(177, 97);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(231, 16);
            this.lblName.TabIndex = 1;
            this.lblName.Text = "Name";
            this.lblName.Visible = false;
            // 
            // lblStrand
            // 
            this.lblStrand.AutoSize = true;
            this.lblStrand.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStrand.Location = new System.Drawing.Point(177, 113);
            this.lblStrand.Name = "lblStrand";
            this.lblStrand.Size = new System.Drawing.Size(79, 14);
            this.lblStrand.TabIndex = 1;
            this.lblStrand.Text = "Grade Strand";
            this.lblStrand.Visible = false;
            // 
            // pbImage
            // 
            this.pbImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbImage.Image = global::LNHSVoting.Properties.Resources.default_candidate;
            this.pbImage.Location = new System.Drawing.Point(17, 79);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(90, 120);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbImage.TabIndex = 3;
            this.pbImage.TabStop = false;
            // 
            // pbBorder
            // 
            this.pbBorder.Location = new System.Drawing.Point(15, 77);
            this.pbBorder.Name = "pbBorder";
            this.pbBorder.Size = new System.Drawing.Size(94, 124);
            this.pbBorder.TabIndex = 7;
            this.pbBorder.TabStop = false;
            // 
            // btnPosition
            // 
            this.btnPosition.BackColor = System.Drawing.Color.Transparent;
            this.btnPosition.BackgroundImage = global::LNHSVoting.Properties.Resources.add;
            this.btnPosition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPosition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPosition.FlatAppearance.BorderSize = 0;
            this.btnPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosition.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPosition.Location = new System.Drawing.Point(123, 236);
            this.btnPosition.Name = "btnPosition";
            this.btnPosition.Size = new System.Drawing.Size(22, 22);
            this.btnPosition.TabIndex = 3;
            this.btnPosition.UseVisualStyleBackColor = false;
            this.btnPosition.Visible = false;
            this.btnPosition.Click += new System.EventHandler(this.btnPosition_Click);
            // 
            // chkForeign
            // 
            this.chkForeign.AutoSize = true;
            this.chkForeign.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkForeign.Location = new System.Drawing.Point(304, 216);
            this.chkForeign.Name = "chkForeign";
            this.chkForeign.Size = new System.Drawing.Size(121, 18);
            this.chkForeign.TabIndex = 8;
            this.chkForeign.Text = "Foreign Candidate";
            this.chkForeign.UseVisualStyleBackColor = true;
            // 
            // CandidateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(440, 300);
            this.Controls.Add(this.chkForeign);
            this.Controls.Add(this.btnPosition);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cmbPosition);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.lblParty);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.lblStrand);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.lblVoterID);
            this.Controls.Add(this.lblBrowse);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.pbBorder);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CandidateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "CandidateForm";
            this.Load += new System.EventHandler(this.CandidateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBorder)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBrowse;
        private System.Windows.Forms.Label lblVoterID;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblParty;
        private System.Windows.Forms.ComboBox cmbPosition;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog ofdImage;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblStrand;
        private System.Windows.Forms.ToolTip ttpCandidate;
        private System.Windows.Forms.PictureBox pbBorder;
        private System.Windows.Forms.Button btnPosition;
        private System.Windows.Forms.CheckBox chkForeign;
    }
}