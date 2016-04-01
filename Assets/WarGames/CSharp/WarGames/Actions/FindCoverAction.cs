using UnityEngine;
using System.Collections;
using ParagonAI;
using System.Text;

namespace WarGames {
    public class FindCoverAction : Actionable {
        private BaseScript baseScript;
        private Vector3 defendTarget;
        private bool foundCover = false;
        private bool coroutineRunning = false;
        private bool lastRanIdle;
        private bool firstRunOfThisAction = true;
        private Coroutine waitCoroutine;
        private Soldier soldier;
        private float TimeToWait;
        public FindCoverAction( BaseScript soldiersBaseScript, Vector3 targetPos, float waitTime ) {
            baseScript = soldiersBaseScript;
            defendTarget = targetPos;
            soldier = baseScript.gameObject.GetComponent<Soldier>();
            TimeToWait = waitTime;
        }
        public void OnComplete() {
        }
        public void OnEnd() {
            //Make sure we leave cover before ending this action.
            LeaveCover();
        }
        public bool NextAICycle( bool inCombat ) {
            if ( firstRunOfThisAction ) {
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                soldier.WriteToLog( "I'm starting a FindCoverAction", "A" );
                firstRunOfThisAction = false;
            }
            if ( inCombat ) {
                if ( lastRanIdle ) {
                    soldier.WriteToLog( "I've entered combat in a FindCoverAction", "A" );
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a FindCoverAction", "A" );
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }
        private bool NextIdleAICycle() {
            bool toReturn = false;
            //baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
            //If we have a cover location
            if ( foundCover ) {
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
                //If we can't reach our cover, find a different piece of cover.
                if ( baseScript.navI.PathPartial() ) {
                    LeaveCover();
                }
                //Start the countdown to leave cover once we reach it.
                ReachCoverAndWait();
            } else {
                //If we don't have cover then attempt to find cover.
                CoverData coverData = baseScript.coverFinderScript.FindDynamicCover( defendTarget, null );
                //If we succeeded in finding cover then set it.
                if ( coverData.foundCover ) {
                    soldier.WriteToLog( "I found dynamic Cover! Position: " + coverData.hidingPosition.ToString(), "A" );
                    SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.coverNodeScript );
                }
            }
            if ( !( coroutineRunning ) && waitCoroutine != null ) {
                toReturn = true;
            }
            return toReturn;
        }
        private bool NextCombatAICycle() {
            GunScript gunScript = baseScript.gunScript;
            //If we are ( or should be ) firing from cover:
            if ( gunScript.IsFiring() || baseScript.shouldFireFromCover ) {
                //Set agent to peek over wall
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodeFiringPos;
            } else {
                //Set agent to hide behind cover
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
            }
            if ( foundCover ) {
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
                //If we can't reach our cover, find a different piece of cover.
                if ( baseScript.navI.PathPartial() ) {
                    LeaveCover();
                }
                //Start the countdown to leave cover once we reach it.
                ReachCoverAndWait();
            } else {
                //If we don't have cover then attempt to find cover.
                CoverData coverData = baseScript.coverFinderScript.FindDynamicCover( baseScript.targetTransform.position, null );
                //If we succeeded in finding cover then set it.
                if ( coverData.foundCover ) {
                    soldier.WriteToLog( "I found dynamic Cover! Position: " + coverData.hidingPosition.ToString(), "A" );
                    SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.coverNodeScript );
                }
            }
            return false;
        }

        private void ReachCoverAndWait() {
            if ( !baseScript.inCover && baseScript.navI.GetRemainingDistance() <= 0 ) {
                baseScript.inCover = true;
                coroutineRunning = true;
                if ( TimeToWait == float.MaxValue ) {
                    float timeToStayInCover = Random.Range( baseScript.minTimeInCover, baseScript.maxTimeInCover );
                    soldier.WriteToLog( "Staying in cover for: " + timeToStayInCover.ToString() + " seconds", "A" );
                    waitCoroutine = baseScript.StartCoroutine( SetTimeToLeaveCover( timeToStayInCover ) );
                } else {
                    soldier.WriteToLog( "Staying in cover for: " + TimeToWait.ToString() + " seconds", "A" );
                    waitCoroutine = baseScript.StartCoroutine( SetTimeToLeaveCover( TimeToWait ) );
                }
                
            }
        }

        public void LeaveCover() {
            //Because we find dynamic cover we need to be sure to remove the node when we are done with it.
            ParagonAI.ControllerScript.currentController.RemoveACoverSpot( baseScript.currentCoverNodeFiringPos );
            //Reset agent's BaseScript cover variables.
            baseScript.inCover = false;
            baseScript.SetOrigStoppingDistance();
            //Reset foundCover so we look for new cover if needed.
            foundCover = false;

            //Resets some variables the CoverFinderScript uses
            baseScript.coverFinderScript.ResetLastCoverPos();
        }
        private void SetCover( Vector3 newCoverPos, Vector3 newCoverFiringSpot, ParagonAI.CoverNodeScript newCoverNodeScript ) {
            //Updates the agents BaseScript to contain the currentCover location data
            baseScript.currentCoverNodePos = newCoverPos;
            baseScript.currentCoverNodeFiringPos = newCoverFiringSpot;
            //Allows the agent to stop perfectly behind cover. No sliding due to momentum.
            baseScript.navI.SetStoppingDistance( 0 );
            //Variable CombatAICycle uses to know if it needs to search for cover.
            foundCover = true;
            //Adds the cover spot to the list of all cover spots on the map.
            ParagonAI.ControllerScript.currentController.AddACoverSpot( baseScript.currentCoverNodeFiringPos );
        }
        private IEnumerator SetTimeToLeaveCover( float timeToLeave ) {
            //While the agent should still be in cover
            while ( timeToLeave > 0 && ( baseScript.currentCoverNodeScript || foundCover ) ) {
                //Counts down amount of seconds left in this coroutine.
                if ( baseScript.inCover ) {
                    //If the agent is in cover then remove a second from the remaining time.
                    timeToLeave--;
                } else {
                    //If the agent is not in cover then remove two seconds from the remaining time.
                    timeToLeave -= 2f;
                }
                //If the agent has a target
                if ( baseScript.targetTransform ) {
                    //Makes the agent leave cover if it is no longer safe.  Uses the cover node's built in methods to check.
                    if ( !Physics.Linecast( baseScript.currentCoverNodePos, baseScript.targetTransform.position, baseScript.currentBehaviour.layerMask.value ) ) {
                        LeaveCover();
                    }
                }
                //Wait for a second then run the coroutine cycle again.
                yield return new WaitForSeconds( 1 );
            }
            //If the amount of time we should be in cover has been met and we are still in cover then leave cover
            if ( baseScript.currentCoverNodeScript || foundCover ) {
                LeaveCover();
                coroutineRunning = false;
            }
        }
        override public string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Find Cover Action:" );
            stringBuilder.AppendLine( "Min time to stay in cover: " + baseScript.minTimeInCover );
            stringBuilder.AppendLine( "Max time to stay in cover: " + baseScript.maxTimeInCover );
            if ( baseScript.inCover ) {
                stringBuilder.AppendLine( "Currently in cover" );
            } else {
                stringBuilder.AppendLine( "Currently not cover" );
            }
            return stringBuilder.ToString( 0, stringBuilder.Length - 1 );
        }
    }
}