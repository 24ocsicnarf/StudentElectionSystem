using LNHSVoting.LNHSVotingDataSetTableAdapters;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LNHSVoting
{
    /// <summary>
    /// Interaction logic for PositionControl.xaml
    /// </summary>
    public partial class PositionItemControl : UserControl
    {

        public PositionItemControl()
        {
            InitializeComponent();
        }
        
        private void tbkPosition_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePositionTextBlock();
        }

        private void tbkName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeNameTextBlock();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            wrpCandidate.ItemWidth = (ActualWidth / 3) < 472 ? (ActualWidth / 2) - 8 : (ActualWidth / 3) - 8;

            ResizeNameTextBlock();
            ResizePositionTextBlock();
        }

        private void ResizeNameTextBlock()
        {
            if (tbkName.ActualWidth == 0) return;

            var actualWidth = cdfCandidate.ActualWidth - recColor.ActualWidth;
            if (tbkName.ActualWidth > actualWidth * 1.5)
            {
                lblName.Width = actualWidth * 1.5;
                tbkName.TextTrimming = TextTrimming.CharacterEllipsis;
                
                ToolTipService.SetShowDuration(tbkName, int.MaxValue);
            }

            if (tbkName.ActualWidth > actualWidth)
            {
                vbName.Width = actualWidth;
                vbName.Stretch = Stretch.Fill;
            }
            else
            {
                vbName.Stretch = Stretch.Uniform;
                lblName.Width = double.NaN;

                tbkName.ToolTip = null;
            }
        }

        private void ResizePositionTextBlock()
        {
            if (tbkPosition.ActualWidth == 0) return;

            var actualWidth = cdfPosition.ActualWidth - recColor.ActualWidth;

            if (tbkPosition.ActualWidth > actualWidth * 1.5)
            {
                tbkPosition.Width = actualWidth * 1.5;
                tbkPosition.TextTrimming = TextTrimming.CharacterEllipsis;

                ToolTipService.SetToolTip(tbkPosition, tbkPosition.Text);
                ToolTipService.SetShowDuration(tbkPosition, int.MaxValue);
            }

            if (tbkPosition.ActualWidth > actualWidth)
            {
                vbPosition.Width = actualWidth;
                vbPosition.Stretch = Stretch.Fill;
            }
            else
            {
                vbPosition.Stretch = Stretch.Uniform;
                tbkPosition.Width = double.NaN;

                ToolTipService.SetToolTip(tbkPosition, null);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ResizeNameTextBlock();
            ResizePositionTextBlock();
        }
    }
}
