using UnityEngine;
using System.Collections;
using ParagonAI;
using System.Text;

namespace WarGames {
    public class FindCoverAction : Actionable {
        private float minTime;
        private float maxTime;
        private BaseScript baseScript;
        private Vector3 defendTarget;
        private bool foundCover = false;
        private bool coroutineRunning = false;
        private Coroutine waitCoroutine;
        public FindCoverAction( BaseScript soldiersBaseScript, float minTimeToStayInCover, float maxTimeToStayInCover, Vector3 targetPos ) {
            minTime = minTimeToStayInCover;
            maxTime = maxTimeToStayInCover;
            baseScript = soldiersBaseScript;
            defendTarget = targetPos;
        }
        public void OnComplete() {
        }
        public void OnEnd() {
            //Make sure we leave cover before ending this action.
            LeaveCover();
        }
        public bool NextAICycle( bool inCombat ) {
            if (inCombat) {
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }
        private bool NextIdleAICycle() {
            Soldier soldier = baseScript.gameObject.GetComponent<Soldier>();
            soldier.WriteToLog( "Started FindCoverAction Idle AICycle", "A".ToCharArray() );
            bool toReturn = false;
            //if (coverFinderScript) {
                //baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
                //If we have a cover location
                if (baseScript.currentCoverNodeScript && foundCover) {
                soldier.WriteToLog( "currentCoverNodeScript or found cover true", "A".ToCharArray() );
                    //If we can't reach our cover, find a different piece of cover.
                    if (baseScript.navI.PathPartial()) {
                        LeaveCover();
                    }
                    //Start the countdown to leave cover once we reach it.
                    if (!baseScript.inCover && baseScript.navI.GetRemainingDistance() <= 0) {
                        baseScript.inCover = true;
                        coroutineRunning = true;
                        waitCoroutine = baseScript.StartCoroutine( SetTimeToLeaveCover( Random.Range( minTime, maxTime ) ) );
                    }
                } else {
                //If we don't have cover then attempt to find cover.
                baseScript.coverFinderScript.currentCoverSeekMethod = CoverFinderScript.CoverSeekMethods.RandomCover;
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                    ParagonAI.CoverData coverData = baseScript.coverFinderScript.FindDynamicCover( defendTarget, baseScript.keyTransform );
                soldier.WriteToLog( coverData.foundCover.ToString(), "A".ToCharArray() );
                //If we succeeded in finding cover then set it.
                //if (coverData.foundCover) {
                SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.coverNodeScript );
                    //}
                }
                if (!(coroutineRunning) && waitCoroutine != null) {
                    toReturn = true;
                }
            //}
            return toReturn;
        }
        private bool NextCombatAICycle() {
            Soldier soldier = baseScript.gameObject.GetComponent<Soldier>();
            soldier.WriteToLog( "Started FindCoverAction Combat AICycle", "A".ToCharArray() );
            CoverFinderScript coverFinderScript = baseScript.coverFinderScript;
            GunScript gunScript = baseScript.gunScript;
            //If the CoverFinderScript is enabled then we should find cover from the enemy agent.
            if (coverFinderScript) {
                //If we are ( or should be ) firing from cover:
                if (gunScript.IsFiring() || baseScript.shouldFireFromCover) {
                    //Set agent to peek over wall
                    baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodeFiringPos;
                } else {
                    //Set agent to hide behind cover
                    baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
                }
                //If we have a cover location
                if (baseScript.currentCoverNodeScript || foundCover) {
                    //If we can't reach our cover, find a different piece of cover.
                    if (baseScript.navI.PathPartial()) {
                        LeaveCover();
                    }
                    //Start the countdown to leave cover once we reach it.
                    if (!baseScript.inCover && baseScript.navI.GetRemainingDistance() <= 0) {
                        baseScript.inCover = true;
                        coroutineRunning = true;
                        baseScript.StartCoroutine( SetTimeToLeaveCover( Random.Range( minTime, maxTime ) ) );
                    }
                } else {
                    //If we don't have cover then attempt to find cover.
                    ParagonAI.CoverData coverData = coverFinderScript.FindDynamicCover( baseScript.targetTransform.position, baseScript.keyTransform );
                    //If we succeeded in finding cover then set it.
                    if (coverData.foundCover) {
                        SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.coverNodeScript );
                    }
                }
            }
            return false;
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
            while (timeToLeave > 0 && (baseScript.currentCoverNodeScript || foundCover)) {
                //Counts down amount of seconds left in this coroutine.
                if (baseScript.inCover) {
                    //If the agent is in cover then remove a quarter of a second from the remaining time.
                    timeToLeave -= 0.25f;
                } else {
                    //If the agent is not in cover then remove a second from the remaining time.
                    timeToLeave--;
                }
                //If the agent has a target
                if (baseScript.targetTransform) {
                    //Makes the agent leave cover if it is no longer safe.  Uses the cover node's built in methods to check.
                    if (!Physics.Linecast( baseScript.currentCoverNodePos, baseScript.targetTransform.position, baseScript.currentBehaviour.layerMask.value )) {
                        LeaveCover();
                    }
                }
                //Wait for a second then run the coroutine cycle again.
                yield return new WaitForSeconds( 1 );
            }
            //If the amount of time we should be in cover has been met and we are still in cover then leave cover
            if (baseScript.currentCoverNodeScript || foundCover) {
                LeaveCover();
                coroutineRunning = false;
            }
        }
        override public string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Find Cover Action:" );
            stringBuilder.AppendLine( "Min time to stay in cover: " + minTime );
            stringBuilder.AppendLine( "Max time to stay in cover: " + maxTime );
            return stringBuilder.ToString( 0, stringBuilder.Length - 1 );
        }
    }
}