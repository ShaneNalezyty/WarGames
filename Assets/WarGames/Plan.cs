using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WarGames;
using UnityEngine;


namespace WarGames {
    public class Plan {
        private List<Action.ActionTemplate> actionList;
        public Plan(List<WarGames.Action.ActionTemplate> listOfActions) {
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
