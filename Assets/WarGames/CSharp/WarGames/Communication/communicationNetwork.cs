using UnityEngine;
using System.Collections;


/// <summary>
/// Contains all classes used to communicate between agents.
/// </summary>
namespace WarGames.Communication {
    /// <summary>
    /// Provides agents a way to communicate information by sending messageable objects.
    /// </summary>
    public class CommunicationNetwork {

        Hashtable agentQueues;

        public CommunicationNetwork() {
            agentQueues = new Hashtable();
        }
        /// <summary>
        /// Adds a Messageable object to a leader's message queue.
        /// </summary>
        /// <param name="l">The LeaderLabel representing the team leader.</param>
        /// <param name="m">The Messageable object.</param>
        /// <seealso cref="WarGames.Communication.Messageable" />
        public void PassMessage( LeaderLabel l, Messageable m ) {
            Queue leadersMessageQueue = GetLeaderQueue( l );
            leadersMessageQueue.Enqueue( m );
        }
        /// <summary>
        /// Adds a Messageable object to a soldier's message queue.
        /// </summary>
        /// <param name="s">The Soldier to receive the Messageable object.</param>
        /// <param name="m">The Messageable object.</param>
        /// <seealso cref="WarGames.Communication.Messageable" />
        public void PassMessage( Soldier s, Messageable m ) {
            Queue soldersQueue = GetSoldierQueue( s );
            soldersQueue.Enqueue( m );
        }
        /// <summary>
        /// Adds the soldier to the CommunicationNetwork. 
        /// Creates a queue to hold Messageable objects for this Soldier.
        /// </summary>
        /// <param name="s">The soldier to add to the CommunicationNetwork</param>
        public void AddSoldier( Soldier s ) {
            agentQueues.Add(s, new Queue());
            s.WriteToLog( "Added to the CommunicationNetwork as soldier", "C" );
        }
        /// <summary>
        /// Adds the TeamLeader to the CommunicationNetwork.
        /// Creates a queue to hold Messageable objects for this TeamLeader
        /// </summary>
        /// <param name="l">The LeaderLabel used to represent the TeamLeader</param>
        public void BecomeTeamLeader( Soldier s ) {
            agentQueues.Add( s.GetLeaderLabel(), new Queue());
            s.WriteToLog( "I have become the leader of team: " + s.GetLeaderLabel(), "T");
        }
        /// <summary>
        /// Gets the next Messageable object for this Soldier.
        /// </summary>
        /// <param name="s">The Soldier requesting the next Messageable object.</param>
        /// <returns>Soldiers next Messageable object in the Soldier's message queue.</returns>
        public Messageable GetMessage( Soldier s) {
            Queue solderQueue = GetSoldierQueue( s );
            if (solderQueue.Count != 0) {
                return (Messageable)solderQueue.Dequeue();
            }
            return null;
        }
        /// <summary>
        /// Gets the next Messageable object for this TeamLeader.
        /// </summary>
        /// <param name="l">The LeaderLabel representing the TeamLeader requesting the next Messageable object.</param>
        /// <returns>TeamLeaders next Messageable object in the TeamLeader's message queue.</returns>
        public Messageable GetMessage( LeaderLabel l ) {
            Queue leaderQueue = GetLeaderQueue( l );
            return (Messageable)leaderQueue.Dequeue();
        }
        /// <summary>
        /// Gets the TeamLeader's Messageable queue.
        /// </summary>
        /// <param name="l">The LeaderLabel representing the TeamLeader.</param>
        /// <returns>The TeamLeader's Messageable queue.</returns>
        private Queue GetLeaderQueue( LeaderLabel l ) {
            return (Queue)agentQueues[l];
        }
        /// <summary>
        /// Gets the Soldier's Messageable queue.
        /// </summary>
        /// <param name="s">The Soldier</param>
        /// <returns>The Soldier's Messageable queue.</returns>
        private Queue GetSoldierQueue( Soldier s ) {
            return (Queue)agentQueues[s];
        }

		public ICollection GetSoldiers() {
			return agentQueues.Keys;
		}
    }
}