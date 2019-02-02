using StudentElection.Main;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace StudentElection
{
    public static class G
    {
        public static List<double> CandidateHOffsets = new List<double>();

        //public static bool IsBlank(this string value)
        //{
        //    if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
        //    {
        //        return true;
        //    }
        //    return false;
        //}

        //public static string ToTitleCase(this string value)
        //{
        //    var cultureInfo = Thread.CurrentThread.CurrentCulture;
        //    var textInfo = cultureInfo.TextInfo;

        //    return textInfo.ToTitleCase(value).Trim();
        //}

        //public static string ToProperCase(this string value)
        //{
        //    string text = value.Trim();
        //    string[] names = text.Split(' ');
        //    StringBuilder newName = new StringBuilder();

        //    if (IsBlank(text)) return "";

        //    foreach (string name in names)
        //    {
        //        newName.AppendFormat("{0}", name[0].ToString().ToUpper());

        //        if (name.Length > 1)
        //            newName.Append(name.Substring(1));

        //        newName.Append(" ");
        //    }

        //    return newName.ToString().Trim();
        //}

        public static void ChangeWindowSize(Window window)
        {
            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
            var smallerThanCommonSize = (bounds.Width < 1600 && bounds.Height < 900);

            if (window is MaintenanceWindow)
            {
                window.Height = bounds.Height * (0.85 - (0.2 * (bounds.Height - 480) / 600));
                window.Width = window.Height * (1.25 - (0.15 * (bounds.Width - 640) / 1280));
            }
            else if (window is BallotWindow)
            {
                window.WindowState = WindowState.Maximized;
                window.WindowStyle = WindowStyle.None;
            }

            PlaceWindowOnCenter(window);
        }

        public static void PlaceWindowOnCenter(Window window)
        {
            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            window.Left = (bounds.Width - window.Width) / 2;
            window.Top = (bounds.Height - window.Height) / 2;
        }

        public static void PlaceWindowOnCenter(System.Windows.Forms.Form form)
        {
            var bounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;

            form.Left = (bounds.Width - form.Width) / 2;
            form.Top = (bounds.Height - form.Height) / 2;
        }

        public static void WaitLang(Window window)
        {
            window.IsEnabled = false;

            if (window is MaintenanceWindow maintenanceWindow)
            {
                maintenanceWindow.Cursor = System.Windows.Input.Cursors.Wait;
            }
            else if (window is MainWindow mainWindow)
            {
                mainWindow.Cursor = System.Windows.Input.Cursors.Wait;
            }
            else if (window is BallotWindow ballotWindow)
            {
                ballotWindow.Cursor = System.Windows.Input.Cursors.Wait;
            }
            else
            {
                window.Cursor = System.Windows.Input.Cursors.Wait;
            }
        }

        public static void WaitLang(System.Windows.Forms.Form form)
        {
            form.Enabled = false;
            form.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        }

        public static void EndWait(Window window)
        {
            window.IsEnabled = true;

            if (window is MaintenanceWindow maintenanceWindow)
            {
                maintenanceWindow.Cursor = System.Windows.Input.Cursors.Arrow;
            }
            else if (window is MainWindow mainWindow)
            {
                mainWindow.Cursor = System.Windows.Input.Cursors.Arrow;
            }
            else if (window is BallotWindow ballotWindow)
            {
                ballotWindow.Cursor = System.Windows.Input.Cursors.Arrow;
            }
            else
            {
                window.Cursor = System.Windows.Input.Cursors.Arrow;
            }
        }

        public static void EndWait(System.Windows.Forms.Form form)
        {
            form.Enabled = true;
            form.Cursor = System.Windows.Forms.Cursors.Arrow;
        }
    }

    public class TabSizeConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TabControl tabControl = values[0] as TabControl;
            double width = (tabControl.ActualWidth - 5) / tabControl.Items.Count;

            return (width < 0) ? 0 : width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
