using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WarGames.Action;
/// <summary>
/// The Planning namespace contains all classes pertaining to the planning behaviours
/// </summary>
namespace WarGames.Planning {
    /// <summary>
    /// IdlePlanningBehaviour defines actions agents will take when not combat.
    /// </summary>
    public class IdlePlanningBehaviour : ParagonAI.CustomAIBehaviour {
        /// <summary>
        /// When inputed a goal will create appropriate plan containing a list of actions that when followed statisfies the goal.
        /// </summary>
        /// <returns>Plan object with actions to satisfy the goal parameter.</returns>
        public Plan createPlanFromGoal() {
            List<Action.ActionInterface> actions = new List<Action.ActionInterface>();
            //actions.Add( new ActionTemplate( baseScript, GameObject.Find( "TargetOne" ).transform ));
            //actions.Add( new ActionTemplate( baseScript, GameObject.Find( "TargetTwo" ).transform ));
            return new WarGames.Planning.Plan(actions);
             
        }
        /// <summary>
        /// Used to set some variables before script is ran
        /// </summary>
        public override void Initiate() {
            behaveLevel = BehaviourLevel.Idle;
            base.Initiate();
        }
        /// <summary>
        /// Ran every specified amount of time (Default: .2 seconds)
        /// </summary>
        public override void AICycle() {
            if (baseScript.actionPlan == null) {
                //If we have no plan create one.
                baseScript.actionPlan = createPlanFromGoal();
            }
            if (!baseScript.actionPlan.hasAction()) {
                //Run the next cycle for the current action
                baseScript.actionPlan.nextAICycle();
            } else {
                //If we don't have a next action then we need to
                //Evaluate if we have satisfied the goal.
            }
        }
        /// <summary>
        /// Ran on each frame useful for cover checks
        /// </summary>
        public override void EachFrame() {

        }
        /// <summary>
        /// Used to clean up any mess we have made in this behaviour before switching to another.
        /// </summary>
        public override void OnEndBehaviour() {
            //Run the action plans end method
            baseScript.actionPlan.onEndPlan();
        }
    }
}