using StudentElection.Classes;
using StudentElection.Dialogs;
using StudentElection.Main;
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
using StudentElection.Repository.Models;
using StudentElection.Services;
using Project.Library.Helpers;
using System.Drawing;

namespace StudentElection.UserControls
{
    /// <summary>
    /// Interaction logic for PositionControl.xaml
    /// </summary>
    public partial class PartyItemControl : UserControl
    {
        public bool AreCandidatesFinalized = false;

        private double _hOffset = -1;
        
        private readonly CandidateService _candidateService = new CandidateService();

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
        private async void btnAddCandidate_Click(object sender, RoutedEventArgs e)
        {
            btnAddCandidate.Cursor = Cursors.Wait;

            Window parentWindow = Window.GetWindow(this);
            MaintenanceWindow window = parentWindow as MaintenanceWindow;

            G.WaitLang(window);

            window.Opacity = 0.5;

            _hOffset = svrCandidate.HorizontalOffset;
            var candidateForm = new CandidateForm();
            candidateForm.Candidate = null;
            candidateForm.SetParty(DataContext as PartyModel);

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

                    var partyId = (DataContext as PartyModel).Id;
                    
                    var candidateParty = await _candidateService.GetCandidatesByPartyAsync(partyId);

                    if (candidateParty.Count() > 0)
                        stkCandidate.Visibility = Visibility.Visible;
                    else
                        stkCandidate.Visibility = Visibility.Hidden;
                    

                    foreach (var candidate in candidateParty)
                    {
                        var candidateControl = new CandidateControl();
                        if (!candidate.PictureFileName.IsBlank())
                        {
                            var imagePath = System.IO.Path.Combine(App.ImageFolderPath, candidate.PictureFileName);
                            using (var bmpTemp = new System.Drawing.Bitmap(imagePath))
                            {
                                candidateControl.imgCandidate.Source = ImageHelper.ImageToImageSource(new System.Drawing.Bitmap(bmpTemp));
                            }
                        }
                        else
                        {
                            candidateControl.imgCandidate.Source = ImageHelper.ImageToImageSource(Properties.Resources.default_candidate);
                        }

                        candidateControl.DataContext = candidate;
                        stkCandidate.Children.Add(candidateControl);
                    }

                    await window.LoadVotersAsync();

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

        private async void tbkParty_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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
            form.Party = DataContext as PartyModel;

            WindowInteropHelper helper = new WindowInteropHelper(window);
            SetWindowLong(new HandleRef(form, form.Handle), -8, helper.Handle.ToInt32());

            G.EndWait(window);
            Cursor = Cursors.Arrow;

            form.ShowDialog();

            G.WaitLang(window);

            var party = await form.GetNewDataAsync();

            if (!form.IsDeleted && party != null) 
            {
                DataContext = party;
                foreach (CandidateControl control in stkCandidate.Children)
                {
                    var candidate = control.DataContext as CandidateModel;
                    candidate.Party = party;

                    //var candidate = new CandidateModel
                    //{
                    //    Alias = temp.Alias,
                    //    Party = party,
                    //    //TODO: PictureByte = temp.PictureByte,
                    //    Position = temp.Position,
                    //    //TODO: PositionVoteCount = temp.PositionVoteCount,
                    //    //TODO: VoteCount = temp.VoteCount
                    //};

                    if (!candidate.PictureFileName.IsBlank())
                    {
                        var imagePath = System.IO.Path.Combine(App.ImageFolderPath, candidate.PictureFileName);
                        using (var bmpTemp = new Bitmap(imagePath))
                        {
                            control.imgCandidate.Source = ImageHelper.ImageToImageSource(new Bitmap(bmpTemp));
                        }
                    }
                    else
                    {
                        control.imgCandidate.Source = ImageHelper.ImageToImageSource(Properties.Resources.default_candidate);
                    }

                    control.DataContext = candidate;
                }
                await window.LoadVotersAsync();
            }
            else
            {
                await window.LoadVotersAsync();
                await window.LoadCandidatesAsync();
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
}
