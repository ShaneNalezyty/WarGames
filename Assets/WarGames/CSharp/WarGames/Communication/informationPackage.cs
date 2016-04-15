using UnityEngine;
using System.Text;
using System;

namespace WarGames.Communication {
    /// <summary>
    /// Information about the battle state that a soldier has collected.
    /// </summary>
    public class InformationPackage {
		private EnemyStatusUpdate[] enemyStatusUpdates;
		private AgentStatusUpdate agentStatusUpdate;
		private DateTime timeStamp;

		public InformationPackage(AgentStatusUpdate agentUpdate, EnemyStatusUpdate[] enemyUpdates) {
			enemyStatusUpdates = enemyUpdates;
			agentStatusUpdate = agentUpdate;
			timeStamp = DateTime.Now;
		}
		public EnemyStatusUpdate[] GetEnemyUpdates() {
			return enemyStatusUpdates;
		}
		public AgentStatusUpdate GetAgentUpdates() {
			return agentStatusUpdate;
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "Information Package" );
			stringBuilder.AppendLine( "Time package was created: " + timeStamp.ToString() );
			stringBuilder.Append( agentStatusUpdate.ToString() );
			foreach ( EnemyStatusUpdate enemyUpdate in enemyStatusUpdates ) {
				stringBuilder.Append( enemyUpdate.ToString() );
			}
			return stringBuilder.ToString();
		}
	}
}