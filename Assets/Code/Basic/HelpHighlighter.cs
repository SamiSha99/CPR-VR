using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HelpHighlighter : MonoBehaviour
{
    public HideObject hiderScript;
    public float radiusRange = 0.25f;
    bool inRange;
    public bool hideIfGrabbingInteractable;
    public XRGrabInteractable relevantInteractable;
    GameObject leftHand, rightHand;
    void Start()
    {
        leftHand = Util.GetPlayer().GetPlayerHandObject();
        rightHand = Util.GetPlayer().GetPlayerHandObject(true);
        if(!gameObject.HasComponent<HideObject>()) hiderScript = gameObject.AddComponent<HideObject>();
        if(hiderScript != null) hiderScript.SetHidden(!inRange);
        DoHighlighter();
    }

    void Update() => DoHighlighter();

    void DoHighlighter()
    {
        Vector3 pos = transform.position;
        Vector3 leftHandPos = leftHand.transform.position, rightHandPos = rightHand.transform.position;
        bool belowDistance = Vector3.Distance(leftHandPos, pos) <= radiusRange  || Vector3.Distance(rightHandPos, pos) <= radiusRange;
        if(ShouldHideOnGrab(belowDistance))
        {
            ToggleHighlighter(false); // don't show if grabbing
        }
        else
            ToggleHighlighter(belowDistance);
        //Util.Print<XREvents>($"Left Hand distance: {Vector3.Distance(leftHandPos, pos)}, Right Hand distance: {Vector3.Distance(rightHandPos, pos)} | Required range: {radiusRange}");
    }
    
    void ToggleHighlighter(bool b)
    {
        if(inRange == b) return;
        inRange = b;
        if(hiderScript != null) hiderScript.SetHidden(!inRange);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radiusRange);
    }

    bool ShouldHideOnGrab(bool belowDistance = false)
    {
        if(!belowDistance) return false;
        if(relevantInteractable == null) return false;
        if(relevantInteractable.interactorsSelecting.Count <= 0) return false;
        return true;
    }
}
