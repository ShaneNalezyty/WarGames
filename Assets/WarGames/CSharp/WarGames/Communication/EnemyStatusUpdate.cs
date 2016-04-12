using UnityEngine;
using System.Collections;
using System.Text;

namespace WarGames.Communication {
	public class EnemyStatusUpdate {
		//Location we saw the enemy agent at.
		private Vector3 targetVector;
		//The fighting status flags gathered sense the agent sent his last EnemyStatusUpdate.
		//We keep a copy of the untrimmed flag string for logging purposes.
		private char flag;

		public Vector3 TargetVector {
			get {
				return targetVector;
			}
		}

		public char Flag {
			get {
				return flag;
			}
		}

		public EnemyStatusUpdate(char fightStatus, Vector3 target) {
			targetVector = new Vector3(target.x, target.y, target.z);
			flag = fightStatus;
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "EnemyStatusUpdate" );
			stringBuilder.AppendLine( "Location of enemy: " + TargetVector.ToString() );
			stringBuilder.AppendLine( "Untrimmed flag stream: " + flag.ToString() );
			return stringBuilder.ToString();
		}
	}
}