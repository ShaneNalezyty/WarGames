using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace WarGames.Communication {
	public class EnemyStatusUpdate {
		//Location we saw the enemy agent at.
		private Vector3 targetVector;
		private bool flag;
		private Soldier soldier;
		private DateTime timeStamp;
		
		public EnemyStatusUpdate(bool fightStatus, Vector3 target, Soldier s) {
			targetVector = new Vector3(target.x, target.y, target.z);
			flag = fightStatus;
			soldier = s;
			timeStamp = DateTime.Now;
		}

		public Vector3 GetTargetVector() {
			return targetVector;
		}

		public bool GetFightStatus() {
			return flag;
		}

		public DateTime GetTimeStamp() {
			return timeStamp;
		}

		public Soldier GetReportingSoldiersKey() {
			return soldier;
		}
		public override string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "EnemyStatusUpdate" );
			stringBuilder.AppendLine( "Location of enemy: " + targetVector.ToString() );
			stringBuilder.AppendLine( "Untrimmed flag stream: " + flag.ToString() );
			return stringBuilder.ToString();
		}
	}
}