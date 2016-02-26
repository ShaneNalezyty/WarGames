using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarGames;
using UnityEngine;


namespace WarGames.Planning {
    /// <summary>
    /// Plan is a Facade object used to make running a list of actions easier.
    /// </summary>
    public class Plan {
        private List<Action.ActionInterface> actionList;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="listOfActions">List of actions for this plan to run</param>
        public Plan(List<Action.ActionInterface> listOfActions) {
            actionList = listOfActions;
        }
        /// <summary>
        /// Invokes the next action's cycle
        /// </summary>
        internal void nextAICycle() {
            //Action Cycles return a boolean representing if they have completed
            bool actionComplete = actionList[0].AICycle();
            if (actionComplete) {
                //If it has move onto the next action.
                nextAction();
            }
        }
        /// <summary>
        /// Checks if we have an action
        /// </summary>
        /// <returns></returns>
        public bool hasAction() {
            return !(actionList.Count == 0);
            
        }
        /// <summary>
        /// Invokes the current action to end
        /// </summary>
        internal void onEndPlan() {
            if (!hasAction()) {
                actionList[0].OnEndAction();
            }
        }
        /// <summary>
        /// Removes the current action and sets the next action as current
        /// </summary>
        private void nextAction() {
            actionList.RemoveAt(0);
        }
    }
}
