using UnityEngine;
using System.Collections;
using ParagonAI;

namespace WarGames {
    /// <summary>
    /// Interface to define an action.
    /// </summary>
    public interface Actionable {
        void OnComplete();
        bool NextAICycle( bool inCombat );
        string ToString();
    }
}