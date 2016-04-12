using UnityEngine;
using System.Collections;
using ParagonAI;
using System.Collections.Generic;

namespace WarGames.Behaviour {
    /// <summary>
    /// Behaviour for planning and executing action plans to satisfy a provided goal while in combat.
    /// </summary>
    /// <seealso cref="WarGames.Behaviour.WarGamesBehaviour" />
    /// <seealso cref="WarGames.Goal" />
    public class CombatPlanningBehaviour : WarGames.Behaviour.WarGamesBehaviour {
        bool foundCover = false;
        bool firstCover = true;
        bool coroutineRunning = false;
        private Coroutine waitCoroutine;
        public override void Initiate() {
            base.Initiate();
            //Set the behaviour level
            behaveLevel = BehaviourLevel.Combat;
            //Log the initiation of this behaviour
            soldier.WriteToLog( "CombatPlanningBehaviour initiated.", "B" );
        }

        public override void AICycle() {
            soldier.CheckMessages();
            if (soldier.GetGoal() == null) {
                return;
            }
            GunScript gunScript = baseScript.gunScript;
            //If we are ( or should be ) firing from cover:
            if (gunScript.IsFiring() || baseScript.shouldFireFromCover) {
                //Set agent to peek over wall
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodeFiringPos;
            } else {
                //Set agent to hide behind cover
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
            }
            if (foundCover) {
                baseScript.currentBehaviour.targetVector = baseScript.currentCoverNodePos;
                //If we can't reach our cover, find a different piece of cover.
                if (baseScript.navI.PathPartial()) {
                    LeaveCover();
                }
                //Start the countdown to leave cover once we reach it.
                ReachCoverAndWait();
            } else {
                //If we don't have cover then attempt to find cover.
                CoverData coverData;
                if (firstCover) {
                    coverData = FindBestCover( baseScript.targetTransform.position, null, soldier.GetGoal().GetDestination(), true );
                    firstCover = false;
                } else {
                    coverData = FindBestCover( baseScript.targetTransform.position, null, soldier.GetGoal().GetDestination(), false );
                }
                //If we succeeded in finding cover then set it.
                if (coverData.foundCover) {
                    soldier.WriteToLog( "I found dynamic Cover! Position: " + coverData.hidingPosition.ToString(), "A" );
                    SetCover( coverData.hidingPosition, coverData.firingPosition, coverData.coverNodeScript );
                }
            }
        }

        public override void EachFrame() {

        }

        public override void OnEndBehaviour() {
            foundCover = false;
            firstCover = false;
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
        private void ReachCoverAndWait() {
            if (!baseScript.inCover && baseScript.navI.GetRemainingDistance() <= 0) {
                baseScript.inCover = true;
                coroutineRunning = true;
                float[] timeRange = soldier.GetGoal().getWaitRange();
                float timeToWait = UnityEngine.Random.Range( timeRange[0], timeRange[1] );
                soldier.WriteToLog( "Staying in cover for: " + timeToWait.ToString() + " seconds", "A" );
                waitCoroutine = StartCoroutine( SetTimeToLeaveCover( timeToWait ) );
            }
        }
        private IEnumerator SetTimeToLeaveCover( float timeToLeave ) {
            //While the agent should still be in cover
            while (timeToLeave > 0 && (baseScript.currentCoverNodeScript || foundCover)) {
                //Counts down amount of seconds left in this coroutine.
                if (baseScript.inCover) {
                    //If the agent is in cover then remove a second from the remaining time.
                    timeToLeave--;
                } else {
                    //If the agent is not in cover then remove two seconds from the remaining time.
                    timeToLeave -= 2f;
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
        private CoverData FindBestCover( Vector3 targetTransformPos, Transform transformToDefend, Vector3 goalPos, bool useFirstCover ) {
            //Grab the NavmeshInterface
            NavmeshInterface navI = baseScript.navI;
            //Grab all the veracities in our navigation area
            Vector3[] verts = navI.GetNavmeshVertices();
            //A lot of variables needed exist in the agents coverFinderScript already
            CoverFinderScript coverScript = baseScript.coverFinderScript;
            //Minimum distance we should stay from an enemy
            float minCoverDistSqrd = coverScript.minCoverDistFromEnemy * coverScript.minCoverDistFromEnemy;
            //Maximum distance we should stay from the enemy
            float maxCoverDistSqrd = coverScript.maxCoverDistFromEnemy * coverScript.maxCoverDistFromEnemy;
            //Grab the layerMask. (The LayerMask contains all the objects other object can collide into. This is important for ray cast checks for sight)
            LayerMask layerMask = ParagonAI.ControllerScript.currentController.GetLayerMask();
            //Grab the agent's transform for location use
            Transform myTransform = baseScript.transform;
            //Set the position to defend to the agent.
            Vector3 positionToDefend = myTransform.position;
            //If we are attempting to defend a location other then the agent then set it as the position to defend.
            if (transformToDefend) {
                positionToDefend = transformToDefend.position;
            }
            //Add how high off the ground line of sight checks for cover safety should be performed
            positionToDefend.y += coverScript.dynamicCoverNodeHeightOffset;
            //Find the distance this agent needs to go down vertically to be hidden
            var hideOffset = coverScript.dynamicCoverNodeFireOffset - coverScript.dynamicCoverNodeHeightOffset;
            //Used to check if the found cover position is a location the agent can fire over or if the agent needs to side step
            Vector3 hidingPosCheckingNow;
            //Holds the distance between the target location and the agent		
            float currDistTarget;
            //Holds the location an agent will stand when hiding in cover
            Vector3 coverHidePos = Vector3.zero;
            //Holds the location an agent will stand when firing from cover
            Vector3 coverFirePos = Vector3.zero;
            //Used to calculate if a found cover node is closer then the previous
            //Initialized to the max possible distance so any node we find that is farther we throw out
            float closestDistToMeSoFarSqr = coverScript.dynamicCoverMaxDistFromMe * coverScript.dynamicCoverMaxDistFromMe;
            //Holds the distance between the agent and the currently found cover location
            float distBetweenMeAndCoverNow;
            //Used to skip side stepping cover check if the agent can crouch fire at the cover location
            bool shouldCont = true;
            //If we are not looking for the first available cover then we will need a List to hold all potential cover nodes.
            List<CoverData> possibleCoverNodes = new List<CoverData>();

            //Then test whether we can hide from enemy fire by either crouching or moving off to the side (distance to move is hideOffset)

            //Use each vertex on the navmesh as a potential "firing position"
            //If we can see the enemy from the firing position and not see them from the hiding position, then it is a valid cover spot.
            for (int i = 0; i < verts.Length; i++) {
                //random value to make sure we don't take the same cover every time
                //if ( Random.value > 0.5 && Vector3.SqrMagnitude( verts[i] - positionToDefend ) > coverScript.minDistBetweenLastCover ) {

                //If the possible location is farther then the set minimum space to travel before the agents last cover location
                if (Vector3.SqrMagnitude( verts[i] - positionToDefend ) > coverScript.minDistBetweenLastCover) {
                    //Get the distance between the possible cover location and the enemy agent
                    currDistTarget = Vector3.SqrMagnitude( verts[i] - targetTransformPos );
                    //Get the distance between the possible cover location and this agent
                    distBetweenMeAndCoverNow = Vector3.SqrMagnitude( verts[i] - positionToDefend );
                    //Check that this location is within all the set bounds
                    if (distBetweenMeAndCoverNow < closestDistToMeSoFarSqr && currDistTarget > minCoverDistSqrd && currDistTarget < maxCoverDistSqrd) {
                        //Set the current location's hight to the hight of the agent weapon
                        verts[i].y += coverScript.dynamicCoverNodeFireOffset;
                        //If we can fire at the enemy agent from this location
                        if (!Physics.Linecast( verts[i], targetTransformPos, layerMask )) {
                            //Set the current location's hight to how high the cover must be to hide the agent
                            verts[i].y -= hideOffset;
                            //Check if this location hides the agent from the enemy agent and check that another agent isn't in this position
                            if (Physics.Raycast( verts[i], targetTransformPos - verts[i], coverScript.maxDistBehindDynamicCover, layerMask ) && !ParagonAI.ControllerScript.currentController.isDynamicCoverSpotCurrentlyUsed( verts[i] )) {
                                shouldCont = true;

                                //If chest high wall
                                hidingPosCheckingNow = verts[i];
                                //Check to make sure we have clear LoS between the firing position and the move position. If so mark it as a possible good location
                                if (!Physics.Linecast( targetTransformPos, verts[i], layerMask ) && Physics.Linecast( hidingPosCheckingNow, targetTransformPos, layerMask )) {
                                    closestDistToMeSoFarSqr = distBetweenMeAndCoverNow;
                                    coverHidePos = hidingPosCheckingNow;
                                    coverFirePos = verts[i];
                                    shouldCont = false;
                                    //If we need cover quickly and should use the first found cover then break out of the search
                                    if (useFirstCover) {
                                        possibleCoverNodes.Add( new ParagonAI.CoverData( true, coverHidePos, coverFirePos, true, null ) );
                                        break;
                                    } else {
                                        possibleCoverNodes.Add( new ParagonAI.CoverData( true, coverHidePos, coverFirePos, true, null ) );
                                    }
                                }

                                //Check for side cover
                                if (shouldCont) {
                                    //Check if three adjacent locations to the cover are a good side step cover location
                                    for (int x = -1; x <= 1; x += 2) {
                                        hidingPosCheckingNow = verts[i] + myTransform.right * x * coverScript.dynamicCoverWidthNeededToHide;
                                        //If we're safe
                                        if (!Physics.Linecast( hidingPosCheckingNow, verts[i], layerMask ) && Physics.Linecast( hidingPosCheckingNow, targetTransformPos, layerMask )) {
                                            closestDistToMeSoFarSqr = distBetweenMeAndCoverNow;
                                            coverHidePos = hidingPosCheckingNow;
                                            coverFirePos = verts[i];
                                            shouldCont = false;
                                            //If we need cover quickly and should use the first found cover then break out of the search
                                            if (useFirstCover) {
                                                possibleCoverNodes.Add( new ParagonAI.CoverData( true, coverHidePos, coverFirePos, true, null ) );
                                                break;
                                            } else {
                                                possibleCoverNodes.Add( new ParagonAI.CoverData( true, coverHidePos, coverFirePos, true, null ) );
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //If we have no goal position then just use a random piece of cover we found.
            if (goalPos.Equals( Vector3.zero )) {
                System.Random random = new System.Random();
                return possibleCoverNodes[random.Next( 0, possibleCoverNodes.Count )];
            }
            //If we found a cover position in our search return the one closest to our goal
            if (possibleCoverNodes.Count > 0) {
                float closestCoverToGoalTarget = float.MaxValue;
                CoverData bestCover = possibleCoverNodes[0];
                foreach (CoverData possibleNode in possibleCoverNodes) {
                    float distanceCheck = Vector3.Distance( possibleNode.hidingPosition, goalPos );
                    if (distanceCheck < closestCoverToGoalTarget) {
                        bestCover = possibleNode;
                    }
                }
                return bestCover;
            }
            //If not just send a CoverData object with null values
            return new ParagonAI.CoverData();
        }
    }
}