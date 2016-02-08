using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WarGames.Action {
    public class Action {

        private ParagonAI.BaseScript baseScript;
        private ActionType type;

        public enum ActionType {
            ProceedTo = 0, FindCover = 1, Defend = 2, Attack = 3,
        }

        public Action( ref ParagonAI.BaseScript script, ActionType actionType) {
            baseScript = script;
            type = actionType;
        }

        public bool AICycle() {
            return true;
        }

        public void OnEndAction() {
            clean();
        }
        /// <summary>
        /// If we are ending in the middle of an action we may need to clean up some mess
        /// For example if an agent is in cover we will want to remove him from cover before moving
        /// to the next behaviour so the location is not incorrectly marked as occupied after he as left.
        /// </summary>
        private void clean() {

        }
    }
}
