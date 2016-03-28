using UnityEngine;
using WarGames.Communication;
using WarGames.Utilities;
using ParagonAI;

namespace WarGames {
    /// <summary>
    /// Script that holds all variables an agent needs to perform as a WarGames Soldier.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class Soldier : MonoBehaviour {
        /// <summary>
        /// Represents this Soldier's TeamLeader.
        /// </summary>
        private LeaderLabel myLeaderLabel;
        /// <summary>
        /// Soldier's PlannerScript
        /// </summary>
        private Planner myPlanner;
        /// <summary>
        /// Soldier's CommunicationNetwork.
        /// </summary>
        private CommunicationNetwork myCommNetwork;
        /// <summary>
        /// Global LogSettings.
        /// </summary>
        private LogSettings myLogSettings;
        /// <summary>
        /// Soldier's Log script
        /// </summary>
        private Log logger;
        private BaseScript baseScript;
        // Use this for initialization
        public void Start() {
            //If the global CommunicationNetwork is not null
            if (CommunicationNetwork.currentCommNetwork != null) {
                //Grab the reference to the CommunicationNetwork
                myCommNetwork = CommunicationNetwork.currentCommNetwork;
                //Add this Soldier to the CommunicationNetwork
                myCommNetwork.AddSoldier( this );
            }
            //If the global LogSettings are not null
            if (LogSettings.currentLogSettings != null) {
                //Grab the reference to the LogSettings
                myLogSettings = LogSettings.currentLogSettings;
            }
            //Create this Soldier's Log object based on LogSettings.
            logger = new Log( myLogSettings.GetLogFolder(), gameObject.name, myLogSettings.GetLogFlags() );
            //Create the Soldier's Planner
            //Grab the soldier's BaseScript
            baseScript = gameObject.GetComponent<ParagonAI.BaseScript>();
            myPlanner = new Planner( logger, baseScript );
            //Grab the Soldier's TeamID from BaseScript.
            myLeaderLabel = new LeaderLabel( baseScript.myTargetScript.myTeamID );
            //Write the name of the agent to the log file.
            WriteToLog( "My agent name is: " + gameObject.name, 'X' );
            //Write the LeaderLabel to the log file.
            WriteToLog( "My LeaderLabel is: " + myLeaderLabel.ToString(), 'T' );
        }
        /// <summary>
        /// Writes to Soldier's log file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="flag">The log level flag.</param>
        public void WriteToLog( string message, char flag ) {
            //If we have a logger
            if (logger != null) {
                //Write the message to the logger
                logger.WriteToLog( message, flag );
            }
        }
        /// <summary>
        /// Sends a Messageable object to the CommunicationNetwork.
        /// </summary>
        /// <param name="m">The messageable object to send</param>
        public void SendMessage( Messageable m ) {
            myCommNetwork.PassMessage( myLeaderLabel, m );
            //Write the Messageable to the log file.
            logger.WriteToLog( "Sent messageable: " + m.ToString(), 'C' );
        }
        /// <summary>
        /// Gets the next Messageable object from the communicationNetwork
        /// </summary>
        /// <returns>The next Messageable object that was sent to this Soldier.</returns>
        public Messageable GetMessage() {
            Messageable m = myCommNetwork.GetMessage( this );
            //Write the Messageable to the log file.
            logger.WriteToLog( "Sent messageable: " + m.ToString(), 'C' );
            return m;
        }
    }
}