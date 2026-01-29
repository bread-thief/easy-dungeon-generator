using UnityEngine;

namespace BreadThief.EasyDungeonGenerator
{
    /// <summary>
    /// Represents a connection point between rooms in the dungeon generation system.
    /// Each RoomConnector component defines a potential attachment point where rooms can be connected.
    /// The component handles visual debugging in the editor and manages connection state.
    /// </summary>
    public class RoomConnector : MonoBehaviour
    {
        private bool _isConnected = false;

        /// <summary>
        /// Gets whether this connector is currently connected to another room.
        /// </summary>
        /// <value>True if the connector is connected; otherwise false.</value>
        public bool IsConnected => _isConnected;

        /// <summary>
        /// Sets the connection state of this connector.
        /// </summary>
        /// <param name="isConnected">True to mark as connected, false to mark as available.</param>
        public void SetIsConnected(bool isConnected)
        {
            _isConnected = isConnected;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = _isConnected ? Color.green : Color.red;
            Gizmos.DrawRay(transform.position, transform.forward * 1f);
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
#endif
    }
}