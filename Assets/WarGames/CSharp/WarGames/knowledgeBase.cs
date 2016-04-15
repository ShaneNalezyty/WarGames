using UnityEngine;
using System.Collections;
using WarGames.Communication;

namespace WarGames {
    /// <summary>
    /// Contains collected information about the battlefield.
    /// Only team leaders have a knowledge base, and only team leaders can access the knowledge base.
    /// </summary>
    public class KnowledgeBase {
		private int soldierCount;
		private Hashtable soldierStatus;
		private Hashtable enemyStatus;

		public KnowledgeBase() {
			soldierStatus = new Hashtable();
			enemyStatus = new Hashtable();
			soldierCount = 0;
		}

		public void AddSoldiers( ICollection soldiers ) {
			if (soldiers.Count > soldierCount) {
				soldierCount = soldiers.Count;
				foreach (Soldier s in soldiers) {
					if (!soldierStatus.ContainsKey( s )) {
						soldierStatus.Add( s, new AgentStatusUpdate( s ) );
					}
				}
			}
		}

		public void UpdateSoldier( AgentStatusUpdate asu ) {
			soldierStatus[asu.GetSoldierKey()] = asu;
		}

		public void UpdateFoundEnemies( EnemyStatusUpdate[] esu ) {
			if (!enemyStatus.ContainsKey( esu[0].GetReportingSoldiersKey() )) {
				enemyStatus.Add( esu[0].GetReportingSoldiersKey(), esu );
			} else {
				enemyStatus[esu[0].GetReportingSoldiersKey()] = esu;
			}
		}

		public override string ToString() {
            return base.ToString();
        }
    }
}