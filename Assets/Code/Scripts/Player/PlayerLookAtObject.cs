using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The look at target requires a collider or rigidbody!!!!
public class PlayerLookAtObject : MonoBehaviour
{
    public Camera _camera;
    public GameObject lookingAtObject;
    public XREvents xrEvents;
    public bool _Debugging;
    private LineRenderer lr;
    [SerializeField] private LineRenderer lineRendererGameObject;
    // Update is called once per frame
    void Update()
    {
        Transform camTrans = _camera.transform;
        Vector3 camPos = _camera.transform.position, camDir = _camera.transform.forward;
        RaycastHit hit;
        if(_Debugging)
        {
            if(lr == null) lr = Instantiate(lineRendererGameObject).GetComponent<LineRenderer>();
            
            lr.startColor = Color.red;
            lr.endColor = Color.red;
            lr.startWidth = 0.01f;
            lr.endWidth = 0.02f;
            
            lr.SetPosition(0, camPos + Vector3.down * 0.05f);
            lr.SetPosition(1, camDir * 50);
        }
        
        if(Physics.Raycast(camPos, camDir, out hit, Mathf.Infinity) && IsHitValid(hit))
        {
            // what we previously were looking at
            // avoid first unlook as "null" 
            if(lookingAtObject != null) xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, false);
            lookingAtObject = hit.collider.gameObject;
            // what we are looking at
            if(lookingAtObject != null) xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, true);
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
