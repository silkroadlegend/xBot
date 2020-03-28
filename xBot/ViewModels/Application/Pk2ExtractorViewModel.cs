using PK2ReaderAPI;
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
            m_SkillDataEncPointerPath = "server_dep/silkroad/textdata/SkillDataEnc.txt",
            m_refShopGroupPath = "server_dep/silkroad/textdata/refShopGroup.txt",
            m_refMappingShopGroupPath = "server_dep/silkroad/textdata/refMappingShopGroup.txt",
            m_refMappingShopWithTabPath = "server_dep/silkroad/textdata/refMappingShopWithTab.txt",
            m_refShopTabPath = "server_dep/silkroad/textdata/refShopTab.txt",
            m_refShopGoodsPath = "server_dep/silkroad/textdata/refShopGoods.txt",
            m_refScrapOfPackageItemPath = "server_dep/silkroad/textdata/refScrapOfPackageItem.txt",
            m_TeleportDataPath = "server_dep/silkroad/textdata/TeleportData.txt",
            m_TeleportBuildingPath = "server_dep/silkroad/textdata/TeleportBuilding.txt",
            m_TeleportLinkPath = "server_dep/silkroad/textdata/TeleportLink.txt";
        #endregion

        #region Private Local references used to extract the Pk2 easier
        /// <summary>
        /// The Pk2 reader used to extract all info
        /// </summary>
        private Pk2Reader m_Pk2;
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
        public string SkillDataEncPointerPath
        {
            get { return m_SkillDataEncPointerPath; }
            set
            {
                // set new value
                m_SkillDataEncPointerPath = value;
                // notify event
                OnPropertyChanged(nameof(SkillDataEncPointerPath));
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
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public Pk2ExtractorViewModel(Window Window,string FullPath)
        {
            // Save references
            m_Window = Window;
            
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
            // Database link
            SQLDatabase db = null;

            await RunCommandAsync(() => IsExtracting, async () =>
            {
                #region Loading Pk2
                WriteProcess("Opening Pk2 file...");
                // Just make sure the Blowfish key is set correctly
                if (string.IsNullOrEmpty(BlowfishKey))
                    BlowfishKey = "169841";

                // Try to open the Pk2 file
                if (!await Task.Run(() => TryLoadPk2(BlowfishKey)))
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
                db = new SQLDatabase();
                string dbPath = FileManager.GetDatabasePath(SilkroadID);

                WriteProcess("Creating database...");
                if (!await Task.Run(() => db.Create(dbPath)))
                {
                    // Show failure error
                    WriteLine("Error creating the database. Please, close all bots and try again!");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                WriteProcess("Connecting to database...");
                if (!await Task.Run(() => db.Connect(dbPath)))
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
                await db.ExecuteUnsafeQueryAsync("CREATE TABLE serverinfo (type VARCHAR(20),data VARCHAR(256))");

                // Add Silkroad files type
                await db.ExecuteUnsafeQueryAsync("INSERT INTO serverinfo (type,data) VALUES (\"type\",\"" + SilkroadFilesType + "\")");

                // Add Locale
                WriteProcess("Extracting locale...");
                byte locale = byte.MinValue;
                if (!await Task.Run(() => TryGetLocale(out locale)))
                {
                    WriteLine("Error extracting the locale used by th client");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                await db.ExecuteUnsafeQueryAsync("INSERT INTO serverinfo (type,data) VALUES (\"locale\",\"" + locale + "\")");
                Locale = locale;

                // Add Version
                WriteProcess("Extracting version...");
                uint version = uint.MinValue;
                if (!await Task.Run(() => TryGetVersion(out version)))
                {
                    WriteLine("Error extracting the version used by the client");
                    WriteProcess("Error");
                    // Abort the extraction
                    db.Close();
                    return;
                }
                await db.ExecuteUnsafeQueryAsync("INSERT INTO serverinfo (type,data) VALUES (\"version\",\"" + version + "\")");
                Version = version;

                // Add Division Info
                WriteProcess("Extracting division info...");
                Dictionary<string, List<string>> divisionsInfo = null;
                if (!await Task.Run(() => TryGetDivisionInfo(out divisionsInfo)))
                {
                    WriteLine("Error extracting the division info used for the connection");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                // To string line representation
                string divisionInfoText = await Task.Run(()=> {
                    List<string> divs = new List<string>();
                    foreach (KeyValuePair<string, List<string>> div in divisionsInfo)
                        divs.Add(div.Key + ":" + string.Join(",", div.Value));
                    return string.Join("|", divs);
                });
                await db.ExecuteUnsafeQueryAsync("INSERT INTO serverinfo (type,data) VALUES (\"divisions\",\"" + divisionInfoText + "\")");
                DivisionInfo = divisionInfoText;

                // Add Gateway port
                WriteProcess("Extracting port...");
                ushort gateport = ushort.MinValue;
                if (!await Task.Run(() => TryGetGateport(out gateport)))
                {
                    WriteLine("Error extracting the port used for the connection");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                await db.ExecuteUnsafeQueryAsync("INSERT INTO serverinfo (type,data) VALUES (\"port\",\"" + gateport + "\")");
                Gateport = gateport;
                #endregion

                #region Extraction: Client data
                // Extract lang to start loading all text references
                WriteProcess("Extracting Language...");
                string langName = string.Empty;
                byte langIndex = byte.MinValue;
                if (!await Task.Run(() => TryGetLanguage(out langIndex, out langName)))
                    WriteLine("Error extracting language, using default index (" + langIndex + ")");
                else
                    WriteLine("Using " + langName + " (" + langIndex + ") as language");

                // Load text references
                WriteLine("Loading TextDataName references...");
                WriteProcess("Loading TextDataName");
                if (!await Task.Run(() => TryLoadTextDataName(langIndex)))
                {
                    WriteLine("Error loading TextDataName file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }

                // Load system references
                WriteLine("Loading TextUISystem references...");
                WriteProcess("Loading TextUISystem");
                if (!await Task.Run(() => TryLoadTextUISystem(langIndex)))
                {
                    WriteLine("Error loading TextUISystem file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }

                // Extract system references to database
                WriteLine("Extracting TextUISystem references...");
                if (!await Task.Run(() => TryAddTextUISystem(db)))
                {
                    WriteLine("Error extracting TextUISystem file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                
                // Extract regions references to database
                WriteLine("Extracting TextZoneName references...");
                if (!await Task.Run(() => TryAddTextZoneName(langIndex, db)))
                {
                    WriteLine("Error extracting TextZoneName file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                
                // Extract ItemData to database
                WriteLine("Extracting ItemData...");
                if (!await Task.Run(() => TryAddItemData(db)))
                {
                    WriteLine("Error extracting ItemData files");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }

                // Extract MagicOption to database
                WriteLine("Extracting MagicOption...");
                if (!await Task.Run(() => TryAddMagicOption(db)))
                {
                    WriteLine("Error extracting MagicOption file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }
                // Extract CharacterData to database
                WriteLine("Extracting CharacterData...");
                if (!await Task.Run(() => TryAddCharacterData(db)))
                {
                    WriteLine("Error extracting CharacterData file");
                    WriteProcess("Error");
                    // Abort the extraction
                    return;
                }

                // TO DO: the whole extraction

                #endregion

                WriteLine("Database generated successfully!");
                WriteProcess("Ready");

                // Everything has been generated just fine
                // m_Window.DialogResult = true;
            });

            // Close database & pk2 before exit
            db?.Dispose();
            m_Pk2?.Close();
        }
        #endregion

        #region Private Helpers
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
            catch { return false; }
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
        /// <returns>Return success</returns>
        private bool TryGetLanguage(out byte LangIndex,out string LangName)
        {
            // Targeting english lang
            byte defaultLangIndex = 8;
            string defaultLangName = "English";
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
                                throw new Exception("Unknown language index");
                        }
                    }
                }
            } catch { }
            // Lang not found or unknown
            LangIndex = defaultLangIndex;
            LangName = defaultLangName;
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
        /// Try to get the name using his reference
        /// </summary>
        /// <param name="SN_Reference">The name givne as reference</param>
        private string GetTextName(string SN_Reference)
        {
            // Try to get the key
            if(m_TextDataNameRef.TryGetValue(SN_Reference, out string result))
                return result;
            // Return empty name as default
            return "";
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
        /// Try to get the message text using his reference
        /// </summary>
        /// <param name="UI_Reference">The name given as reference</param>
        private string GetTextSystem(string UI_Reference)
        {
            // Try to get the key
            if (m_TextUISystemRef.TryGetValue(UI_Reference, out string result))
                return result;
            // Return empty name as default
            return "";
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
                db.ExecuteUnsafeQuery(sql);
                
                // Cache queries
                db.Begin();
                int i = int.MinValue;
                foreach (var kv in m_TextUISystemRef)
                {
                    // Display progress
                    WriteProcess("Extracting TextUISystem " + ((++i) * 100 / m_TextUISystemRef.Count) + "%");

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
                db.ExecuteUnsafeQuery(sql);

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
                                    WriteProcess("Extracting " + FileName + " " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");

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
                db.ExecuteUnsafeQuery(sql);

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
                                WriteProcess("Extracting " + FileName + " " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");

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
                db.ExecuteUnsafeQuery(sql);

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
                                WriteProcess("Extracting " + FileName + " " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");

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
                                db.Bind("items", string.Join("|", itemTypes));
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
                db.ExecuteUnsafeQuery(sql);

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
                                WriteProcess("Extracting " + FileName + " " + (reader.BaseStream.Position * 100 / reader.BaseStream.Length) + "%");
                                
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
        #endregion
    }
}