using UnityEngine;
using System.Collections;

namespace WarGames {
    /// <summary>
    /// Contains a desirable gamestate created by a team leader.
    /// Sent to agents using a goalMessage
    /// </summary>
    /// <seealso cref="WarGames.Communication.GoalMessage" />
    public class Goal {
        private GoalType goalType;

        public Goal(GoalType type) {
            goalType = type;
        }
        public enum GoalType {
            ProceedToTarget = 0, Patrol = 1, Attack = 6, Defend = 7
        }
    }
}