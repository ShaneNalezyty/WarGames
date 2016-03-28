using UnityEngine;
using System.Collections;

namespace WarGames.Communication {
    /// <summary>
    /// Interface for all message objects to be passed over the communication network.
    /// </summary>
    public interface Messageable {
        string ToString();
    }
}