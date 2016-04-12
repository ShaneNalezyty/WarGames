using UnityEngine;
using System.Text;
using WarGames.Utilities;

namespace WarGames.Communication {
	public class AgentStatusUpdate {
		private Goal goal;
		private Plan plan;
		private float planProgress;
		private float health;
		private Transform transform;
		
		public AgentStatusUpdate( Soldier s ) {
			goal = ObjectCopier.Clone( s.GetGoal() );
			plan = ObjectCopier.Clone( s.GetPlan() );
			planProgress = plan.GetPercentDone();
			health = s.GetBaseScript().gameObject.GetComponent<ParagonAI.HealthScript>().health;
			transform = s.transform;
		}
		public Goal Goal {
			get {
				return goal;
			}
		}

		public Plan Plan {
			get {
				return plan;
			}
		}

		public float PlanProgress {
			get {
				return planProgress;
			}
		}

		public float Health {
			get {
				return health;
			}
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "AgentStatusUpdate" );
			stringBuilder.AppendLine( "Agent's Goal: " + goal.ToString() );
			stringBuilder.AppendLine( "Agent's Plan: " + goal.ToString() );
			stringBuilder.AppendLine( "Agent's Health: " + health.ToString() );
			stringBuilder.AppendLine( "Agent's Position: " + transform.ToString() );
			return stringBuilder.ToString();
		}
	}
}