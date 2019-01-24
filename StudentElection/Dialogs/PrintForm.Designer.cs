namespace StudentElection.Dialogs
{
    //using StudentElection.StudentElectionDataSetTableAdapters;

    partial class PrintForm
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
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.voteResultTableAdapter = new StudentElection.MSAccess.StudentElectionDataSetTableAdapters.VoteResultTableAdapter();
            this.studentElectionDataSet = new StudentElection.MSAccess.StudentElectionDataSet();
            ((System.ComponentModel.ISupportInitialize)(this.studentElectionDataSet)).BeginInit();
            this.SuspendLayout();
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "VoteResultsDataSet";
            reportDataSource1.Value = null;
            reportDataSource2.Name = "MiscDataSet";
            reportDataSource2.Value = null;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "StudentElection.Reports.VoteResultReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(624, 444);
            this.reportViewer1.TabIndex = 0;
            // 
            // voteResultTableAdapter
            // 
            this.voteResultTableAdapter.ClearBeforeFill = true;
            // 
            // studentElectionDataSet
            // 
            this.studentElectionDataSet.DataSetName = "StudentElectionDataSet";
            this.studentElectionDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // PrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 444);
            this.Controls.Add(this.reportViewer1);
            this.Name = "PrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Print";
            this.Load += new System.EventHandler(this.PrintForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.studentElectionDataSet)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private MSAccess.StudentElectionDataSetTableAdapters.VoteResultTableAdapter voteResultTableAdapter;
        private MSAccess.StudentElectionDataSet studentElectionDataSet;
    }
}