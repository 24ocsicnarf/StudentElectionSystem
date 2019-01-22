using StudentElection.Classes;
using StudentElection.Main;
using StudentElection.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using StudentElection.Repository.Models;
using Project.Library.Helpers;

namespace StudentElection.UserControls
{
    /// <summary>
    /// Interaction logic for CandidateControl.xaml
    /// </summary>
    public partial class CandidateControl : UserControl
    {
        //public bool AreCandidatesFinalized = false;
        private bool _isPressed = false;

        public CandidateControl()
        {
            InitializeComponent();

            PreviewMouseLeftButtonDown += CandidateControl_PreviewMouseLeftButtonDown;
        }

        private void CandidateControl_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _isPressed = true;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            bdrCandidate.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            _isPressed = false;

            bdrCandidate.BorderBrush = new SolidColorBrush(Color.FromArgb(0, 64, 64, 64));
        }

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        private async void UserControl_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isPressed) return;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            Cursor = Cursors.Wait;

            G.WaitLang(window);

            window.Opacity = 0.5;

            var candidateViewer = new CandidateViewerForm();
            candidateViewer.Candidate = DataContext as CandidateModel;

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(candidateViewer, candidateViewer.Handle), -8, helper.Handle.ToInt32());

            candidateViewer.Tag = window;

            G.EndWait(window);

            Cursor = Cursors.Hand;

            candidateViewer.ShowDialog();

            G.WaitLang(window);
            
            if (candidateViewer.IsUpdated)
                DataContext = await candidateViewer.GetNewDataAsync();
            else if (candidateViewer.IsDeleted)
                await window.LoadCandidatesAsync();

            await window.LoadVotersAsync();

            G.EndWait(window);

            window.Opacity = 1;
        }

        private void Deselect()
        {
            stkNamePosition.Background = new SolidColorBrush(Colors.Black);
            tbkName.Foreground = new SolidColorBrush(Colors.White);
            tbkPosition.Foreground = new SolidColorBrush(Colors.White);
        }

        public void Select()
        {
            stkNamePosition.Background = new SolidColorBrush(Colors.White);
            tbkName.Foreground = new SolidColorBrush(Colors.Black);
            tbkPosition.Foreground = new SolidColorBrush(Colors.Black);
        }

        private void tbkPosition_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePositionBlock();
        }

        private void tbkName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeNameTextBlock();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeNameTextBlock();
            ResizePositionBlock();
        }

        private void ResizeNameTextBlock()
        {

            if (tbkName.ActualWidth > 144)
            {
                tbkName.Width = 144;
                tbkName.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkName.ToolTip = new TextBlock()
                {
                    Text = tbkName.Text,
                    TextWrapping = TextWrapping.Wrap
                };
                ToolTipService.SetShowDuration(tbkName, int.MaxValue);
            }

            if (tbkName.ActualWidth > 96)
            {
                vbName.Width = 96;
                vbName.Stretch = Stretch.Fill;
            }
            else if (tbkName.ActualWidth < 96)
            {
                vbName.Stretch = Stretch.Uniform;
                tbkName.Width = double.NaN;
                tbkName.ToolTip = null;
            }
        }

        private void ResizePositionBlock()
        {
            if (tbkPosition.ActualWidth > 144)
            {
                tbkPosition.Width = 144;
                tbkPosition.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkPosition.ToolTip = new TextBlock()
                {
                    Text = tbkPosition.Text,
                    TextWrapping = TextWrapping.Wrap
                };
                ToolTipService.SetShowDuration(tbkPosition, int.MaxValue);
            }

            if (tbkPosition.ActualWidth > 96)
            {
                vbPosition.Width = 96;
                vbPosition.Stretch = Stretch.Fill;
            }
            else if (tbkPosition.ActualWidth < 96)
            {
                vbPosition.Stretch = Stretch.Uniform;
                tbkName.Width = double.NaN;
                tbkPosition.ToolTip = null;
            }
        }
    }
}
