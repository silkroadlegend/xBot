using System.Windows;
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

        #region UI Behavior Methods
        /// <summary>
        /// Drag the window when the control is click holding
        /// </summary>
        private void AnyControl_DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        #endregion
    }
}
