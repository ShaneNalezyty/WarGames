using UnityEngine;
using System;
using System.Collections;
using System.Text;
using ParagonAI;

namespace WarGames {
    public class AttackAction : Actionable {
        private BaseScript baseScript;
        private Soldier soldier;
        private bool lastRanIdle;
        private bool firstRunOfThisAction = true;
		private FindCoverAction findCoverAction;
		private Vector3 target;
		private NavmeshInterface navI;
		private float totalDistance;

		public AttackAction( BaseScript soldiersBaseScript, Vector3 targetAreaToAttack ) {
			baseScript = soldiersBaseScript;
			target = targetAreaToAttack;
			soldier = baseScript.gameObject.GetComponent<Soldier>();
			navI = baseScript.navI;
			totalDistance = Vector3.Distance( baseScript.gameObject.transform.position, targetAreaToAttack );
		}

		public bool NextAICycle( bool inCombat ) {
            if ( firstRunOfThisAction ) {
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                soldier = baseScript.gameObject.GetComponent<Soldier>();
                soldier.WriteToLog( "I'm starting a AttackAction", "A" );
                firstRunOfThisAction = false;
            }
            if ( inCombat ) {
                if ( lastRanIdle ) {
                    soldier.WriteToLog( "I've entered combat in a AttackAction", "A" );
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a AttackAction", "A" );
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }

        private bool NextIdleAICycle() {
			findCoverAction = null;
			baseScript.SetSpeed( baseScript.runSpeed );
			baseScript.currentBehaviour.targetVector = baseScript.keyTransform.position;
			if ( !navI.PathPending() && navI.GetRemainingDistance() < ( totalDistance / 10 ) ) {
				//If we have reached at least 90% to the location then this action is over
				return true;
			}
			return false;
		}

        private bool NextCombatAICycle() {
			//If we enter combat we need to adjust our cover to be relevant to our target
			if ( findCoverAction == null ) {
				//If this agent is entering combat we need to create a findCoverAction
				findCoverAction = new FindCoverAction( baseScript, baseScript.targetTransform.position, float.MaxValue, target );
			}
			return findCoverAction.NextAICycle( true );
		}

        public void OnComplete() {

        }
        public void OnEnd() {

        }
    }
}