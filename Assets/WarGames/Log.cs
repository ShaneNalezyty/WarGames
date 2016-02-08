using System;
using System.IO;

namespace WarGames {
    /// <summary>
    /// Creates a log file for agents to write information to
    /// </summary>
    public class Log {
        private string fileName;
        public Log( string folderName, string agentName ) {
            //Check if the log directory for this program instance has been created yet.
            if (!Directory.Exists( folderName )) {
                Directory.CreateDirectory( folderName );
            }
            //Create the log file for this agent
            fileName = folderName + "/" + agentName + ".txt";
            write( "START" );
        }

        public void write( string info ) {
            string data = DateTime.Now.ToString() + " -- " + info + Environment.NewLine;
            File.AppendAllText( fileName, data );
        }
    }
}
