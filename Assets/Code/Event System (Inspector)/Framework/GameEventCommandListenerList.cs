using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventCommandListenerList : GameEventCommandListener
{
    [System.Serializable]
    public struct CommandListener
    {
        public GameEventCommand gameEvent;
        public string command;
        public UnityEvent<string> onEventTriggered;
    };

    public List<CommandListener> listeners = new List<CommandListener>();

    protected override void OnEnable()
    {
        foreach(CommandListener l in listeners) l.gameEvent?.AddListener(this);
        base.OnEnable();
    }
    protected override void OnDisable() 
    { 
        foreach(CommandListener l in listeners) l.gameEvent?.RemoveListener(this);
        base.OnDisable();
    }

    public override void OnEventTriggered(string command)
    {
        base.OnEventTriggered(command);
        foreach(CommandListener l in listeners)
        {
            if(l.command != command) continue;
            if(l.gameEvent == null && onEventTriggered == null) continue;
            l.onEventTriggered?.Invoke(command);
        }
    }
}
