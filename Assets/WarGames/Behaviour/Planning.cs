using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using WarGames.Action;

namespace WarGames.Behaviour {
    public class Planning : ParagonAI.CustomAIBehaviour {

        public Plan createPlanFromGoal() {
            List<ActionTemplate> actions = new List<ActionTemplate>();
            actions.Add( new ActionTemplate( baseScript, GameObject.Find( "TargetOne" ).transform ));
            actions.Add( new ActionTemplate( baseScript, GameObject.Find( "TargetTwo" ).transform ));
            return new WarGames.Plan(actions);
             
        }

        public override void Initiate() {
            behaveLevel = BehaviourLevel.Idle;
            base.Initiate();
        }

        public override void AICycle() {
            if (baseScript.actionPlan == null) {
                baseScript.actionPlan = createPlanFromGoal();
                
            }
            if (!baseScript.actionPlan.hasAction()) {
                baseScript.actionPlan.nextAICycle();
            } else {

            }
        }

        public override void EachFrame() {

        }
        
        public override void OnEndBehaviour() {
            baseScript.actionPlan.onEndPlan();
        }
    }
}