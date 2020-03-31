using Microsoft.Win32;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using xBot.Utility;

namespace xBot
{
    public class MainWindowViewModel : BaseViewModel
    {
        #region Private Properties
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window m_Window;
        /// <summary>
        /// The text logged in the application
        /// </summary>
        private string m_TextLogged;
        /// <summary>
        /// The title of the application
        /// </summary>
        private string m_Title;
        /// <summary>
        /// Full title bar of the application
        /// </summary>
        private string m_FullTitle;
        #endregion

        #region Public Properties
        /// <summary>
        /// Name of the application
        /// </summary>
        public string AppName { get; } = "xBot";
        /// <summary>
        /// Title of the application
        /// </summary>
        public string Title {
            get {
                return m_Title;
            }
            set {
                m_Title = value;
                // notify event
                OnPropertyChanged(nameof(Title));

                // updates the full app name
                FullTitle = AppName + " - " + value;
            }
        }
        /// <summary>
        /// Full window title of the application
        /// </summary>
        public string FullTitle
        {
            get {
                return m_FullTitle;
            }
            set
            {
                m_FullTitle = value;
                // notify event
                OnPropertyChanged(nameof(FullTitle));
            }
        }
        /// <summary>
        /// The application version used in assembly
        /// </summary>
        public string Version
        {
            get {
                // Get the version
                string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                // Return first three digits
                return assemblyVersion.Remove(assemblyVersion.LastIndexOf("."));
            }
        }
        /// <summary>
        /// Text being logged by the application
        /// </summary>
        public string TextLogged
        {
            get { return m_TextLogged; }
            set
            {
                // set new value
                m_TextLogged = value;
                // notify event
                OnPropertyChanged(nameof(TextLogged));
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Minimize the window
        /// </summary>
        public ICommand CommandMinimize { get; set; }
        /// <summary>
        /// Switch the window between restore and maximize
        /// </summary>
        public ICommand CommandRestore { get; set; }
        /// <summary>
        /// Close the window
        /// </summary>
        public ICommand CommandClose { get; set; }
        /// <summary>
        /// Add silkroad server information
        /// </summary>
        public ICommand CommandAddSilkroad { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public MainWindowViewModel(Window Window)
        {
            // Save a reference
            m_Window = Window;

            // Set version as noticeable title on starting
            Title = "v"+Version;

            // Add the first line in the logger
            m_TextLogged = string.Format("{0} Welcome to {1} v{2} | Created by Engels \"JellyBitz\" Quintero" +
                Environment.NewLine + "{0} Discord : JellyBitz#7643 | FaceBook : @ImJellyBitz",
               DateTime.Now.ToShortFormat(), AppName, Version);

            // Commands setup
            CommandMinimize = new RelayCommand(() => m_Window.WindowState = WindowState.Minimized);
            CommandRestore = new RelayCommand(()=> {
                // Check the WindowState and change it
                m_Window.WindowState = m_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
            CommandClose = new RelayCommand(m_Window.Close);

            CommandAddSilkroad = new RelayCommand(AddSilkroad);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Writes an newline with the value provided to the application logger
        /// </summary>
        /// <param name="Value">The text to be logged</param>
        public void WriteLine(string Value){
            this.TextLogged += Environment.NewLine + DateTime.Now.ToShortFormat() + " " + Value;
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Add silkroad server information
        /// </summary>
        public void AddSilkroad()
        {
            // Build dialog to search the Media.pk2 path
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyComputer),
                ValidateNames = true,
                Title = "Select your \"Media.pk2\" file",
                FileName = "Media.pk2",
                Filter = "Media.pk2|media.pk2|pk2 files (*.pk2)|*.pk2|All files (*.*)|*.*",
                FilterIndex = 0
            };
            // Confirm that the file has been selected
            if (fileDialog.ShowDialog() != true)
                return;

            // Prepares the extractor to be shown as dialog
            Pk2Extractor extractorWindow = new Pk2Extractor {
                // Set parent to lock it as dialog
                Owner = m_Window
            };
            // Creates the viewmodel that will handle all the process
            Pk2ExtractorViewModel extractorWindowVM = new Pk2ExtractorViewModel(extractorWindow, fileDialog.FileName);
            // Set the viewmodel
            extractorWindow.DataContext = extractorWindowVM;
            
            // Confirm that pk2 has been closed/extracted successfully
            if (extractorWindow.ShowDialog() != true)
                return;

            WriteLine("Silkroad Server [" + extractorWindowVM.SilkroadName+"] has been added");
        }
        #endregion
    }
}
