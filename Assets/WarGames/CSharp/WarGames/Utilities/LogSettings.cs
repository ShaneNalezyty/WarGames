using UnityEngine;
using System.IO;

namespace WarGames.Utilities {
    /// <summary>
    /// LogSettings contains variables to share between all agents. 
    /// These variables are the log folder location, and log flags.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class LogSettings : MonoBehaviour {
        /// <summary>
        /// All agents need the same name for the log folder, because the folder is named with the date and time the name must be created once here and grabbed by every agent.
        /// </summary>
        private string logFolder;
        /// <summary>
        /// Flags determine what will be recorded in the logs. The Flags: B:Behaviours A:Actions, G:Goals, P:Plan, C:Communication Network, K:Knowledge Base, T:Team Info, E:Errors
        /// Note Flag X exists, but cannot be disabled. 
        /// </summary>
        private char[] logFlags;
        /// <summary>
        /// Pointer to shared logSettings object.
        /// </summary>
        private bool deleteOldLogs = false;

        public static WarGames.Utilities.LogSettings currentLogSettings = null;

        void Awake() {
            //If we want old logs to be deleted before we start logging.
            if ( DeleteOldLogs() ) {
                clearFolder( "./WarGames-Logs" );
            }
            //Sets the first logSettings object to run to the shared.
            currentLogSettings = this;
            //Set the log flags.
            logFlags = "BAGPCKT".ToCharArray();
            
        }

        void Start() {
            //Generate a name for the log file indicating this run
            logFolder = "./WarGames-Logs/" + System.DateTime.Now.ToString( "yyyy-MM-dd_hh-mm-ss" );
        }
        /// <summary>
        /// Gets the location of the folder to store log files for this instance of WarGames.
        /// </summary>
        /// <returns>Log folder location</returns>
        public string GetLogFolder() {
            return logFolder;
        }
        /// <summary>
        /// Gets the log flags.
        /// </summary>
        /// <returns>Enabled log flags</returns>
        public char[] GetLogFlags() {
            return logFlags;
        }
        public bool DeleteOldLogs() {
            return deleteOldLogs;
        }
        private void clearFolder( string FolderName ) {
            DirectoryInfo dir = new DirectoryInfo( FolderName );
            foreach ( FileInfo fi in dir.GetFiles() ) {
                fi.Delete();
            }
            foreach ( DirectoryInfo di in dir.GetDirectories() ) {
                clearFolder( di.FullName );
                di.Delete();
            }
        }
    }
}

