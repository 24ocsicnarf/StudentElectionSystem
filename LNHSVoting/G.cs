﻿using LNHSVoting.Main;
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

namespace LNHSVoting
{
    public static class G
    {
        public enum UserType
        {
            Voter,
            Admin,
            SuperAdmin
        }

        public enum SexType
        {
            Male,
            Female
        }

        public static List<double> CandidateHOffsets = new List<double>();

        public static bool IsBlank(this string value)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
            {
                return true;
            }
            return false;
        }

        public static string ToTitleCase(this string value)
        {
            var cultureInfo = Thread.CurrentThread.CurrentCulture;
            var textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(value).Trim();
        }

        public static string ToProperCase(this string value)
        {
            string text = value.Trim();
            string[] names = text.Split(' ');
            StringBuilder newName = new StringBuilder();

            if (IsBlank(text)) return "";

            foreach (string name in names)
            {
                newName.AppendFormat("{0}", name[0].ToString().ToUpper());

                if (name.Length > 1)
                    newName.Append(name.Substring(1));

                newName.Append(" ");
            }

            return newName.ToString().Trim();
        }

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
                //window.Width = bounds.Width * (0.85 - (0.4 * (bounds.Width - 640) / 1280));
                //window.Height = window.Width * (1 - (0.25 * (1 - (bounds.Height - 480) / 600)));

                window.WindowState = WindowState.Maximized;
                window.WindowStyle = WindowStyle.None;

                //if (window.Owner == null)
                //{
                //    var blackWindow = new Window();
                //    blackWindow.Background = new SolidColorBrush(Colors.Black);
                //    blackWindow.ResizeMode = ResizeMode.NoResize;
                //    blackWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                //    blackWindow.WindowStyle = WindowStyle.None;
                //    blackWindow.WindowState = WindowState.Maximized;
                //    blackWindow.ShowInTaskbar = false;
                //    window.Owner = blackWindow;
                //}
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
            window.Cursor = System.Windows.Input.Cursors.Wait;
        }

        public static void WaitLang(System.Windows.Forms.Form form)
        {
            form.Enabled = false;
            form.Cursor = System.Windows.Forms.Cursors.WaitCursor;
        }

        public static void EndWait(Window window)
        {
            window.IsEnabled = true;
            window.Cursor = System.Windows.Input.Cursors.Arrow;
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
            double width = (tabControl.ActualWidth - 4) / tabControl.Items.Count;

            return (width < 0) ? 0 : width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
