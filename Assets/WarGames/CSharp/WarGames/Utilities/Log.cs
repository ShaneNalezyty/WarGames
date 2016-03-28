using UnityEngine;
using System.Collections;
using System.IO;
using System;

/// <summary>
/// Contains WarGames logging functionality.
/// </summary>
namespace WarGames.Utilities {
    /// <summary>
    /// Allows for the creation of agent log files.
    /// </summary>
    public class Log {
        /// <summary>
        /// The folder path and file name to write logs to.
        /// </summary>
        private string folderFileName;
        /// <summary>
        /// The log flags that determine what log messages will write to log.
        /// </summary>
        private char[] logFlags;
        /// <summary>
        /// Initializes a new instance of the <see cref="Log"/> class. 
        /// </summary>
        /// <param name="folderName">Name of the folder to save the log file in.</param>
        /// <param name="agentName">Name of the agent creating this instance.</param>
        /// <param name="flags">Flags determine what will be recorded in the logs. The Flags: B:Behaviours A:Actions, G:Goals, P:Plan, C:Communication Network, K:Knowledge Base, T:Team Info</param>
        public Log(string folderName, string agentName, char[] flags) {
            //All flags: BAGPCKT

            //Save logFlags for later use.
            logFlags = flags;
            //Check if the log directory for this program instance has been created yet.
            if (!Directory.Exists( folderName )) {
                //If not create it.
                Directory.CreateDirectory( folderName );
            }
            //Append the file name to the end of the folder path.
            folderFileName = ( folderName + "/" + agentName + ".txt" );
        }
        /// <summary>
        /// Writes to this agents log. Will only write if the logFlag was enabled on this objects creation.
        /// </summary>
        /// <param name="message">The message to add to this agents log. A time stamp is appended to the front.</param>
        /// <param name="logFlag">The log flag. 
        /// The Flags: B:Behaviours A:Actions, G:Goals, P:Plan, C:Communication Network, K:Knowledge Base, T:Team Info. 
        /// The X flag will always output to the log no matter log flag settings.</param>
        public void WriteToLog(string message, char logFlag) {
            //If logFlag is enabled then write the message to the log.
            if (FlagIsEnabled( logFlag )){
                WritelineWithTime( message );
            }
        }
        /// <summary>
        /// Wrapper function to write to the file.
        /// Function concatenates the date and time before the message and a new line character to the end.
        /// </summary>
        /// <param name="message">The log message to write</param>
        private void WritelineWithTime( string message ) {
            File.AppendAllText( folderFileName, DateTime.Now + ": " + message + "\n");
        }
        /// <summary>
        /// Checks if the flag is enabled for logging.
        /// </summary>
        /// <param name="flag">The flag to check.</param>
        /// <returns>True if flag is enabled. 
        /// False if the flag is disabled.</returns>
        private bool FlagIsEnabled( char flag ) {
            //The X flag is special. Represents log messages that should always print.
            if (flag.Equals( 'X' ) || flag.Equals( 'x' )) {
                return true;
            }
            //For every enabled flag.
            foreach (char enabledFlag in logFlags) {
                //If an enabled flag matches the to check flag
                if (enabledFlag.Equals( flag )) {
                    return true;
                }
            }
            return false;
        }
    }
}