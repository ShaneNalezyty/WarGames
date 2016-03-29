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
        private LeaderLabel leaderLabel;
        /// <summary>
        /// Soldier's PlannerScript
        /// </summary>
        private Planner planner;
        /// <summary>
        /// Soldier's CommunicationNetwork.
        /// </summary>
        private CommunicationNetwork commNetwork;
        /// <summary>
        /// Global LogSettings.
        /// </summary>
        private LogSettings logSettings;
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
                commNetwork = CommunicationNetwork.currentCommNetwork;
                //Add this Soldier to the CommunicationNetwork
                commNetwork.AddSoldier( this );
            }
            //If the global LogSettings are not null
            if (LogSettings.currentLogSettings != null) {
                //Grab the reference to the LogSettings
                logSettings = LogSettings.currentLogSettings;
                //Create this Soldier's Log object based on LogSettings.
                logger = new Log( logSettings.GetLogFolder(), gameObject.name, logSettings.GetLogFlags() );
            }
            //Create the Soldier's Planner
            //Grab the soldier's BaseScript
            baseScript = gameObject.GetComponent<ParagonAI.BaseScript>();
            gameObject.AddComponent<Planner>();
            planner = gameObject.GetComponent<Planner>();
            //Grab the Soldier's TeamID from BaseScript.
            leaderLabel = new LeaderLabel( baseScript.myTargetScript.myTeamID );
            //Write the name of the agent to the log file.
            WriteToLog( "My agent name is: " + gameObject.name, "X".ToCharArray() );
            //Write the LeaderLabel to the log file.
            WriteToLog( "My LeaderLabel is: " + leaderLabel.ToString(), "T".ToCharArray() );
        }
        public void CheckMessages() {
            Messageable message = GetMessage();
            if (message != null) {
                if (message is GoalMessage) {
                    WriteToLog( "Soldier received a new GoalMessage", "GC".ToCharArray() );
                    GoalMessage goalMessage = (GoalMessage)message;
                    SetGoal( goalMessage.GetGoal() );
                } else if (message is InformationMessage) {
                    //Should never happen
                    WriteToLog( "ERROR: Soldier received a InformationMessage", "EC".ToCharArray() );
                } else if (message is RequestMessage) {
                    WriteToLog( "Soldier received a new RequestMessage", "GC".ToCharArray() );
                    InformationPackage informationPackage = new InformationPackage();
                    // TODO: Collect information in package
                    SendMessage( new InformationMessage( informationPackage ) );
                }
            }
        }
        /// <summary>
        /// Writes to Soldier's log file.
        /// </summary>
        /// <param name="message">The message to write.</param>
        /// <param name="flag">The log level flag.</param>
        public void WriteToLog( string message, char[] flags ) {
            //If we have a logger
            if (logger != null) {
                //Write the message to the logger
                logger.WriteToLog( message, flags );
            }
        }
        /// <summary>
        /// Sends a Messageable object to the CommunicationNetwork.
        /// </summary>
        /// <param name="m">The messageable object to send</param>
        public void SendMessage( Messageable m ) {
            if (commNetwork != null) {
                commNetwork.PassMessage( leaderLabel, m );
                //Write the Messageable to the log file.
                logger.WriteToLog( "Sent messageable: " + m.ToString(), "C".ToCharArray() );
            }
            
        }
        /// <summary>
        /// Gets the next Messageable object from the communicationNetwork
        /// </summary>
        /// <returns>The next Messageable object that was sent to this Soldier.</returns>
        public Messageable GetMessage() {
            if (commNetwork != null) {
                Messageable m = commNetwork.GetMessage( this );
                //Write the Messageable to the log file.
                if (m != null) {
                    logger.WriteToLog( "Got messageable: " + m.ToString(), "C".ToCharArray() );
                }
                return m;
            }
            return null;
        }
        public BaseScript GetBaseScript() {
            return baseScript;
        }
        public LeaderLabel GetLeaderLabel() {
            return leaderLabel;
        }
        public Plan GetPlan() {
            if (planner != null) {
                return planner.GetPlan();
            }
            return null;
        }
        public void SetGoal( Goal g ) {
            planner.SetGoal( g );
        }
    }
}