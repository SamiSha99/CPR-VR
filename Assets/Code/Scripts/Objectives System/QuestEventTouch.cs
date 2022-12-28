using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class QuestEventTouch : MonoBehaviour
{
    public static event Action<GameObject, bool> onItemTouched;
    //public List<string> allowedNames;

    void Awake()
    {
        gameObject.SetActive(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        //if(allowedNames.Count <= 0) return;
        
        //if(!IsAllowed(other.gameObject.name)) return;
        GlobalHelper.Print<QuestEventTouch>("other" + other.gameObject.name);
        onItemTouched?.Invoke(other.gameObject, false);
    }

    private void OnTriggerExit(Collider other)
    {
        //if(allowedNames.Count <= 0) return;
        //if(!IsAllowed(other.gameObject.name)) return;
        GlobalHelper.Print<QuestEventTouch>("other" + other.gameObject.name);
        onItemTouched?.Invoke(other.gameObject, true);
    }

    private bool IsAllowed(string n)
    {
        //foreach(string _name in allowedNames) if(_name == n) return true;
        return false;
    }
}
