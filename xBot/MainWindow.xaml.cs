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
        /// Auto scrolls the caret to the buttom but only if is in bottom
        /// </summary>
        private void TextBox_TextChanged_AutoScroll(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;

            // Get bottom caret indexes
            int indexStart = t.GetLastVisibleLineIndex();
            int indexEnds = t.Text.Length - 1;
            // Scroll if caret index was in bottom
            if (t.CaretIndex >= indexStart && t.CaretIndex <= indexEnds)
                t.CaretIndex = indexEnds;
        }
        #endregion
    }
}
