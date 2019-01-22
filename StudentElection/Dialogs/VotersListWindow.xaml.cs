using StudentElection.Classes;
using StudentElection;
using System;
using System.Collections.Generic;
using System.Data;
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
using static StudentElection.G;

namespace StudentElection.Dialogs
{
    /// <summary>
    /// Interaction logic for VotersListWindow.xaml
    /// </summary>
    public partial class VotersListWindow : Window
    {
        private bool _isCanceled = true;

        private CollectionView _cvVoter;
        
        private bool _wasMoved;
        private System.Windows.Forms.Cursor _previousCursor;
        private Point _posPressed;
        private GridViewColumn _columnClicked;
        private bool _isAscendingVoter = true;
        List<Voter> _listVoter;


        public VotersListWindow()
        {
            InitializeComponent();

            LoadVoters();

            Microsoft.Win32.SystemEvents.DisplaySettingsChanged += (s, ev) =>
            {
                G.PlaceWindowOnCenter(this);
            };
        }

        public Voter GetNewData()
        {
            if (_isCanceled)
            {
                return null;
            }

            return (lvVoter.SelectedItem as Voter);
        }

        private bool FilterVoter(object obj)
        {
            var voter = obj as Voter;
            bool isForeign = (obj as Voter).IsForeign;
            bool containsText = true;

            if (!txtVoterFilter.Text.IsBlank())
            {
                containsText = ((voter.LastName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.FirstName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.MiddleName.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.GradeLevel.ToString().IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.StrandSection.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.Sex.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.BirthdateString.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0) ||
                       (voter.VoterID.IndexOf(txtVoterFilter.Text, StringComparison.OrdinalIgnoreCase) == 0));

                return containsText;
            }
            else
            {
                return true;
            }
        }

        private void LoadVoters()
        {
            _listVoter  = new List<Voter>();
            
            if (_columnClicked == null)
            {
                _columnClicked = new GridViewColumn();
                _columnClicked.Header = gvVoter.Columns[0].Header;
                _isAscendingVoter = true;
            }
            
            _listVoter = Voters.Dictionary.Values.Where(v => !Candidates.Dictionary.Keys.Contains(v.ID)).ToList();

            SortVoter();

            _cvVoter = (CollectionView)CollectionViewSource.GetDefaultView(lvVoter.ItemsSource);
            _cvVoter.Filter = FilterVoter;
        }

        private void txtVoterFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvVoter.ItemsSource).Refresh();
        }

        private void btnSelectVoter_Click(object sender, RoutedEventArgs e)
        {
            _isCanceled = false;
            
            Close();
        }

        private void lvVoter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            
            btnSelectVoter.IsEnabled = lv.SelectedItems.Count == 1;
        }
        
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void lvVoter_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is GridViewRowPresenter))
                dep = LogicalTreeHelper.GetParent(dep);

            if (dep == null) return;

            _isCanceled = false;

            lvVoter.SelectedItem = (dep as GridViewRowPresenter).Content;

            Close();
        }

        private void chkForeign_Checked(object sender, RoutedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(lvVoter.ItemsSource).Refresh();
            
            if (lvVoter.SelectedIndex >= 0)
            {
                lvVoter.ScrollIntoView(lvVoter.SelectedItem);
            }
        }

        private void lvVoter_PreviewMouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _wasMoved = e.LeftButton == MouseButtonState.Released;
        }

        private void lvVoter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _previousCursor = System.Windows.Forms.Cursor.Current;
            _posPressed = e.GetPosition(this);
        }

        private void lvVoter_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released && _previousCursor == System.Windows.Forms.Cursors.Arrow)
            {
                var posRelease = e.GetPosition(this);
                if (Math.Abs(_posPressed.X - posRelease.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(_posPressed.Y - posRelease.Y) > SystemParameters.MinimumHorizontalDragDistance || _wasMoved) return;
                _wasMoved = false;

                var dep = (DependencyObject)e.OriginalSource;

                while ((dep != null) && !(dep is GridViewColumnHeader))
                    dep = VisualTreeHelper.GetParent(dep);

                if (dep == null)
                    return;

                var previousClicked = _columnClicked;

                _columnClicked = new GridViewColumn()
                {
                    Header = (dep as GridViewColumnHeader).Content
                };
                var columns = gvVoter.Columns.Select(x => x.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()).ToList();

                var oldIndex = columns.IndexOf(previousClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart());
                var newIndex = columns.ToList().IndexOf(_columnClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart());

                if (oldIndex == newIndex)
                {
                    _isAscendingVoter = !_isAscendingVoter;
                    if (_isAscendingVoter)
                        _columnClicked.Header = _columnClicked.Header.ToString().Replace("▼", "▲");
                    else
                        _columnClicked.Header = _columnClicked.Header.ToString().Replace("▲", "▼");
                }
                else
                {
                    _isAscendingVoter = true;
                    _columnClicked.Header = !_columnClicked.Header.ToString().Contains("▼") ? "▲ " + _columnClicked.Header : "▼ " + _columnClicked.Header;

                    gvVoter.Columns[oldIndex].Header = previousClicked.Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart();
                }
                gvVoter.Columns[newIndex].Header = _columnClicked.Header;

                SortVoter();

                _cvVoter = (CollectionView)CollectionViewSource.GetDefaultView(lvVoter.ItemsSource);
                _cvVoter.Filter = FilterVoter;

                if (lvVoter.SelectedIndex >= 0)
                {
                    lvVoter.ScrollIntoView(lvVoter.SelectedItem);
                }
            }
        }


        private void SortVoter()
        {
            //START SORTING
            if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[0].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()) || _columnClicked.Header == null)
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[1].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[2].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.FirstName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.FirstName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[3].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.MiddleName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.MiddleName).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[4].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.GradeLevel).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.GradeLevel).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[5].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.StrandSection).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.StrandSection).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[6].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.SexType).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.SexType).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            else if (_columnClicked.Header.ToString().Contains(gvVoter.Columns[7].Header.ToString().Replace('▲', ' ').Replace('▼', ' ').TrimStart()))
            {
                if (_isAscendingVoter)
                {
                    lvVoter.ItemsSource = _listVoter.OrderBy(x => x.Birthdate).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
                else
                {
                    lvVoter.ItemsSource = _listVoter.OrderByDescending(x => x.Birthdate).
                        ThenBy(x => x.FullName).
                        ThenBy(x => x.VoterID);
                }
            }
            //END SORTING
        }

        private void Window_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void btnEditVoter_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}