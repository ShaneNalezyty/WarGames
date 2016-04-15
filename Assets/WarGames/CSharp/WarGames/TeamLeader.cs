using UnityEngine;
using System.Collections;
using System.Text;
using WarGames.Communication;

namespace WarGames {
    /// <summary>
    /// Script that holds all variables an agent needs to perform as a WarGames TeamLeader.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TeamLeader : MonoBehaviour {
        /// <summary>
        /// The TeamLeaders KnowledgeBase.
        /// </summary>
        private KnowledgeBase knowledgeBase;
        private Soldier soldier;

        public void Start() {
            knowledgeBase = new KnowledgeBase();
            soldier = gameObject.GetComponent<Soldier>();
            soldier.GetCommNetwork().BecomeTeamLeader( soldier );
        }

		public void CheckMessages() {
			Messageable message = GetMessage();
			if (message != null) {
				if (message is GoalMessage) {
				} else if (message is InformationMessage) {
					InformationMessage msg = (InformationMessage)message;
					knowledgeBase.UpdateFoundEnemies( msg.InformationPackage.GetEnemyUpdates() );
					knowledgeBase.UpdateSoldier( msg.InformationPackage.GetAgentUpdates() );
				} else if (message is RequestMessage) {
				}
			}
		}
		public Messageable GetMessage() {
			if (soldier.GetCommNetwork() != null) {
				Messageable m = soldier.GetCommNetwork().GetMessage( soldier.GetLeaderLabel() );
				//Write the Messageable to the log file.
				if (m != null) {
					WriteToLog( "Got messageable: " + m.ToString(), "C" );
				}
				return m;
			}
			return null;
		}

		private void WriteToLog( string message, string flags ) {
            soldier.WriteToLog( message, flags );
        }
        public override string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Team Leader Info: " );
            stringBuilder.AppendLine( "Knowledge Base Entries: " );
            stringBuilder.AppendLine( knowledgeBase.ToString() );
            return stringBuilder.ToString();
        }
    }
}