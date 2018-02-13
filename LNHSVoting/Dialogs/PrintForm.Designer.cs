namespace LNHSVoting
{
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
            this.components = new System.ComponentModel.Container();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource1 = new Microsoft.Reporting.WinForms.ReportDataSource();
            Microsoft.Reporting.WinForms.ReportDataSource reportDataSource2 = new Microsoft.Reporting.WinForms.ReportDataSource();
            this.VoteResultsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.LNHSVotingDataSet = new LNHSVoting.LNHSVotingDataSet();
            this.MachineBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.reportViewer1 = new Microsoft.Reporting.WinForms.ReportViewer();
            this.VoteResultsTableAdapter = new LNHSVoting.LNHSVotingDataSetTableAdapters.VoteResultsTableAdapter();
            this.MachineTableAdapter = new LNHSVoting.LNHSVotingDataSetTableAdapters.MachineTableAdapter();
            ((System.ComponentModel.ISupportInitialize)(this.VoteResultsBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LNHSVotingDataSet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MachineBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // VoteResultsBindingSource
            // 
            this.VoteResultsBindingSource.DataMember = "VoteResults";
            this.VoteResultsBindingSource.DataSource = this.LNHSVotingDataSet;
            // 
            // LNHSVotingDataSet
            // 
            this.LNHSVotingDataSet.DataSetName = "LMNSVotingDataSet";
            this.LNHSVotingDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // MachineBindingSource
            // 
            this.MachineBindingSource.DataMember = "Machine";
            this.MachineBindingSource.DataSource = this.LNHSVotingDataSet;
            // 
            // reportViewer1
            // 
            this.reportViewer1.Dock = System.Windows.Forms.DockStyle.Fill;
            reportDataSource1.Name = "VoteResultsDataSet";
            reportDataSource1.Value = this.VoteResultsBindingSource;
            reportDataSource2.Name = "MiscDataSet";
            reportDataSource2.Value = this.MachineBindingSource;
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource1);
            this.reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            this.reportViewer1.LocalReport.ReportEmbeddedResource = "LNHSVoting.Reports.VoteResultReport.rdlc";
            this.reportViewer1.Location = new System.Drawing.Point(0, 0);
            this.reportViewer1.Name = "reportViewer1";
            this.reportViewer1.Size = new System.Drawing.Size(624, 444);
            this.reportViewer1.TabIndex = 0;
            // 
            // VoteResultsTableAdapter
            // 
            this.VoteResultsTableAdapter.ClearBeforeFill = true;
            // 
            // MachineTableAdapter
            // 
            this.MachineTableAdapter.ClearBeforeFill = true;
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
            ((System.ComponentModel.ISupportInitialize)(this.VoteResultsBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LNHSVotingDataSet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MachineBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource VoteResultsBindingSource;
        private LNHSVotingDataSet LNHSVotingDataSet;
        private LNHSVotingDataSetTableAdapters.VoteResultsTableAdapter VoteResultsTableAdapter;
        private System.Windows.Forms.BindingSource MachineBindingSource;
        private LNHSVotingDataSetTableAdapters.MachineTableAdapter MachineTableAdapter;
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
    }
}