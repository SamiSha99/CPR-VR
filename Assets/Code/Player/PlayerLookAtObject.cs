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
    private bool isLookingAtObject;
    [SerializeField] private LineRenderer lineRendererGameObject;

    public const float DEFAULT_LOOK_RANGE = 50;

    // Update is called once per frame
    void Update()
    {
        Transform camTrans = _camera.transform;
        Vector3 camPos = _camera.transform.position + Vector3.down * 0.075f, camDir = _camera.transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(camPos + camDir*lookRange, camDir*-1, lookRange, ~LayerMask.GetMask("Player", "Controller"));

        if(hits.Length <= 0)
        {
            if(isLookingAtObject)
            {
                xrEvents.ProcessLookAtEvent(null, transform.gameObject, false);
                lookingAtObject = null;
                isLookingAtObject = false;
            }
        }
        else foreach(RaycastHit h in hits)
        {
            if(!IsHitValid(h)) continue;
            
            GameObject go = h.collider.gameObject;
            if(go == lookingAtObject)
            {
                focusTime += Time.deltaTime;
                xrEvents.ProcessLookAtEventContinous(go, transform.gameObject, true, focusTime);
                break;
            }
            focusTime = 0;
            // what we previously were looking at
            // avoid first unlook as "null" 
            if(lookingAtObject != null) xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, false);
            lookingAtObject = h.collider.gameObject;
            // what we are looking at
            if(lookingAtObject != null) xrEvents.ProcessLookAtEvent(lookingAtObject, transform.gameObject, true);
            isLookingAtObject = true;
            break;
        }

        if(showHelpLine)
        {
            if(lr == null)
            {
                lr = Instantiate(lineRendererGameObject).GetComponent<LineRenderer>();
                lr.startColor = Color.red;
                lr.endColor = Color.red;
                lr.startWidth = 0.015f;
                lr.endWidth = 0.015f;
                lr.alignment = LineAlignment.View;
            }

            lr.SetPosition(0, camPos);
            lr.SetPosition(1, camPos + camDir * lookRange);
        }
        else if(lr != null)        
            Destroy(lr);
    
    }

    bool IsHitValid(RaycastHit hit)
    {
        if(hit.collider == null) return false;
        if(hit.collider.gameObject == null) return false;
        // looking at yourself?
        if(hit.transform.root == transform.root) return false;
        return true;
    }

    public void SetHelperColor(Color c)
    {
        if(lr == null) return;
        lr.startColor = c;
        lr.endColor = c;
    }
}
