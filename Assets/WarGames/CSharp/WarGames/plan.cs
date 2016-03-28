using UnityEngine;
using System.Collections;
using System.Text;

namespace WarGames {
    /// <summary>
    /// Contains a set of actions that when executed sequentially satisfies a goal.
    /// </summary>
    /// <seealso cref="WarGames.Goal" />
    public class Plan {
        private Goal goalToSatisfy;
        private ArrayList actionPlan;
        private int currentAction;
        public Plan( Goal goal, ArrayList actions ) {
            actionPlan = actions;
            actionPlan.TrimToSize();
            goalToSatisfy = goal;
            currentAction = 0;
        }
        public void NextAICycle( bool inCombat ) {
            Actionable currentAction = GetCurrentAction();
            bool actionComplete = currentAction.NextAICycle( inCombat );
            if (actionComplete) {
                CompleteAction();
            }
        }
        private Actionable GetCurrentAction() {
            return (Actionable)actionPlan[currentAction];
        }

        private void CompleteAction() {
            GetCurrentAction().OnComplete();
            currentAction++;
        }
        public int GetPercentDone() {
            return (int)(100*(currentAction / actionPlan.Capacity));
        }
        public bool Satisfies( Goal checkGoal ) {
            return goalToSatisfy.Equals( checkGoal );
        }
        override public string ToString() {
            StringBuilder stringbuilder = new StringBuilder();
            stringbuilder.AppendLine( "This plan contains " + actionPlan.Capacity + " actions." );
            stringbuilder.AppendLine( "This plan is currently " + GetPercentDone() + "% complete." );
            stringbuilder.AppendLine( "Goal this plan satisfies: " );
            stringbuilder.AppendLine( goalToSatisfy.ToString() );
            stringbuilder.AppendLine( "List of actions: " );
            foreach (Actionable action in actionPlan) {
                stringbuilder.AppendLine( action.ToString() );
            }
            stringbuilder.AppendLine( "End of plan." );
            return stringbuilder.ToString();
        }
    }
}