using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xBot.Data
{
    /// <summary>
    /// Manager class to handle path, files and folders
    /// </summary>
    public static class FileManager
    {
       /// <summary>
       /// Get the directory folder path and create it if is necessary.
       /// </summary>
       /// <param name="Type">Folder type</param>
       /// <returns></returns>
        public static string GetDirectory(DirectoryType Type)
        {
            string path = "";

            // Select a valid path
            switch (Type)
            {
                case DirectoryType.Data:
                    path = "Data";
                    break;
                case DirectoryType.Config:
                    path = "Config";
                    break;
                case DirectoryType.Minimap:
                    path = "Minimap";
                    break;
            }

            // Create directory if is necessary
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        /// <summary>
        /// Get the database file path
        /// </summary>
        /// <param name="Name">Unique name to identify the server</param>
        /// <returns></returns>
        public static string GetDatabasePath(string Name)
        {
            // Get the base path
            string path = GetDirectory(DirectoryType.Data);

            // Add the directory path which contains the database
            path += "\\" + Name;

            // Create directory for the if is necessary
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Create the full path to database
            path += "\\Database.sqlite3";

            return path;
        }

        public enum DirectoryType : byte
        {
            Data,
            Config,
            Minimap
        }
    }
}
