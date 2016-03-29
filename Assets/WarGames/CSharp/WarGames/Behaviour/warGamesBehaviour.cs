using UnityEngine;
using System.Collections;
using WarGames;
/// <summary>
/// Contains all WarGames behaviours
/// </summary>
namespace WarGames.Behaviour {
    /// <summary>
    /// Wrapper class for ParagonAI.CustomAIBehaviour
    /// </summary>
    /// <seealso cref="ParagonAI.CustomAIBehaviour" />
    public class WarGamesBehaviour : ParagonAI.CustomAIBehaviour {
        /// <summary>
        /// The soldierScript holds all variables a soldier needs. 
        /// Team leaders contain a solderScript.
        /// </summary>
        public Soldier soldier;
        /// <summary>
        /// The leaderScript holds all variables a team leader needs.
        /// Only team leaders contain a leader script.
        /// </summary>
        public TeamLeader leader;
        public override void Initiate() {
            base.Initiate();
            //Grab the soldier script
            soldier = gameObject.GetComponent<Soldier>();
            //Grab the team leader script. If this is null then this agent is not a team leader.
            leader = gameObject.GetComponent<TeamLeader>();
        }

        public override void AICycle() {

        }

        public override void EachFrame() {

        }

        public override void OnEndBehaviour() {

        }
    }
}