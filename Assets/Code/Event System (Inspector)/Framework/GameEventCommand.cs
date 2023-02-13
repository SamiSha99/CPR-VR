using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
