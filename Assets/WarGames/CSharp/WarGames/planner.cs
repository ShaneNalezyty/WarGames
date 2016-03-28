using UnityEngine;
using System.Collections;
using System.Text;
using WarGames.Utilities;
using ParagonAI;
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
    public class Planner : MonoBehaviour {
        /// <summary>
        /// A Soldier's current Goal.
        /// </summary>
        private Goal goal;
        /// <summary>
        /// A Soldier's current Plan that satisfies the current Goal.
        /// </summary>
        private Plan plan;

        private BaseScript baseScript;

        private Log logger;

        private float CycleTime = 0.2f;

        public Planner( Log log, BaseScript soldiersBaseScript ) {
            logger = log;
            CreatePlan( goal );
            baseScript = soldiersBaseScript;
            StartCoroutine( "PlanningCoroutine" );
        }

        IEnumerator PlanningCoroutine() {
            if (plan.Satisfies( goal )) {


            } else {
                CreatePlan( goal );
            }
            yield return new WaitForSeconds( CycleTime );
        }

        private void CreatePlan( Goal goalToSatisfy ) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Planner has began replanning for new goal." );
            stringBuilder.AppendLine( "Ditched Plan: " );
            stringBuilder.AppendLine( plan.ToString() );
            stringBuilder.AppendLine( "Goal new plan should satisfy: " );
            stringBuilder.AppendLine( goal.ToString() );
            logger.WriteToLog( stringBuilder.ToString(), 'P' );
            //TODO: Create plans
            //If we have no goal we should just wander around till we receive one.
            if (goalToSatisfy == null) {
                //Create plan with only one WanderAction
                ArrayList actions = new ArrayList();
                actions.Add( new WanderAction( baseScript, 20 ) );
                plan = new Plan( goalToSatisfy, actions );
            } else {

            }
            stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Planner has completed replanning." );
            stringBuilder.AppendLine( "New Plan: " );
            stringBuilder.AppendLine( plan.ToString() );
            logger.WriteToLog( stringBuilder.ToString(), 'P' );
        }

        private void LogPlan() {
            logger.WriteToLog( plan.ToString(), 'P' );
        }

        private void LogGoal() {
            logger.WriteToLog( goal.ToString(), 'G' );
        }

    }
}