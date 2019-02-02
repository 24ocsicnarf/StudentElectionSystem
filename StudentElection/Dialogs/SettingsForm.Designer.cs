namespace StudentElection.Dialogs
{
    partial class SettingsForm
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
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnOpenOrCloseElection = new System.Windows.Forms.Button();
            this.lblTookPlaceOn = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblCurrentElectionTitle = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblClosedAt = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblCandidatesFinalizedAt = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.lblTookPlaceOnLabel = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.lblUndoFinalize = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lblCurrentDatabase = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSetupDatabase = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(161, 243);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(114, 23);
            this.btnEdit.TabIndex = 11;
            this.btnEdit.Text = "EDIT ELECTION";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnOpenOrCloseElection
            // 
            this.btnOpenOrCloseElection.BackColor = System.Drawing.Color.White;
            this.btnOpenOrCloseElection.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnOpenOrCloseElection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenOrCloseElection.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenOrCloseElection.ForeColor = System.Drawing.Color.Black;
            this.btnOpenOrCloseElection.Location = new System.Drawing.Point(11, 243);
            this.btnOpenOrCloseElection.Name = "btnOpenOrCloseElection";
            this.btnOpenOrCloseElection.Size = new System.Drawing.Size(144, 23);
            this.btnOpenOrCloseElection.TabIndex = 12;
            this.btnOpenOrCloseElection.Text = "CLOSE ELECTION";
            this.btnOpenOrCloseElection.UseVisualStyleBackColor = false;
            this.btnOpenOrCloseElection.Click += new System.EventHandler(this.btnOpenOrCloseElection_Click);
            // 
            // lblTookPlaceOn
            // 
            this.lblTookPlaceOn.AutoSize = true;
            this.lblTookPlaceOn.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTookPlaceOn.Location = new System.Drawing.Point(11, 96);
            this.lblTookPlaceOn.Name = "lblTookPlaceOn";
            this.lblTookPlaceOn.Size = new System.Drawing.Size(22, 14);
            this.lblTookPlaceOn.TabIndex = 6;
            this.lblTookPlaceOn.Text = "---";
            // 
            // lblDescription
            // 
            this.lblDescription.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.ForeColor = System.Drawing.Color.DimGray;
            this.lblDescription.Location = new System.Drawing.Point(14, 36);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(261, 40);
            this.lblDescription.TabIndex = 7;
            // 
            // lblCurrentElectionTitle
            // 
            this.lblCurrentElectionTitle.AutoSize = true;
            this.lblCurrentElectionTitle.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentElectionTitle.Location = new System.Drawing.Point(11, 22);
            this.lblCurrentElectionTitle.Name = "lblCurrentElectionTitle";
            this.lblCurrentElectionTitle.Size = new System.Drawing.Size(85, 14);
            this.lblCurrentElectionTitle.TabIndex = 8;
            this.lblCurrentElectionTitle.Text = "(No election)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(11, 8);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(95, 14);
            this.label7.TabIndex = 10;
            this.label7.Text = "Current election";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 14);
            this.label2.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.SeaGreen;
            this.label3.Location = new System.Drawing.Point(11, 119);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "Candidates finalized at";
            // 
            // lblClosedAt
            // 
            this.lblClosedAt.AutoSize = true;
            this.lblClosedAt.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblClosedAt.Location = new System.Drawing.Point(11, 189);
            this.lblClosedAt.Name = "lblClosedAt";
            this.lblClosedAt.Size = new System.Drawing.Size(19, 14);
            this.lblClosedAt.TabIndex = 6;
            this.lblClosedAt.Text = "---";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.IndianRed;
            this.label9.Location = new System.Drawing.Point(11, 175);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(57, 14);
            this.label9.TabIndex = 5;
            this.label9.Text = "Closed at";
            // 
            // lblCandidatesFinalizedAt
            // 
            this.lblCandidatesFinalizedAt.AutoSize = true;
            this.lblCandidatesFinalizedAt.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCandidatesFinalizedAt.Location = new System.Drawing.Point(11, 133);
            this.lblCandidatesFinalizedAt.Name = "lblCandidatesFinalizedAt";
            this.lblCandidatesFinalizedAt.Size = new System.Drawing.Size(19, 14);
            this.lblCandidatesFinalizedAt.TabIndex = 6;
            this.lblCandidatesFinalizedAt.Text = "---";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(11, 105);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(117, 14);
            this.label10.TabIndex = 6;
            // 
            // lblTookPlaceOnLabel
            // 
            this.lblTookPlaceOnLabel.AutoSize = true;
            this.lblTookPlaceOnLabel.ForeColor = System.Drawing.Color.Black;
            this.lblTookPlaceOnLabel.Location = new System.Drawing.Point(11, 82);
            this.lblTookPlaceOnLabel.Name = "lblTookPlaceOnLabel";
            this.lblTookPlaceOnLabel.Size = new System.Drawing.Size(79, 14);
            this.lblTookPlaceOnLabel.TabIndex = 5;
            this.lblTookPlaceOnLabel.Text = "Election date";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(294, 301);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.label7);
            this.tabPage1.Controls.Add(this.btnEdit);
            this.tabPage1.Controls.Add(this.lblCurrentElectionTitle);
            this.tabPage1.Controls.Add(this.btnOpenOrCloseElection);
            this.tabPage1.Controls.Add(this.lblDescription);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.lblTookPlaceOn);
            this.tabPage1.Controls.Add(this.lblClosedAt);
            this.tabPage1.Controls.Add(this.lblTookPlaceOnLabel);
            this.tabPage1.Controls.Add(this.lblUndoFinalize);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.lblCandidatesFinalizedAt);
            this.tabPage1.Location = new System.Drawing.Point(4, 23);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(8);
            this.tabPage1.Size = new System.Drawing.Size(286, 274);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "ELECTION";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // lblUndoFinalize
            // 
            this.lblUndoFinalize.AutoSize = true;
            this.lblUndoFinalize.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblUndoFinalize.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUndoFinalize.ForeColor = System.Drawing.Color.Black;
            this.lblUndoFinalize.Location = new System.Drawing.Point(11, 147);
            this.lblUndoFinalize.Name = "lblUndoFinalize";
            this.lblUndoFinalize.Size = new System.Drawing.Size(36, 14);
            this.lblUndoFinalize.TabIndex = 5;
            this.lblUndoFinalize.Text = "Undo";
            this.lblUndoFinalize.Visible = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Location = new System.Drawing.Point(4, 23);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(286, 274);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SYSTEM";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblCurrentDatabase);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSetupDatabase);
            this.groupBox1.Location = new System.Drawing.Point(8, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(270, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Database";
            // 
            // lblCurrentDatabase
            // 
            this.lblCurrentDatabase.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCurrentDatabase.Location = new System.Drawing.Point(114, 18);
            this.lblCurrentDatabase.Margin = new System.Windows.Forms.Padding(0);
            this.lblCurrentDatabase.Name = "lblCurrentDatabase";
            this.lblCurrentDatabase.Size = new System.Drawing.Size(150, 14);
            this.lblCurrentDatabase.TabIndex = 14;
            this.lblCurrentDatabase.Text = "Current database:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 18);
            this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 14);
            this.label1.TabIndex = 14;
            this.label1.Text = "Current database:";
            // 
            // btnSetupDatabase
            // 
            this.btnSetupDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnSetupDatabase.FlatAppearance.BorderSize = 0;
            this.btnSetupDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSetupDatabase.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSetupDatabase.Location = new System.Drawing.Point(6, 43);
            this.btnSetupDatabase.Name = "btnSetupDatabase";
            this.btnSetupDatabase.Size = new System.Drawing.Size(258, 23);
            this.btnSetupDatabase.TabIndex = 13;
            this.btnSetupDatabase.Text = "SET UP DATABASE";
            this.btnSetupDatabase.UseVisualStyleBackColor = false;
            this.btnSetupDatabase.Click += new System.EventHandler(this.btnSetupDatabase_Click);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(294, 301);
            this.Controls.Add(this.tabControl1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnOpenOrCloseElection;
        private System.Windows.Forms.Label lblTookPlaceOn;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblCurrentElectionTitle;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblClosedAt;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblCandidatesFinalizedAt;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label lblTookPlaceOnLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSetupDatabase;
        private System.Windows.Forms.Label lblUndoFinalize;
        private System.Windows.Forms.Label lblCurrentDatabase;
        private System.Windows.Forms.Label label1;
    }
}