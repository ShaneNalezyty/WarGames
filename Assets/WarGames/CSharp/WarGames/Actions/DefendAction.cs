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
        private NavmeshInterface navI;
        private float watchTime;

        public DefendAction( BaseScript soldiersBaseScript, Vector3 targetAreaToWatch, float timeToWatch ) {
            baseScript = soldiersBaseScript;
            target = targetAreaToWatch;
            soldier = baseScript.gameObject.GetComponent<Soldier>();
            navI = baseScript.navI;
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
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a DefendAction", "A" );
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }

        private bool NextIdleAICycle() {
            throw new NotImplementedException();
        }

        private bool NextCombatAICycle() {
            //If we enter combat we need to adjust our cover to be relevant to our target
            if ( findCoverAction == null ) {
                //If this agent is entering combat we need to create a findCoverAction
                findCoverAction = new FindCoverAction( baseScript, baseScript.targetTransform.position, float.MaxValue );
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