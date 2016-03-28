using UnityEngine;
using System.Collections;

namespace WarGames {
    /// <summary>
    /// Label to represent a TeamLeader
    /// </summary>
    public class LeaderLabel {
        /// <summary>
        /// The leader identifier
        /// </summary>
        private int leaderID;
        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderLabel"/> class.
        /// </summary>
        /// <param name="newLeaderID">The new leader identifier.</param>
        public LeaderLabel( int newLeaderID ) {
            leaderID = newLeaderID;
        }
        /// <summary>
        /// Gets the leader identifier.
        /// </summary>
        /// <returns>The leader identifier</returns>
        public int getLeaderID() {
            return leaderID;
        }
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents TeamLeader's identifier.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this TeamLeader's identifier.
        /// </returns>
        override public string ToString() {
            return getLeaderID().ToString();
        }
    }
}