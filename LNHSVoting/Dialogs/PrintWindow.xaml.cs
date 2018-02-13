using LNHSVoting.Classes;
using Syncfusion.Windows.Reports;
using Syncfusion.Windows.Reports.Viewer;
using Syncfusion.Windows.Tools.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LNHSVoting.Dialogs
{
    /// <summary>
    /// Interaction logic for PrintWindow.xaml
    /// </summary>
    public partial class PrintWindow : RibbonWindow
    {
        public PrintWindow()
        {
            InitializeComponent();
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            viewer.ViewMode = ViewMode.Print;
            viewer.ProcessingMode = ProcessingMode.Remote;
            viewer.ReportPath = System.IO.Path.GetFullPath(@"Reports\ElectionPartialAndUnofficialResultReportDesign.rdl");
            viewer.ExportByteCompleted += (s, ev) =>
            {
                MessageBox.Show("Successfully exported!");
            };
            //viewer.DataSources.Clear();
            //viewer.DataSources.Add(new ReportDataSource { Name = "VoteResults", Value = Ballots.Results });
            viewer.RefreshReport();
        }
    }
}
