using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventCommandListener : MonoBehaviour
{
    public GameEventCommand gameEvent;
    public string command;
    public UnityEvent<string> onEventTriggered;
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
    public void OnEventTriggered(string command)
    {
        if(!IsValidListener()) return;
        //GlobalHelper.Print<GameEventCommandListener>($"Comparing between {this.command} == {command} bool result: {this.command == command}");
        if(this.command != command) return;
        //GlobalHelper.Print<GameEventCommandListener>("PASSED THE EVENT COMMAND CHECK!");
        onEventTriggered?.Invoke(command);
    }
    private bool IsValidListener()
    {
        return gameEvent != null && onEventTriggered != null;
    }
}