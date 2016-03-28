using UnityEngine;
using System.Collections;
using System.Text;
using ParagonAI;

namespace WarGames {
    public class WanderAction : Actionable {
        private BaseScript baseScript;
        private bool haveWanderPoint;
        private int wanderDistance;
        private bool foundCover;
        public WanderAction(BaseScript soldiersBaseScript, int distance) {
            baseScript = soldiersBaseScript;
            wanderDistance = distance;
        }
        public void OnComplete() {

        }
        public bool NextAICycle( bool inCombat ) {
            if (inCombat) {
                return NextCombatAICycle();
            } else {
                return NextIdleAICycle();
            }
        }
        private bool NextIdleAICycle() {
            Vector3 targetVector = baseScript.currentBehaviour.targetVector;
            NavmeshInterface navI = baseScript.currentBehaviour.navI;
            if (!haveWanderPoint) {
                targetVector = FindDestinationWithinRadius( baseScript.currentBehaviour.myTransform.position );
                haveWanderPoint = true;
            } else if (!navI.PathPending() && navI.GetRemainingDistance() < (wanderDistance / 10)) {
                haveWanderPoint = false;
            }
            return false;
        }
        private bool NextCombatAICycle() {
            CoverFinderScript coverFinderScript = baseScript.coverFinderScript;
            GunScript gunScript = baseScript.gunScript;
            Vector3 targetVector = baseScript.currentBehaviour.targetVector;
            NavmeshInterface navI = baseScript.navI;
            if (coverFinderScript) {
                //If we are ( or should be ) firing from cover:
                if (gunScript.IsFiring() || baseScript.shouldFireFromCover) {
                    //Set agent to peek over wall
                    targetVector = baseScript.currentCoverNodeFiringPos;
                } else {
                    //Set agent to hide behind cover
                    targetVector = baseScript.currentCoverNodePos;
                }
                //If we have a cover location
                if (baseScript.currentCoverNodeScript || foundCover) {
                    //If we can't reach our cover, find a different piece of cover.
                    if (navI.PathPartial()) {
                        LeaveCover();
                    }
                    //Start the countdown to leave cover once we reach it.
                    if (!baseScript.inCover && navI.GetRemainingDistance() <= 0) {
                        baseScript.inCover = true;
                        StartCoroutine( SetTimeToLeaveCover( Random.Range( minTimeInCover, maxTimeInCover ) ) );
                    } else {
                        //If we don't have cover, find cover.
                        ParagonAI.CoverData coverData = coverFinderScript.FindCover( baseScript.targetTransform, baseScript.keyTransform );

                        if (coverData.foundCover) {
                            SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.isDynamicCover, coverData.coverNodeScript );
                            //Play vocalization
                            if (soundScript)
                                soundScript.PlayCoverAudio();
                        }
                        //If we can't find cover, charge at our target.
                        else if (baseScript.targetTransform) {
                            targetVector = baseScript.targetTransform.position;
                        }
                    }
                }
            }
            return false;
        }
        public void LeaveCover() {
            //Because we find dynamic cover we need to be sure to remove the node when we are done with it.
            ParagonAI.ControllerScript.currentController.RemoveACoverSpot( baseScript.currentCoverNodeFiringPos );

            baseScript.inCover = false;
            baseScript.SetOrigStoppingDistance();

            foundDynamicCover = false;

            if (!baseScript.shouldFireFromCover) {
                coverFinderScript.ResetLastCoverPos();
            }
        }
        override public string ToString() {
            StringBuilder stringBuilder = new StringBuilder();
            return stringBuilder.ToString();
        }
        public Vector3 FindDestinationWithinRadius( Vector3 originPos ) {
            //Returns destination within a square.
            return new Vector3( originPos.x + (Random.value - 0.5f) * wanderDistance, originPos.y, originPos.z + (Random.value - 0.5f) * wanderDistance );
        }
    }
}