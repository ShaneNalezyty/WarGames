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
		public EnemyStatusUpdate[] getEnemyUpdates() {
			return enemyStatusUpdates;
		}
		public Goal getGoal() {
			return agentStatusUpdate.Goal;
		}
		public Plan getPlan() {
			return agentStatusUpdate.Plan;
		}
		public float getPlanProgress() {
			return agentStatusUpdate.PlanProgress;
		}
		public float getAgentHealth() {
			return agentStatusUpdate.Health;
		}
		public DateTime getTimeStamp() {
			return timeStamp;
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