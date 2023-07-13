using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class GameEventListenerInfo
{
    [HideInInspector]
    public string name;
    [Tooltip("The Game Event, Game Events are scriptable objects that are referenced into a script and invoked when needed and are made in the Project window.")]
    public GameEventCommand gameEvent;
    [Tooltip("The command that is required for this to trigger. Use this if you want to make it trigger a specific listener.")]
    public string command;
    [Min(0)]
    [Tooltip("Delay the calling of this listener by the specified time, 0 = instant.")]
    public float delay;
    [Tooltip("List of inspector events to trigger.")]
    public UnityEvent<string> onEventTriggered;

    public void Validate()
    {
        if(gameEvent != null) 
            name = $"{gameEvent.name} ";
        if(command != "") 
            name += $"({command})";
        if(delay > 0) 
            name += $" in {delay} " + "second" + (delay != 1.0f ? "s" : "");   
    }

    public void OnEventTriggered(string command)
    {
        if(onEventTriggered == null) return;
        if(gameEvent == null) return;
        if(this.command != command) return;
        onEventTriggered.Invoke(command);
    }
}
