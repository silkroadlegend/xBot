using Pk2ReaderAPI;
using Pk2ReaderAPI.Formats;
using Pk2ReaderAPI.Files;
using SilkroadSecurityAPI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using xBot.Data;
using xBot.Utility;
using System.Drawing.Imaging;
using System.Threading;

namespace xBot
{
    public class Pk2ExtractorViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// The window this view model controls
        /// </summary>
        private Window m_Window;
        /// <summary>
        /// Token to cancel the asynchronized extraction processes
        /// </summary>
        CancellationTokenSource m_ExtractionCancelToken;
        /// <summary>
        /// The full path to the Pk2 file
        /// </summary>
        private string m_FullPath;
        /// <summary>
        /// The text logged in the application
        /// </summary>
        private string m_TextLogged;
        /// <summary>
        /// The process logged in the application
        /// </summary>
        private string m_ProcessLogged = "Ready";
        /// <summary>
        /// The name identification to the given silkroad server
        /// </summary>
        private string m_SilkroadName;
        /// <summary>
        /// Flag indicating if the <see cref="CommandStartExtraction"/> is running
        /// </summary>
        private bool m_IsExtracting;
        /// <summary>
        /// The client version found into the Pk2
        /// </summary>
        private uint m_Version;
        /// <summary>
        /// The locale type found into the Pk2
        /// </summary>
        private byte m_Locale;
        /// <summary>
        /// The locale type found into the Pk2
        /// </summary>
        private string m_DivisionInfo;
        /// <summary>
        /// The connection port found into the Pk2
        /// </summary>
        private ushort m_Gateport;
        #endregion

        #region Private Advanced Properties
        /// <summary>
        /// Blowfish key used to decrypt the Pk2 file
        /// </summary>
        private string m_BlowfishKey = "169841";
        /// <summary>
        /// The server file type choosen
        /// </summary>
        private SilkroadFilesType m_SilkroadFilesType;
        /// <summary>
        /// Pk2 Path to the file
        /// </summary>
        private string
            m_VersionPath = "SV.T",
            m_DivisionInfoPath = "DivisionInfo.txt",
            m_GateportPath = "Gateport.txt",
            m_TypePath = "Type.txt",
            m_TextDataNamePointerPath = "server_dep/silkroad/textdata/TextDataName.txt",
            m_TextUISystemPath = "server_dep/silkroad/textdata/TextUISystem.txt",
            m_TextZoneNamePath = "server_dep/silkroad/textdata/TextZoneName.txt",
            m_ItemDataPointerPath = "server_dep/silkroad/textdata/ItemData.txt",
            m_MagicOptionPath = "server_dep/silkroad/textdata/MagicOption.txt",
            m_CharacterDataPointerPath = "server_dep/silkroad/textdata/CharacterData.txt",
            m_LevelDataPath = "server_dep/silkroad/textdata/LevelData.txt",
            m_SkillMasteryDataPath = "server_dep/silkroad/textdata/SkillMasteryData.txt",
            m_SkillDataPointerPath = "server_dep/silkroad/textdata/SkillDataEnc.txt",
            m_refShopGroupPath = "server_dep/silkroad/textdata/refShopGroup.txt",
            m_refMappingShopGroupPath = "server_dep/silkroad/textdata/refMappingShopGroup.txt",
            m_refMappingShopWithTabPath = "server_dep/silkroad/textdata/refMappingShopWithTab.txt",
            m_refShopTabPath = "server_dep/silkroad/textdata/refShopTab.txt",
            m_refScrapOfPackageItemPath = "server_dep/silkroad/textdata/refScrapOfPackageItem.txt",
            m_refShopGoodsPath = "server_dep/silkroad/textdata/refShopGoods.txt",
            m_TeleportDataPath = "server_dep/silkroad/textdata/TeleportData.txt",
            m_TeleportBuildingPath = "server_dep/silkroad/textdata/TeleportBuilding.txt",
            m_TeleportLinkPath = "server_dep/silkroad/textdata/TeleportLink.txt";
        /// <summary>
        /// Flag indicating if the icons images will be extracted
        /// </summary>
        private bool m_ExtractIcons = true;
        /// <summary>
        /// Flag indicating if the minimap images will be extracted
        /// </summary>
        private bool m_ExtractMinimap = true;
        #endregion

        #region Private Local references used to extract the Pk2 easier
        /// <summary>
        /// The Pk2 reader used to extract all info
        /// </summary>
        private Pk2Reader m_Pk2;
        /// <summary>
        /// The database connection to allocate all the server info
        /// </summary>
        private SQLDatabase m_Database;
        private Dictionary<string, string> m_TextDataNameRef;
        private Dictionary<string, string> m_TextUISystemRef;
        #endregion
        
        #region Public Properties
        /// <summary>
        /// Name of the application
        /// </summary>
        public string AppName { get; } = "Pk2 Extractor";
        /// <summary>
        /// Title of the application
        /// </summary>
        public string Title {
            get {
                return "xBot - "+AppName;
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
        /// Silkroad unique ID to identify the server
        /// </summary>
        public string SilkroadID { get; }
        /// <summary>
        /// Prefered Silkroad name to identify the server
        /// </summary>
        public string SilkroadName
        {
            get { return m_SilkroadName; }
            set
            {
                // set new value
                m_SilkroadName = value;
                // notify event
                OnPropertyChanged(nameof(SilkroadName));
            }
        }
        /// <summary>
        /// Check if the database is being extracted
        /// </summary>
        public bool IsExtracting {
            get { return m_IsExtracting; }
            set {
                // Avoid re-notify event 
                if (m_IsExtracting == value)
                    return;
                // set new value
                m_IsExtracting = value;
                // notify event
                OnPropertyChanged(nameof(IsExtracting));
            }
        }
        /// <summary>
        /// Version used by the client
        /// </summary>
        public uint Version {
            get { return m_Version; }
            set
            {
                // set new value
                m_Version = value;
                // notify event
                OnPropertyChanged(nameof(Version));
            }
        }
        /// <summary>
        /// Localization used by the client
        /// </summary>
        public byte Locale
        {
            get { return m_Locale; }
            set
            {
                // set new value
                m_Locale = value;
                // notify event
                OnPropertyChanged(nameof(Locale));
            }
        }
        /// <summary>
        /// Division info used for the client connection
        /// </summary>
        public string DivisionInfo
        {
            get { return m_DivisionInfo; }
            set
            {
                // set new value
                m_DivisionInfo = value;
                // notify event
                OnPropertyChanged(nameof(DivisionInfo));
            }
        }
        /// <summary>
        /// Gateway port used for the client connection
        /// </summary>
        public ushort Gateport
        {
            get { return m_Gateport; }
            set
            {
                // set new value
                m_Gateport = value;
                // notify event
                OnPropertyChanged(nameof(Gateport));
            }
        }
        #endregion

        #region Public Advanced Properties
        /// <summary>
        /// Blowfish being logged by the application
        /// </summary>
        public string BlowfishKey
        {
            get { return m_BlowfishKey; }
            set
            {
                // set new value
                m_BlowfishKey = value;
                // notify event
                OnPropertyChanged(nameof(BlowfishKey));
            }
        }
        /// <summary>
        /// All possible servers files that application can support
        /// </summary>
        public ObservableCollection<SilkroadFilesType> SilkroadFilesTypes { get; } 
        /// <summary>
        /// The choosen type of server, this can change the behavior on the application
        /// </summary>
        public SilkroadFilesType SilkroadFilesType
        {
            get { return m_SilkroadFilesType; }
            set {
                // set new value
                m_SilkroadFilesType = value;
                // notify event
                OnPropertyChanged(nameof(SilkroadFilesType));
            }
        }
        
        /// <summary>
        /// Pk2 path to the Silkroad version file
        /// </summary>
        public string VersionPath
        {
            get { return m_VersionPath; }
            set
            {
                // set new value
                m_VersionPath = value;
                // notify event
                OnPropertyChanged(nameof(VersionPath));
            }
        }
        /// <summary>
        /// Pk2 path to the DivisionInfo file
        /// </summary>
        public string DivisionInfoPath
        {
            get { return m_DivisionInfoPath; }
            set
            {
                // set new value
                m_DivisionInfoPath = value;
                // notify event
                OnPropertyChanged(nameof(DivisionInfoPath));
            }
        }
        /// <summary>
        /// Pk2 path to the Gateport file
        /// </summary>
        public string GateportPath
        {
            get { return m_GateportPath; }
            set
            {
                // set new value
                m_GateportPath = value;
                // notify event
                OnPropertyChanged(nameof(GateportPath));
            }
        }
        /// <summary>
        /// Pk2 path to the Type file
        /// </summary>
        public string TypePath
        {
            get { return m_TypePath; }
            set
            {
                // set new value
                m_TypePath = value;
                // notify event
                OnPropertyChanged(nameof(TypePath));
            }
        }
        /// <summary>
        /// Pk2 path to the pointer TextDataName file names
        /// </summary>
        public string TextDataNamePointerPath
        {
            get { return m_TextDataNamePointerPath; }
            set
            {
                // set new value
                m_TextDataNamePointerPath = value;
                // notify event
                OnPropertyChanged(nameof(TextDataNamePointerPath));
            }
        }
        /// <summary>
        /// Pk2 path to the TextUISystem file
        /// </summary>
        public string TextUISystemPath
        {
            get { return m_TextUISystemPath; }
            set
            {
                // set new value
                m_TextUISystemPath = value;
                // notify event
                OnPropertyChanged(nameof(TextUISystemPath));
            }
        }
        /// <summary>
        /// Pk2 path to the TextZoneName file
        /// </summary>
        public string TextZoneNamePath
        {
            get { return m_TextZoneNamePath; }
            set
            {
                // set new value
                m_TextZoneNamePath = value;
                // notify event
                OnPropertyChanged(nameof(TextZoneNamePath));
            }
        }
        /// <summary>
        /// Pk2 path to the pointer ItemData file names
        /// </summary>
        public string ItemDataPointerPath
        {
            get { return m_ItemDataPointerPath; }
            set
            {
                // set new value
                m_ItemDataPointerPath = value;
                // notify event
                OnPropertyChanged(nameof(ItemDataPointerPath));
            }
        }
        /// <summary>
        /// Pk2 path to the MagicOption file
        /// </summary>
        public string MagicOptionPath
        {
            get { return m_MagicOptionPath; }
            set
            {
                // set new value
                m_MagicOptionPath = value;
                // notify event
                OnPropertyChanged(nameof(MagicOptionPath));
            }
        }
        /// <summary>
        /// Pk2 path to the pointer CharacterData file names
        /// </summary>
        public string CharacterDataPointerPath
        {
            get { return m_CharacterDataPointerPath; }
            set
            {
                // set new value
                m_CharacterDataPointerPath = value;
                // notify event
                OnPropertyChanged(nameof(CharacterDataPointerPath));
            }
        }
        /// <summary>
        /// Pk2 path to the LevelData file
        /// </summary>
        public string LevelDataPath
        {
            get { return m_LevelDataPath; }
            set
            {
                // set new value
                m_LevelDataPath = value;
                // notify event
                OnPropertyChanged(nameof(LevelDataPath));
            }
        }
        /// <summary>
        /// Pk2 path to the SkillMasteryData file
        /// </summary>
        public string SkillMasteryDataPath
        {
            get { return m_SkillMasteryDataPath; }
            set
            {
                // set new value
                m_SkillMasteryDataPath = value;
                // notify event
                OnPropertyChanged(nameof(SkillMasteryDataPath));
            }
        }
        /// <summary>
        /// Pk2 path to the pointer SkillDataEnc file names
        /// </summary>
        public string SkillDataPointerPath
        {
            get { return m_SkillDataPointerPath; }
            set
            {
                // set new value
                m_SkillDataPointerPath = value;
                // notify event
                OnPropertyChanged(nameof(SkillDataPointerPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refShopGroup file
        /// </summary>
        public string refShopGroupPath
        {
            get { return m_refShopGroupPath; }
            set
            {
                // set new value
                m_refShopGroupPath = value;
                // notify event
                OnPropertyChanged(nameof(refShopGroupPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refMappingShopGroup file
        /// </summary>
        public string refMappingShopGroupPath
        {
            get { return m_refMappingShopGroupPath; }
            set
            {
                // set new value
                m_refMappingShopGroupPath = value;
                // notify event
                OnPropertyChanged(nameof(refMappingShopGroupPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refMappingShopWithTab file
        /// </summary>
        public string refMappingShopWithTabPath
        {
            get { return m_refMappingShopWithTabPath; }
            set
            {
                // set new value
                m_refMappingShopWithTabPath = value;
                // notify event
                OnPropertyChanged(nameof(refMappingShopWithTabPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refShopTab file
        /// </summary>
        public string refShopTabPath
        {
            get { return m_refShopTabPath; }
            set
            {
                // set new value
                m_refShopTabPath = value;
                // notify event
                OnPropertyChanged(nameof(refShopTabPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refScrapOfPackageItem file
        /// </summary>
        public string refScrapOfPackageItemPath
        {
            get { return m_refScrapOfPackageItemPath; }
            set
            {
                // set new value
                m_refScrapOfPackageItemPath = value;
                // notify event
                OnPropertyChanged(nameof(refScrapOfPackageItemPath));
            }
        }
        /// <summary>
        /// Pk2 path to the refShopGoods file
        /// </summary>
        public string refShopGoodsPath
        {
            get { return m_refShopGoodsPath; }
            set
            {
                // set new value
                m_refShopGoodsPath = value;
                // notify event
                OnPropertyChanged(nameof(refShopGoodsPath));
            }
        }
        /// <summary>
        /// Pk2 path to the TeleportData file
        /// </summary>
        public string TeleportDataPath
        {
            get { return m_TeleportDataPath; }
            set
            {
                // set new value
                m_TeleportDataPath = value;
                // notify event
                OnPropertyChanged(nameof(TeleportDataPath));
            }
        }
        /// <summary>
        /// Pk2 path to the TeleportBuilding file
        /// </summary>
        public string TeleportBuildingPath
        {
            get { return m_TeleportBuildingPath; }
            set
            {
                // set new value
                m_TeleportBuildingPath = value;
                // notify event
                OnPropertyChanged(nameof(TeleportBuildingPath));
            }
        }
        /// <summary>
        /// Pk2 path to the TeleportLink file
        /// </summary>
        public string TeleportLinkPath
        {
            get { return m_TeleportLinkPath; }
            set
            {
                // set new value
                m_TeleportLinkPath = value;
                // notify event
                OnPropertyChanged(nameof(TeleportLinkPath));
            }
        }
        /// <summary>
        /// Flag indicating if the icon images will be extracted if not exists
        /// </summary>
        public bool ExtractIcons
        {
            get { return m_ExtractIcons; }
            set
            {
                // set new value
                m_ExtractIcons = value;
                // notify event
                OnPropertyChanged(nameof(ExtractIcons));
            }
        }
        /// <summary>
        /// Flag indicating if the minimap folder will be extracted if not exists
        /// </summary>
        public bool ExtractMinimap
        {
            get { return m_ExtractMinimap; }
            set
            {
                // set new value
                m_ExtractMinimap = value;
                // notify event
                OnPropertyChanged(nameof(ExtractMinimap));
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
        /// Starts the Pk2 extraction
        /// </summary>
        public ICommand CommandStartExtraction { get; set; }
        /// <summary>
        /// Cancel the Pk2 extraction inmediatly
        /// </summary>
        public ICommand CommandCancelExtraction { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Pk2ExtractorViewModel(Window Window,string FullPath)
        {
            // Save references
            m_Window = Window;
            // Async cancel (safe abort)
            m_ExtractionCancelToken = new CancellationTokenSource();

            // Set path to work
            m_FullPath = FullPath;
            // Silkroad identification
            SilkroadID = GetHashMD5(FullPath);
            m_SilkroadName = Directory.GetParent(FullPath).Name;

            // Set files by default
            SilkroadFilesTypes = new ObservableCollection<SilkroadFilesType>() {
                SilkroadFilesType.vSRO_1188
            };
            m_SilkroadFilesType = SilkroadFilesTypes[0];

            // Add the first line in the logger
            m_TextLogged = DateTime.Now.ToShortFormat() + " Pk2 Extractor | Created by Engels \"JellyBitz\" Quintero";

            // Commands setup
            CommandMinimize = new RelayCommand(() => {
                // Minimize the parent if has
                if(Window.Owner != null)
                   Window.Owner.WindowState = WindowState.Minimized;
                m_Window.WindowState = WindowState.Minimized;
            });
            CommandRestore = new RelayCommand(()=> {
                // Check the WindowState and change it
                m_Window.WindowState = m_Window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
            CommandClose = new RelayCommand(m_Window.Close);
            
            CommandStartExtraction = new RelayCommand(async () => await StartExtraction());
            // Cancel if is extracting only
            CommandCancelExtraction = new RelayCommand(() => { if (IsExtracting) m_ExtractionCancelToken.Cancel(); });
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
        /// The main process of the window,
        /// starts the extraction of all information and generates a database with it
        /// </summary>
        public async Task StartExtraction()
        {
            // Lock the command
            await RunCommandAsync(() => IsExtracting, async () => {
                try
                {
                    await ExtractionProcess(m_ExtractionCancelToken.Token);
                }
                catch {
                    System.Diagnostics.Debug.WriteLine("Extraction process has been canceled");
                }
                finally {
                    // Close database & pk2 before exit
                    m_Database?.Close();
                    m_Pk2?.Close();
                }
            });
        }
        #endregion

        #region Private Helpers
        private async Task ExtractionProcess(CancellationToken token)
        {
            // Starts a timer to check the extraction time on finish
            System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();

            #region Loading Pk2
            WriteLine("Loading the Pk2 file...");
            WriteProcess("Opening Pk2 file...");
            // Just make sure the Blowfish key is set correctly
            if (string.IsNullOrEmpty(BlowfishKey))
                BlowfishKey = "169841";

            // Try to open the Pk2 file
            if (!await Task.Run(() => TryLoadPk2(BlowfishKey)).WaitAsync(token))
            {
                // Show failure error
                WriteLine("Error opening the Pk2 file. Wrong blowfish key");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            WriteLine("Pk2 file has been loaded correctly!");
            WriteProcess("Opened");
            #endregion

            #region Database connection
            // Try to create, and connect to the database
            string dbPath = FileManager.GetDatabaseFile(SilkroadID);

            this.m_Database = new SQLDatabase();
            WriteProcess("Creating database...");
            if (!await Task.Run(() => this.m_Database.Create(dbPath)).WaitAsync(token))
            {
                // Show failure error
                WriteLine("Error creating the database. Please, close all bots and try again!");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            WriteProcess("Connecting to database...");
            if (!await Task.Run(() => m_Database.Connect(dbPath)).WaitAsync(token))
            {
                // Show failure error
                WriteLine("Error connecting to the database. Please, close all bots and try again!");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            WriteLine("Database created and connected successfully!");
            #endregion

            #region Extraction: Silkroad Information
            WriteLine("Extracting server information...");
            WriteProcess("Extracting server information");

            // Create the table which contains the basic info
            await m_Database.ExecuteQuickQueryAsync("CREATE TABLE serverinfo (type VARCHAR(20),data VARCHAR(256))");

            // Add Silkroad files type
            await m_Database.ExecuteQuickQueryAsync("INSERT INTO serverinfo (type,data) VALUES ('type','" + SilkroadFilesType + "')");

            // Add Locale
            WriteProcess("Extracting locale...");
            byte locale = byte.MinValue;
            if (!await Task.Run(()=> TryGetLocale(out locale)).WaitAsync(token))
            {
                WriteLine("Error extracting the locale used by th client");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            await this.m_Database.ExecuteQuickQueryAsync("INSERT INTO serverinfo (type,data) VALUES ('locale','" + locale + "')");
            Locale = locale;

            // Add Version
            WriteProcess("Extracting version...");
            uint version = uint.MinValue;
            if (!await Task.Run(() => TryGetVersion(out version)).WaitAsync(token))
            {
                WriteLine("Error extracting the version used by the client");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            await this.m_Database.ExecuteQuickQueryAsync("INSERT INTO serverinfo (type,data) VALUES ('version','" + version + "')");
            Version = version;

            // Add Division Info
            WriteProcess("Extracting division info...");
            Dictionary<string, List<string>> divisionsInfo = null;
            if (!await Task.Run(() => TryGetDivisionInfo(out divisionsInfo)).WaitAsync(token))
            {
                WriteLine("Error extracting the division info used for the connection");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            // To string line representation
            string divisionInfoText;
            {
                List<string> divs = new List<string>();
                foreach (KeyValuePair<string, List<string>> div in divisionsInfo)
                    divs.Add(div.Key + ":" + string.Join(",", div.Value));
                divisionInfoText = string.Join("|", divs);
            }
            await m_Database.ExecuteQuickQueryAsync("INSERT INTO serverinfo (type,data) VALUES ('divisions','" + divisionInfoText + "')");
            DivisionInfo = divisionInfoText;

            // Add Gateway port
            WriteProcess("Extracting port...");
            ushort gateport = ushort.MinValue;
            if (!await Task.Run(() => TryGetGateport(out gateport)).WaitAsync(token))
            {
                WriteLine("Error extracting the port used for the connection");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }
            await this.m_Database.ExecuteQuickQueryAsync("INSERT INTO serverinfo (type,data) VALUES ('port','" + gateport + "')");
            Gateport = gateport;
            #endregion

            #region Extraction: Client data
            // Extract lang to start loading all text references
            WriteProcess("Extracting language...");
            string langName = string.Empty;
            byte langIndex = byte.MinValue;
            if (!await Task.Run(() => TryGetLanguage(out langIndex, out langName)).WaitAsync(token))
                WriteLine("Error extracting language, using default index (" + langIndex + ")");
            else
                WriteLine("Using " + langName + " (" + langIndex + ") as language");

            // Load text references
            WriteLine("Loading names references...");
            WriteProcess("Loading TextDataName");
            if (!await Task.Run(() => TryLoadTextDataName(langIndex)).WaitAsync(token))
            {
                WriteLine("Error loading TextDataName file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Load system references
            WriteLine("Loading system text references...");
            WriteProcess("Loading TextUISystem");
            if (!await Task.Run(() => TryLoadTextUISystem(langIndex)).WaitAsync(token))
            {
                WriteLine("Error loading TextUISystem file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract system references to database
            WriteLine("Extracting system text references...");
            if (!await Task.Run(() => TryAddTextUISystem(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting TextUISystem file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract regions references to database
            WriteLine("Extracting region names references...");
            if (!await Task.Run(() => TryAddTextZoneName(langIndex, m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting TextZoneName file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract ItemData to database
            WriteLine("Extracting items...");
            if (!await Task.Run(() => TryAddItemData(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting ItemData files");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract MagicOption to database
            WriteLine("Extracting magic options...");
            if (!await Task.Run(() => TryAddMagicOption(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting MagicOption file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract CharacterData to database
            WriteLine("Extracting character objects...");
            if (!await Task.Run(() => TryAddCharacterData(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting CharacterData files");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract LevelData to database
            WriteLine("Extracting levels informaton...");
            if (!await Task.Run(() => TryAddLevelData(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting LevelData file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract SkillMasteryData to database
            WriteLine("Extracting masteries...");
            if (!await Task.Run(() => TryAddSkillMasteryData(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting SkillMasteryData file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract SkillData to database
            WriteLine("Extracting skills...");
            if (!await Task.Run(() => TryAddSkillData(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting SkillData files");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract ShopData to database
            WriteLine("Extracting shops...");
            if (!await Task.Run(() => TryAddShops(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting shop files references");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract TeleportBuilding to database
            WriteLine("Extracting buildings...");
            if (!await Task.Run(() => TryAddBuildings(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting buildings file");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            // Extract TeleportBuilding to database
            WriteLine("Extracting teleports...");
            if (!await Task.Run(() => TryAddTeleportLinks(m_Database)).WaitAsync(token))
            {
                WriteLine("Error extracting teleport files");
                WriteProcess("Error");
                // Abort the extraction
                return;
            }

            WriteLine("Database generated successfully!");
            #endregion

            #region Extraction: Image & Icons
            if (ExtractIcons)
            {
                // Extract icons to silkroad folder
                WriteLine("Extracting icon images...");
                if (!await Task.Run(() => TryExtractIcons(m_Database)).WaitAsync(token))
                {
                    WriteLine("Error extracting icon images");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
            }
            if (ExtractMinimap)
            {
                var a = m_Pk2;
                // Extract minimap to app folder
                WriteLine("Extracting minimap images...");
                if (!await Task.Run(() => TryExtractMinimap()).WaitAsync(token))
                {
                    WriteLine("Error extracting minimap images");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
            }
            #endregion

            // Everything has been extracted just fine
            timer.Stop();
            WriteLine("All has been extracted successfully in "
                + (Math.Round(timer.Elapsed.TotalMinutes) > 0 ? Math.Round(timer.Elapsed.TotalMinutes) + "min " : "")
                + (timer.Elapsed.Seconds > 0 ? timer.Elapsed.Seconds + "s " : "")
            );

            // Auto close this
            for (byte i = 3; i > 0; i--)
            {
                WriteProcess("Closing in " + i + "...");
                await Task.Delay(1000);
            }
            m_Window.DialogResult = true;
        }
        /// <summary>
        /// Creates a hash based on a MD5 algorithm.
        /// </summary>
        /// <param name="value">The value to be converted</param>
        private string GetHashMD5(string value)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(value));

            // Create a new Stringbuilder to collect the bytes and create a string
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        /// <summary>
        /// Try to open and load the Pk2 file (this task can take a noticeable time)
        /// </summary>
        /// <param name="BlowfishKey">Blowfish key to open the Pk2 file</param>
        /// <returns>Return success</returns>
        private bool TryLoadPk2(string BlowfishKey)
        {
            // Try to read the Pk2 file
            try
            {
                // Warning, this process can take a noticeable time
                m_Pk2 = new Pk2Reader(m_FullPath, BlowfishKey);
                return true;
            }
            catch {
                m_Pk2.Close();
                return false;
            }
        }
        /// <summary>
        /// Try to get the Silkroad version from Pk2
        /// </summary>
        /// <returns>Return success</returns>
        private bool TryGetVersion(out uint Version)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = m_Pk2.GetFileStream(VersionPath);
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // Reading the encrypted file
                int versionLength = buffer.ReadInt32();
                byte[] versionBuffer = buffer.ReadBytes(versionLength);

                // Initialize the blowfish to decrypt the file
                Blowfish bf = new Blowfish();
                bf.Initialize(Encoding.ASCII.GetBytes("SILKROADVERSION"), 0, versionLength);

                // Decrypting
                versionBuffer = bf.Decode(versionBuffer);

                // Only four starting bytes contains the numbers
                Version = uint.Parse(Encoding.ASCII.GetString(versionBuffer, 0, 4));

                // Success
                return true;
            }
            catch {
                Version = uint.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Try to get the file localization type
        /// </summary>
        /// <returns>Return success</returns>
        private bool TryGetLocale(out byte Locale)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = m_Pk2.GetFileStream(DivisionInfoPath);
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // Read first byte only
                Locale = buffer.ReadByte();

                // Success
                return true;
            }
            catch
            {
                Locale = byte.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Try to get the gateway list by division names
        /// </summary>
        /// <returns>Return success</returns>
        private bool TryGetDivisionInfo(out Dictionary<string, List<string>> DivisionInfo)
        {
            try
            {
                // Localize the file and prepare to read it 
                Stream data = m_Pk2.GetFileStream(DivisionInfoPath);
                BinaryReader buffer = new BinaryReader(data, Encoding.ASCII);

                // initialize
                DivisionInfo = new Dictionary<string, List<string>>();

                // Ignore locale byte
                buffer.ReadByte();

                // Reading all divitions
                byte divisionCount = buffer.ReadByte();
                for (byte i = 0; i < divisionCount; i++)
                {
                    // Division Name
                    string name = new string(buffer.ReadChars(buffer.ReadInt32()));
                    // skip value (0)
                    buffer.ReadByte();

                    // Division hosts
                    byte hostCount = buffer.ReadByte();

                    List<string> hosts = new List<string>(hostCount);
                    for (byte j = 0; j < hostCount; j++)
                    {
                        // host address
                        string host = new string(buffer.ReadChars(buffer.ReadInt32()));
                        // skip value (0)
                        buffer.ReadByte();

                        // Add host
                        hosts.Add(host);
                    }

                    // Add/overwrite division
                    DivisionInfo[name] = hosts;
                }

                // Success
                return true;
            }
            catch
            {
                DivisionInfo = null;
                return false;
            }
        }
        /// <summary>
        /// Try to get the port available to connect to the server
        /// </summary>
        /// <returns>Return success</returns>
        private bool TryGetGateport(out ushort Gateport)
        {
            try
            {
                // Localize the file and prepare to read it 
                string data = m_Pk2.GetFileText(GateportPath);

                // The file contains the port only
                Gateport = ushort.Parse(data.Trim());

                // Success
                return true;
            }
            catch
            {
                Gateport = ushort.MinValue;
                return false;
            }
        }
        /// <summary>
        /// Try to get the language used by the client to read text references
        /// </summary>
        /// <param name="LangIndex">Column position of the language</param>
        /// <param name="LangName">Language name description</param>
        /// <param name="ex">The exception produced if cannot get the values</param>
        /// <returns>Return success</returns>
        private bool TryGetLanguage(out byte LangIndex,out string LangName)
        {
            try
            {
                string[] lines = m_Pk2.GetFileText(TypePath).Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Go through every .ini line
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains("Language"))
                    {
                        // Check what's language has been used
                        string type = lines[i].Split('=')[1].Replace("\"", "").Trim();
                        LangName = type;
                        switch (type)
                        {
                            case "English":
                                LangIndex = 8;
                                return true;
                            case "Vietnam":
                                LangIndex = 9;
                                return true;
                            case "Russia":
                                LangIndex = 10;
                                return true;
                            default:
                                // Unknown language, just kill the process
                                throw new FileFormatException("Unknown language index");
                        }
                    }
                }
            }catch { }
            // Lang not found or unknown, set default english language
            LangIndex = 8;
            LangName = "English";
            return false;
        }
        /// <summary>
        /// Shortcut to execute an action for the data file(s) from Pk2 path
        /// </summary>
        /// <param name="Path">Pk2 path to the file</param>
        /// <param name="isPointer">Flag to indicate if the file path is actually a file pointing the real data files</param>
        /// <param name="Action">The action to execute for every data file</param>
        private void ForEachDataFile(string Path,bool isPointer, Action<string,string> Action)
        {
            // Check if the file is a pointer to multiple files
            if (isPointer)
            {
                string[] files = m_Pk2.GetFileText(Path).Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                // Add directory
                string dirName = System.IO.Path.GetDirectoryName(Path);
                // Execute the action for each file
                for (int i = 0; i < files.Length; i++)
                    Action.Invoke(dirName + "\\" + files[i], files[i]);
            }
            else
            {
                // Just execute the action on this path
                Action.Invoke(Path,System.IO.Path.GetFileName(Path));
            }
        }
        /// <summary>
        /// Load to memory all data name references
        /// </summary>
        /// <param name="LangIndex">The language to be loaded</param>
        /// <returns>Return success</returns>
        private bool TryLoadTextDataName(byte LangIndex)
        {
            try
            {
                // Init Data holders
                m_TextDataNameRef = new Dictionary<string, string>();
                string line;
                string[] data;
                // Go through evry file
                ForEachDataFile(TextDataNamePointerPath, true, (FilePath,FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split('\t');

                                // Make sure is not empty or broken
                                if (data.Length > LangIndex && data[LangIndex] != "0")
                                    m_TextDataNameRef[data[1]] = data[LangIndex];
                            }
                        }
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Get the name using his reference. Returns an empty string if is not found
        /// </summary>
        /// <param name="SN_Reference">The name givne as reference</param>
        private string GetTextName(string SN_Reference)
        {
            // Try to get the key
            if(m_TextDataNameRef.TryGetValue(SN_Reference, out string result))
                return result;
            // Return empty name as default
            return string.Empty;
        }
        /// <summary>
        /// Load to memory all system text references
        /// </summary>
        /// <param name="LangIndex">The language to be loaded</param>
        /// <returns>Return success</returns>
        private bool TryLoadTextUISystem(byte LangIndex)
        {
            try
            {
                // Init Data holders
                m_TextUISystemRef = new Dictionary<string, string>();
                string line;
                string[] data;
                // Go through evry file
                ForEachDataFile(TextUISystemPath, false, (FilePath,FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split('\t');

                                // Make sure is not empty or broken
                                if (data.Length > LangIndex && data[LangIndex] != "0")
                                    m_TextUISystemRef[data[1]] = data[LangIndex];
                            }
                        }
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Get the message text using his reference. Returns an empty string if is not found
        /// </summary>
        /// <param name="UI_Reference">The name given as reference</param>
        private string GetTextSystem(string UI_Reference)
        {
            // Try to get the key
            if (m_TextUISystemRef.TryGetValue(UI_Reference, out string result))
                return result;
            // Return empty name as default
            return string.Empty;
        }
        /// <summary>
        /// Try to add all TextUISystem references to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddTextUISystem(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE textuisystem ("
                    + "_index INTEGER PRIMARY KEY," // (probably) increase the sqlite performance
                    + "servername VARCHAR(64) UNIQUE,"
                    + "text VARCHAR(256)"
                    + ");";
                db.ExecuteQuickQuery(sql);
                
                // Cache queries
                db.Begin();
                int i = int.MinValue;
                foreach (var kv in m_TextUISystemRef)
                {
                    // Display progress
                    WriteProcess("Extracting TextUISystem (" + ((++i) * 100 / m_TextUISystemRef.Count) + "%)");

                    // INSERT
                    db.Prepare("INSERT INTO textuisystem (_index,servername,text) VALUES (?,?,?);");
                    db.Bind("_index", i);
                    db.Bind("servername", kv.Key);
                    db.Bind("text", kv.Value);
                    db.ExecuteQuery();
                }
                db.End();

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add all TextZoneName references to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddTextZoneName(byte LangIndex,SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE regions ("
                    + "_index INTEGER PRIMARY KEY," // (probably) increase the sqlite performance
                    + "servername VARCHAR(64) UNIQUE,"
                    + "name VARCHAR(64)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Data holders
                string line;
                string[] data;
                int i = 0;

                // Go through evry file
                ForEachDataFile(TextZoneNamePath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split('\t');

                                // Make sure is not empty or broken
                                if (data.Length > LangIndex && data[LangIndex] != "0")
                                {

                                    // Display progress
                                    WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                                    // CHECK IF EXISTS
                                    db.Prepare("SELECT servername FROM regions WHERE servername=?");
                                    db.Bind("servername", data[1]);
                                    db.ExecuteQuery();
                                    if (db.GetResult().Count == 0)
                                    {
                                        // INSERT
                                        db.Prepare("INSERT INTO regions (_index,servername,name) VALUES (?,?,?)");
                                        db.Bind("_index", i++);
                                    }
                                    else
                                    {
                                        // UPDATE
                                        db.Prepare("UPDATE regions SET name=? WHERE servername=?");
                                    }
                                    db.Bind("servername", data[1]);
                                    db.Bind("name", data[LangIndex]);
                                    db.ExecuteQuery();
                                }
                            }
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add all ItemData to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddItemData(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE items ("
                    + "id INTEGER PRIMARY KEY,"
                    + "servername VARCHAR(64),"
                    + "name VARCHAR(64),"
                    + "stack_limit INTEGER,"
                    + "tid2 INTEGER,"
                    + "tid3 INTEGER,"
                    + "tid4 INTEGER,"
                    + "level INTEGER,"
                    + "icon VARCHAR(64)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Data holders
                string line,name;
                string[] data;

                // Go through evry file
                ForEachDataFile(ItemDataPointerPath, true, (FilePath, FileName) => {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data is enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split(new char[] { '\t' }, StringSplitOptions.None);

                                // Display progress
                                WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                                // Try to extract name if has one
                                name = string.Empty;
                                if (data[5] != "xxx")
                                    name = GetTextName(data[5]);

                                // CHECK IF EXISTS
                                db.Prepare("SELECT id FROM items WHERE id=?");
                                db.Bind("id", data[1]);
                                db.ExecuteQuery();
                                if (db.GetResult().Count == 0)
                                {
                                    // INSERT
                                    db.Prepare("INSERT INTO items (id,servername,name,stack_limit,tid2,tid3,tid4,level,icon) VALUES (?,?,?,?,?,?,?,?,?);");
                                }
                                else
                                {
                                    // UPDATE
                                    db.Prepare("UPDATE items SET servername=?,name=?,stack_limit=?,tid2=?,tid3=?,tid4=?,level=?,icon=? WHERE id=?");
                                }
                                db.Bind("id", data[1]);
                                db.Bind("servername", data[2]);
                                db.Bind("name", name);
                                db.Bind("stack_limit", data[57]);
                                db.Bind("tid2", data[10]);
                                db.Bind("tid3", data[11]);
                                db.Bind("tid4", data[12]);
                                db.Bind("level", data[33]);
                                // Normal data has 160 positions approx. this fixes an unnexpected behavior on some servers
                                db.Bind("icon", (data.Length > 150 ? data[54] : data[50]).ToLower());
                                db.ExecuteQuery();
                            }
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add MagicOption to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddMagicOption(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE magicoptions ("
                    + "id INTEGER PRIMARY KEY,"
                    + "servername VARCHAR(64),"
                    + "name VARCHAR(64),"
                    + "degree INTEGER,"
                    + "items VARCHAR(64),"
                    + "value FLOAT,"
                    + "value_max INTEGER,"
                    + "increase BOOLEAN"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init Data holders
                string line,name;
                string[] data;
                List<string> itemTypes = new List<string>();

                // Go through evry file
                ForEachDataFile(MagicOptionPath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split('\t');

                                // Display progress
                                WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                                #region Extracting: Name
                                // Convert to readable name (this way since it's harcoded in client)
                                switch (data[2])
                                {
                                    case "MATTR_INT":
                                        name = GetTextSystem("PARAM_INT");
                                        break;
                                    case "MATTR_STR":
                                        name = GetTextSystem("PARAM_STR");
                                        break;
                                    case "MATTR_DUR":
                                        name = GetTextSystem("PARAM_DUR");
                                        break;
                                    case "MATTR_SOLID":
                                        name = GetTextSystem("PARAM_SOLID");
                                        break;
                                    case "MATTR_LUCK":
                                        name = GetTextSystem("PARAM_LUCK");
                                        break;
                                    case "MATTR_ASTRAL":
                                        name = GetTextSystem("PARAM_ASTRAL");
                                        break;
                                    case "MATTR_ATHANASIA":
                                        name = GetTextSystem("PARAM_ATHANASIA");
                                        break;
                                    // Armor (only)
                                    case "MATTR_HP":
                                        name = GetTextSystem("PARAM_HP");
                                        break;
                                    case "MATTR_MP":
                                        name = GetTextSystem("PARAM_MP");
                                        break;
                                    case "MATTR_ER":
                                        name = GetTextSystem("PARAM_ER");
                                        break;
                                    // Weapons & shield
                                    case "MATTR_HR":
                                        name = GetTextSystem("PARAM_HR");
                                        break;
                                    case "MATTR_EVADE_BLOCK":
                                        name = GetTextSystem("PARAM_IGNORE_BLOCKING");
                                        break;
                                    case "MATTR_EVADE_CRITICAL":
                                        name = GetTextSystem("PARAM_EVADE_CRITICAL");
                                        break;
                                    // Accesories
                                    case "MATTR_RESIST_FROSTBITE":
                                        name = GetTextSystem("PARAM_COLD_RESIST");
                                        break;
                                    case "MATTR_RESIST_ESHOCK":
                                        name = GetTextSystem("PARAM_ESHOCK_RESIST");
                                        break;
                                    case "MATTR_RESIST_BURN":
                                        name = GetTextSystem("PARAM_BURN_RESIST");
                                        break;
                                    case "MATTR_RESIST_POISON":
                                        name = GetTextSystem("PARAM_POISON_RESIST");
                                        break;
                                    case "MATTR_RESIST_ZOMBIE":
                                        name = GetTextSystem("PARAM_ZOMBI_RESIST");
                                        break;
                                    default:
                                        name = "";
                                        break;
                                }
                                #endregion

                                // Add all items assignables to the stone
                                itemTypes.Clear();
                                for (byte j = 30; j < data.Length && data[j] == "1"; j += 2)
                                    itemTypes.Add(data[j - 1]);

                                // CHECK IF EXISTS
                                db.Prepare("SELECT id FROM magicoptions WHERE id=?");
                                db.Bind("id", data[1]);
                                db.ExecuteQuery();
                                if (db.GetResult().Count == 0)
                                {
                                    // INSERT
                                    db.Prepare("INSERT INTO magicoptions (id,servername,name,degree,items,value,value_max,increase) VALUES (?,?,?,?,?,?,?,?);");
                                }
                                else
                                {
                                    // UPDATE
                                    db.Prepare("UPDATE items magicoptions servername=?,name=?,degree=?,items=?,value=?,value_max=?,increase=? WHERE id=?");
                                }
                                db.Bind("id", data[1]);
                                db.Bind("servername", data[2]);
                                db.Bind("name", name);
                                db.Bind("degree", data[4]);
                                db.Bind("items", string.Join(",", itemTypes));
                                db.Bind("value", data[5]);
                                db.Bind("value_max", int.Parse(data[10]) & ushort.MaxValue);
                                db.Bind("increase", data[3] == "+");
                                db.ExecuteQuery();
                            }
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add CharacterData to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddCharacterData(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE characters ("
                    + "id INTEGER PRIMARY KEY,"
                    + "servername VARCHAR(64), "
                    + "name VARCHAR(64),"
                    + "tid2 INTEGER,"
                    + "tid3 INTEGER,"
                    + "tid4 INTEGER,"
                    + "hp INTEGER,"
                    + "level INTEGER"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init Data holders
                string line;
                string[] data;

                // Go through evry file
                ForEachDataFile(CharacterDataPointerPath, true, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines
                            if ((line = reader.ReadLine()) == null)
                                continue;

                            // Data enabled in game
                            if (line.StartsWith("1\t"))
                            {
                                data = line.Split('\t');

                                // Display progress
                                WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");
                                
                                // CHECK IF EXISTS
                                db.Prepare("SELECT id FROM characters WHERE id=?");
                                db.Bind("id", data[1]);
                                db.ExecuteQuery();
                                if (db.GetResult().Count == 0)
                                {
                                    // INSERT
                                    db.Prepare("INSERT INTO characters (id,servername,name,tid2,tid3,tid4,hp,level) VALUES (?,?,?,?,?,?,?,?)");
                                }
                                else
                                {
                                    // UPDATE
                                    db.Prepare("UPDATE characters SET servername=?,name=?,tid2=?,tid3=?,tid4=?,hp=?,level=? WHERE id=?");
                                }
                                db.Bind("id", data[1]);
                                db.Bind("servername", data[2]);
                                db.Bind("name", (data[5] != "xxx"? GetTextName(data[5]):string.Empty));
                                db.Bind("tid2", data[10]);
                                db.Bind("tid3", data[11]);
                                db.Bind("tid4", data[12]);
                                db.Bind("hp", data[59]);
                                db.Bind("level", data[57]);
                                db.ExecuteQuery();
                            }
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add LevelData to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddLevelData(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE leveldata ("
                    + "level INTEGER PRIMARY KEY,"
                    + "player INTEGER,"
                    + "mastery_sp INTEGER,"
                    + "pet INTEGER,"
                    + "trader INTEGER,"
                    + "thief INTEGER,"
                    + "hunter INTEGER"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init Data holders
                string line;
                string[] data;

                // Go through evry file
                ForEachDataFile(LevelDataPath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines and comments
                            line = reader.ReadLine();
                            if (line == null || line.StartsWith("//"))
                                continue;

                            data = line.Split('\t');

                            // Display progress
                            WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                            // INSERT
                            db.Prepare("INSERT INTO leveldata (level,player,mastery_sp,pet,trader,thief,hunter) VALUES (?,?,?,?,?,?,?)");
                            db.Bind("level", data[0]);
                            db.Bind("player", data[1]);
                            db.Bind("mastery_sp", data[2]);
                            db.Bind("pet", data[5]);
                            db.Bind("trader", data[6] == "-1" ? "0" : data[6]); // for safe ulong casting
                            db.Bind("thief", data[7] == "-1" ? "0" : data[7]);
                            db.Bind("hunter", data[8] == "-1" ? "0" : data[8]);
                            db.ExecuteQuery();
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add SkillMasteryData to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddSkillMasteryData(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE masteries ("
                    + "id INTEGER PRIMARY KEY,"
                    + "name VARCHAR(64),"
                    + "description VARCHAR(256),"
                    + "type VARCHAR(64),"
                    + "weapon_types VARCHAR(12),"
                    + "icon VARCHAR(64)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init Data holders
                string line,name,desc,type;
                string[] data;

                // Go through evry file
                ForEachDataFile(SkillMasteryDataPath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            // Skip possible empty lines and comments
                            line = reader.ReadLine();
                            if (line == null || line.StartsWith("//"))
                                continue;

                            data = line.Split('\t');

                            // Avoid wrong data
                            if (data.Length == 13 && data[2] != "xxx")
                            {
                                // Display progress
                                WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                                // Extract name if has one
                                name = GetTextSystem(data[2]);
                                if (name == string.Empty)
                                    name = GetTextName(data[2]);
                                if (name == string.Empty)
                                    name = data[2];
                                
                                // Extract description if has one
                                desc = GetTextSystem(data[4]);
                                if (desc == string.Empty)
                                    desc = GetTextName(data[4]);
                                if (desc == string.Empty)
                                    desc = data[4];

                                // Extract type if has one
                                type = GetTextSystem(data[5]);
                                if (desc == string.Empty)
                                    type = GetTextName(data[5]);
                                if (type == string.Empty)
                                    type = data[5];

                                // INSERT 
                                db.Prepare("INSERT INTO masteries (id,name,description,type,weapon_types,icon) VALUES (?,?,?,?,?,?)");
                                db.Bind("id", data[0]);
                                db.Bind("name", name);
                                db.Bind("description", desc);
                                db.Bind("type", type);
                                db.Bind("weapon_types", data[8] + "," + data[9] + "," + data[10]);
                                db.Bind("icon", data[11]);
                                db.ExecuteQuery();
                            }
                        }

                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add SkillData to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddSkillData(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE skills ("
                    + "id INTEGER PRIMARY KEY,"
                    + "servername VARCHAR(64),"
                    + "name VARCHAR(64),"
                    + "description VARCHAR(1024),"
                    + "casttime INTEGER,"
                    + "duration INTEGER,"
                    + "cooldown INTEGER,"
                    + "mp INTEGER,"
                    + "level INTEGER,"
                    + "mastery_id INTEGER,"
                    + "mastery_sp INTEGER,"
                    + "group_id INTEGER,"
                    + "group_name VARCHAR(64),"
                    + "chain_skill_id INTEGER,"
                    + "weapon_primary INTEGER,"
                    + "weapon_secondary INTEGER,"
                    + "target_required BOOLEAN,"
                    + "parameters VARCHAR(256),"
                    + "icon VARCHAR(64)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init Data holders
                string line,duration;
                string[] data;
                List<string> parameters = new List<string>();
                byte parameters_MAX = 30;

                // Go through evry file
                ForEachDataFile(SkillDataPointerPath, true, (FilePath, FileName) =>
                {
                    // Display progress
                    WriteProcess("Processing " + FileName + "...");

                    // Keep memory safe, decrypts the file if is necessary
                    using (StreamReader reader = new StreamReader(new MemoryStream(SkillData.Decrypt(m_Pk2.GetFileBytes(FilePath)))))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            // Skip empty or disabled lines
                            if (line == null || !line.StartsWith("1\t"))
                                continue;

                            data = line.Split('\t');

                            // Display progress
                            WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                            // Add skill params (just a few, not sure how it determinates his count)
                            parameters.Clear();
                            for (byte i = 0; i < parameters_MAX; i++)
                                parameters.Add(data[SkillData.Param1 + i]);

                            // filter extraction
                            switch (data[SkillData.Param1])
                            {
                                // Buff type
                                case "3":
                                case "10":
                                    duration = SkillData.ReadParamValue(parameters.ToArray(), SkillData.ParamType.SKILL_DURATION);
                                    // Check if cannto be found
                                    if (duration == string.Empty)
                                        // means infinite time
                                        duration = "1";
                                    else if (duration.StartsWith("-"))
                                        // fix negative values
                                        duration = ((uint)int.Parse(duration)).ToString();
                                    break;
                                default:
                                    duration = "0";
                                    break;
                            }

                            // CHECK IF EXISTS
                            db.Prepare("SELECT id FROM skills WHERE id=?");
                            db.Bind("id", data[SkillData.ID]);
                            db.ExecuteQuery();
                            if (db.GetResult().Count == 0)
                            {
                                // INSERT
                                db.Prepare("INSERT INTO skills (id,servername,name,description,casttime,duration,cooldown,mp,level,mastery_id,mastery_sp,group_id,group_name,chain_skill_id,weapon_primary,weapon_secondary,target_required,parameters,icon) VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                            }
                            else
                            {
                                // UPDATE
                                db.Prepare("UPDATE skills SET servername=?,name=?,description=?,casttime=?,duration=?,cooldown=?,mp=?,level=?,mastery_id=?,mastery_sp=?,group_id=?,group_name=?,chain_skill_id=?,weapon_primary=?,weapon_secondary=?,target_required=?,parameters=?,icon=? WHERE id=?");
                            }
                            db.Bind("id", data[SkillData.ID]);
                            db.Bind("servername", data[SkillData.Basic_Code]);
                            db.Bind("name", data[SkillData.UI_SkillName] != "xxx" ? GetTextName(data[SkillData.UI_SkillName]) : string.Empty);
                            db.Bind("description", data[SkillData.UI_SkillToolTip_Desc] != "xxx" ? GetTextName(data[SkillData.UI_SkillToolTip_Desc]) : string.Empty);
                            db.Bind("casttime", int.Parse(data[SkillData.Action_PreparingTime]) + int.Parse(data[SkillData.Action_CastingTime]) + int.Parse(data[SkillData.Action_ActionDuration]));
                            db.Bind("duration", duration);
                            db.Bind("cooldown", data[SkillData.Action_ReuseDelay]);
                            db.Bind("mana", data[SkillData.Consume_MP]);
                            db.Bind("level", data[SkillData.ReqCommon_MasteryLevel1]);
                            db.Bind("mastery_id", data[SkillData.ReqCommon_Mastery1]);
                            db.Bind("mastery_sp", data[SkillData.ReqLearn_SP]);
                            db.Bind("group_id", data[SkillData.GroupID]);
                            db.Bind("group_name", data[SkillData.Basic_Group]);
                            db.Bind("chain_skill_id", data[SkillData.Basic_ChainCode]);
                            db.Bind("weapon_primary", data[SkillData.ReqCast_Weapon1]);
                            db.Bind("weapon_secondary", data[SkillData.ReqCast_Weapon2]);
                            db.Bind("target_required", data[SkillData.Target_Required]);
                            db.Bind("parameters", string.Join(",", parameters));
                            db.Bind("icon", data[SkillData.UI_IconFile]);
                            db.ExecuteQuery();
                        }

                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add ShopGoods to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddShops(SQLDatabase db)
        {
            try
            {
                List<Shop> shops = new List<Shop>();

                #region Loading shops references
                // Init data holders
                string line;
                string[] data;
                
                WriteProcess("Loading refShopGroup.txt");
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refShopGroupPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                         data = line.Split('\t');

                        // Load shop group
                        Shop shop = new Shop()
                        {
                            NPCServerName = data[4],
                            // Fix group mall
                            StoreGroupName = data[3].StartsWith("GROUP_MALL_") ? data[3].Substring(6) : data[3]
                        };
                        // Add
                        shops.Add(shop);
                    }
                }

                WriteProcess("Loading refMappingShopGroup.txt");
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refMappingShopGroupPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        // Loads the shop name
                        foreach (Shop shop in shops)
                        {
                            if (shop.StoreGroupName.StartsWith("MALL"))
                            {
                                // Fix group mall
                                if (shop.StoreGroupName == data[3])
                                {
                                    shop.StoreName = data[3];
                                    shop.StoreGroupName = data[2];
                                }
                            }
                            else if (shop.StoreGroupName == data[2])
                            {
                                shop.StoreName = data[3];
                            }
                        }
                    }
                }

                WriteProcess("Loading refMappingShopWithTab.txt");
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refMappingShopWithTabPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        // Assign groups to the shops
                        foreach (Shop shop in shops)
                        {
                            if (shop.StoreName == data[2])
                            {
                                Shop.Group group = new Shop.Group()
                                {
                                    Name = data[3]
                                };
                                shop.Groups.Add(group);
                            }
                        }
                    }
                }

                WriteProcess("Loading refShopTab.txt");
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refShopTabPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        // Loads the tab references for every group
                        foreach (Shop shop in shops)
                        {
                            foreach (Shop.Group group in shop.Groups)
                            {
                                if (group.Name == data[4])
                                {
                                    Shop.Group.Tab tab = new Shop.Group.Tab()
                                    {
                                        Name = data[3],
                                        Title = GetTextSystem(data[5])
                                    };
                                    group.Tabs.Add(tab);
                                }
                            }
                        }
                    }
                }

                WriteProcess("Loading refScrapOfPackageItem.txt");
                // Load references to all possibles items package
                Dictionary<string, string[]> refScrapOfPackageItem = new Dictionary<string, string[]>();
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refScrapOfPackageItemPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        // Extract item magic options
                        string[] magicOptions = new string[byte.Parse(data[7])];
                        for (byte j = 0; j < magicOptions.Length; j++)
                            magicOptions[j] = data[j + 8];

                        // 0 = itemServerName
                        // 1 = plus
                        // 2 = durability or buyStack (behaviour depends on ID's)
                        // 3 = magic params (blue options)
                        refScrapOfPackageItem[data[2]] = new string[] { data[3], data[4], data[6], string.Join(",", magicOptions) };
                    }
                }

                WriteProcess("Loading refShopGoods.txt");
                // Keep memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(refShopGoodsPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        // Finally load items to tabs from shops
                        foreach (Shop shop in shops)
                        {
                            foreach (Shop.Group group in shop.Groups)
                            {
                                foreach (Shop.Group.Tab tab in group.Tabs)
                                {
                                    if (tab.Name == data[2])
                                    {
                                        if (refScrapOfPackageItem.TryGetValue(data[3], out string[] _refScrapOfPackageItem))
                                        {
                                            // Create item
                                            Shop.Group.Tab.Item item = new Shop.Group.Tab.Item()
                                            {
                                                Name = _refScrapOfPackageItem[0],
                                                Slot = data[4],
                                                Plus = _refScrapOfPackageItem[1],
                                                Durability = _refScrapOfPackageItem[2],
                                                MagicParams = _refScrapOfPackageItem[3]
                                            };
                                            // Add
                                            tab.Items.Add(item);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                // Create table
                string sql = "CREATE TABLE shops ("
                    + "character_servername VARCHAR(64),"
                    + "tab INTEGER,"
                    + "slot INTEGER,"
                    + "item_servername VARCHAR(64),"
                    + "plus INTEGER,"
                    + "durability INTEGER,"
                    + "magicparams VARCHAR(256),"
                    + "PRIMARY KEY (character_servername,tab,slot)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Cache queries
                db.Begin();
                for (int s = 0; s < shops.Count; s++)
                {
                    int tabCount = 0;
                    for (int g = 0; g < shops[s].Groups.Count; g++)
                    {
                        for (int t = 0; t < shops[s].Groups[g].Tabs.Count; t++)
                        {
                            for (int i = 0; i < shops[s].Groups[g].Tabs[t].Items.Count; i++)
                            {
                                // Display progress
                                WriteProcess("Extracting Shops (" + (s * 100 / shops.Count) + "%)");

                                Shop.Group.Tab.Item item = shops[s].Groups[g].Tabs[t].Items[i];

                                // CHECK IF EXISTS
                                db.Prepare("SELECT character_servername FROM shops WHERE character_servername=? AND tab=? AND slot=?");
                                db.Bind("character_servername", shops[s].NPCServerName);
                                db.Bind("tab", tabCount);
                                db.Bind("slot", i);
                                db.ExecuteQuery();
                                if (db.GetResult().Count == 0)
                                {
                                    // INSERT
                                    db.Prepare("INSERT INTO shops (character_servername,tab,slot,item_servername,plus,durability,magicparams) VALUES (?,?,?,?,?,?,?)");
                                }
                                else
                                {
                                    // UPDATE
                                    db.Prepare("UPDATE shops SET item_servername=?,plus=?,durability=?,magicparams=? WHERE character_servername=? AND tab=? AND slot=?");
                                }
                                db.Bind("character_servername", shops[s].NPCServerName);
                                db.Bind("tab", tabCount);
                                db.Bind("slot", i);
                                db.Bind("item_servername", item.Name);
                                db.Bind("plus", item.Plus);
                                db.Bind("durability", item.Durability);
                                db.Bind("magicparams", item.MagicParams);
                                db.ExecuteQuery();
                            }
                            tabCount++;
                        }
                    }
                }
                // Commit
                db.End();
                
                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add TeleportBuildings to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddBuildings(SQLDatabase db)
        {
            try
            {
                // Create the table
                string sql = "CREATE TABLE buildings ("
                    +"id INTEGER PRIMARY KEY,"
                    +"servername VARCHAR(64), "
                    +"name VARCHAR(64),"
                    +"tid2 INTEGER,"
                    +"tid3 INTEGER,"
                    +"tid4 INTEGER"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Init data holders
                string line;
                string[] data;

                // Go through evry file
                ForEachDataFile(TeleportBuildingPath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            // Skip empty or disabled lines
                            if(line == null || !line.StartsWith("1\t"))
                                continue;

                            data = line.Split('\t');

                            // Display progress
                            WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");

                            // CHECK IF EXISTS
                            db.Prepare("SELECT id FROM buildings WHERE id=?");
                            db.Bind("id", data[1]);
                            db.ExecuteQuery();
                            if (db.GetResult().Count == 0)
                            {
                                // INSERT
                                db.Prepare("INSERT INTO buildings (id,servername,name,tid2,tid3,tid4) VALUES(?,?,?,?,?,?)");
                            }
                            else
                            {
                                // UPDATE
                                db.Prepare("UPDATE buildings SET servername=?,name=?,tid2=?,tid3=?,tid4=? WHERE id=?");
                            }
                            db.Bind("id", data[1]);
                            db.Bind("servername", data[2]);
                            db.Bind("name", GetTextName(data[5]));
                            db.Bind("tid2", data[10]);
                            db.Bind("tid3", data[11]);
                            db.Bind("tid4", data[12]);
                            db.ExecuteQuery();
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to add TeleportLinks to database
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryAddTeleportLinks(SQLDatabase db)
        {
            try
            {
                // Init data holders
                string line;
                string[] data;
                Dictionary<string, string[]> TeleportData = new Dictionary<string, string[]>();

                #region Loading Teleport Data
                WriteProcess("Loading TeleportData.txt");
                // Keep Memory safe
                using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(TeleportDataPath)))
                {
                    while (!reader.EndOfStream)
                    {
                        line = reader.ReadLine();
                        // Skip empty or disabled lines
                        if(line == null || !line.StartsWith("1\t"))
                            continue;

                        data = line.Split('\t');

                        TeleportData[data[1]] = data;
                    }
                }
                #endregion

                // Create transactional table, JOIN teleports with links since will be required all the time
                string sql = "CREATE TABLE teleportlinks ("
                    + "source_id INTEGER,"
                    + "destination_id INTEGER,"
                    + "source_name VARCHAR(64),"
                    + "destination_name VARCHAR(64),"
                    + "id INTEGER,"
                    + "servername VARCHAR(64),"
                    + "tid1 INTEGER,"
                    + "tid2 INTEGER,"
                    + "tid3 INTEGER,"
                    + "tid4 INTEGER,"
                    + "gold INTEGER,"
                    + "level INTEGER,"
                    + "source_region INTEGER,"
                    + "source_x INTEGER,"
                    + "source_z INTEGER,"
                    + "source_y INTEGER,"
                    + "destination_region INTEGER,"
                    + "destination_x INTEGER,"
                    + "destination_z INTEGER,"
                    + "destination_y INTEGER,"
                    + "PRIMARY KEY (source_id, destination_id)"
                    + ");";
                db.ExecuteQuickQuery(sql);

                // Data holders
                string sourceName, destinationName, tid1, tid2, tid3, tid4;

                // Go through evry file
                ForEachDataFile(TeleportLinkPath, false, (FilePath, FileName) =>
                {
                    // Keep memory safe
                    using (StreamReader reader = new StreamReader(m_Pk2.GetFileStream(FilePath)))
                    {
                        // Cache queries
                        db.Begin();

                        while (!reader.EndOfStream)
                        {
                            line = reader.ReadLine();
                            // Skip empty or disabled lines
                            if(line == null || !line.StartsWith("1\t"))
                                continue;

                            data = line.Split('\t');

                            // Display progress
                            WriteProcess("Extracting " + FileName + " (" + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%)");
                            
                            // Extract source name if has one
                            db.Prepare("SELECT name,tid2,tid3,tid4 FROM characters WHERE id=?");
                            db.Bind("id",TeleportData[data[1]][3]);
                            db.ExecuteQuery();

                            var result = db.GetResult();
                            if(result.Count == 0)
                            {
                                // Teleports without gate
                                sourceName = GetTextName(TeleportData[data[1]][4]);
                                tid1 = "4";
                                tid2 = tid3 = tid4 = "0";
                            }
                            else
                            {
                                sourceName = result[0]["name"];
                                tid1 = "1"; // character type
                                tid2 = result[0]["tid2"];
                                tid3 = result[0]["tid3"];
                                tid4 = result[0]["tid4"];
                            }

                            // Extract destination name if has one
                            db.Prepare("SELECT name,tid2,tid3,tid4 FROM characters WHERE id=?");
                            db.Bind("id",TeleportData[data[1]][3]);
                            db.ExecuteQuery();

                            result = db.GetResult();
                            if(result.Count == 0)
                                destinationName = GetTextName(TeleportData[data[2]][4]);
                            else
                                destinationName = result[0]["name"];

                            // CHECK IF EXISTS
                            db.Prepare("SELECT source_id FROM teleportlinks WHERE source_id=? AND destination_id=?");
                            db.Bind("source_id",data[1]);
                            db.Bind("destination_id",data[2]);
                            db.ExecuteQuery();
                            if (db.GetResult().Count == 0)
                            {
                                // INSERT
                                db.Prepare("INSERT INTO teleportlinks (source_id,destination_id,source_name,destination_name,id,servername,tid1,tid2,tid3,tid4,gold,level,source_region,source_x,source_y,source_z,destination_region,destination_x,destination_y,destination_z) VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)");
                            }
                            else
                            {
                                // UPDATE
                                db.Prepare("UPDATE teleportlinks SET source_name=?,destination_name=?,id=?,servername=?,tid1=?,tid2=?,tid3=?,tid4=?,gold=?,level=?,source_region=?,source_x=?,source_y=?,source_z=?,destination_region=?,destination_x=?,destination_y=?,destination_z=? WHERE source_id=? AND destination_id=?");
                            }
                            db.Bind("source_id",data[1]);
                            db.Bind("destination_id",data[2]);
                            db.Bind("source_name", sourceName);
                            db.Bind("destination_name", destinationName);
                            db.Bind("id", TeleportData[data[1]][3]);
                            db.Bind("servername", TeleportData[data[1]][2]);
                            db.Bind("tid1", tid1);
                            db.Bind("tid2", tid2);
                            db.Bind("tid3", tid3);
                            db.Bind("tid4", tid4);
                            db.Bind("gold", data[3]);
                            db.Bind("level", data[8]);
                            db.Bind("source_region", (ushort)short.Parse(TeleportData[data[1]][5]));
                            db.Bind("source_x", int.Parse(TeleportData[data[1]][6]));
                            db.Bind("source_z", int.Parse(TeleportData[data[1]][7]));
                            db.Bind("source_y", int.Parse(TeleportData[data[1]][8]));
                            db.Bind("destination_region", (ushort)short.Parse(TeleportData[data[2]][5]));
                            db.Bind("destination_x", int.Parse(TeleportData[data[2]][6]));
                            db.Bind("destination_z", int.Parse(TeleportData[data[2]][7]));
                            db.Bind("destination_y", int.Parse(TeleportData[data[2]][8]));
                            db.ExecuteQuery();
                        }
                        // Commit
                        db.End();
                    }
                });

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to extract all icons used by the database previously generated
        /// </summary>
        /// <param name="db">The database connection</param>
        /// <returns>Return success</returns>
        private bool TryExtractIcons(SQLDatabase db)
        {
            try
            {
                // The folder path to allocate all icons
                string folderPath = FileManager.GetSilkroadFolder(SilkroadID);

                // Check tables with icons
                string[] tables = new string[] {
                    "items","skills"
                };
                foreach(string table in tables)
                {
                    WriteProcess("Checking " + table + " icons...");

                    // Get all icon paths
                    db.ExecuteQuery("SELECT icon FROM "+ table +" GROUP BY icon");
                    var rows = db.GetResult();
                    foreach (var row in rows)
                    {
                        // Fix icon path
                        string iconPath = "icon\\" + row["icon"];

                        // Check if the file exists
                        Pk2File DDJFile = m_Pk2.GetFile(iconPath);
                        if (DDJFile == null)
                            continue;

                        // Check path if the file already exists
                        string saveFilePath = Path.ChangeExtension(Path.GetFullPath(folderPath + iconPath), "png");
                        if (File.Exists(saveFilePath))
                            continue;

                        // Check directory if exists
                        string saveFolderPath = Path.GetDirectoryName(saveFilePath);
                        if (!Directory.Exists(saveFolderPath))
                            Directory.CreateDirectory(saveFolderPath);

                        // Display progress
                        WriteProcess("Creating " + iconPath);

                        // Just in case, avoid wrong formats
                        try
                        {
                            // Read DDJ
                            JMXVDDJ _JMXVDDJ = new JMXVDDJ(m_Pk2.GetFileStream(DDJFile));
                            // Save as PNG
                            _JMXVDDJ.Texture.Save(saveFilePath,ImageFormat.Png);
                        }
                        catch { }
                    }
                }

                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Try to extract all minimap images
        /// </summary>
        /// <returns>Return success</returns>
        private bool TryExtractMinimap()
        {
            try
            {
                // Display progress
                WriteProcess("Checking Worldmap images...");
                Pk2Folder minimapFolder = m_Pk2.GetFolder("Minimap");
                // Just in case
                if (minimapFolder == null)
                    WriteLine("Error, Worldmap images folder not found");
                else
                    ExtractDDJToImages(minimapFolder, FileManager.GetWorldMapFolder(), ImageFormat.Jpeg);
                
                // Display progress
                WriteProcess("Checking Dungeon images...");
                minimapFolder = m_Pk2.GetFolder("Minimap_d");
                // Just in case
                if (minimapFolder == null)
                    WriteLine("Error, Dungeon images folder not found");
                else ExtractDDJToImages(minimapFolder, FileManager.GetDungeonMapFolder(), ImageFormat.Jpeg);

                // Success
                return true;
            }
            catch { return false; }
        }

        /// <summary>
        /// Extract all .ddj files to the output folder path converted to images
        /// </summary>
        /// <param name="Folder">The Pk2 folder with ddj files</param>
        /// <param name="OutputPath">Output folder path to allocate the converted images</param>
        /// <param name="Format">Image format to be converted</param>
        /// <param name="Culture">Culture used to normalize filenames</param>
        private void ExtractDDJToImages(Pk2Folder Folder, string OutputPath, ImageFormat Format)
        {
            // Set the new image extension
            string ext = Format == ImageFormat.Jpeg ? "jpg" : Format.ToString().ToLower();
            // Check all files
            foreach (Pk2File file in Folder.Files)
            {
                // Avoid different extensions
                if (file.GetExtension().ToUpper() != "DDJ")
                    continue;

                // Check path if the file already exists
                string saveFilePath = Path.ChangeExtension(Path.GetFullPath(OutputPath + "\\" + file.Name), ext);
                if (File.Exists(saveFilePath))
                    continue;

                // Display progress
                WriteProcess("Creating " + file.Name);

                // Just in case, avoid wrong formats
                try
                {
                    // Read DDJ
                    JMXVDDJ _JMXVDDJ = new JMXVDDJ(m_Pk2.GetFileStream(file));
                    // Save as PNG
                    _JMXVDDJ.Texture.Save(saveFilePath, Format);
                }
                catch { }
            }
            // Check sub folders
            foreach (Pk2Folder folder in Folder.SubFolders)
                ExtractDDJToImages(folder, OutputPath, Format);
        }
        #endregion
    }
}