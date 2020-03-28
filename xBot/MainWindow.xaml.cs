using System.Windows;
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
