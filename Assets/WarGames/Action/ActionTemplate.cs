using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WarGames.Action {
    public class ActionTemplate {

        private ParagonAI.BaseScript baseScript;
        private Transform target;

        public ActionTemplate(ParagonAI.BaseScript script, Transform targetTransform) {
            baseScript = script;
            target = targetTransform;
        }

        public bool AICycle() {
            return true;
        }

        public void OnEndAction() {

        }
    }
}
