using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

namespace xBot.Data
{
    /// <summary>
    /// Wrapper class to handle SQLite connections
    /// </summary>
    public class SQLDatabase : IDisposable
    {
        #region Private Members
        /// <summary>
        /// Database connection link
        /// </summary>
        private SQLiteConnection m_Connection;
        /// <summary>
        /// Database connection query command used for synchronized prepared commands
        /// </summary>
        private SQLiteCommand m_Command;
        #endregion

        #region Public Properties
        /// <summary>
        /// Path to the database file
        /// </summary>
        public string Path { get; private set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public SQLDatabase()
        {
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Creates a zero-byte database file to be used correctly by SQLite.
        /// This action will try override the file if exists
        /// </summary>
        /// <param name="Path">Path to the output database file</param>
        /// <returns>Return success</returns>
        public bool Create(string Path)
        {
            try
            { 
                // Check if the file already exists and delete it
                if (File.Exists(Path))
                {
                    File.SetAttributes(Path, FileAttributes.Normal);
                    File.Delete(Path);
                }
                // Creates a blank database file
                SQLiteConnection.CreateFile(Path);
                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Creates a connection to the database.
        /// </summary>
        /// <param name="Path">Path to the database file</param>
        /// <returns>Return success</returns>
        public bool Connect(string Path)
        {
            try
            {
                string a = System.IO.Path.GetFullPath(Path);
                // Creates the connection by guessing the database exists
                m_Connection = new SQLiteConnection("Data Source=" + System.IO.Path.GetFullPath(Path) + ";Version=3;");
                // Initialize synchronized commands
                m_Command = new SQLiteCommand(m_Connection);
                // Maximum time to wait queue while executes a query
                m_Command.CommandTimeout = 1000;
                // Try to open
                m_Connection.Open();
                // Save the path reference
                this.Path = Path;
                // Success
                return true;
            }
            catch { return false; }
        }
        /// <summary>
        /// Prepares a synchronized query by binding
        /// </summary>
        /// <param name="CommandText">SQLite query</param>
        /// <returns>Return sucess</returns>
        public bool Prepare(string CommandText)
        {
            if (m_Connection != null)
            {
                // Initialize new params
                m_Command.Parameters.Clear();
                // Set the query to work with
                m_Command.CommandText = CommandText;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Bind column value to the query previously prepared
        /// </summary>
        /// <param name="Column">Column name</param>
        /// <param name="Value">Binding value</param>
        public void Bind(string Column, object Value)
        {
            m_Command.Parameters.Add(new SQLiteParameter(Column, Value));
        }
        /// <summary>
        /// Execute query previously prepared/binded and return the number of columns inserted/affected
        /// </summary>
        public int ExecuteQuery()
        {
            return m_Command.ExecuteNonQuery();
        }
        /// <summary>
        /// Prepare and execute a new query and return the number of columns inserted/affected
        /// </summary>
        public int ExecuteQuery(string CommandText)
        {
            Prepare(CommandText);
            return ExecuteQuery();
        }
        /// <summary>
        /// Gets a list with all rows and columns values from <see cref="ExecuteQuery"/> or <see cref="ExecuteQuery(string)"/>
        /// </summary>
        public List<NameValueCollection> GetResult()
        {
            List<NameValueCollection> result = new List<NameValueCollection>();
            // Gets the database reader
            SQLiteDataReader dataReader = m_Command.ExecuteReader();
            // Reads rows to end
            while (dataReader.Read())
            {   
                // Store all columns values
                result.Add(dataReader.GetValues());
            }
            dataReader.Close();
            // Returns row/column values
            return result;
        }
        /// <summary>
        /// Start a transaction, it will remain open until it is committed or rolled back.
        /// <para>Commit: <see cref="End(string)"/>, Rollback: <see cref="Rollback(string)"/></para>
        /// </summary>
        /// <param name="Name">Transaction name</param>
        public void Begin(string Name = null)
        {
            // Check if the transaction name exists
            ExecuteQuickQuery("BEGIN" + (string.IsNullOrWhiteSpace(Name) ? "" : " " + Name));
        }
        /// <summary>
        /// Ends a transaction and commit the changes to the database
        /// <para>Commit: <see cref="End"/>, Rollback: </para>
        /// </summary>
        /// <param name="Name">Transaction name</param>
        public void End(string Name = null)
        {
            // Check if the transaction name exists
            ExecuteQuickQuery("END" + (string.IsNullOrWhiteSpace(Name) ? "" : " " + Name));
        }
        /// <summary>
        /// Cancel the transaction and roll back all the proposed changes
        /// <para>Commit: <see cref="End"/>, Rollback: </para>
        /// </summary>
        /// <param name="Name">Transaction name</param>
        public void Rollback(string Name = null)
        {
            // Check if the transaction name exists
            ExecuteQuickQuery("ROLLBACK" + (string.IsNullOrWhiteSpace(Name) ? "" : " " + Name));
        }
        /// <summary>
        /// Close the database connection
        /// </summary>
        public void Close()
        {
            if (m_Connection != null)
            {
                // Check if for some reason is already closed
                if (m_Connection.State != System.Data.ConnectionState.Closed)
                {
                    // Dispose objects
                    m_Command.Dispose();
                    m_Connection.Close();
                    m_Connection.Dispose();
                    m_Connection = null;
                }
            }
        }
        /// <summary>
        /// Executes an unsafe query without having conflict at the current synchronized binding
        /// </summary>
        /// <param name="CommandText">SQL query</param>
        public void ExecuteQuickQuery(string CommandText)
        {
            if (m_Connection != null)
            {
                // Set the query to work with and execute inmediatly
                SQLiteCommand command = new SQLiteCommand(CommandText, m_Connection);
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Public Async Methods
        /// <summary>
        /// Executes an unsafe query without creates conflict on the current synchronized binding
        /// and return the number of columns inserted/affected
        /// </summary>
        /// <param name="CommandText">SQL query</param>
        public async Task<int> ExecuteQuickQueryAsync(string CommandText)
        {
            // Set the query to work with and execute inmediatly
            SQLiteCommand command = new SQLiteCommand(CommandText, m_Connection);
            return await command.ExecuteNonQueryAsync();
        }
        /// <summary>
        /// Executes an unsafe query without creates conflict on the current synchronized binding and return the result inmediatly
        /// </summary>
        /// <returns>List containing all rows and column values</returns>
        public async Task<List<NameValueCollection>> GetResultFromQuickQueryAsync(string CommandText)
        {
            List<NameValueCollection> result = new List<NameValueCollection>();
            if (m_Connection != null)
            {
                // Working with Task.Run() since it's prefered to work with SQLiteDataReader (because NameValueCollection)

                // Set the query to work with and execute inmediatly
                SQLiteCommand command = new SQLiteCommand(CommandText, m_Connection);
                await Task.Run(() => command.ExecuteNonQuery());

                // Gets the database reader
                SQLiteDataReader dataReader = await Task.Run(() => command.ExecuteReader());
                // Reads rows to end
                while (await Task.Run(() => dataReader.Read()))
                {
                    // Store all columns values
                    result.Add(await Task.Run(() => dataReader.GetValues()));
                }
            }
            return result;
        }
        #endregion

        #region Destructor
        /// <summary>
        /// Free all resources used by the connection
        /// </summary>
        public void Dispose()
        {
            Close();
            Path = null;
        }
        #endregion
    }
}