using UnityEngine;
using System.Collections;
using ParagonAI;
using System.Text;
using System;
using System.Collections.Generic;

namespace WarGames {
	public class WarGamesFindCoverScript : MonoBehaviour {
		private BaseScript baseScript;
		private bool foundCover = false;
		private bool coroutineRunning = false;
		private Coroutine waitCoroutine;
		private Soldier soldier;
		private List<CoverData> prevCover = new List<CoverData>();

		public void Awake() {
			baseScript = gameObject.GetComponent<BaseScript>();
			soldier = gameObject.GetComponent<Soldier>();
		}
		public bool IsCoverCoroutineRunning() {
			return coroutineRunning;
		}

		private bool CoverToCloseToOldCover( CoverData old, CoverData newCD ) {
			float dist = Vector3.SqrMagnitude( old.hidingPosition - newCD.hidingPosition );
			soldier.WriteToLog( "distance: " + dist, "A" );
			return dist < 100;
		}
		
		public void ReachCoverAndWait( CoverData coverData, Goal goal, bool holdingPosision ) {
			float[] coverWaitRange;
			if (holdingPosision) {
				coverWaitRange = new float[] { float.MaxValue, float.MaxValue };
			} else {
				coverWaitRange = goal.GetWaitRange();
			}
			if (!baseScript.inCover && baseScript.navI.GetRemainingDistance() <= 0) {
				baseScript.inCover = true;
				coroutineRunning = true;
				//waitCoroutine = StartCoroutine( SetTimeToLeaveCover(UnityEngine.Random.Range( coverWaitRange[0], coverWaitRange[1] ) ) );
				waitCoroutine = StartCoroutine( SetTimeToLeaveCover( 3f ) );
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
			if (coroutineRunning) {
				StopAllCoroutines();
				coroutineRunning = false;
			}
		}
		public void SetCover( Vector3 newCoverPos, Vector3 newCoverFiringSpot ) {
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
			while (timeToLeave > 0 && (baseScript.inCover || foundCover)) {
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
						timeToLeave = 0f;
						LeaveCover();
					}
				}
				//Wait for a second then run the coroutine cycle again.
				yield return new WaitForSeconds( 1 );
			}
			//If the amount of time we should be in cover has been met and we are still in cover then leave cover
			if (baseScript.inCover || foundCover) {
				LeaveCover();
				soldier.WriteToLog( "Should end sdfsdsdfsdffs", "A" );
			}
		}

		public float GetPercentToGoalDist( Vector3 startPosition, Vector3 goalPosition ) {
			float totalDist = Vector3.SqrMagnitude( startPosition - goalPosition );
			float currentDist = Vector3.SqrMagnitude( transform.position - goalPosition );
			return currentDist / totalDist;
		}

		public char[] DirectionTowards( Vector3 startPosition, Vector3 goalPosition ) {
			char[] direction = new char[2];
			int distanceBetweenX = Mathf.Abs( (int)(startPosition.x - goalPosition.x) );
			int distanceBetweenZ = Mathf.Abs( (int)(startPosition.z - goalPosition.z) );
			if (distanceBetweenX >= distanceBetweenZ) {
				if (startPosition.x > goalPosition.x) {
					direction[0] = 'x';
					direction[1] = 'l';
				} else {
					direction[0] = 'x';
					direction[1] = 'h';
				}
			} else {
				if (startPosition.z > goalPosition.z) {
					direction[0] = 'z';
					direction[1] = 'l';
				} else {
					direction[0] = 'z';
					direction[1] = 'h';
				}
			}
			return direction;
		}

		public CoverData FindCover( Vector3 targetVector, Goal goal, bool useFirstCover, bool holdingPosition, char[] direction, float percentToGoal ) {
			soldier.WriteToLog( "percent to goal: " + percentToGoal, "A" );
			//If we are not engaging a target and are aggression is VeryHigh don't even look for cover
			if (baseScript.IsEnaging() && goal.GetAggressionLevel() == Goal.AggressionLevel.VeryHigh) {
				return new CoverData();
			}

			//Navigation Things
			NavmeshInterface navI = baseScript.navI;
			Vector3[] verts = navI.GetNavmeshVertices();

			//Position agent was in when starting to look for cover
			Vector3 agentPosition = transform.position;

			//Objects that break line of sight
			LayerMask layerMask = baseScript.GetLayerMask();

			//ParagonAI's CoverFinderScript to grab variables I need
			CoverFinderScript coverScript = baseScript.coverFinderScript;
			agentPosition.y += coverScript.dynamicCoverNodeHeightOffset;
			float fireOffset = coverScript.dynamicCoverNodeFireOffset;
			float hideOffset = coverScript.dynamicCoverNodeFireOffset - coverScript.dynamicCoverNodeHeightOffset;
			float maxDist = coverScript.maxDistBehindDynamicCover;
			float agentWidth = coverScript.dynamicCoverWidthNeededToHide;

			//Holds all the possible CoverNodes
			List<CoverData> possibleCoverNodes = new List<CoverData>();

			//To make sure we don't use a location right next to our current location
			int minDistanceBetweenLastCover = 20;
			soldier.WriteToLog( "Possible verts: " + verts.Length, "A" );
			for (int i = 0; i < verts.Length; i++) {
				//If the possible location is farther then the set minimum space to travel before the agents last cover location
				if (Vector3.SqrMagnitude( verts[i] - agentPosition ) > minDistanceBetweenLastCover) {
					//Get the current distance between the agent and this cover
					//float distBetweenAgentAndCover = Vector3.SqrMagnitude( verts[i] - targetVector );
					//Check that we can hide and fire at current location, check that no other agent is in this location, and check this location is on the proper side of map.
					bool goodCover;
					if (!baseScript.IsEnaging()) {
						goodCover = !CheckProperSide( verts[i], targetVector, direction );
					} else {
						goodCover = CheckLocationBounds( verts[i], targetVector, layerMask, fireOffset, hideOffset, maxDist ) && CheckProperSide( verts[i], targetVector, direction );
					}
					//if (CheckLocationBounds( verts[i], targetVector, layerMask, fireOffset, hideOffset, maxDist ) && CheckProperSide( verts[i], targetVector, direction )) {
					if (goodCover) {
						//Used to skip side stepping cover check if the agent can crouch fire at the cover location
						bool shouldContinue = true;
						Vector3 hidingPosCheckingNow = verts[i];

						if (!baseScript.IsEnaging()) {
							possibleCoverNodes.Add( new CoverData( true, hidingPosCheckingNow, verts[i], true, null ) );
						}
						//Check to make sure we have clear LoS between the firing position and the move position.
						if (!Physics.Linecast( targetVector, verts[i], layerMask ) && Physics.Linecast( hidingPosCheckingNow, targetVector, layerMask )) {
							possibleCoverNodes.Add( new CoverData( true, hidingPosCheckingNow, verts[i], true, null ) );
							shouldContinue = false;
							if (useFirstCover) {
								break;
							}
						}

						//Check for side cover
						if (shouldContinue) {
							for (int x = -1; x <= 1; x += 2) {
								hidingPosCheckingNow = verts[i] + transform.right * x * agentWidth;
								//If we're safe
								if (!Physics.Linecast( hidingPosCheckingNow, verts[i], layerMask ) && Physics.Linecast( hidingPosCheckingNow, targetVector, layerMask )) {
									possibleCoverNodes.Add( new CoverData( true, hidingPosCheckingNow, verts[i], true, null ) );
									if (useFirstCover) {
										break;
									}
								}
							}
						}
					}
				}
			}
			foreach (CoverData oldCover in prevCover) {
				foreach (CoverData possibleCover in possibleCoverNodes) {
					if (possibleCover.Equals( oldCover )) {
						possibleCoverNodes.Remove( possibleCover );
					} else if( CoverToCloseToOldCover(oldCover, possibleCover) ) {
						possibleCoverNodes.Remove( possibleCover );
					}
				}
			}
				CoverData toReturn = new CoverData();
			//Once all possible locations have been found pick the best
			if (possibleCoverNodes.Count == 0) {
				soldier.WriteToLog( "No Cover Found!", "A" );
				//If we didn't find any good cover return a CoverNode indicating that
				return toReturn;
			} else if (possibleCoverNodes.Count == 1) {
				soldier.WriteToLog( "Only one cover found!", "A" );
				//If we only have one location use that
				toReturn = possibleCoverNodes[0];
			} else {
				//If we have options we should pick the best based on aggression level / holding Position
				//If we are attempting to hold a position then just use the closest cover
				CoverData closestCover = GetClosestCover( possibleCoverNodes );
				if (holdingPosition) {
					soldier.WriteToLog( "Using Closest Cover!", "A" );
					return closestCover;
				}
				CoverData closestCoverTowardGoal = GetClosestCoverTowardGoal( possibleCoverNodes, goal.GetDestination() );
				CoverData secondClosestCoverTowardGoal = GetSecondClosestCoverTowardGoal( possibleCoverNodes, goal.GetDestination() );
				if (baseScript.IsEnaging()) {
					soldier.WriteToLog( "getting in combat cover ", "A" );
					//If we are in combat use the cover closest to us
					switch (goal.GetAggressionLevel()) {
					case Goal.AggressionLevel.VeryLow:
						toReturn = closestCover;
						break;
					case Goal.AggressionLevel.Low:
						toReturn = closestCover;
						break;
					case Goal.AggressionLevel.Moderate:
						toReturn = closestCover;
						break;
					case Goal.AggressionLevel.High:
						toReturn = closestCover;
						break;
					case Goal.AggressionLevel.VeryHigh:
						toReturn = closestCoverTowardGoal;
						break;
					}
				} else {
					soldier.WriteToLog( "getting out of combat cover ", "A" );
					//VeryLow = 0, Low = 1, Moderate = 2, High = 3, VeryHigh = 4
					//If we are not in combat select cover based on aggression level
					switch (goal.GetAggressionLevel()) {
					case Goal.AggressionLevel.VeryLow:
						toReturn = closestCoverTowardGoal;
						break;
					case Goal.AggressionLevel.Low:
						if (percentToGoal > .5f) {
							toReturn = secondClosestCoverTowardGoal;
							break;
						} else {
							toReturn = closestCoverTowardGoal;
							break;
						}
					case Goal.AggressionLevel.Moderate:
						if (percentToGoal > .5f) {
							break;
						} else if (percentToGoal > .25f) {
							toReturn = secondClosestCoverTowardGoal;
							break;
						} else {
							toReturn = closestCoverTowardGoal;
							break;
						}
					case Goal.AggressionLevel.High:
						if (percentToGoal > .25f) {
							break;
						} else {
							toReturn = secondClosestCoverTowardGoal;
							break;
						}
					case Goal.AggressionLevel.VeryHigh:
						break;
					}
				}
			}
			return toReturn;
		}

		private CoverData GetSecondClosestCoverTowardGoal( List<CoverData> coverDataList, Vector3 goalPosition ) {
			CoverData closestCover = GetClosestCoverTowardGoal( coverDataList, goalPosition );
			float secondClosestCover = float.MaxValue;
			CoverData secondClosestCoverNode = null;
			foreach (CoverData possibleCover in coverDataList) {
				float currentDistance = Vector3.SqrMagnitude( possibleCover.firingPosition - transform.position );
				if (currentDistance < secondClosestCover) {
					if (Vector3.SqrMagnitude( possibleCover.firingPosition - goalPosition ) > Vector3.SqrMagnitude( transform.position - goalPosition )) {
						//If the cover location is farther from the goal then us then don't use it
						continue;
					}
					if (secondClosestCoverNode == closestCover) {
						//If this node is the same as the closest then don't use it
						continue;
					}
					secondClosestCover = currentDistance;
					secondClosestCoverNode = possibleCover;
				}
			}
			return secondClosestCoverNode;
		}

		private CoverData GetClosestCover( List<CoverData> coverDataList ) {
			float closestCover = float.MaxValue;
			CoverData closestCoverNode = null;
			foreach (CoverData possibleCover in coverDataList) {
				float currentDistance = Vector3.SqrMagnitude( possibleCover.firingPosition - transform.position );
				if (currentDistance < closestCover) {
					closestCover = currentDistance;
					closestCoverNode = possibleCover;
				}
			}
			return closestCoverNode;
		}

		private CoverData GetClosestCoverTowardGoal( List<CoverData> coverDataList, Vector3 goalPosition ) {
			float closestCover = float.MaxValue;
			CoverData closestCoverNode = null;
			foreach (CoverData possibleCover in coverDataList) {
				float currentDistance = Vector3.SqrMagnitude( possibleCover.firingPosition - transform.position );
				if (currentDistance < closestCover) {
					if (Vector3.SqrMagnitude( possibleCover.firingPosition - goalPosition ) > Vector3.SqrMagnitude( transform.position - goalPosition )) {
						//If the cover location is farther from the goal then us then don't use it
						continue;
					}
					closestCover = currentDistance;
					closestCoverNode = possibleCover;
				}
			}
			return closestCoverNode;
		}

		private bool CheckProperSide( Vector3 locationToCheck, Vector3 goalLocation, char[] direction ) {
			//If we are approaching primarily about the x axis
			if (direction[0].Equals( 'x' )) {
				//If the location needs to be a higher x value then the target
				if (direction[1].Equals( 'h' )) {
					return locationToCheck.x > goalLocation.x;
				} else {
					//Else the location needs to be a lower x value then the target
					return locationToCheck.x < goalLocation.x;
				}
			} else {
				//Else we are approaching primarily about the z axis
				//If the location needs to be a higher z value then the target
				if (direction[1].Equals( 'h' )) {
					return locationToCheck.z > goalLocation.z;
				} else {
					//Else the location needs to be a lower z value then the target
					return locationToCheck.z < goalLocation.z;
				}
			}
		}

		private bool CheckLocationBounds( Vector3 locationToCheck, Vector3 targetVector, LayerMask layerMask, float fireOffset, float hideOffset, float maxDist ) {
			//Set the current location's hight to the hight of the agent weapon
			locationToCheck.y += fireOffset;
			//If we can fire at the target location
			if (!Physics.Linecast( locationToCheck, targetVector, layerMask )) {
				//Set the current location's hight to how high the cover must be to hide the agent
				locationToCheck.y -= hideOffset;
				//Check if this location hides the agent from the enemy agent and check that another agent isn't in this position
				if (Physics.Raycast( locationToCheck, targetVector - locationToCheck, maxDist, layerMask ) && LocationOccupied( locationToCheck )) {
					return true;
				}
			}
			return false;
		}

		private bool LocationOccupied( Vector3 locationToCheck ) {
			return !ParagonAI.ControllerScript.currentController.isDynamicCoverSpotCurrentlyUsed( locationToCheck );
		}
		override public string ToString() {
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine( "Find Cover Action:" );
			stringBuilder.AppendLine( "Min time to stay in cover: " + baseScript.minTimeInCover );
			stringBuilder.AppendLine( "Max time to stay in cover: " + baseScript.maxTimeInCover );
			/*if (!goalPosition.Equals( Vector3.zero )) {
				stringBuilder.AppendLine( "The position the agent should reach is: " + goalPosition.ToString() );
			} else {
				stringBuilder.AppendLine( "The agent has no location it should be trying to reach" );
			}
			if (baseScript.inCover) {
				stringBuilder.AppendLine( "Currently in cover" );
			} else {
				stringBuilder.AppendLine( "Currently not cover" );
			}*/
			return stringBuilder.ToString( 0, stringBuilder.Length - 1 );
		}
	}
}