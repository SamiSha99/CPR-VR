using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestEventTouch : MonoBehaviour
{
    public static event Action<GameObject, bool> onItemTouched;
    public List<string> allowedNames;
    [Tooltip("Hide the gameobject on run time, assuming we are handling it somewhere else.")]
    public bool _hidden;
    [Tooltip("runs SetActive(false)")]
    public bool deactivateOnCompletion;
    [HideInInspector] public GameObject _lastTouchCollider, _lastUntouchCollider;
    void Awake()
    {
        if(_hidden) Hide();
    }
    void OnTriggerEnter(Collider other)
    {
        if(!IsAllowed(other.gameObject.name)) return;
        _lastTouchCollider = other.gameObject;
        GlobalHelper.Invoke(this, () => ProcessEvent(other.gameObject, false), 0.01f);
    }
    void OnTriggerExit(Collider other)
    {
        if(!IsAllowed(other.gameObject.name)) return;
        _lastUntouchCollider = other.gameObject;
        GlobalHelper.Invoke(this, () => ProcessEvent(other.gameObject, true), 0.01f);
    }
    private void ProcessEvent(GameObject o, bool untouch)
    {
        onItemTouched?.Invoke(o, untouch);
        if(deactivateOnCompletion) gameObject.SetActive(false);
    }

    public void Hide() => ToggleHidden(true);
    public void Unhide() => ToggleHidden(false);
    private void ToggleHidden(bool _hidden = false) => transform.ToggleHidden(_hidden);
    
    private bool IsAllowed(string n)
    {
        if(allowedNames.Count <= 0) return false;
        foreach(string _name in allowedNames) if(_name == n) return true;
        return false;
    }
}
