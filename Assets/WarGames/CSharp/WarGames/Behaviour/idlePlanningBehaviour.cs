using UnityEngine;
using System.Collections;
using WarGames.Communication;

namespace WarGames.Behaviour {
    /// <summary>
    /// Behaviour for planning and executing action plans to satisfy a provided goal while not in combat.
    /// </summary>
    /// <seealso cref="WarGames.Behaviour.WarGamesBehaviour" />
    /// <seealso cref="WarGames.Goal" />
    public class IdlePlanningBehaviour : WarGames.Behaviour.WarGamesBehaviour {
        private Goal currentGoal = null;
		private bool holdingPosition = false;
		private WarGamesFindCoverScript findCoverScript;
		private Vector3 startPosition;
		
		private Coroutine waitInCoverCoroutine = null;

        public override void Initiate() {
            base.Initiate();
            //Set the behaviour level
            behaveLevel = BehaviourLevel.Idle;
            //Log the initiation of this behaviour
            soldier.WriteToLog( "IdlePlanningBehaviour initiated.", "B" );
			findCoverScript = soldier.GetFindCoverScript();
			startPosition = baseScript.transform.position;
        }

        public override void AICycle() {
            soldier.CheckMessages();
			if (currentGoal == null && soldier.GetGoal() == null) {
				//If we haven't gotten any new goals then don't do anything
				targetVector = transform.position;
				return;
			}
			if (HasNewGoal()) {
				//If we have a new goal reset any values
				holdingPosition = false;
				currentGoal = soldier.GetGoal();
				startPosition = transform.position;
				LeaveCover();
			}

			if (holdingPosition) {
				//Check if the agent is in a cover position from the goal destination
				if (baseScript.inCover) {
					ParagonAI.CoverData  coverLocation = findCoverScript.FindCover( currentGoal.GetDestination(), currentGoal, true, true, DirectionTowardsGoal( startPosition, currentGoal.GetDestination() ), GetPercentToGoalDist() );
					if (coverLocation.foundCover == true) {
						findCoverScript.SetCover( coverLocation.hidingPosition, coverLocation.firingPosition );
					} else {
						return;
					}
				} else {
					//If we are in cover then don't do anything till we get a new goal
					return;
				}
			} else {

			}
        }

		private float GetPercentToGoalDist() {
			float totalDist = Vector3.SqrMagnitude( startPosition - currentGoal.GetDestination() );
			float currentDist = Vector3.SqrMagnitude( transform.position - currentGoal.GetDestination() );
			return currentDist / totalDist;
		}

		private char[] DirectionTowardsGoal(Vector3 startPosition, Vector3 goalPosition) {
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

		private void LeaveCover() {
			if (baseScript.inCover) {
				if (waitInCoverCoroutine != null) {
					StopCoroutine( waitInCoverCoroutine );
				}
				findCoverScript.LeaveCover();
			}
		}

		private bool HasNewGoal() {
			if (currentGoal == null && soldier.GetGoal() != null) {
				return true;
			}
			return !currentGoal.Equals( soldier.GetGoal() );
		}

        public override void EachFrame() {
			base.EachFrame();
        }

        public override void OnEndBehaviour() {
			base.OnEndBehaviour();
        }

		public Vector3 GetStartPosition() {
			return startPosition;
		}
    }
}