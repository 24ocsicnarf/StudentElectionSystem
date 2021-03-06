﻿using Project.Library.Helpers;
using StudentElection.Repository.Models;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentElection.UserControls
{
    /// <summary>
    /// Interaction logic for CandidateBallotControl.xaml
    /// </summary>
    public partial class CandidateBallotControl : UserControl
    {
        public bool IsPressed, IsReleased;
        public bool IsSelected;
        public static int SelectedCount;

        public CandidateBallotControl()
        {
            InitializeComponent();
            IsSelected = false;

            SelectedCount = 0;
        }
        
        public void Select()
        {
            recEgg.Fill = new SolidColorBrush(Colors.Black);
            grdBackground.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));

            IsSelected = true;

            SelectedCount++;
        }

        public void Deselect()
        {
            recEgg.Fill = new SolidColorBrush(Color.FromArgb(0, 224, 224, 224));
            grdBackground.Background = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
            
            IsSelected = false;

            SelectedCount--;
        }

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                grdBackground.Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
            }
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            IsPressed = false;
            IsReleased = false;

            if (!IsSelected)
            {
                recEgg.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                grdBackground.Background = new SolidColorBrush(Color.FromArgb(255, 224, 224, 224));
            }
        }

        public int GetSelectedCount()
        {
            return SelectedCount;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //ResizeNameTextBlock();
            //ResizeAliasTextBlock();
            //ResizePartyTextBlock();
        }

        private void tbkName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeNameTextBlock();
        }

        private void tbkAlias_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeAliasTextBlock();
        }

        private void tbkParty_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void ResizeNameTextBlock()
        {
            var actualWidth = ActualWidth - 148;
            if (tbkName.ActualWidth > actualWidth * 1.5)
            {
                tbkName.Width = actualWidth * 1.5;
                tbkName.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkName.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkName.Text
                };
                ToolTipService.SetShowDuration(tbkName, int.MaxValue);
            }

            //Console.WriteLine(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width + "x" + System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height + " - " + actualWidth);

            if (tbkName.ActualWidth > actualWidth)
            {
                vbName.Width = actualWidth;
                vbName.Stretch = Stretch.Fill;
            }
            else
            {
                vbName.Stretch = Stretch.Uniform;
                tbkName.Width = double.NaN;
                //tbkName.FontSize = 20;
                tbkName.ToolTip = null;
            }
        }

        private void ResizeAliasTextBlock()
        {
            var actualWidth = ActualWidth - 148;
            if (tbkAlias.ActualWidth > actualWidth * 1.5)
            {
                tbkAlias.Width = actualWidth * 1.5;
                tbkAlias.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkAlias.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkAlias.Text
                };
                ToolTipService.SetShowDuration(tbkAlias, int.MaxValue);
            }

            if (tbkAlias.ActualWidth > actualWidth)
            {
                vbAlias.Width = actualWidth;
                vbAlias.Stretch = Stretch.Fill;
            }
            else
            {
                vbAlias.Stretch = Stretch.Uniform;
                tbkAlias.Width = double.NaN;
                ToolTipService.SetToolTip(tbkAlias, null);
            }
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var candidate = e.NewValue as CandidateModel;

            //imgCandidate.Source = ImageHelper.ImageToImageSourceAsync(Properties.Resources.default_candidate);
            if (!string.IsNullOrWhiteSpace(candidate.PictureFileName))
            {
                var imagePath = System.IO.Path.Combine(App.ImageFolderPath, candidate.PictureFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    imgCandidate.Source = ImageHelper.ImageToImageSource(imagePath);
                }
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            imgCandidate.Source = null;
            UpdateLayout();
            GC.Collect();
        }

        private void ResizePartyTextBlock()
        {
            var actualWidth = ActualWidth - 148;
            if (tbkParty.ActualWidth > actualWidth * 1.5)
            {
                tbkParty.Width = actualWidth * 1.5;
                tbkParty.TextTrimming = TextTrimming.CharacterEllipsis;

                tbkParty.ToolTip = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = tbkParty.Text
                };
                ToolTipService.SetShowDuration(tbkParty, int.MaxValue);
            }

            if (tbkParty.ActualWidth > actualWidth)
            {
                vbParty.Width = actualWidth;
                vbParty.Stretch = Stretch.Fill;
            }
            else
            {
                vbParty.Stretch = Stretch.Uniform;
                tbkParty.Width = double.NaN;
                ToolTipService.SetToolTip(tbkParty, null);
            }
        }
    }
}
