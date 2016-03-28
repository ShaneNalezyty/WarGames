using UnityEngine;
using System.Collections;

namespace WarGames.Behaviour {
    /// <summary>
    /// Behaviour for planning and executing action plans to satisfy a provided goal while not in combat.
    /// </summary>
    /// <seealso cref="WarGames.Behaviour.WarGamesBehaviour" />
    /// <seealso cref="WarGames.Goal" />
    public class IdlePlanningBehaviour : WarGames.Behaviour.WarGamesBehaviour {
        
        public override void Initiate() {
            base.Initiate();
            //Set the behaviour level
            behaveLevel = BehaviourLevel.Idle;
            //Log the initiation of this behaviour
            soldierScript.writeToLog( "IdlePlanningBehaviour initiated.", 'B' );
        }

        public override void AICycle() {

        }

        public override void EachFrame() {

        }

        public override void OnEndBehaviour() {

        }
    }
}