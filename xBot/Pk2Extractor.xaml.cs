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
        /// <param name="FullPath">Path to the Pk2 file</param>
        public Pk2Extractor(string FullPath)
        {
            InitializeComponent();
            // Init view
            DataContext = new Pk2ExtractorViewModel(this,FullPath);
        }
        /// <summary>
        /// Creates a window that handles all about pk2 extraction using an specific view model
        /// </summary>
        /// <param name="ViewModel"></param>
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
        /// Auto scrolls the caret to the buttom.
        /// <see cref="TextBox_Scroll_AutoScroll(object, RoutedEventArgs)"/> needs to be set in the same TextBox
        /// </summary>
        private void TextBox_TextChanged_AutoScroll(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            
            // Auto scroll if caret was in bottom
            if (t.Tag == null || (bool)t.Tag)
                t.ScrollToEnd();
        }
        /// <summary>
        /// Auto scrolls the caret to the buttom.
        /// <see cref="TextBox_TextChanged_AutoScroll(object, TextChangedEventArgs)"/> needs to be set in the same TextBox
        /// </summary>
        private void TextBox_Scroll_AutoScroll(object sender, RoutedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            ScrollViewer s = ((ScrollViewer)e.OriginalSource);

            // Check if caret is on bottom
            t.Tag = s.VerticalOffset == s.ScrollableHeight;

            e.Handled = true;
        }
        #endregion

        private void AppExtractor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Cancel extraction when is closing
            Pk2ExtractorViewModel context = (Pk2ExtractorViewModel)DataContext;
            if (context.IsExtracting)
                context.CommandCancelExtraction.Execute(null);
        }
    }
}
