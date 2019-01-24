//using StudentElection.Classes;
using Project.Library.Helpers;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StudentElection.UserControls
{
    /// <summary>
    /// Interaction logic for CandidateResultControl.xaml
    /// </summary>
    public partial class CandidateResultControl : UserControl
    {
        public CandidateResultControl()
        {
            InitializeComponent();
        }

        private void tbkCount_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var voteResult = (e.NewValue as VoteResultModel);

            imgCandidate.Source = ImageHelper.ImageToImageSource(Properties.Resources.default_candidate);
            if (!voteResult.PictureFileName.IsBlank())
            {
                var imagePath = System.IO.Path.Combine(App.ImageFolderPath, voteResult.PictureFileName);
                if (System.IO.File.Exists(imagePath))
                {
                    using (var bmpTemp = new System.Drawing.Bitmap(imagePath))
                    {
                        imgCandidate.Source = ImageHelper.ImageToImageSource(new System.Drawing.Bitmap(bmpTemp));
                    }
                }
            }

            if (voteResult.VoteCount == 1)
            {
                tbkVoteLabel.Text = "vote";
            }
            else
            {
                tbkVoteLabel.Text = "votes";
            }
        }

        private void tbkName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeNameTextBlock();
            ResizeAliasTextBlock();
            ResizePartyTextBlock();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeNameTextBlock();
        }

        private void ResizeNameTextBlock()
        {
            if (tbkName.ActualWidth == 0) return;

            var actualWidth = ActualWidth - 114;
            if (tbkName.ActualWidth > actualWidth * 1.5)
            {
                tbkName.Width = actualWidth * 1.5;
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
                tbkName.Width = double.NaN;

                ToolTipService.SetToolTip(tbkName, null);
            }
        }


        private void ResizeAliasTextBlock()
        {
            if (tbkAlias.ActualWidth == 0) return;

            var actualWidth = ActualWidth - 114;
            if (tbkAlias.ActualWidth > actualWidth * 1.5)
            {
                tbkAlias.Width = actualWidth * 1.5;
                tbkAlias.TextTrimming = TextTrimming.CharacterEllipsis;

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


        private void ResizePartyTextBlock()
        {
            if (tbkParty.ActualWidth == 0) return;

            var actualWidth = ActualWidth - 114;
            if (tbkParty.ActualWidth > actualWidth * 1.5)
            {
                tbkParty.Width = actualWidth * 1.5;
                tbkParty.TextTrimming = TextTrimming.CharacterEllipsis;

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

        private void tbkParty_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizePartyTextBlock();
        }

        private void tbkAlias_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ResizeAliasTextBlock();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            ResizeNameTextBlock();
            ResizeAliasTextBlock();
            ResizePartyTextBlock();
        }
    }
}
