using LNHSVoting.Classes;
using LNHSVoting.LNHSVotingDataSetTableAdapters;
using LNHSVoting.Main;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace LNHSVoting
{
    /// <summary>
    /// Interaction logic for PositionControl.xaml
    /// </summary>
    public partial class PartyItemControl : UserControl
    {

        public bool AreCandidatesFinalized = false;

        private double _hOffset = -1;

        public PartyItemControl()
        {
            InitializeComponent();

            tbkParty.DataContextChanged += (s, ev) =>
            {
                tbkParty.Width = double.NaN;
            };
        }
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong);
        private void btnAddCandidate_Click(object sender, RoutedEventArgs e)
        {
            btnAddCandidate.Cursor = Cursors.Wait;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            G.WaitLang(window);

            window.Opacity = 0.5;

            _hOffset = svrCandidate.HorizontalOffset;
            var candidateForm = new CandidateForm();
            candidateForm.Candidate = null;
            candidateForm.SetParty(DataContext as Party);

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(candidateForm, candidateForm.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(window);

            btnAddCandidate.Cursor = Cursors.Hand;

            candidateForm.ShowDialog();
            G.WaitLang(window);

            if (candidateForm.IsLoadPosition)
            {
                G.WaitLang(window);

                var form = new PositionForm();
                form.Tag = window;

                G.EndWait(window);
                form.ShowDialog();
            }
            else if (!candidateForm.IsCanceled)
            {
                try
                {
                    stkCandidate.Children.Clear();

                    var candidateParty = Candidates.Dictionary.Values.Where(x => x.Party.ID == (DataContext as Party).ID).OrderBy(x => x.Position.Order);

                    if (candidateParty.Count() > 0)
                        stkCandidate.Visibility = Visibility.Visible;
                    else
                        stkCandidate.Visibility = Visibility.Hidden;
                    

                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        candidateControl.DataContext = candidate;
                        stkCandidate.Children.Add(candidateControl);
                    }

                    window.LoadVoters();

                    lblCount.Content = stkCandidate.Children.Count + "";

                    if (_hOffset != -1)
                    {
                        svrCandidate.ScrollToHorizontalOffset(_hOffset);
                    }
                }
                catch (Exception ex)
                {
                    G.EndWait(window);

                    MessageBox.Show(ex.GetBaseException().Message + "\n" + ex.StackTrace, "PROGRAM ERROR: " + ex.Source, MessageBoxButton.OK, MessageBoxImage.Stop);

                    Application.Current?.Shutdown();
                }
            }
            
            G.EndWait(window);
            window.Opacity = 1;
        }

        private void tbkParty_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (AreCandidatesFinalized)
            {
                return;
            }

            var tbk = sender as TextBlock;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            G.WaitLang(window);
            Cursor = Cursors.Wait;

            window.Opacity = 0.5;
            
            var form = new PartyForm();
            form.Party = DataContext as Party;

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(window);
            Cursor = Cursors.Arrow;

            form.ShowDialog();

            G.WaitLang(window);

            var party = form.GetNewData();

            if (!form.IsDeleted && party != null) 
            {
                DataContext = party;
                foreach (CandidateControl control in stkCandidate.Children)
                {
                    var temp = control.DataContext as Candidate;
                    var candidate = new Candidate()
                    {
                        Alias = temp.Alias,
                        Party = party,
                        PictureByte = temp.PictureByte,
                        Position = temp.Position,
                        PositionVoteCount = temp.PositionVoteCount,
                        VoteCount = temp.VoteCount
                    };
                    
                    control.DataContext = candidate;
                }
                window.LoadVoters();
            }
            else
            {
                window.LoadVoters();
                window.LoadCandidates();
            }
            
            window.Opacity = 1;

            G.EndWait(window);
        }

        private void svrCandidate_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToHorizontalOffset(scv.HorizontalOffset - e.Delta);
            e.Handled = true;
        }

        private void tbkParty_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void ResizePartyTextBlock()
        {
            if (tbkParty.ActualWidth == 0) return;

            var partyWidth = ActualWidth - 48 - lblCount.ActualWidth - btnAddCandidate.ActualWidth - btnAddCandidate.Margin.Left - btnAddCandidate.Margin.Right - btnAddCandidate.Padding.Left - btnAddCandidate.Padding.Right;

            if (tbkParty.ActualWidth > partyWidth * 1.5)
            {
                tbkParty.Width = partyWidth * 1.5;
                tbkParty.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkParty.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkParty.Text
                };
                ToolTipService.SetShowDuration(tbkParty, int.MaxValue);
            }

            if (tbkParty.ActualWidth > partyWidth)
            {
                vbParty.Width = partyWidth;
                vbParty.Stretch = Stretch.Fill;
            }
            else
            {
                vbParty.Stretch = Stretch.Uniform;
                tbkParty.Width = double.NaN;
                tbkParty.ToolTip = null;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ResizePartyTextBlock();
        }
    }

    public class ToUpperValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return value.ToString().ToUpper();
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
