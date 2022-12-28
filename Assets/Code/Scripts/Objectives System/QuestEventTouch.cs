using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestEventTouch : MonoBehaviour
{
    public static event Action<GameObject, bool> onItemTouched;
    public List<string> allowedNames;
    public GameObject _lastCollider;
    void Awake()
    {
        gameObject.SetActive(false);
    }
    void OnTriggerEnter(Collider other)
    {
        if(allowedNames.Count <= 0) return;
        if(!IsAllowed(other.gameObject.name)) return;
        _lastCollider = other.gameObject;
        Invoke("OnItemTouched", 0.01f);
        return;
    }

    void RealEventTriggerTest()
    {
        onItemTouched?.Invoke(_lastCollider, false);
    }
    void OnTriggerExit(Collider other)
    {
        if(allowedNames.Count <= 0) return;
        if(!IsAllowed(other.gameObject.name)) return;
        _lastCollider = other.gameObject;
        Invoke("OnItemUnTouched", 0.01f);
    }
    
    private void OnItemTouched() => ProcessEvent(_lastCollider, false);
    private void OnItemUnTouched() => ProcessEvent(_lastCollider, false);

    private void ProcessEvent(GameObject o, bool untouch)
    {
        onItemTouched?.Invoke(o, untouch);
    }

    private bool IsAllowed(string n)
    {
        foreach(string _name in allowedNames) if(_name == n) return true;
        return false;
    }
}
