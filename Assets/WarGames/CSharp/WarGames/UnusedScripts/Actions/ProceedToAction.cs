using UnityEngine;
using System.Collections;
using ParagonAI;
using System;
using System.Text;

namespace WarGames {
    public class ProceedToAction : Actionable {
        private BaseScript baseScript;
        private Soldier soldier;
        private bool lastRanIdle;
        private bool firstRunOfThisAction = true;
        private Vector3 target;
        private NavmeshInterface navI;
        private float totalDistance;
        private FindCoverAction findCoverAction;

        public ProceedToAction( BaseScript soldiersBaseScript, Vector3 targetPos ) {
            baseScript = soldiersBaseScript;
            target = targetPos;
            soldier = baseScript.gameObject.GetComponent<Soldier>();
            navI = baseScript.navI;
            totalDistance = Vector3.Distance( baseScript.gameObject.transform.position, targetPos );
        }

        public bool NextAICycle( bool inCombat ) {
            if ( firstRunOfThisAction ) {
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                soldier = baseScript.gameObject.GetComponent<Soldier>();
                soldier.WriteToLog( "I'm starting a ProceedToAction", "A" );
                firstRunOfThisAction = false;
            }
            if ( inCombat ) {
                if ( lastRanIdle ) {
                    soldier.WriteToLog( "I've entered combat in a ProceedToAction", "A" );
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a ProceedToAction", "A" );
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }

        private bool NextIdleAICycle() {
            baseScript.currentBehaviour.targetVector = baseScript.keyTransform.position;
            if ( !navI.PathPending() && navI.GetRemainingDistance() < ( totalDistance / 10 ) ) {
                //If we have reached at least 90% to the location then this action is over
                return true;
            }
            return false;
        }

        private bool NextCombatAICycle() {
            //If we are moving and enter combat we should go take cover
            if ( findCoverAction == null ) {
                //If this agent is entering combat we need to create a findCoverAction
                findCoverAction = new FindCoverAction( baseScript, baseScript.targetTransform.position, float.MaxValue, target );
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
        public override string ToString() {
            //Return that this is a ProceedToAction and the target location.
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "ProceedToAction: " );
            stringBuilder.AppendLine( "TargetLocation: " + target );
            return stringBuilder.ToString( 0, stringBuilder.Length - 1 );
        }
    }
}

