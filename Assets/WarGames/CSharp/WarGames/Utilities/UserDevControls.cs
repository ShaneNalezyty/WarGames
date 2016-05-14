using System.Collections;
using UnityEngine;

namespace WarGames {
	public class UserDevControls : MonoBehaviour {
		void Start() {
			Time.timeScale = 0f;
		}
		/*void Update() {
			if (Input.GetButtonDown( "FullSpeed" )) {
				Time.timeScale = 1f;
			} else if (Input.GetButtonDown( "ThreeQuarterSpeed" )) {
				Time.timeScale = .75F;
			} else if (Input.GetButtonDown( "HalfSpeed" )) {
				Time.timeScale = .5F;
			} else if (Input.GetButtonDown( "QuarterSpeed" )) {
				Time.timeScale = .25F;
			} else if (Input.GetButtonDown( "ZeroSpeed" )) {
				Time.timeScale = 0F;
			} else if (Input.GetButtonDown( "GiveGoals" )) {
				ArrayList teamOneGOs = getTeam( "1" );
				ArrayList teamTwoGOs = getTeam( "2" );
				Vector3[] teamTwoGoalLocations = { new Vector3( 21, 0, 0 ), new Vector3( 15, 0, 0 ), new Vector3( 7, 0, 0 ), new Vector3( -2, 0, 0 ) };
				Vector3[] teamOneGoalLocations = { new Vector3( 21, 0, -75 ), new Vector3( 14, 0, -75 ), new Vector3( 10, 0, -75 ), new Vector3( 10, 0, -75 ) };
				for (int i = 0; i < 4; i++) {
					GameObject togo = (GameObject)teamOneGOs[i];
					Soldier s = togo.GetComponent<Soldier>();
					s.SetGoal( new Goal( Goal.AggressionLevel.Low, teamOneGoalLocations[i] ) );
					GameObject ttgo = (GameObject)teamTwoGOs[i];
					Soldier st = ttgo.GetComponent<Soldier>();
					st.SetGoal( new Goal( Goal.AggressionLevel.Moderate, teamTwoGoalLocations[i] ) );
				}
			}
		}*/
		void Update() {
			if (Input.GetButtonDown( "FullSpeed" )) {
				Time.timeScale = 1f;
			} else if (Input.GetButtonDown( "ThreeQuarterSpeed" )) {
				Time.timeScale = .75F;
			} else if (Input.GetButtonDown( "HalfSpeed" )) {
				Time.timeScale = .5F;
			} else if (Input.GetButtonDown( "QuarterSpeed" )) {
				Time.timeScale = .25F;
			} else if (Input.GetButtonDown( "ZeroSpeed" )) {
				Time.timeScale = 0F;
			} else if (Input.GetButtonDown( "GiveGoals" )) {
				GameObject temp = GameObject.Find( "WG11" );
				Soldier s = temp.GetComponent<Soldier>();
				ParagonAI.BaseScript bs = temp.GetComponent<ParagonAI.BaseScript>();
				bs.SetSpeed( bs.alertSpeed );
				s.SetGoal( new Goal( Goal.AggressionLevel.Low, new Vector3( 10, 0, -75 ) ) );
			}
		}
		private ArrayList getTeam(string teamNumber) {
			ArrayList toReturn = new ArrayList();
			for (int i = 0; i < 4; i++) {
				toReturn.Add( GameObject.Find( "WG" + teamNumber + (i + 1) ) );
			}
			return toReturn;
		}
	}
}