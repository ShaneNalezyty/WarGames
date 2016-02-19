using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
/// <summary>
/// The Action namespace contains the interface action and all classes that implement it.
/// </summary>
namespace WarGames.Action {
    /// <summary>
    /// Interface all actions must implement 
    /// </summary>
    public class ActionTemplate {

        private ParagonAI.BaseScript baseScript;
        private Transform target;

        public ActionTemplate(ParagonAI.BaseScript script, Transform targetTransform) {
            baseScript = script;
            target = targetTransform;
        }
        /// <summary>
        /// All Actions need a AI cycle with run time logic in it.
        /// </summary>
        /// <returns>true if this cycle completed the action.</returns>
        public bool AICycle() {
            return true;
        }
        /// <summary>
        /// Ran on the distruction of a action.
        /// </summary>
        public void OnEndAction() {

        }
    }
}
