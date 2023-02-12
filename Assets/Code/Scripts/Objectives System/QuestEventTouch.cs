using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;

public class QuestEventTouch : MonoBehaviour
{
    public static event Action<GameObject, GameObject, bool> onItemTouched;
    public List<string> allowedNames;
    public List<GameObject> allowedGameObjects;
    public Transform snapPoint;
    [Tooltip("Runs SetActive(false).")]
    public bool deactivateOnCompletion;
    public bool _DestroyObject;
    [HideInInspector] public GameObject _lastTouchCollider, _lastUntouchCollider;
    void OnTriggerEnter(Collider other)
    {
        if(!IsAllowed(other.gameObject)) return;
        _lastTouchCollider = other.gameObject;
        Util.Invoke(this, () => ProcessEvent(other.gameObject, false), 0.01f);
    }
    void OnTriggerExit(Collider other)
    {
        if(!IsAllowed(other.gameObject)) return;
        _lastUntouchCollider = other.gameObject;
        Util.Invoke(this, () => ProcessEvent(other.gameObject, true), 0.01f);
    }
    private void ProcessEvent(GameObject o, bool untouch)
    {
        XRGrabInteractable2 xr2 = o.GetComponent<XRGrabInteractable2>();
        GameObject instigator = null;
        if(xr2 != null) instigator = xr2._lastInteractorSelect;
        //Util.Print<QuestEventTouch>("" + instigator);
        onItemTouched?.Invoke(o, instigator, untouch);
        
        if(snapPoint != null && !untouch)
        {
            TryReleasingGrabbedObject(o);
            o.transform.SetPositionAndRotation(snapPoint.position, snapPoint.rotation);
        }

        if(_DestroyObject) Destroy(o);
        if(deactivateOnCompletion) gameObject.SetActive(false);
    }
    private bool IsAllowed(GameObject go)
    {
        foreach(string _name in allowedNames) if(_name == go.name) return true;
        foreach(GameObject o in allowedGameObjects) if(o == go) return true;
        return false;
    }
    // Yuck, probably a better approach?
    void TryReleasingGrabbedObject(GameObject o)
    {
        o.GetComponent<XRGrabInteractable>().enabled = false;    
    }
}
