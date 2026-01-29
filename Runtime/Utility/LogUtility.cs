using UnityEngine;

namespace BreadThief.EasyDungeonGenerator
{
    /// <summary>
    /// Defines the severity levels for log messages used throughout the dungeon generation system.
    /// Each level corresponds to a different type of log output with distinct visual styling.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Standard informational message with no special styling
        /// </summary>
        NORMAL = 0,
        /// <summary>
        /// Warning message indicating potential issues that don't prevent generation
        /// </summary>
        WARNING = 1,
        /// <summary>
        /// Error message indicating critical failures that may stop generation
        /// </summary>
        ERROR = 2,
        /// <summary>
        /// Success message indicating successful operations or completion
        /// </summary>
        SUCCESSFUL = 3,
    }

    /// <summary>
    /// Provides centralized logging functionality for the dungeon generation system.
    /// Formats log messages with consistent styling and prefixes based on message severity.
    /// All messages are prefixed with [EasyDungeonGenerator] for easy filtering in Unity's console.
    /// </summary>
    public static class LogUtility
    {
        /// <summary>
        /// Logs a message to the Unity console with formatting based on the specified message type.
        /// </summary>
        /// <param name="message">The text message to display in the console</param>
        /// <param name="type">The severity/type of message (defaults to NORMAL)</param>
        public static void Log(string message, MessageType type = MessageType.NORMAL)
        {
            switch (type)
            {
                case MessageType.WARNING:
                    Debug.LogWarning($"<color=white>[EasyDungeonGenerator]</color> {message}");
                    break;
                case MessageType.ERROR:
                    Debug.LogError($"<color=white>[EasyDungeonGenerator]</color> {message}");
                    break;
                case MessageType.SUCCESSFUL:
                    Debug.Log($"<color=white>[EasyDungeonGenerator]</color> <color=green>{message}</color>");
                    break;
                default:
                    Debug.Log($"<color=white>[EasyDungeonGenerator]</color> {message}");
                    break;
            }
        }
    }
}