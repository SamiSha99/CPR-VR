using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventCommandListener : MonoBehaviour
{
    public GameEventCommand gameEvent;
    public string command;
    public UnityEvent<string> onEventTriggered;
    void OnEnable() => gameEvent.AddListener(this);
    void OnDisable() => gameEvent.RemoveListener(this);
    public void OnEventTriggered(string command)
    {
        GlobalHelper.Print<GameEventCommandListener>($"Comparing between {this.command} == {command} bool result: {this.command == command}");
        if(this.command != command) return;
        GlobalHelper.Print<GameEventCommandListener>("PASSED THE EVENT COMMAND CHECK!");
        onEventTriggered.Invoke(command);
    }
}