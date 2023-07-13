using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventCommandListener : MonoBehaviour
{
    [SerializeField] public List<GameEventListenerInfo> listenersInfo;
    protected void OnEnable() => listenersInfo?.ForEach(l => l.gameEvent?.AddListener(l));
    protected void OnDisable() => listenersInfo?.ForEach(l => l.gameEvent?.RemoveListener(l));
    public void OnEventTriggered(string command) => listenersInfo?.ForEach(l => l.OnEventTriggered(command));        
    public void OnValidate() => listenersInfo?.ForEach(l => l.Validate());
}