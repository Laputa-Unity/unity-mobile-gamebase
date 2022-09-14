using System;
using UnityEngine;

public class Door : MonoBehaviour
{
    public DoorType DoorType;

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (DoorType == DoorType.WinDoor)
            {
                GameManager.Instance.OnWinGame();
            }
            else
            {
                GameManager.Instance.OnLoseGame();
            }
        }
    }
}

public enum DoorType
{
    WinDoor,
    LoseDoor,
}
