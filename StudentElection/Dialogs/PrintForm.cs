using Microsoft.Reporting.WinForms;
using StudentElection.Repository.Models;
using StudentElection.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentElection.Dialogs
{
    public partial class PrintForm : Form
    {
        private readonly ElectionService _electionService = new ElectionService();

        private ElectionModel _currentElection;

        public PrintForm()
        {
            InitializeComponent();
        }

        private async void PrintForm_Load(object sender, EventArgs e)
        {
            _currentElection = await _electionService.GetCurrentElectionAsync();

            Print();
        }

        public void Print()
        {
            voteResultTableAdapter.FillVoteResults(studentElectionDataSet.VoteResult, _currentElection.Id);
            
            reportViewer1.SetDisplayMode(DisplayMode.PrintLayout);
            reportViewer1.ZoomPercent = 100;
            reportViewer1.ZoomMode = ZoomMode.Percent;

            reportViewer1.LocalReport.DisplayName = _currentElection.Title;
            reportViewer1.LocalReport.SetParameters(new ReportParameter("ImageFolderPath", App.ImageFolderPath));
        }
    }
}
