using UnityEngine;
using WarGames.Communication;
using WarGames.Utilities;
using ParagonAI;
using System.Text;
using System.Collections.Generic;

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
        /// Soldier's CommunicationNetwork.
        /// </summary>
        private CommunicationNetwork commNetwork;
        /// <summary>
        /// Soldier's Log script
        /// </summary>
        private Log logger;
        private BaseScript baseScript;
        private Goal currentGoal;
		private WarGamesFindCoverScript findCoverScript;
        // Use this for initialization
        public void Start() {
            //Grab the soldier's BaseScript
            baseScript = gameObject.GetComponent<ParagonAI.BaseScript>();
            //Grab the Soldier's TeamID from BaseScript.
            leaderLabel = new LeaderLabel( baseScript.myTargetScript.myTeamID );
            //If the global CommunicationNetwork is not null
            if (CommunicationDistributor.currentCommDistributor != null) {
                //Grab the reference to the CommunicationNetwork for this team
                commNetwork = CommunicationDistributor.currentCommDistributor.getCommNetwork( GetLeaderLabel() );
                //Add this Soldier to the CommunicationNetwork
                commNetwork.AddSoldier( this );
                WriteToLog( "I have been added to the communication network for team" + GetLeaderLabel(), "C" );
            }
            //If the global LogSettings are not null
            if (LogSettings.currentLogSettings != null) {
                //Create this Soldier's Log object based on LogSettings.
                logger = new Log( LogSettings.currentLogSettings, gameObject.name );
            }
			gameObject.AddComponent<WarGamesFindCoverScript>();
			findCoverScript = gameObject.GetComponent<WarGamesFindCoverScript>();
            //Write the name of the agent to the log file.
            WriteToLog( "My agent name is: " + gameObject.name, "X" );
            //Write the LeaderLabel to the log file.
            WriteToLog( "My LeaderLabel is: " + leaderLabel.ToString(), "T" );
			SetGoal( new Goal( Goal.AggressionLevel.High, new Vector3( 3.5f, 0, -73.1f ) ) );
        }
        public void CheckMessages() {
            Messageable message = GetMessage();
            if (message != null) {
                if (message is GoalMessage) {
                    WriteToLog( "Soldier received a new GoalMessage", "GC" );
                    GoalMessage goalMessage = (GoalMessage)message;
                    SetGoal( goalMessage.GetGoal() );
                } else if (message is InformationMessage) {
                    //Should never happen
                    WriteToLog( "ERROR: Soldier received a InformationMessage", "EC" );
                } else if (message is RequestMessage) {
                    WriteToLog( "Soldier received a new RequestMessage", "GC" );
					InformationPackage informationPackage = BuildInformationPackage();
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
        public void WriteToLog( string message, string flags ) {
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
                WriteToLog( "Sent messageable: " + m.ToString(), "C" );
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
                    WriteToLog( "Got messageable: " + m.ToString(), "C" );
                }
                return m;
            }
            return null;
        }
		public InformationPackage BuildInformationPackage() {
			AgentStatusUpdate agentUpdate = new AgentStatusUpdate( this );
			List<Target> currentVisibleTargets = baseScript.myTargetScript.getAllVisibleTargets();
			List<EnemyStatusUpdate> enemyUpdates = new List<EnemyStatusUpdate>();
			foreach ( Target target in currentVisibleTargets ) {
				if ( target.transform.Equals( baseScript.targetTransform ) ) {
					enemyUpdates.Add( new EnemyStatusUpdate( true, target.transform.position, this ) );
				} else {
					enemyUpdates.Add( new EnemyStatusUpdate( false, target.transform.position, this ) );
				}
			}
			InformationPackage toReturn = new InformationPackage( agentUpdate, enemyUpdates.ToArray() );
			return toReturn;
		}
        public BaseScript GetBaseScript() {
            return baseScript;
        }
        public LeaderLabel GetLeaderLabel() {
            return leaderLabel;
        }
		public Goal GetGoal() {
            return currentGoal;
		}
        public void SetGoal( Goal g ) {
            currentGoal = g;
        }
        public CommunicationNetwork GetCommNetwork() {
            return commNetwork;
        }
		public WarGamesFindCoverScript GetFindCoverScript() {
			return findCoverScript;
		}
    }
}