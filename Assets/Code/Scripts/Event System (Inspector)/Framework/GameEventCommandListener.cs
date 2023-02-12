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
        //Util.Print<GameEventCommandListener>($"Comparing between {command} == {this.command} bool result: {this.command == command} in GameObject: {transform.gameObject.name}");
        if(this.command != command) return;
        //Util.Print<GameEventCommandListener>("PASSED THE EVENT COMMAND CHECK!");
        onEventTriggered?.Invoke(command);
    }
    private bool IsValidListener()
    {
        return gameEvent != null && onEventTriggered != null;
    }
}