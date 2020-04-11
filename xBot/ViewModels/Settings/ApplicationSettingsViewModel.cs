using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;

namespace xBot
{
    /// <summary>
    /// View model class to store everything about application settings
    /// </summary>
    public class ApplicationSettingsViewModel : BaseViewModel
    {
        #region Private Members
        /// <summary>
        /// Path to the config file
        /// </summary>
        private string m_ConfigPath;

        /// <summary>
        /// Locking object to queue all saving actions
        /// </summary>
        private readonly object lockSaving = new object();
        #endregion

        #region Public Properties
        /// <summary>
        /// Check if the config file is being saved
        /// </summary>
        public bool IsSaving { get; private set; }
        /// <summary>
        /// List with all Silkroads servers added
        /// </summary>
        public ObservableCollection<SilkroadSetupViewModel> Silkroads { get; } = new ObservableCollection<SilkroadSetupViewModel>();
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the application default config
        /// </summary>
        /// <param name="ConfigPath">Path to the config file</param>
        public ApplicationSettingsViewModel(string ConfigPath)
        {
            m_ConfigPath = ConfigPath;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Save the config file overriding the previous one
        /// </summary>
        public void Save()
        {
            lock (lockSaving)
            {
                IsSaving = true;

                #region JSON Creation
                // Create all nodes manually
                JObject root = new JObject();

                // Create node
                JObject nSilkroadServers = new JObject();
                root["Silkroad Servers"] = nSilkroadServers;

                foreach (var sr in Silkroads)
                {
                    // Link child node
                    JObject nSilkroadServer = new JObject();
                    nSilkroadServers[sr.ID] = nSilkroadServer;

                    // Fill node
                    nSilkroadServer["Name"] = sr.Name;
                    nSilkroadServer["Launcher"] = sr.LauncherPath;
                    nSilkroadServer["Client"] = sr.ClientPath;
                    nSilkroadServer["Path"] = sr.Path;
                    nSilkroadServer["Files Type"] = sr.FilesType.ToString();
                    nSilkroadServer["Blowfish Key"] = sr.BlowfishKey;
                    nSilkroadServer["Media Pk2"] = sr.MediaPk2FileName;
                    nSilkroadServer["Division Info"] = sr.DivisionInfo;
                    nSilkroadServer["Port"] = sr.Gateport;
                    nSilkroadServer["Version"] = sr.Version;
                    nSilkroadServer["Locale"] = sr.Locale;

                    nSilkroadServer["TypePath"] = sr.TypePath;
                    nSilkroadServer["DivisionInfoPath"] = sr.DivisionInfoPath;
                    nSilkroadServer["GateportPath"] = sr.GateportPath;
                    nSilkroadServer["VersionPath"] = sr.VersionPath;
                    nSilkroadServer["CharacterDataPointerPath"] = sr.CharacterDataPointerPath;
                    nSilkroadServer["ItemDataPointerPath"] = sr.ItemDataPointerPath;
                    nSilkroadServer["LevelDataPath"] = sr.LevelDataPath;
                    nSilkroadServer["MagicOptionPath"] = sr.MagicOptionPath;
                    nSilkroadServer["refMappingShopGroupPath"] = sr.refMappingShopGroupPath;
                    nSilkroadServer["refMappingShopWithTabPath"] = sr.refMappingShopWithTabPath;
                    nSilkroadServer["refScrapOfPackageItemPath"] = sr.refScrapOfPackageItemPath;
                    nSilkroadServer["refShopGoodsPath"] = sr.refShopGoodsPath;
                    nSilkroadServer["refShopGroupPath"] = sr.refShopGroupPath;
                    nSilkroadServer["refShopTabPath"] = sr.refShopTabPath;
                    nSilkroadServer["SkillDataPointerPath"] = sr.SkillDataPointerPath;
                    nSilkroadServer["SkillMasteryDataPath"] = sr.SkillMasteryDataPath;
                    nSilkroadServer["TeleportBuildingPath"] = sr.TeleportBuildingPath;
                    nSilkroadServer["TeleportDataPath"] = sr.TeleportDataPath;
                    nSilkroadServer["TeleportLinkPath"] = sr.TeleportLinkPath;
                    nSilkroadServer["TextDataNamePointerPath"] = sr.TextDataNamePointerPath;
                    nSilkroadServer["TextUISystemPath"] = sr.TextUISystemPath;
                    nSilkroadServer["TextZoneNamePath"] = sr.TextZoneNamePath;
                }

                // Override file
                try
                {
                    File.WriteAllText(m_ConfigPath, root.ToString());
                }
                catch { }

                #endregion

                IsSaving = false;
            }
        }
        /// <summary>
        /// Try to load the config file
        /// </summary>
        /// <returns>Return success</returns>
        public bool Load()
        {
            if (File.Exists(m_ConfigPath))
            {
                try
                {
                    #region JSON Parsing
                    // Main node
                    JObject root = JObject.Parse(File.ReadAllText(m_ConfigPath));

                    Silkroads.Clear();
                    // Check node existence
                    if(root.TryGetValue("Silkroad Servers", out JToken nSilkroadServers))
                    {
                        // Go through every child
                        foreach (JProperty key in ((JObject)nSilkroadServers).Properties())
                        {
                            JObject nSilkroadServer = (JObject)nSilkroadServers[key.Name];

                            // Read node
                            SilkroadSetupViewModel setup = new SilkroadSetupViewModel((string)nSilkroadServer["Path"], key.Name)
                            {
                                Name = (string)nSilkroadServer["Name"],
                                LauncherPath = (string)nSilkroadServer["Launcher"],
                                ClientPath = (string)nSilkroadServer["Client"],
                                FilesType = (string)nSilkroadServer["Files Type"],
                                BlowfishKey = (string)nSilkroadServer["Blowfish Key"],
                                MediaPk2FileName = (string)nSilkroadServer["Media Pk2"],
                                DivisionInfo = (string)nSilkroadServer["Division Info"],
                                Gateport = (ushort)nSilkroadServer["Port"],
                                Version = (uint)nSilkroadServer["Version"],
                                Locale = (byte)nSilkroadServer["Locale"],

                                TypePath = (string)nSilkroadServer["TypePath"],
                                DivisionInfoPath = (string)nSilkroadServer["DivisionInfoPath"],
                                GateportPath = (string)nSilkroadServer["GateportPath"],
                                VersionPath = (string)nSilkroadServer["VersionPath"],
                                CharacterDataPointerPath = (string)nSilkroadServer["CharacterDataPointerPath"],
                                ItemDataPointerPath = (string)nSilkroadServer["ItemDataPointerPath"],
                                LevelDataPath = (string)nSilkroadServer["LevelDataPath"],
                                MagicOptionPath = (string)nSilkroadServer["MagicOptionPath"],
                                refMappingShopGroupPath = (string)nSilkroadServer["refMappingShopGroupPath"],
                                refMappingShopWithTabPath = (string)nSilkroadServer["refMappingShopWithTabPath"],
                                refScrapOfPackageItemPath = (string)nSilkroadServer["refScrapOfPackageItemPath"],
                                refShopGoodsPath = (string)nSilkroadServer["refShopGoodsPath"],
                                refShopGroupPath = (string)nSilkroadServer["refShopGroupPath"],
                                refShopTabPath = (string)nSilkroadServer["refShopTabPath"],
                                SkillDataPointerPath = (string)nSilkroadServer["SkillDataPointerPath"],
                                SkillMasteryDataPath = (string)nSilkroadServer["SkillMasteryDataPath"],
                                TeleportBuildingPath = (string)nSilkroadServer["TeleportBuildingPath"],
                                TeleportDataPath = (string)nSilkroadServer["TeleportDataPath"],
                                TeleportLinkPath = (string)nSilkroadServer["TeleportLinkPath"],
                                TextDataNamePointerPath = (string)nSilkroadServer["TextDataNamePointerPath"],
                                TextUISystemPath = (string)nSilkroadServer["TextUISystemPath"],
                                TextZoneNamePath = (string)nSilkroadServer["TextZoneNamePath"]
                            };

                            // Add
                            Silkroads.Add(setup);
                        }
                    }

                    #endregion

                    // Success
                    return true;
                }
                catch { }
            }
            return false;
        }
        /// <summary>
        /// Asynchronous version method of <see cref="Save"/>
        /// </summary>
        public async void SaveAsync()
        {
            await System.Threading.Tasks.Task.Run(() => Save());
        }
        #endregion
    }
}
