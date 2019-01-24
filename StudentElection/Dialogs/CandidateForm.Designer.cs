namespace StudentElection.Dialogs
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
            this.txtAlias = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblParty = new System.Windows.Forms.Label();
            this.cmbPosition = new System.Windows.Forms.ComboBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.ofdImage = new System.Windows.Forms.OpenFileDialog();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.ttpCandidate = new System.Windows.Forms.ToolTip(this.components);
            this.pbBorder = new System.Windows.Forms.PictureBox();
            this.btnPosition = new System.Windows.Forms.Button();
            this.txtFirstName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtMiddleName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLastName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtSuffix = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbYearLevel = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSection = new System.Windows.Forms.TextBox();
            this.positionModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dtBirthdate = new System.Windows.Forms.DateTimePicker();
            this.label10 = new System.Windows.Forms.Label();
            this.cmbSex = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbBorder)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionModelBindingSource)).BeginInit();
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
            this.lblBrowse.TabIndex = 11;
            this.lblBrowse.Text = "BROWSE";
            this.lblBrowse.Click += new System.EventHandler(this.lblBrowse_Click);
            // 
            // txtAlias
            // 
            this.txtAlias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAlias.Location = new System.Drawing.Point(200, 242);
            this.txtAlias.MaxLength = 30;
            this.txtAlias.Name = "txtAlias";
            this.txtAlias.Size = new System.Drawing.Size(248, 22);
            this.txtAlias.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(161, 245);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 14);
            this.label6.TabIndex = 1;
            this.label6.Text = "Alias*";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(142, 273);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 14);
            this.label7.TabIndex = 1;
            this.label7.Text = "Position*";
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
            this.cmbPosition.Location = new System.Drawing.Point(200, 270);
            this.cmbPosition.Name = "cmbPosition";
            this.cmbPosition.Size = new System.Drawing.Size(248, 22);
            this.cmbPosition.TabIndex = 10;
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(310, 325);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 23);
            this.btnAdd.TabIndex = 12;
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
            this.btnCancel.Location = new System.Drawing.Point(382, 325);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(66, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ofdImage
            // 
            this.ofdImage.Filter = "Image file (*.png, *.jpg, *.bmp) | *.png; *.jpg; *.bmp";
            this.ofdImage.ShowHelp = true;
            // 
            // pbImage
            // 
            this.pbImage.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.pbImage.Image = global::StudentElection.Properties.Resources.default_candidate;
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
            this.btnPosition.BackgroundImage = global::StudentElection.Properties.Resources.add;
            this.btnPosition.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.btnPosition.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPosition.FlatAppearance.BorderSize = 0;
            this.btnPosition.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPosition.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPosition.Location = new System.Drawing.Point(85, 240);
            this.btnPosition.Name = "btnPosition";
            this.btnPosition.Size = new System.Drawing.Size(22, 22);
            this.btnPosition.TabIndex = 3;
            this.btnPosition.UseVisualStyleBackColor = false;
            this.btnPosition.Visible = false;
            this.btnPosition.Click += new System.EventHandler(this.btnPosition_Click);
            // 
            // txtFirstName
            // 
            this.txtFirstName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFirstName.Location = new System.Drawing.Point(200, 77);
            this.txtFirstName.MaxLength = 30;
            this.txtFirstName.Name = "txtFirstName";
            this.txtFirstName.Size = new System.Drawing.Size(248, 22);
            this.txtFirstName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(129, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 14);
            this.label1.TabIndex = 1;
            this.label1.Text = "First Name*";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(117, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "Middle Name";
            // 
            // txtMiddleName
            // 
            this.txtMiddleName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMiddleName.Location = new System.Drawing.Point(200, 105);
            this.txtMiddleName.MaxLength = 30;
            this.txtMiddleName.Name = "txtMiddleName";
            this.txtMiddleName.Size = new System.Drawing.Size(248, 22);
            this.txtMiddleName.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(129, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 14);
            this.label3.TabIndex = 1;
            this.label3.Text = "Last Name*";
            // 
            // txtLastName
            // 
            this.txtLastName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLastName.Location = new System.Drawing.Point(200, 132);
            this.txtLastName.MaxLength = 30;
            this.txtLastName.Name = "txtLastName";
            this.txtLastName.Size = new System.Drawing.Size(170, 22);
            this.txtLastName.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(376, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 14);
            this.label4.TabIndex = 1;
            this.label4.Text = "Suffix";
            // 
            // txtSuffix
            // 
            this.txtSuffix.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSuffix.Location = new System.Drawing.Point(418, 132);
            this.txtSuffix.MaxLength = 30;
            this.txtSuffix.Name = "txtSuffix";
            this.txtSuffix.Size = new System.Drawing.Size(30, 22);
            this.txtSuffix.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(134, 164);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "Birthdate";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(129, 204);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 14);
            this.label8.TabIndex = 1;
            this.label8.Text = "Year Level*";
            // 
            // cmbYearLevel
            // 
            this.cmbYearLevel.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbYearLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbYearLevel.DropDownWidth = 48;
            this.cmbYearLevel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbYearLevel.FormattingEnabled = true;
            this.cmbYearLevel.Items.AddRange(new object[] {
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8",
            "9",
            "10",
            "11",
            "12"});
            this.cmbYearLevel.Location = new System.Drawing.Point(200, 201);
            this.cmbYearLevel.MaxDropDownItems = 12;
            this.cmbYearLevel.Name = "cmbYearLevel";
            this.cmbYearLevel.Size = new System.Drawing.Size(52, 22);
            this.cmbYearLevel.TabIndex = 7;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(255, 204);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(55, 14);
            this.label9.TabIndex = 1;
            this.label9.Text = "Section*";
            // 
            // txtSection
            // 
            this.txtSection.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSection.Location = new System.Drawing.Point(312, 202);
            this.txtSection.MaxLength = 30;
            this.txtSection.Name = "txtSection";
            this.txtSection.Size = new System.Drawing.Size(136, 22);
            this.txtSection.TabIndex = 8;
            // 
            // positionModelBindingSource
            // 
            this.positionModelBindingSource.DataSource = typeof(StudentElection.Repository.Models.PositionModel);
            // 
            // dtBirthdate
            // 
            this.dtBirthdate.Checked = false;
            this.dtBirthdate.CustomFormat = "yyyy-MM-dd";
            this.dtBirthdate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtBirthdate.Location = new System.Drawing.Point(200, 160);
            this.dtBirthdate.Name = "dtBirthdate";
            this.dtBirthdate.ShowCheckBox = true;
            this.dtBirthdate.Size = new System.Drawing.Size(124, 22);
            this.dtBirthdate.TabIndex = 4;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(330, 164);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 14);
            this.label10.TabIndex = 1;
            this.label10.Text = "Sex";
            // 
            // cmbSex
            // 
            this.cmbSex.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbSex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSex.DropDownWidth = 48;
            this.cmbSex.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbSex.FormattingEnabled = true;
            this.cmbSex.Items.AddRange(new object[] {
            "Male",
            "Female"});
            this.cmbSex.Location = new System.Drawing.Point(363, 161);
            this.cmbSex.MaxDropDownItems = 12;
            this.cmbSex.Name = "cmbSex";
            this.cmbSex.Size = new System.Drawing.Size(85, 22);
            this.cmbSex.TabIndex = 7;
            // 
            // CandidateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(460, 360);
            this.Controls.Add(this.dtBirthdate);
            this.Controls.Add(this.cmbSex);
            this.Controls.Add(this.cmbYearLevel);
            this.Controls.Add(this.btnPosition);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.cmbPosition);
            this.Controls.Add(this.pbImage);
            this.Controls.Add(this.txtSuffix);
            this.Controls.Add(this.txtSection);
            this.Controls.Add(this.txtLastName);
            this.Controls.Add(this.txtMiddleName);
            this.Controls.Add(this.txtFirstName);
            this.Controls.Add(this.txtAlias);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblParty);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label6);
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
            ((System.ComponentModel.ISupportInitialize)(this.positionModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblBrowse;
        private System.Windows.Forms.TextBox txtAlias;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblParty;
        private System.Windows.Forms.ComboBox cmbPosition;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.OpenFileDialog ofdImage;
        private System.Windows.Forms.ToolTip ttpCandidate;
        private System.Windows.Forms.PictureBox pbBorder;
        private System.Windows.Forms.Button btnPosition;
        private System.Windows.Forms.TextBox txtFirstName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMiddleName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLastName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtSuffix;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbYearLevel;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSection;
        private System.Windows.Forms.BindingSource positionModelBindingSource;
        private System.Windows.Forms.DateTimePicker dtBirthdate;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox cmbSex;
    }
}