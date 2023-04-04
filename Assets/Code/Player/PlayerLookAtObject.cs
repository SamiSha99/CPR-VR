using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The look at target requires a collider or rigidbody!!!!
public class PlayerLookAtObject : MonoBehaviour
{
    public Camera _camera;
    private GameObject lookingAtObject;
    public XREvents xrEvents;
    public bool showHelpLine;
    private LineRenderer lr;
    public float focusTime = 0, lookRange = DEFAULT_LOOK_RANGE;
    [SerializeField] private LineRenderer lineRendererGameObject;

    public const float DEFAULT_LOOK_RANGE = 50;

    // Update is called once per frame
    void Update()
    {
        Transform camTrans = _camera.transform;
        Vector3 camPos = _camera.transform.position + Vector3.down * 0.075f, camDir = _camera.transform.forward;
        RaycastHit hit;
        if(showHelpLine)
        {
            if(lr == null)
            {
                lr = Instantiate(lineRendererGameObject).GetComponent<LineRenderer>();
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.startWidth = 0.01f;
                lr.endWidth = 0.015f;
            }

            lr.SetPosition(0, camPos);
            lr.SetPosition(1, camPos + camDir * lookRange);
        }
        else if(lr != null)        
            Destroy(lr);
        
        if(Physics.Raycast(camPos, camDir, out hit, lookRange) && IsHitValid(hit))
        {
            // what we previously were looking at
            // avoid first unlook as "null" 
            if(lookingAtObject != null)
            {
                xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, false);
            }
            lookingAtObject = hit.collider.gameObject;
            // what we are looking at
            if(lookingAtObject != null)
            {
                xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, true);
            }
        }
    
    }

    bool IsHitValid(RaycastHit hit)
    {
        if(hit.collider == null) return false;
        if(hit.collider.gameObject == null) return false;
        
        GameObject go = hit.collider.gameObject;
        
        // We are already looking at it, no need to consider it again.
        // Assume focus
        if(go == lookingAtObject)
        {
            focusTime += Time.deltaTime;
            xrEvents.ProcessLookAtEventContinous(go, transform.gameObject, true, focusTime);
            return false;
        }

        // New looking at target
        focusTime = 0;
        return true;
    }

    public void SetHelperColor(Color c)
    {
        if(lr == null) return;
        lr.startColor = c;
        lr.endColor = c;
    }
}
