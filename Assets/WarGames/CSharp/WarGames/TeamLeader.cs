using UnityEngine;
using System.Collections;
using WarGames;
namespace WarGames {
    /// <summary>
    /// Script that holds all variables an agent needs to perform as a WarGames TeamLeader.
    /// </summary>
    /// <seealso cref="UnityEngine.MonoBehaviour" />
    public class TeamLeader : MonoBehaviour {
        /// <summary>
        /// The TeamLeaders KnowledgeBase.
        /// </summary>
        private KnowledgeBase myKnowledgeBase;
        // Use this for initialization
        public void Start() {
            myKnowledgeBase = new KnowledgeBase();
        }
    }
}