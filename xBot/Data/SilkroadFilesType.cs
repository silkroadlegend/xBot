namespace xBot.Data
{
    /// <summary>
    /// All possibles silkroad types
    /// </summary>
    public class SilkroadFilesType
    {
        #region Background Work Setup
        /// <summary>
        /// The value representing this object as string
        /// </summary>
        private string Value;
        /// <summary>
        /// Constructor
        /// </summary>
        private SilkroadFilesType(string Value)
        {
            this.Value = Value;
        }
        /// <summary>
        /// Returns the type represented formally
        /// </summary>
        public override string ToString()
        {
            return Value;
        }
        #endregion

        #region Types initialization
        /// <summary>
        /// Silkroad files that almost all privates servers use because the support on internet
        /// </summary>
        public static SilkroadFilesType vSRO_1188 { get; } = new SilkroadFilesType("vSRO 1.188");       
        #endregion
    }

}
