using UnityEngine;
using System.Text;
using WarGames.Utilities;
using System;

namespace WarGames.Communication {
	public class AgentStatusUpdate {
		private Goal goal;
		private float goalProgress;
		private float health;
		private Transform transform;
		private Soldier key;
		private DateTime timeStamp;

		public AgentStatusUpdate( Soldier s ) {
			goal = ObjectCopier.Clone( s.GetGoal() );
			goalProgress = Vector3.SqrMagnitude( transform.position - goal.GetDestination() );
			health = s.GetBaseScript().gameObject.GetComponent<ParagonAI.HealthScript>().health;
			transform = s.transform;
			key = s;
			timeStamp = DateTime.Now;
		}
		public Goal GetGoal() {
			return goal;
		}

		public float GetGoalProgress() {
			return goalProgress;
		}

		public float GetHealth() {
			return health;
		}
		public Soldier GetSoldierKey() {
			return key;
		}
		public DateTime GetTimeStamp() {
			return timeStamp;
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "AgentStatusUpdate" );
			stringBuilder.AppendLine( "Agent's Goal: " + goal.ToString() );
			stringBuilder.AppendLine( "Agent's Health: " + health.ToString() );
			stringBuilder.AppendLine( "Agent's Position: " + transform.ToString() );
			return stringBuilder.ToString();
		}
	}
}