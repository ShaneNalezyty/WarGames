using UnityEngine;
using System.Collections;

namespace WarGames.Behaviour {
    /// <summary>
    /// Behaviour for planning and executing action plans to satisfy a provided goal while in combat.
    /// </summary>
    /// <seealso cref="WarGames.Behaviour.WarGamesBehaviour" />
    /// <seealso cref="WarGames.Goal" />
    public class CombatPlanningBehaviour : WarGames.Behaviour.WarGamesBehaviour {
        public override void Initiate() {
            base.Initiate();
            //Set the behaviour level
            behaveLevel = BehaviourLevel.Combat;
            //Log the initiation of this behaviour
            soldierScript.writeToLog( "CombatPlanningBehaviour initiated.", 'B' );
        }

        public override void AICycle() {

        }

        public override void EachFrame() {

        }

        public override void OnEndBehaviour() {

        }
    }
}