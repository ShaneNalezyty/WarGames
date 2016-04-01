using UnityEngine;
using System.Collections;
using System.Text;

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
            soldier.getCommNetwork().BecomeTeamLeader( soldier );
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