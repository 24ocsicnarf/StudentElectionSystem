﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LNHSVoting
{
    public partial class PrintForm : Form
    {
        public PrintForm()
        {
            InitializeComponent();
        }

        private void PrintForm_Load(object sender, EventArgs e)
        {
            Print();
        }

        public void Print()
        {
            this.VoteResultsTableAdapter.Fill(this.LNHSVotingDataSet.VoteResults);
            this.MachineTableAdapter.Fill(this.LNHSVotingDataSet.Machine);
            
            reportViewer1.SetDisplayMode(Microsoft.Reporting.WinForms.DisplayMode.PrintLayout);
            reportViewer1.ZoomPercent = 100;
            reportViewer1.ZoomMode = Microsoft.Reporting.WinForms.ZoomMode.Percent;
            reportViewer1.LocalReport.DisplayName = "SSG Elections 2018 Partial and Unofficial Results - " + Classes.Machine.Tag;
        }
    }
}
