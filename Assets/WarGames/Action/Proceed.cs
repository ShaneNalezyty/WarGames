using UnityEngine;
using System.Collections;
using ParagonAI;
using System;
using System.Collections.Generic;

namespace WarGames.Action {
    /// <summary>
    /// Proceed is an action used to move an agent to a new location
    /// </summary>
    public class Proceed : WarGames.Action.ActionInterface {
        public BaseScript baseScript {
            get {
                return baseScript;
            }
        }

        public List<Transform> targets {
            get {
                return targets;
            }
        }

        public bool AICycle() {
            return false;
        }

        public void OnEndAction() {
            
        }
    }
}

