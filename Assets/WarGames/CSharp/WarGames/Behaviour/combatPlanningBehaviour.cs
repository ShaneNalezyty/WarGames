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
        private bool useFirstCover;
        private Goal currentGoal = null;
		private Vector3 startPosition;
		private WarGamesFindCoverScript warGamesFindCoverScript;
		private CoverData currentCover;
		private bool enteringCover;
        public override void Initiate() {
            base.Initiate();
            //Set the behaviour level
            behaveLevel = BehaviourLevel.Combat;
            //Log the initiation of this behaviour
            soldier.WriteToLog( "CombatPlanningBehaviour initiated.", "B" );
			warGamesFindCoverScript = soldier.GetFindCoverScript();
			startPosition = baseScript.transform.position;
			currentCover = new CoverData();
			enteringCover = false;
			aICycleFinished = true;
		}

        public override void AICycle() {
            soldier.CheckMessages();
			if (currentGoal == null || soldier.GetGoal() == null) {
				currentGoal = soldier.GetGoal();
				//If we haven't gotten any new goals then don't do anything
				targetVector = transform.position;
				return;
			}
			if (!currentGoal.Equals( soldier.GetGoal() )) {
				//If we have a new goal reset any values
				currentGoal = soldier.GetGoal();
				startPosition = transform.position;
				LeaveCover();
			}

			if (!baseScript.inCover && currentCover.foundCover) {
				if (enteringCover) {
					warGamesFindCoverScript.ReachCoverAndWait( currentCover, currentGoal, false );
				} else {
					targetVector = currentCover.hidingPosition;
					warGamesFindCoverScript.SetCover( currentCover.hidingPosition, currentCover.firingPosition );
					enteringCover = true;
				}
			} else {
				enteringCover = false;
				if (!warGamesFindCoverScript.IsCoverCoroutineRunning()) {
					currentCover = warGamesFindCoverScript.FindCover( baseScript.targetTransform.position, currentGoal, useFirstCover, false, warGamesFindCoverScript.DirectionTowards( startPosition, currentGoal.GetDestination() ), warGamesFindCoverScript.GetPercentToGoalDist( startPosition, currentGoal.GetDestination() ) );
					if (useFirstCover) {
						useFirstCover = false;
					}
				}
			}

		}

		private void LeaveCover() {
			warGamesFindCoverScript.LeaveCover();
		}

		public override void EachFrame() {

        }

        public override void OnEndBehaviour() {
            useFirstCover = true;
        }
    }
}