using Ookii.Dialogs.Wpf;
using System;
using System.IO;
using System.Reflection;
using System.Security;
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
        /// The process logged in the application
        /// </summary>
        private string m_ProcessLogged = "Ready";
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
        public string Title
        {
            get
            {
                return m_Title;
            }
            set
            {
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
            get
            {
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
            get
            {
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
        /// <summary>
        /// Process logged being executed by the application
        /// </summary>
        public string ProcessLogged
        {
            get { return m_ProcessLogged; }
            set
            {
                if (m_ProcessLogged == value)
                    return;
                // set new value
                m_ProcessLogged = value;
                // notify event
                OnPropertyChanged(nameof(ProcessLogged));
            }
        }
        /// <summary>
        /// Application general settings
        /// </summary>
        public ApplicationSettingsViewModel Settings { get; private set; }
        /// <summary>
        /// Character settings
        /// </summary>
        public CharSettingsViewModel CharSettings { get; } = new CharSettingsViewModel("");
        /// <summary>
        /// Login process controller
        /// </summary>
        public LoginViewModel Login { get; private set; }
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
        /// <summary>
        /// Removes a silkroad server information
        /// </summary>
        public ICommand CommandRemoveSilkroad { get; set; }
        /// <summary>
        /// Updates a silkroad server information
        /// </summary>
        public ICommand CommandUpdateSilkroad { get; set; }
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
            Title = "v" + Version;

            // Add the first line in the logger
            m_TextLogged = string.Format("{0} Welcome to {1} v{2} | Created by Engels \"JellyBitz\" Quintero" +
                Environment.NewLine + "{0} Discord : JellyBitz#7643 | FaceBook : @ImJellyBitz",
               DateTime.Now.ToShortFormat(), AppName, Version);

            #region Commands Setup
            // Windows commands
            CommandMinimize = new RelayCommand(() => m_Window.WindowState = WindowState.Minimized);
            CommandRestore = new RelayCommand(() =>
            {
                // Check the WindowState and change it
                m_Window.WindowState = m_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
            CommandClose = new RelayCommand(m_Window.Close);
            // App commands
            CommandAddSilkroad = new RelayCommand(AddSilkroad);
            CommandRemoveSilkroad = new RelayParameterizedCommand(RemoveSilkroad);
            CommandUpdateSilkroad = new RelayParameterizedCommand(UpdateSilkroad);
            #endregion

            // On Load stuffs
            InitializeSettings("xBot.Settings.json");
            InitializeLogin(Environment.GetCommandLineArgs());
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Writes an newline with the value provided to the application logger
        /// </summary>
        /// <param name="Value">The text to be logged</param>
        public void WriteLine(string Value)
        {
            this.TextLogged += Environment.NewLine + DateTime.Now.ToShortFormat() + " " + Value;
        }
        /// <summary>
        /// Writes a process description to the application process logger
        /// </summary>
        /// <param name="Value">The text to be logged</param>
        public void WriteProcess(string Value)
        {
            this.ProcessLogged = Value;
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Add silkroad server information
        /// </summary>
        private void AddSilkroad()
        {
            // Build dialog to search the Silkroad path
            VistaFolderBrowserDialog folderBrowser = new VistaFolderBrowserDialog
            {
                Description = "Please, select your Silkroad Online folder",
                RootFolder = Environment.SpecialFolder.MyComputer,
                UseDescriptionForTitle = true
            };

            // Confirm that the folder has been selected
            if (folderBrowser.ShowDialog() != true)
                return;

            // Prepares the extractor to be shown as dialog
            Pk2Extractor extractorWindow = new Pk2Extractor
            {
                // Set parent to lock it as dialog
                Owner = m_Window
            };
            // Creates the viewmodel that will handle all the process
            Pk2ExtractorViewModel extractorWindowVM = new Pk2ExtractorViewModel(extractorWindow, folderBrowser.SelectedPath);
            // Set the viewmodel
            extractorWindow.DataContext = extractorWindowVM;

            // Confirm that pk2 has been closed/extracted successfully
            if (extractorWindow.ShowDialog() != true)
                return;

            // Remove duplicates, just in case!
            for (int i = 0; i < Settings.Silkroads.Count; i++)
                if (Settings.Silkroads[i].ID == extractorWindowVM.Silkroad.ID)
                    Settings.Silkroads.RemoveAt(i--);
            // Fix duplicated names, just in case!
            for (int i = 0; i < Settings.Silkroads.Count; i++)
            {
                if (Settings.Silkroads[i].Name == extractorWindowVM.Silkroad.Name)
                {
                    extractorWindowVM.Silkroad.Name += "*";
                    i = 0;
                }
            }


            // Added Successfully
            Settings.Silkroads.Add(extractorWindowVM.Silkroad);
            Settings.Save();
            WriteLine("Silkroad Server [" + extractorWindowVM.Silkroad.Name + "] has been added");
        }
        /// <summary>
        /// Remove a silkroad server information
        /// </summary>
        /// <param name="Silkroad">Silkroad selected : <see cref="SilkroadSetupViewModel"/></param>
        private void RemoveSilkroad(object SelectedSilkroad)
        {
            // Make sure it's selected
            if(SelectedSilkroad != null)
            {
                var setup = (SilkroadSetupViewModel)SelectedSilkroad;
                Settings.Silkroads.Remove(setup);
                Settings.Save();
                WriteLine("Silkroad Server [" + setup.Name + "] has been removed");
            }
        }
        /// <summary>
        /// Updates the silkroad server information from selected one
        /// </summary>
        /// <param name="Silkroad">Silkroad selected : <see cref="SilkroadSetupViewModel"/></param>
        private void UpdateSilkroad(object SelectedSilkroad)
        {
            // Make sure it's selected
            if (SelectedSilkroad != null)
            {
                var setup = (SilkroadSetupViewModel)SelectedSilkroad;
                
                // Prepares the extractor to be shown as dialog
                Pk2Extractor extractorWindow = new Pk2Extractor
                {
                    // Set parent to lock it as dialog
                    Owner = m_Window
                };
                // Creates the viewmodel that will handle all the process
                Pk2ExtractorViewModel extractorWindowVM = new Pk2ExtractorViewModel(extractorWindow, setup.Path, setup);
                // Set the viewmodel
                extractorWindow.DataContext = extractorWindowVM;

                // Show extractor dialog
                extractorWindow.ShowDialog();

                // Fix duplicated names, just in case!
                for (int i = 0; i < Settings.Silkroads.Count; i++)
                {
                    if (Settings.Silkroads[i] != setup && Settings.Silkroads[i].Name == setup.Name )
                    {
                        setup.Name += "*";
                        i = 0;
                    }
                }

                // Override the changes no matter what
                Settings.SaveAsync();
                WriteLine("Silkroad Server [" + setup.Name + "] has been updated");
            }
        }
        #endregion

        #region Private Helpers
        /// <summary>
        /// Load or creates the application settings
        /// </summary>
        private void InitializeSettings(string ConfigPath)
        {
            // Temporally
            var settings = new ApplicationSettingsViewModel(ConfigPath);

            // Try load
            if (!settings.Load())
            {
                // Try to keep a backup if possible
                if(File.Exists(ConfigPath))
                {
                    try {
                        File.Move(ConfigPath, ConfigPath + "." + DateTime.Now.ToString("dd-mm-yyyy.HH-mm") + ".bkp");
                    } catch { }
                }
                // Create a clean config
                settings.Save();
                WriteLine("New settings has been created");
            }

            // Set app settings
            Settings = settings;
        }
        /// <summary>
        /// Initialize the login by using command line or default
        /// </summary>
        private void InitializeLogin(string[] args)
        {
            Login = new LoginViewModel();

            // TO DO:
            // Load command line

            // Set default selection for quick login (UX stuff)
            if (Login.SilkroadSelected == null && Settings.Silkroads.Count > 0)
                Login.SilkroadSelected = Settings.Silkroads[0];
        }
        #endregion
    }
}
