namespace Silkroad
{
    /// <summary>
    /// All possibles silkroad files types
    /// </summary>
    public class FilesType
    {
        #region Background Work Setup
        /// <summary>
        /// The value representing this object as string
        /// </summary>
        private string Value;
        /// <summary>
        /// Constructor
        /// </summary>
        private FilesType(string Value)
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
        /// <summary>
        /// Converts a string value to file type, Returns null if the value is not a valid representation
        /// </summary>
        /// <param name="Value">The value to convert</param>
        public static implicit operator FilesType(string Value)
        {
            if (Value == vSRO_1188.Value)
                return vSRO_1188;
            return null;
        }
        #endregion

        #region Types initialization
        /// <summary>
        /// Silkroad files that almost all privates servers use because the community support on internet
        /// </summary>
        public static FilesType vSRO_1188 { get; } = new FilesType("vSRO 1.188");   
        #endregion
    }
}