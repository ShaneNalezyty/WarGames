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

        private Soldier soldier;

        private float CycleTime = 0.2f;

        public void Start() {
            soldier = gameObject.GetComponent<Soldier>();
            baseScript = soldier.GetBaseScript();
            CreatePlan( goal );
            StartCoroutine( "PlanningCoroutine" );
        }

        public void SetGoal(Goal newGoal) {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Planner received a new goal: " );
            stringBuilder.AppendLine( newGoal.ToString() );
            soldier.WriteToLog( stringBuilder.ToString(), "G".ToCharArray() );
            goal = newGoal;
        }

        IEnumerator PlanningCoroutine() {
            if (!plan.Satisfies( goal )) {
                CreatePlan( goal );
            }
            yield return new WaitForSeconds( CycleTime );
        }

        private void CreatePlan( Goal goalToSatisfy ) {
            WritePrePlanningLogInfo();
            //TODO: Create plans
            //If we have no goal we should just wander around till we receive one.
            if (goalToSatisfy == null) {
                //Create plan with only one WanderAction
                SetPlan( CreateWanderPlan( goalToSatisfy, 20 ) );
                //ArrayList actions = new ArrayList();
                //actions.Add( new FindCoverAction( baseScript, 5f, 10f, new Vector3( 0, 0, 0 ) ) );
                //SetPlan( new Plan( goalToSatisfy, actions ) );
            } else {

            }
            WritePostPlanningLogInfo();
        }

        private void WritePostPlanningLogInfo() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Planner has completed replanning." );
            stringBuilder.AppendLine( "New Plan: " );
            stringBuilder.Append( plan.ToString() );
            soldier.WriteToLog( stringBuilder.ToString( 0, stringBuilder.Length - 1 ), "P".ToCharArray() );
        }

        private void WritePrePlanningLogInfo() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine( "Planner has began replanning for new goal." );
            stringBuilder.AppendLine( "Ditched Plan: " );
            if (plan != null) {
                stringBuilder.AppendLine( plan.ToString() );
            } else {
                stringBuilder.AppendLine( "Null Plan" );
            }
            stringBuilder.AppendLine( "Goal new plan should satisfy: " );
            if (goal != null) {
                stringBuilder.AppendLine( goal.ToString() );
            } else {
                stringBuilder.AppendLine( "Null Goal" );
            }
            soldier.WriteToLog( stringBuilder.ToString( 0, stringBuilder.Length - 1 ), "P".ToCharArray() );
        }

        private Plan CreateWanderPlan( Goal goalToSatisfy, int distance ) {
            ArrayList actions = new ArrayList();
            actions.Add( new WanderAction( baseScript, distance ) );
            return new Plan( goalToSatisfy, actions );
        }
        private void SetPlan(Plan p) {
            if (plan != null) {
                plan.EndAction();
            }
            plan = p;
        }
        public Plan GetPlan() {
            return plan;
        }
    }
}