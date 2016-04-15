using UnityEngine;
using System.Collections;
using System.Text;
using System;

namespace WarGames {
	/// <summary>
	/// Contains a desirable gamestate created by a team leader.
	/// Sent to agents using a goalMessage
	/// </summary>
	/// <seealso cref="WarGames.Communication.GoalMessage" />
	[System.Serializable]
	public class Goal {
        private AggressionLevel aggressionLevel;
        private Vector3 destination;

        public Goal(AggressionLevel level, Vector3 goalDestination) {
            aggressionLevel = level;
            destination = goalDestination;
        }
        public enum AggressionLevel {
            VeryLow = 0, Low = 1, Moderate = 2, High = 3, VeryHigh = 4
        }
        public float[] GetWaitRange() {
            float[] toReturn = new float[2];
            switch (aggressionLevel) {
            case AggressionLevel.VeryLow:
                toReturn[0] = 5f;
                toReturn[1] = 9f;
                break;
            case AggressionLevel.Low:
                toReturn[0] = 4f;
                toReturn[1] = 8f;
                break;
            case AggressionLevel.Moderate:
                toReturn[0] = 3f;
                toReturn[1] = 6f;
                break;
            case AggressionLevel.High:
                toReturn[0] = 2f;
                toReturn[1] = 4f;
                break;
            case AggressionLevel.VeryHigh:
                toReturn[0] = 1f;
                toReturn[1] = 2f;
                break;
            }
            return toReturn;
        }
        public AggressionLevel GetAggressionLevel() {
            return aggressionLevel;
        }
        public Vector3 GetDestination() {
            return destination;
        }
        public override string ToString() {
            return base.ToString();
        }
        public override bool Equals( object obj ) {
			Goal checkWith = (Goal)obj;
			if (aggressionLevel == checkWith.GetAggressionLevel()) {
				if (destination == checkWith.GetDestination()) {
					return true;
				}
			}
            return false;
        }
        public override int GetHashCode() {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append( aggressionLevel.GetHashCode() );
            stringBuilder.Append( destination.x );
            stringBuilder.Append( destination.y );
            stringBuilder.Append( destination.z );
            return Int32.Parse( stringBuilder.ToString() );
        }
    }
}