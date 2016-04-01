using UnityEngine;
using System.Collections;

namespace WarGames.Communication {
    public class CommunicationDistributor : MonoBehaviour {

        public static WarGames.Communication.CommunicationDistributor currentCommDistributor = null;

        private Hashtable communicationNetworks;

        void Start() {
            currentCommDistributor = this;
            communicationNetworks = new Hashtable();
        }

        public CommunicationNetwork getCommNetwork( LeaderLabel l ) {
            CommunicationNetwork net = hasNetwork( l );
            if ( net != null ) {
                return net;
            } else {
                net = addCommNetwork( l );
                return net;
            }
        }

        private CommunicationNetwork addCommNetwork( LeaderLabel l ) {
            communicationNetworks.Add( l, new CommunicationNetwork() );
            return hasNetwork( l );
        }

        private CommunicationNetwork hasNetwork( LeaderLabel l ) {
            if ( communicationNetworks.ContainsKey( l ) ) {
                return (CommunicationNetwork)communicationNetworks[l];
            }
            return null;
        }
    }
}