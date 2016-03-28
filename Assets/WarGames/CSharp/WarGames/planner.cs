using UnityEngine;
using System.Collections;
using WarGames;
/// <summary>
/// WarGames is the senior seminar project of Shane Nalezyty.
/// Contains logic intended to extend Tactical Shooter AI in the Unity Assets Store.
/// Adds new agent AI to allow for agent authority hierarchies, long term goal distribution, and agent goal oriented action planning.
/// </summary>
namespace WarGames {
    /// <summary>
    /// Contains an agents current goal and action plan.
    /// Contains methods for creating action plans to satisfies provided goals.
    /// </summary>
    public class Planner {
        /// <summary>
        /// A Soldier's current Goal.
        /// </summary>
        private Goal myGoal;
        /// <summary>
        /// A Soldier's current Plan that satisfies the current Goal.
        /// </summary>
        private Plan myPlan;
    }
}