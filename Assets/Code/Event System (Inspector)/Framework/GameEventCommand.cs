using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A game event based on observers and listeners, uses a "command" to filter out the result and target a specific listeners with such command
[CreateAssetMenu(fileName = "GameEventCommand", menuName = "Event/Command Event", order = 1)]
public class GameEventCommand : ScriptableObject
{
    List<GameEventListenerInfo> listeners = new List<GameEventListenerInfo>();

#if UNITY_EDITOR
    [SerializeField]
    [Tooltip("Does nothing, this is only here to document all command types this GameEvent accepts.\n\nThe command is formatted as follows:\ncommand_here + [any command from the list below].\nThis is case sensitive!")]
    List<string> commandList = new List<string>();
    [SerializeField] [TextArea(5,10)]
    string AdditionalNotes;
#endif

    public void TriggerEvent(string command = "")
    {
        for (int i = listeners.Count - 1; i >= 0; i--) listeners[i].OnEventTriggered(command);
    }
    public void AddListener(GameEventListenerInfo listener) => listeners.Add(listener);
    public void RemoveListener(GameEventListenerInfo listener) => listeners.Remove(listener);
}
