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
        public interface ActionInterface {
        ParagonAI.BaseScript baseScript {
            get;
        }

        List<Transform> targets {
            get;
        }
        /// <summary>
        /// All Actions need a AI cycle with run time logic in it.
        /// </summary>
        /// <returns>true if this cycle completed the action.</returns>
        bool AICycle();
        /// <summary>
        /// Ran on the distruction of a action.
        /// </summary>
        void OnEndAction();
    }
}
