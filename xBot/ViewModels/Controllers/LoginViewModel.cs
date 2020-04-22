using System.IO;
using System.Security;
using System.Threading.Tasks;
using System.Windows.Input;
namespace xBot
{
    public class LoginViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// Silkroad setup details to login
        /// </summary>
        private SilkroadSetupViewModel m_SilkroadSelected;
        /// <summary>
        /// Silkroad username/ID
        /// </summary>
        private string m_Username;
        /// <summary>
        /// Silkroad password
        /// </summary>
        private SecureString m_Password;
        /// <summary>
        /// Silkroad captcha
        /// </summary>
        private string m_Captcha;
        /// <summary>
        /// Server selected
        /// </summary>
        private string m_Server;
        /// <summary>
        /// Character selected
        /// </summary>
        private string m_Character;
        #endregion

        #region Public Properties
        /// <summary>
        /// The silkroad choosen to play
        /// </summary>
        public SilkroadSetupViewModel SilkroadSelected
        {
            get
            {
                return m_SilkroadSelected;
            }
            set
            {
                m_SilkroadSelected = value;
                // Raise event
                OnPropertyChanged(nameof(SilkroadSelected));
            }
        }
        /// <summary>
        /// Silkroad Username/ID
        /// </summary>
        public string Username
        {
            get
            {
                return m_Username;
            }
            set
            {
                m_Username = value;
                // Raise event
                OnPropertyChanged(nameof(Username));
            }
        }
        /// <summary>
        /// Silkroad Password
        /// </summary>
        public SecureString Password
        {
            get
            {
                return m_Password;
            }
            set
            {
                m_Password = value;
                // Raise event
                OnPropertyChanged(nameof(Password));
            }
        }
        /// <summary>
        /// Captcha used to get login
        /// </summary>
        public string Captcha
        {
            get
            {
                return m_Captcha;
            }
            set
            {
                m_Captcha = value;
                // Raise event
                OnPropertyChanged(nameof(Captcha));
            }
        }
        /// <summary>
        /// Server choosen to login
        /// </summary>
        public string Server
        {
            get
            {
                return m_Server;
            }
            set
            {
                m_Server = value;
                // Raise event
                OnPropertyChanged(nameof(Server));
            }
        }
        /// <summary>
        /// Character name selected
        /// </summary>
        public string Character
        {
            get
            {
                return m_Character;
            }
            set
            {
                m_Character = value;
                // Raise event
                OnPropertyChanged(nameof(Character));
            }
        }
        #endregion

        #region Commands
        /// <summary>
        /// Starts the launcher used by the silkroad selected
        /// </summary>
        public ICommand CommandStartLauncher { get; set; }
        /// <summary>
        /// Starts the game used the connection specified
        /// </summary>
        public ICommand CommandStartGame { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public LoginViewModel()
        {
            CommandStartLauncher = new RelayCommand(StartLauncher);
            CommandStartGame = new RelayParameterizedCommand(async(parameter) => await StartGame(parameter));
        }
        #endregion

        #region Command Methods
        /// <summary>
        /// Starts silkroad launcher as simple as shortcut
        /// </summary>
        private void StartLauncher()
        {
            if(SilkroadSelected != null && File.Exists(SilkroadSelected.LauncherPath))
                System.Diagnostics.Process.Start(SilkroadSelected.LauncherPath);
        }
        /// <summary>
        /// Starts the client process
        /// </summary>
        /// <param name="IsClientless">Connection client method</param>

        private async Task StartGame(object IsClientless)
        {
            // TO DO:
            // Start loader
            // Handle proxy taking care of connection method
            // Redirect stuffs (ag-gw)
            // Subscribe parsing
            // blabla..

            await Task.Delay(1);
        }
        #endregion
    }
}
