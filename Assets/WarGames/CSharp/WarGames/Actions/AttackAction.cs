using UnityEngine;
using System;
using System.Collections;
using System.Text;
using ParagonAI;

namespace WarGames {
    public class AttackAction : Actionable {
        private BaseScript baseScript;
        private Soldier soldier;
        private bool lastRanIdle;
        private bool firstRunOfThisAction = true;

        public bool NextAICycle( bool inCombat ) {
            if ( firstRunOfThisAction ) {
                baseScript.coverFinderScript.shouldUseDynamicCover = true;
                soldier = baseScript.gameObject.GetComponent<Soldier>();
                soldier.WriteToLog( "I'm starting a AttackAction", "A" );
                firstRunOfThisAction = false;
            }
            if ( inCombat ) {
                if ( lastRanIdle ) {
                    soldier.WriteToLog( "I've entered combat in a AttackAction", "A" );
                }
                lastRanIdle = false;
                //If in combat then run the combat version of this action.
                return NextCombatAICycle();
            } else {
                if ( !lastRanIdle ) {
                    soldier.WriteToLog( "I've left combat in a AttackAction", "A" );
                }
                lastRanIdle = true;
                //If not in combat then run the idle version of this action.
                return NextIdleAICycle();
            }
        }

        private bool NextIdleAICycle() {
            throw new NotImplementedException();
        }

        private bool NextCombatAICycle() {
            throw new NotImplementedException();
        }

        public void OnComplete() {

        }
        public void OnEnd() {

        }
    }
}