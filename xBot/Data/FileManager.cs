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
        /// Get the file path to the database 
        /// </summary>
        /// <param name="SilkroadID">ID to identify the silkroad server</param>
        public static string GetDatabaseFile(string SilkroadID)
        {
            // Base path
            string path = "Data\\" + SilkroadID + "\\";

            // Create directory if not exists
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Add path to file
            path += "Database.sqlite3";

            return path;
        }
        /// <summary>
        /// Get the folder path to the icons
        /// </summary>
        /// <param name="SilkroadID">ID to identify the silkroad server</param>
        public static string GetSilkroadFolder(string SilkroadID)
        {
            // Base path
            string path = "Data\\" + SilkroadID+"\\";

            // Create directory if not exists
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        /// <summary>
        /// Get the folder path to the world map images
        /// </summary>
        public static string GetWorldMapFolder()
        {
            // Base path
            string path = "Minimap";

            // Create directory if not exists
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
        /// <summary>
        /// Get the folder path to the dungeon map images
        /// </summary>
        public static string GetDungeonMapFolder()
        {
            // Base path
            string path = "Minimap\\d";

            // Create directory if not exists
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
