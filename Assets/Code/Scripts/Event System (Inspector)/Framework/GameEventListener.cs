using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent onEventTriggered;
    void OnEnable()
    {
        if(!IsValidListener()) return;
        gameEvent.AddListener(this);
    }
    void OnDisable() 
    { 
        if(!IsValidListener()) return;
        gameEvent.RemoveListener(this); 
    }
    public void OnEventTriggered()
    {
        if(!IsValidListener()) return;
        onEventTriggered?.Invoke();
    }
    private bool IsValidListener()
    {
        return gameEvent != null && onEventTriggered != null;
    }
}