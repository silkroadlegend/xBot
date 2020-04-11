using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace xBot
{
    /// <summary>
    /// Interaction logic for the MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            // Init view
            DataContext = new MainWindowViewModel(this);
        }

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
    }
}
