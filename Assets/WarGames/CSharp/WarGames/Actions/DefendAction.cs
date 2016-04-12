using UnityEngine;
using System;
using System.Collections;
using System.Text;
using ParagonAI;

namespace WarGames {
    public class DefendAction : Actionable {
        private BaseScript baseScript;
        private Soldier soldier;
        private bool lastRanIdle;
        private bool firstRunOfThisAction = true;
        private FindCoverAction findCoverAction;
        private Vector3 target;
        private float watchTime;

        public DefendAction( BaseScript soldiersBaseScript, Vector3 targetAreaToWatch, float timeToWatch ) {
            baseScript = soldiersBaseScript;
            target = targetAreaToWatch;
            soldier = baseScript.gameObject.GetComponent<Soldier>();
            watchTime = timeToWatch;
        }

        public bool NextAICycle( bool inCombat ) {
            if ( firstRunOfThisAction ) {
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                soldier = baseScript.gameObject.GetComponent<Soldier>();
                soldier.WriteToLog( "I'm starting a DefendAction", "A" );
                firstRunOfThisAction = false;
            }
            if ( inCombat ) {
                if ( lastRanIdle ) {
                    soldier.WriteToLog( "I've entered combat in a DefendAction", "A" );
					findCoverAction = null;
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a DefendAction", "A" );
					findCoverAction = null;
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }

        private bool NextIdleAICycle() {
			baseScript.SetSpeed( baseScript.alertSpeed );
			//Find a cover location that looks at the target position
			if ( findCoverAction == null ) {
				findCoverAction = new FindCoverAction( baseScript, target, watchTime, Vector3.zero );
			}
			return findCoverAction.NextAICycle( true );
		}

        private bool NextCombatAICycle() {
            //If we enter combat we need to adjust our cover to be relevant to our target
            if ( findCoverAction == null ) {
                //If this agent is entering combat we need to create a findCoverAction
                findCoverAction = new FindCoverAction( baseScript, baseScript.targetTransform.position, watchTime, target );
            }
            return findCoverAction.NextAICycle( true );
        }
        public void OnComplete() {

        }
        public void OnEnd() {
            //Make sure if we end that we leave any cover
            if ( findCoverAction != null ) {
                findCoverAction.LeaveCover();
            }
        }
    }
}