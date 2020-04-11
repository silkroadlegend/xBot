using System.IO;
using xBot.Data;
using xBot.Utility;

namespace xBot
{
    public class SilkroadSetupViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// The name description given to identify the silkroad server
        /// </summary>
        private string m_Name;
        /// <summary>
        /// The server file type
        /// </summary>
        private SilkroadFilesType m_SilkroadFilesType;
        /// <summary>
        /// Path to the launcher executable
        /// </summary>
        private string m_LauncherPath = "Silkroad.exe";
        /// <summary>
        /// Path to the client executable
        /// </summary>
        private string m_ClientPath = "sro_client.exe";
        /// <summary>
        /// Blowfish key used to decrypt the Pk2 file
        /// </summary>
        private string m_BlowfishKey = "169841";
        /// <summary>
        /// Name of media.pk2 file, in case it changed
        /// </summary>
        private string m_MediaPk2FileName = "media.pk2";
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
        #endregion

        #region Public Properties
        /// <summary>
        /// Path to the silkroad folder
        /// </summary>
        public string Path { get; }
        /// <summary>
        /// Unique identifier to the silkroad server
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// Prefered Silkroad name to identify the server
        /// </summary>
        public string Name
        {
            get { return m_Name; }
            set
            {
                // set new value
                m_Name = value;
                // notify event
                OnPropertyChanged(nameof(Name));
            }
        }
        /// <summary>
        /// The choosen type of server, this can change the behavior on the files and extraction
        /// </summary>
        public SilkroadFilesType SilkroadFilesType
        {
            get { return m_SilkroadFilesType; }
            set
            {
                // set new value
                m_SilkroadFilesType = value;
                // notify event
                OnPropertyChanged(nameof(SilkroadFilesType));
            }
        }
        /// <summary>
        /// Location path of the silkroad launcher
        /// </summary>
        public string LauncherPath
        {
            get { return m_LauncherPath; }
            set
            {
                // set new value
                m_LauncherPath = value;
                // notify event
                OnPropertyChanged(nameof(LauncherPath));
            }
        }
        /// <summary>
        /// Location path of the silkroad client
        /// </summary>
        public string ClientPath
        {
            get { return m_ClientPath; }
            set
            {
                // set new value
                m_ClientPath = value;
                // notify event
                OnPropertyChanged(nameof(ClientPath));
            }
        }
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
        /// Filename from the media.pk2 file
        /// </summary>
        public string MediaPk2FileName
        {
            get { return m_MediaPk2FileName; }
            set
            {
                // set new value
                m_MediaPk2FileName = value;
                // notify event
                OnPropertyChanged(nameof(MediaPk2FileName));
            }
        }
        /// <summary>
        /// Version used by the client
        /// </summary>
        public uint Version
        {
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
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a Silkroad item which contains all information
        /// used to extract and generate data from the client files
        /// </summary>
        /// <param name="Path">Path to the Silkroad folder</param>
        public SilkroadSetupViewModel(string Path,string ID)
        {
            this.Name = new DirectoryInfo(Path).Name;
            this.Path = Path;
            this.ID = ID;
            m_LauncherPath = Path + "\\" + m_LauncherPath;
            m_ClientPath = Path + "\\"+ m_ClientPath;
        }
        #endregion
    }
}