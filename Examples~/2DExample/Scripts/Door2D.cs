using BreadThief.EasyDungeonGenerator;
using UnityEngine;

public class Door2D : MonoBehaviour
{
    private RoomConnector _roomConnector;

    private void Awake()
    {
        _roomConnector = GetComponentInParent<RoomConnector>();
    }

    private void Update()
    {
        gameObject.SetActive(!_roomConnector.IsConnected);
    }
}