using UnityEngine;
using ParagonAI;
using System.Collections;
using WarGames.Communication;
using System.Collections.Generic;
using System.Threading;

namespace WarGames.Behaviour {
	/// <summary>
	/// Behaviour for planning and executing action plans to satisfy a provided goal while not in combat.
	/// </summary>
	/// <seealso cref="WarGames.Behaviour.WarGamesBehaviour" />
	/// <seealso cref="WarGames.Goal" />
	public class IdlePlanningBehaviour : WarGames.Behaviour.WarGamesBehaviour {
		private Goal currentGoal = null;
		private bool holdingPosition = false;
		private WarGamesFindCoverScript warGamesFindCoverScript;
		private Vector3 startPosition;
		private CoverData currentCover;
		private bool enteringCover;

		public override void Initiate() {
			base.Initiate();
			//Set the behaviour level
			behaveLevel = BehaviourLevel.Idle;
			//Log the initiation of this behaviour
			soldier.WriteToLog( "IdlePlanningBehaviour initiated.", "B" );
			warGamesFindCoverScript = soldier.GetFindCoverScript();
			startPosition = baseScript.transform.position;
			currentCover = new CoverData();
			enteringCover = false;
			aICycleFinished = true;
		}

		public override void AICycle() {
			//aICycleFinished = false;
			soldier.CheckMessages();
			if (currentGoal == null || soldier.GetGoal() == null) {
				currentGoal = soldier.GetGoal();
				//If we haven't gotten any new goals then don't do anything
				targetVector = transform.position;
				return;
			}
			if (!currentGoal.Equals( soldier.GetGoal() )) {
				//If we have a new goal reset any values
				holdingPosition = false;
				currentGoal = soldier.GetGoal();
				startPosition = transform.position;
				LeaveCover();
			}

			if (!baseScript.inCover && currentCover.foundCover) {
				if (enteringCover) {
					warGamesFindCoverScript.ReachCoverAndWait( currentCover, currentGoal, holdingPosition );
				} else {
					targetVector = currentCover.hidingPosition;
					warGamesFindCoverScript.SetCover( currentCover.hidingPosition, currentCover.firingPosition );
					enteringCover = true;
				}
			} else {
				enteringCover = false;
				//if (!warGamesFindCoverScript.IsCoverCoroutineRunning() ) {
				soldier.WriteToLog( "Starting Looking for Cover", "A" );
				currentCover = warGamesFindCoverScript.FindCover( currentGoal.GetDestination(), currentGoal, false, holdingPosition, warGamesFindCoverScript.DirectionTowards( startPosition, currentGoal.GetDestination() ), warGamesFindCoverScript.GetPercentToGoalDist( startPosition, currentGoal.GetDestination() ) );
				soldier.WriteToLog( "Found cover? : " + currentCover.foundCover + "Location: " + currentCover.hidingPosition.ToString(), "A" );
				if (!currentCover.foundCover) {
					targetVector = currentGoal.GetDestination();
				}
				soldier.WriteToLog( "distance between soldier and cover: " + Vector3.SqrMagnitude( currentCover.hidingPosition - transform.position ), "A" );
				//}
			}
			soldier.WriteToLog( "End of AICycle", "A" );
			aICycleFinished = true;
		}

		private void LeaveCover() {
			warGamesFindCoverScript.LeaveCover();
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