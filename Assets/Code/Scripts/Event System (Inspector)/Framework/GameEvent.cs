using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A game event based on observers and listeners, not a good solution for params but good for basic stuff
[CreateAssetMenu(fileName = "GameEvent", menuName = "Event/Normal Event", order = 0)]
public class GameEvent : ScriptableObject
{
    List<GameEventListener> listeners = new List<GameEventListener>();

    public void TriggerEvent()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
            listeners[i].OnEventTriggered();
    }

    public void AddListener(GameEventListener listener)
    {
        listeners.Add(listener);
    }
    
    public void RemoveListener(GameEventListener listener)
    {
        listeners.Remove(listener);
    }
}