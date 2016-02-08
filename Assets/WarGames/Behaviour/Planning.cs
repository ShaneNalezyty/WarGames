using UnityEngine;
using System.Collections;

namespace WarGames.Behaviour {
    public class Planning : ParagonAI.CustomAIBehaviour {
        Plan plan;
        
        public override void Initiate() {
            plan = baseScript.actionPlan;
            behaveLevel = BehaviourLevel.Idle;
            base.Initiate();
        }

        public override void AICycle() {
            if (!plan.hasAction()) {
                plan.nextAICycle();
            } else {

            }
        }

        public override void EachFrame() {

        }
        
        public override void OnEndBehaviour() {
            plan.onEndPlan();
        }
    }
}