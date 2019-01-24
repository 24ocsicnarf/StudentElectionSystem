//using StudentElection.Classes;
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
    /// Interaction logic for VotedCandidateControl.xaml
    /// </summary>
    public partial class VotedCandidateControl : UserControl
    {
        public VotedCandidateControl()
        {
            InitializeComponent();
        }

        private void tbkName_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var candidate = e.NewValue as CandidateModel;
            if (candidate.Id == 0)
            {
                tbkName.Text = "(No candidate chosen)";
                tbkName.Opacity = 0.5;

                recParty.ToolTip = null;
            }
        }

        private void tbkName_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (tbkName.ActualWidth > 455)
            {
                tbkName.TextTrimming = TextTrimming.CharacterEllipsis;
                tbkName.Width = 455;

                tbkName.ToolTip = new TextBlock()
                {
                    Text = tbkName.Text,
                    TextWrapping = TextWrapping.Wrap
                };
                ToolTipService.SetShowDuration(tbkName, int.MaxValue);
            }

            if (tbkName.ActualWidth > 310)
            {
                vbName.Stretch = Stretch.Fill;
                vbName.Width = 310;
            }
            else
            {
                vbName.Stretch = Stretch.Uniform;
                tbkName.ToolTip = null;
            }
        }

        private void tbkPosition_SizeChanged(object sender, SizeChangedEventArgs e)
        {

            if (tbkPosition.ActualWidth > 270)
            {
                tbkPosition.TextTrimming = TextTrimming.CharacterEllipsis;
                tbkPosition.Width = 270;

                tbkPosition.ToolTip = new TextBlock()
                {
                    Text = tbkPosition.Text,
                    TextWrapping = TextWrapping.Wrap
                };
                ToolTipService.SetShowDuration(tbkPosition, int.MaxValue);
            }

            if (tbkPosition.ActualWidth > 180)
            {
                vbPosition.Stretch = Stretch.Fill;
                vbPosition.Width = 180;
            }
            else
            {
                vbPosition.Stretch = Stretch.Uniform;
                tbkPosition.ToolTip = null;
            }
        }
    }
}
