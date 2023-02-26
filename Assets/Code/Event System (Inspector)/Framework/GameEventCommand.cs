using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A game event based on observers and listeners, uses a "command" to filter out the result and target a specific listeners with such command
[CreateAssetMenu(fileName = "GameEventCommand", menuName = "Event/Command Event", order = 1)]
public class GameEventCommand : ScriptableObject
{
    List<GameEventCommandListener> listeners = new List<GameEventCommandListener>();
    public void TriggerEvent(string command)
    {
        for (int i = listeners.Count - 1; i >= 0; i--) listeners[i].OnEventTriggered(command);
    }
    public void AddListener(GameEventCommandListener listener) => listeners.Add(listener);
    public void RemoveListener(GameEventCommandListener listener) => listeners.Remove(listener);
}
