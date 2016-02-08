using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarGames;
using UnityEngine;


namespace WarGames {
    public class Plan {
        private List<Action.Action> actionList;
        public Plan(List<WarGames.Action.Action> listOfActions) {
            actionList = listOfActions;
        }

        internal void nextAICycle() {
            bool actionComplete = actionList[0].AICycle();
            nextAction();
        }
        
        public bool hasAction() {
            return !(actionList.Count == 0);
        }
        internal void onEndPlan() {
            if (!hasAction()) {
                actionList[0].OnEndAction();
            }
        }
        private void nextAction() {
            actionList.RemoveAt(0);
        }
    }
}
