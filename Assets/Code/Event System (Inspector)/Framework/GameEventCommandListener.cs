using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventCommandListener : MonoBehaviour
{
    public GameEventCommand gameEvent;
    public string command;
    public UnityEvent<string> onEventTriggered;
    protected virtual void OnEnable()
    {
        if(!IsValidListener()) return;
        gameEvent.AddListener(this);
    }
    protected virtual void OnDisable() 
    { 
        if(!IsValidListener()) return;
        gameEvent.RemoveListener(this); 
    }
    public virtual void OnEventTriggered(string command)
    {
        if(!IsValidListener()) return;
        if(this.command != command) return;
        onEventTriggered?.Invoke(command);
    }
    private bool IsValidListener()
    {
        return gameEvent != null && onEventTriggered != null;
    }
}