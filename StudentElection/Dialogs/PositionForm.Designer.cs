namespace StudentElection.Dialogs
{
    partial class PositionForm
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
            this.txtPosition = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.label3 = new System.Windows.Forms.Label();
            this.dgPositions = new System.Windows.Forms.DataGridView();
            this.WinnersCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.YearLevel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nudNumberOfWinners = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbWhoCanVote = new System.Windows.Forms.ComboBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.WhoCanVoteWarningLabel = new System.Windows.Forms.Label();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.titleDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.rankDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.positionModelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgPositions)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfWinners)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionModelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // txtPosition
            // 
            this.txtPosition.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPosition.Location = new System.Drawing.Point(129, 256);
            this.txtPosition.MaxLength = 40;
            this.txtPosition.Name = "txtPosition";
            this.txtPosition.Size = new System.Drawing.Size(203, 22);
            this.txtPosition.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 258);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "Position Title";
            // 
            // btnDown
            // 
            this.btnDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnDown.Enabled = false;
            this.btnDown.FlatAppearance.BorderSize = 0;
            this.btnDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDown.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDown.Location = new System.Drawing.Point(272, 212);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(60, 23);
            this.btnDown.TabIndex = 6;
            this.btnDown.Text = "DOWN";
            this.btnDown.UseVisualStyleBackColor = false;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnUp.Enabled = false;
            this.btnUp.FlatAppearance.BorderSize = 0;
            this.btnUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUp.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUp.Location = new System.Drawing.Point(206, 212);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(60, 23);
            this.btnUp.TabIndex = 5;
            this.btnUp.Text = "UP";
            this.btnUp.UseVisualStyleBackColor = false;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdd.Location = new System.Drawing.Point(194, 366);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(66, 23);
            this.btnAdd.TabIndex = 3;
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
            this.btnCancel.Location = new System.Drawing.Point(266, 366);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(66, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "CANCEL";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Visible = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.DimGray;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "Double click a position to edit";
            // 
            // btnDelete
            // 
            this.btnDelete.BackColor = System.Drawing.Color.White;
            this.btnDelete.Enabled = false;
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDelete.Location = new System.Drawing.Point(12, 212);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(60, 23);
            this.btnDelete.TabIndex = 7;
            this.btnDelete.Text = "DELETE";
            this.btnDelete.UseVisualStyleBackColor = false;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 286);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "Number of winners";
            this.label3.Click += new System.EventHandler(this.label3_Click);
            // 
            // dgPositions
            // 
            this.dgPositions.AllowUserToAddRows = false;
            this.dgPositions.AllowUserToDeleteRows = false;
            this.dgPositions.AllowUserToResizeRows = false;
            this.dgPositions.AutoGenerateColumns = false;
            this.dgPositions.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgPositions.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgPositions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPositions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.titleDataGridViewTextBoxColumn,
            this.WinnersCount,
            this.YearLevel,
            this.rankDataGridViewTextBoxColumn});
            this.dgPositions.DataSource = this.positionModelBindingSource;
            this.dgPositions.Location = new System.Drawing.Point(12, 26);
            this.dgPositions.MultiSelect = false;
            this.dgPositions.Name = "dgPositions";
            this.dgPositions.ReadOnly = true;
            this.dgPositions.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgPositions.RowHeadersVisible = false;
            this.dgPositions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgPositions.Size = new System.Drawing.Size(320, 180);
            this.dgPositions.TabIndex = 13;
            this.dgPositions.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgPositions_CellDoubleClick);
            this.dgPositions.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgPositions_CellFormatting);
            this.dgPositions.SelectionChanged += new System.EventHandler(this.dgPositions_SelectionChanged);
            // 
            // WinnersCount
            // 
            this.WinnersCount.DataPropertyName = "WinnersCount";
            this.WinnersCount.HeaderText = "No. of Winners";
            this.WinnersCount.Name = "WinnersCount";
            this.WinnersCount.ReadOnly = true;
            this.WinnersCount.Width = 60;
            // 
            // YearLevel
            // 
            this.YearLevel.DataPropertyName = "YearLevel";
            this.YearLevel.HeaderText = "Who can vote?";
            this.YearLevel.Name = "YearLevel";
            this.YearLevel.ReadOnly = true;
            this.YearLevel.Width = 90;
            // 
            // nudNumberOfWinners
            // 
            this.nudNumberOfWinners.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudNumberOfWinners.Location = new System.Drawing.Point(129, 284);
            this.nudNumberOfWinners.Maximum = new decimal(new int[] {
            999,
            0,
            0,
            0});
            this.nudNumberOfWinners.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumberOfWinners.Name = "nudNumberOfWinners";
            this.nudNumberOfWinners.Size = new System.Drawing.Size(50, 22);
            this.nudNumberOfWinners.TabIndex = 1;
            this.nudNumberOfWinners.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.nudNumberOfWinners.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 313);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 14);
            this.label4.TabIndex = 5;
            this.label4.Text = "Who can vote?";
            this.toolTip1.SetToolTip(this.label4, "In addition, you can only add candidates with the year level of the selected vote" +
        "rs");
            this.label4.Click += new System.EventHandler(this.label3_Click);
            // 
            // cmbWhoCanVote
            // 
            this.cmbWhoCanVote.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cmbWhoCanVote.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbWhoCanVote.DropDownWidth = 48;
            this.cmbWhoCanVote.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbWhoCanVote.FormattingEnabled = true;
            this.cmbWhoCanVote.Items.AddRange(new object[] {
            "All voters",
            "Grade 1 only",
            "Grade 2 only",
            "Grade 3 only",
            "Grade 4 only",
            "Grade 5 only",
            "Grade 6 only",
            "Grade 7 only",
            "Grade 8 only",
            "Grade 9 only",
            "Grade 10 only",
            "Grade 11 only",
            "Grade 12 only"});
            this.cmbWhoCanVote.Location = new System.Drawing.Point(129, 310);
            this.cmbWhoCanVote.MaxDropDownItems = 12;
            this.cmbWhoCanVote.Name = "cmbWhoCanVote";
            this.cmbWhoCanVote.Size = new System.Drawing.Size(163, 22);
            this.cmbWhoCanVote.TabIndex = 2;
            this.cmbWhoCanVote.SelectedIndexChanged += new System.EventHandler(this.cmbWhoCanVote_SelectedIndexChanged);
            // 
            // toolTip1
            // 
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // WhoCanVoteWarningLabel
            // 
            this.WhoCanVoteWarningLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WhoCanVoteWarningLabel.ForeColor = System.Drawing.Color.Firebrick;
            this.WhoCanVoteWarningLabel.Location = new System.Drawing.Point(129, 335);
            this.WhoCanVoteWarningLabel.Name = "WhoCanVoteWarningLabel";
            this.WhoCanVoteWarningLabel.Size = new System.Drawing.Size(203, 28);
            this.WhoCanVoteWarningLabel.TabIndex = 5;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Visible = false;
            // 
            // titleDataGridViewTextBoxColumn
            // 
            this.titleDataGridViewTextBoxColumn.DataPropertyName = "Title";
            this.titleDataGridViewTextBoxColumn.HeaderText = "Position Title";
            this.titleDataGridViewTextBoxColumn.Name = "titleDataGridViewTextBoxColumn";
            this.titleDataGridViewTextBoxColumn.ReadOnly = true;
            this.titleDataGridViewTextBoxColumn.Width = 150;
            // 
            // rankDataGridViewTextBoxColumn
            // 
            this.rankDataGridViewTextBoxColumn.DataPropertyName = "Rank";
            this.rankDataGridViewTextBoxColumn.HeaderText = "Rank";
            this.rankDataGridViewTextBoxColumn.Name = "rankDataGridViewTextBoxColumn";
            this.rankDataGridViewTextBoxColumn.ReadOnly = true;
            this.rankDataGridViewTextBoxColumn.Visible = false;
            // 
            // positionModelBindingSource
            // 
            this.positionModelBindingSource.DataSource = typeof(StudentElection.Repository.Models.PositionModel);
            // 
            // PositionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(344, 401);
            this.Controls.Add(this.cmbWhoCanVote);
            this.Controls.Add(this.nudNumberOfWinners);
            this.Controls.Add(this.dgPositions);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnUp);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.btnDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.WhoCanVoteWarningLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtPosition);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PositionForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Position";
            this.Load += new System.EventHandler(this.PositionForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgPositions)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudNumberOfWinners)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.positionModelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtPosition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDelete;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dgPositions;
        private System.Windows.Forms.BindingSource positionModelBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn winnerCountDataGridViewTextBoxColumn;
        private System.Windows.Forms.NumericUpDown nudNumberOfWinners;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbWhoCanVote;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn titleDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn WinnersCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn YearLevel;
        private System.Windows.Forms.DataGridViewTextBoxColumn rankDataGridViewTextBoxColumn;
        private System.Windows.Forms.Label WhoCanVoteWarningLabel;
    }
}