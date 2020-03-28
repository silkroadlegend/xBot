using System.IO;
using System.Text;

namespace xBot.Utility
{
    /// <summary>
    /// Extensions helper for StreamReader
    /// </summary>
    public static class StreamReaderHelpers
    {
        /// <summary>
		/// StreamReader support for using differents limiters
		/// </summary>
		public static string ReadLine(this StreamReader StreamReader, string delimiter)
        {
            char nextChar;
            StringBuilder line = new StringBuilder();
            int matchIndex = 0;

            while (StreamReader.Peek() > 0)
            {
                nextChar = (char)StreamReader.Read();
                line.Append(nextChar);
                if (nextChar == delimiter[matchIndex])
                {
                    if (matchIndex == delimiter.Length - 1)
                    {
                        return line.ToString().Substring(0, line.Length - delimiter.Length);
                    }
                    matchIndex++;
                }
                else
                {
                    matchIndex = 0;
                }
            }
            return line.Length == 0 ? null : line.ToString();
        }
    }
}
