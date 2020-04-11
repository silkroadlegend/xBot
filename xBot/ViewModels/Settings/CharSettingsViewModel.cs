using System.IO;
namespace xBot
{
    /// <summary>
    /// View model class to store everything about character settings
    /// </summary>
    public class CharSettingsViewModel
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
        #endregion

        #region Constructor
        /// <summary>
        /// Creates the character default config
        /// </summary>
        /// <param name="ConfigPath">Path to the config file</param>
        public CharSettingsViewModel(string ConfigPath)
        {
            m_ConfigPath = ConfigPath;
        }
        #endregion

        #region Public Methods
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
                    // TO DO: 
                    // The actual loading...

                    // Success
                    return true;
                }
                catch { }
            }
            return false;
        }
        /// <summary>
        /// Save the config file overriding the previous one
        /// </summary>
        /// <returns>Return success</returns>
        public bool Save()
        {
            lock (lockSaving)
            {
                IsSaving = true;

                // TO DO: 
                // The actual saving...

                IsSaving = false;
            }
            return true;
        }
        #endregion
    }
}
