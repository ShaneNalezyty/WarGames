using UnityEngine;
using System.Collections;

/// <summary>
/// Contains all classes used to communicate between agents.
/// </summary>
namespace WarGames.Communication {
    /// <summary>
    /// Provides agents a way to communicate information by sending messageable objects.
    /// </summary>
    public class communicationNetwork {

        private ArrayList soldiers = new ArrayList { new ArrayList(), new ArrayList() };
        private ArrayList leaders = new ArrayList { new ArrayList(), new ArrayList() };

        communicationNetwork() {

        }

        public void passMessage( leaderLabel l, messageable m ) {
            Queue leadersMessageQueue = getLeaderQueue( l );
            leadersMessageQueue.Enqueue( m );
        }

        public void passMessage( soldier s, messageable m ) {
            Queue soldersQueue = getSoldierQueue( s );
            soldersQueue.Enqueue( m );
        }

        public void addSoldier( soldier s ) {
            ArrayList solderObjects = (ArrayList)soldiers[0];
            ArrayList messageQueue = (ArrayList)soldiers[1];
            solderObjects.Add( s );
            messageQueue.Add( new Queue() );
            
        }

        public void addLeader( leaderLabel l ) {
            ArrayList leaderLabel = (ArrayList)leaders[0];
            ArrayList messageQueue = (ArrayList)leaders[1];
            leaderLabel.Add( l );
            messageQueue.Add( new Queue() );
        }

        public messageable getMessage( soldier s) {
            Queue solderQueue = getSoldierQueue( s );
            return (messageable)solderQueue.Dequeue();
        }

        public messageable getMessage( leaderLabel l ) {
            Queue leaderQueue = getLeaderQueue( l );
            return (messageable)leaderQueue.Dequeue();
        }

        private Queue getLeaderQueue( leaderLabel l ) {
            ArrayList leaderLabel = (ArrayList)leaders[0];
            ArrayList messageQueue = (ArrayList)leaders[1];
            int leaderIndex = leaderLabel.IndexOf( l );
            return (Queue)messageQueue[leaderIndex];
        }

        private Queue getSoldierQueue( soldier s ) {
            ArrayList solderObjects = (ArrayList)soldiers[0];
            ArrayList messageQueue = (ArrayList)soldiers[1];
            int soldierIndex = solderObjects.IndexOf( s );
            return (Queue)solderObjects[soldierIndex];
        }
    }
}