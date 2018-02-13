namespace LNHSVoting
{
    partial class CandidateViewerForm
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
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblVoterID = new System.Windows.Forms.Label();
            this.lblStrand = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.lblAlias = new System.Windows.Forms.Label();
            this.lblPosition = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.lblSex = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblBirthdate = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblParty = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ttpCandidate = new System.Windows.Forms.ToolTip(this.components);
            this.lblAbbreviation = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.pbBackImage = new System.Windows.Forms.PictureBox();
            this.pnlInfo = new System.Windows.Forms.Panel();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.lblCandidatePage = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackImage)).BeginInit();
            this.pnlInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // pbImage
            // 
            this.pbImage.BackColor = System.Drawing.Color.White;
            this.pbImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbImage.Image = global::LNHSVoting.Properties.Resources.default_candidate;
            this.pbImage.Location = new System.Drawing.Point(368, 22);
            this.pbImage.Name = "pbImage";
            this.pbImage.Size = new System.Drawing.Size(90, 120);
            this.pbImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbImage.TabIndex = 4;
            this.pbImage.TabStop = false;
            // 
            // lblVoterID
            // 
            this.lblVoterID.Font = new System.Drawing.Font("Arial Narrow", 9.7F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVoterID.Location = new System.Drawing.Point(342, 146);
            this.lblVoterID.Margin = new System.Windows.Forms.Padding(3);
            this.lblVoterID.Name = "lblVoterID";
            this.lblVoterID.Size = new System.Drawing.Size(143, 15);
            this.lblVoterID.TabIndex = 6;
            this.lblVoterID.Text = "WWWWWWWWWWWW";
            this.lblVoterID.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lblStrand
            // 
            this.lblStrand.AutoSize = true;
            this.lblStrand.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStrand.Location = new System.Drawing.Point(115, 50);
            this.lblStrand.Margin = new System.Windows.Forms.Padding(3);
            this.lblStrand.Name = "lblStrand";
            this.lblStrand.Size = new System.Drawing.Size(79, 14);
            this.lblStrand.TabIndex = 7;
            this.lblStrand.Text = "Grade Strand";
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true;
            this.lblName.AutoSize = true;
            this.lblName.BackColor = System.Drawing.Color.Transparent;
            this.lblName.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblName.Location = new System.Drawing.Point(115, 20);
            this.lblName.Margin = new System.Windows.Forms.Padding(3);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(59, 14);
            this.lblName.TabIndex = 8;
            this.lblName.Text = "Full Name";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(38, 18);
            this.label7.Margin = new System.Windows.Forms.Padding(3);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 14);
            this.label7.TabIndex = 9;
            this.label7.Text = "Position";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(60, -2);
            this.label6.Margin = new System.Windows.Forms.Padding(3);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(35, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "Alias";
            // 
            // lblAlias
            // 
            this.lblAlias.AutoEllipsis = true;
            this.lblAlias.AutoSize = true;
            this.lblAlias.BackColor = System.Drawing.Color.Transparent;
            this.lblAlias.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAlias.Location = new System.Drawing.Point(115, 146);
            this.lblAlias.Margin = new System.Windows.Forms.Padding(3);
            this.lblAlias.Name = "lblAlias";
            this.lblAlias.Size = new System.Drawing.Size(30, 14);
            this.lblAlias.TabIndex = 10;
            this.lblAlias.Text = "Alias";
            // 
            // lblPosition
            // 
            this.lblPosition.AutoEllipsis = true;
            this.lblPosition.AutoSize = true;
            this.lblPosition.BackColor = System.Drawing.Color.Transparent;
            this.lblPosition.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPosition.Location = new System.Drawing.Point(115, 166);
            this.lblPosition.Margin = new System.Windows.Forms.Padding(3);
            this.lblPosition.Name = "lblPosition";
            this.lblPosition.Size = new System.Drawing.Size(49, 14);
            this.lblPosition.TabIndex = 9;
            this.lblPosition.Text = "Position";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(396, 252);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(72, 24);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "CLOSE";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnEdit.FlatAppearance.BorderSize = 0;
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEdit.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Location = new System.Drawing.Point(318, 252);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(72, 24);
            this.btnEdit.TabIndex = 0;
            this.btnEdit.Text = "EDIT";
            this.btnEdit.UseVisualStyleBackColor = false;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // lblSex
            // 
            this.lblSex.AutoSize = true;
            this.lblSex.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSex.Location = new System.Drawing.Point(115, 70);
            this.lblSex.Margin = new System.Windows.Forms.Padding(3);
            this.lblSex.Name = "lblSex";
            this.lblSex.Size = new System.Drawing.Size(27, 14);
            this.lblSex.TabIndex = 13;
            this.lblSex.Text = "Sex";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(45, 20);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Full Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 50);
            this.label2.Margin = new System.Windows.Forms.Padding(3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "Grade/Strand";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(80, 70);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "Sex";
            // 
            // lblBirthdate
            // 
            this.lblBirthdate.AutoSize = true;
            this.lblBirthdate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBirthdate.Location = new System.Drawing.Point(115, 90);
            this.lblBirthdate.Margin = new System.Windows.Forms.Padding(3);
            this.lblBirthdate.Name = "lblBirthdate";
            this.lblBirthdate.Size = new System.Drawing.Size(57, 14);
            this.lblBirthdate.TabIndex = 13;
            this.lblBirthdate.Text = "Birthdate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(44, 90);
            this.label8.Margin = new System.Windows.Forms.Padding(3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 14);
            this.label8.TabIndex = 5;
            this.label8.Text = "Birthdate";
            // 
            // lblParty
            // 
            this.lblParty.AutoEllipsis = true;
            this.lblParty.AutoSize = true;
            this.lblParty.BackColor = System.Drawing.Color.Transparent;
            this.lblParty.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParty.ForeColor = System.Drawing.Color.Black;
            this.lblParty.Location = new System.Drawing.Point(115, 186);
            this.lblParty.Margin = new System.Windows.Forms.Padding(3);
            this.lblParty.Name = "lblParty";
            this.lblParty.Size = new System.Drawing.Size(35, 14);
            this.lblParty.TabIndex = 14;
            this.lblParty.Text = "Party";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(55, 38);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 14);
            this.label4.TabIndex = 15;
            this.label4.Text = "Party";
            // 
            // ttpCandidate
            // 
            this.ttpCandidate.AutoPopDelay = 5000;
            this.ttpCandidate.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ttpCandidate.ForeColor = System.Drawing.Color.Black;
            this.ttpCandidate.InitialDelay = 500;
            this.ttpCandidate.ReshowDelay = 100;
            // 
            // lblAbbreviation
            // 
            this.lblAbbreviation.AutoSize = true;
            this.lblAbbreviation.BackColor = System.Drawing.Color.Transparent;
            this.lblAbbreviation.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAbbreviation.ForeColor = System.Drawing.Color.Black;
            this.lblAbbreviation.Location = new System.Drawing.Point(115, 206);
            this.lblAbbreviation.Margin = new System.Windows.Forms.Padding(3);
            this.lblAbbreviation.Name = "lblAbbreviation";
            this.lblAbbreviation.Size = new System.Drawing.Size(35, 14);
            this.lblAbbreviation.TabIndex = 14;
            this.lblAbbreviation.Text = "Party";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(12, 252);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(72, 24);
            this.btnDelete.TabIndex = 2;
            this.btnDelete.Text = "DELETE";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // pbBackImage
            // 
            this.pbBackImage.Location = new System.Drawing.Point(366, 20);
            this.pbBackImage.Name = "pbBackImage";
            this.pbBackImage.Size = new System.Drawing.Size(94, 124);
            this.pbBackImage.TabIndex = 16;
            this.pbBackImage.TabStop = false;
            // 
            // pnlInfo
            // 
            this.pnlInfo.Controls.Add(this.label4);
            this.pnlInfo.Controls.Add(this.label6);
            this.pnlInfo.Controls.Add(this.label7);
            this.pnlInfo.Location = new System.Drawing.Point(14, 148);
            this.pnlInfo.Name = "pnlInfo";
            this.pnlInfo.Size = new System.Drawing.Size(95, 57);
            this.pnlInfo.TabIndex = 17;
            // 
            // btnNext
            // 
            this.btnNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnNext.FlatAppearance.BorderSize = 0;
            this.btnNext.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNext.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.ForeColor = System.Drawing.Color.Black;
            this.btnNext.Location = new System.Drawing.Point(413, 167);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(45, 24);
            this.btnNext.TabIndex = 18;
            this.btnNext.Text = ">>";
            this.btnNext.UseVisualStyleBackColor = false;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnPrev.FlatAppearance.BorderSize = 0;
            this.btnPrev.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPrev.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrev.ForeColor = System.Drawing.Color.Black;
            this.btnPrev.Location = new System.Drawing.Point(368, 167);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(45, 24);
            this.btnPrev.TabIndex = 18;
            this.btnPrev.Text = "<<";
            this.btnPrev.UseVisualStyleBackColor = false;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // lblCandidatePage
            // 
            this.lblCandidatePage.Font = new System.Drawing.Font("Arial Narrow", 9.7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCandidatePage.ForeColor = System.Drawing.Color.Black;
            this.lblCandidatePage.Location = new System.Drawing.Point(350, 197);
            this.lblCandidatePage.Margin = new System.Windows.Forms.Padding(3);
            this.lblCandidatePage.Name = "lblCandidatePage";
            this.lblCandidatePage.Size = new System.Drawing.Size(126, 15);
            this.lblCandidatePage.TabIndex = 6;
            this.lblCandidatePage.Text = "0 of 0";
            this.lblCandidatePage.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblCandidatePage.Click += new System.EventHandler(this.label5_Click);
            // 
            // CandidateViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(480, 288);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlInfo);
            this.Controls.Add(this.lblAbbreviation);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblParty);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblBirthdate);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.lblSex);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lblPosition);
            this.Controls.Add(this.lblAlias);
            this.Controls.Add(this.lblStrand);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblCandidatePage);
            this.Controls.Add(this.lblVoterID);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.pbBackImage);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "CandidateViewerForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "lblAbbreviation";
            this.Load += new System.EventHandler(this.CandidateViewerForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBackImage)).EndInit();
            this.pnlInfo.ResumeLayout(false);
            this.pnlInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label lblVoterID;
        private System.Windows.Forms.Label lblStrand;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label lblAlias;
        private System.Windows.Forms.Label lblPosition;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label lblSex;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblBirthdate;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblParty;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolTip ttpCandidate;
        private System.Windows.Forms.Label lblAbbreviation;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.PictureBox pbBackImage;
        private System.Windows.Forms.Panel pnlInfo;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Label lblCandidatePage;
    }
}