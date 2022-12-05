using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportationCollision : MonoBehaviour
{
    public GameEvent onCollidingWithTeleporter;
    public void OnInteractWithTeleporter()
    {
        onCollidingWithTeleporter.TriggerEvent();
    }
}
