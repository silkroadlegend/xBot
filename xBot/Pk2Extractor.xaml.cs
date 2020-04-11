using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace xBot
{
    /// <summary>
    /// Interaction logic for the Pk2Extractor.xaml
    /// </summary>
    public partial class Pk2Extractor : Window
    {
        #region Constructor
        /// <summary>
        /// Creates a window that handles all about pk2 extraction
        /// </summary>
        public Pk2Extractor()
        {
            InitializeComponent();
        }
        #endregion

        #region Events about UI behavior only
        /// <summary>
        /// Drag the window when the control is click holding
        /// </summary>
        private void Control_MouseDown_DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        /// <summary>
        /// Check if the app has been made scroll movement
        /// </summary>
        private bool isAppScroll;
        /// <summary>
        /// The lastest scroll offset (moved by user)
        /// </summary>
        private double AppScrollVerticalOffset = -1;
        /// <summary>
        /// Save the scroll offset every time the user tries to scroll
        /// </summary>
        private void ScrollViewer_ScrollChanged_AutoScroll(object sender, RoutedEventArgs e)
        {
            ScrollViewer s = (ScrollViewer)e.OriginalSource;
            ScrollChangedEventArgs se = (ScrollChangedEventArgs)e;

            // User scroll
            if (se.ExtentHeightChange == 0)
            {
                // Check if the scroll was made by this app
                if (isAppScroll)
                {
                    isAppScroll = false;
                }
                else
                {
                    // Check if scroll is in bottom
                    if (s.ScrollableHeight - s.VerticalOffset < 15)
                    {
                        AppScrollVerticalOffset = -1;
                    }
                    else
                    {
                        // Save the scroll offset
                        AppScrollVerticalOffset = s.VerticalOffset;
                    }
                }
            }

            // Route handled
            e.Handled = true;
        }
        /// <summary>
        /// Scroll the textbox to the offset saved
        /// </summary>
        private void TextBox_TextChanged_AutoScroll(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;

            isAppScroll = true;
            // Keep last scroll offsets
            if (AppScrollVerticalOffset == -1)
                t.ScrollToEnd();
            else
                t.ScrollToVerticalOffset(AppScrollVerticalOffset);
        }
        #endregion

        /// <summary>
        /// Called when the window is closed from any source
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Make sure to cancel extraction when is closing for an external reason
            Pk2ExtractorViewModel context = (Pk2ExtractorViewModel)DataContext;
            if (context.IsExtracting)
                context.CommandCancelExtraction.Execute(null);
        }
        /// <summary>
        /// Called when a button is clicked
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            switch (b.Name)
            {
                case "btnLauncher":
                    {
                        // Build dialog to search the Launcher.exe path
                        OpenFileDialog fileDialog = new OpenFileDialog
                        {
                            Multiselect = false,
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                            ValidateNames = true,
                            Title = "Please, select your \"Silkroad.exe\" executable file",
                            FileName = "Silkroad.exe",
                            Filter = "Silkroad.exe|silkroad.exe|executables (*.exe)|*.exe",
                            FilterIndex = 0
                        };
                        // Confirm that the file has been selected
                        if (fileDialog.ShowDialog() != true)
                            return;
                        // Set the new path
                        ((Pk2ExtractorViewModel)DataContext).Silkroad.LauncherPath = fileDialog.FileName;
                    }
                    break;
                case "btnClient":
                    {
                        // Build dialog to search the sro_client.exe path
                        OpenFileDialog fileDialog = new OpenFileDialog
                        {
                            Multiselect = false,
                            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                            ValidateNames = true,
                            Title = "Please, select your \"sro_client.exe\" executable file",
                            FileName = "sro_client.exe",
                            Filter = "sro_client.exe|sro_client.exe|executables (*.exe)|*.exe",
                            FilterIndex = 0
                        };
                        // Confirm that the file has been selected
                        if (fileDialog.ShowDialog() != true)
                            return;
                        // Set the new path
                        ((Pk2ExtractorViewModel)DataContext).Silkroad.ClientPath = fileDialog.FileName;
                    }
                    break;
            }
        }
    }
}
