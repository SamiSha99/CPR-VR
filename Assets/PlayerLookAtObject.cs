using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLookAtObject : MonoBehaviour
{
    public Camera _camera;
    public GameObject lookingAtObject;
    public XREvents xrEvents;
    // Update is called once per frame
    void Update()
    {
        Transform camTrans = _camera.transform;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity) && IsHitValid(hit))
        {
            xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, false);
            lookingAtObject = hit.collider.gameObject;
        }
    
    }

    bool IsHitValid(RaycastHit hit)
    {
        if(hit.collider == null) return false;
        if(hit.collider.gameObject == null) return false;
        GameObject go = hit.collider.gameObject;
        // We are already looking at it, no need to consider it again.
        // Possible implementation for a continous look?
        if(go == lookingAtObject) return false;
        // New looking at target
        return true;
    }
}
