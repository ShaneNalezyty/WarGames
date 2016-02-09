using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WarGames.Action {
    public class ActionTemplate {

        private ParagonAI.BaseScript baseScript;
        private ActionType type;
        private Transform target;

        public enum ActionType {
            ProceedTo = 0, FindCover = 1, Defend = 2, Attack = 3,
        }

        public ActionTemplate(ParagonAI.BaseScript script, ActionType actionType, Transform target) {
            baseScript = script;
            type = actionType;
        }

        public bool AICycle() {
            return true;
        }

        public void OnEndAction() {

        }

        public ActionType getType() {
            return type;
        }
    }
}
