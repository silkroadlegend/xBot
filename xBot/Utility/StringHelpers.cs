using System.Security;
using System.Text;
namespace xBot.Utility
{
    /// <summary>
    /// Extensions helper for string types
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Creates a hash based on a MD5 algorithm.
        /// </summary>
        /// <param name="String">The value to be converted</param>
        public static string ToMD5Hash(this string String)
        {
            // Convert the input string to a byte array and compute the hash.
            byte[] data = System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(String));

            // Create a new Stringbuilder to collect the bytes and create a string
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
                sBuilder.Append(data[i].ToString("x2"));

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }
        /// <summary>
        /// Creates a secure string from this instance
        /// </summary>
        /// <param name="String">The value to be converted</param>
        public static SecureString ToSecureString(this string String)
        {
            var secureString = new SecureString();

            foreach (char c in String)
                secureString.AppendChar(c);

            secureString.MakeReadOnly();
            return secureString;
        }
    }
}
