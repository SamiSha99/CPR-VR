using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateGrabRay : MonoBehaviour
{
    const string LEFT_GRAB_RAY_NAME = "Left Grab Ray";
    const string RIGHT_GRAB_RAY_NAME = "Right Grab Ray";
    const string GRAB_RAY_DISABLED = " (DISABLED)";

    public GameObject leftGrabRay, rightGrabRay;
    public XRDirectInteractor leftDirectGrab, rightDirectGrab;
    public XRRayInteractor  leftRayInteractor, rightRayInteractor;
    // For now, rays are entirely disabled, only by direct grabbing.
    public bool leftGrabRayEnabled = false, rightGrabRayEnabled = false;
    
    void Awake()
    {
        EnableGrabRay(leftGrabRayEnabled, rightGrabRayEnabled);
    }

    void Update()
    {
        if(leftGrabRay != null && leftDirectGrab != null && leftRayInteractor != null)
            leftGrabRay.SetActive(leftGrabRayEnabled && leftDirectGrab.interactablesSelected.Count == 0);        
        if(rightGrabRay != null && rightDirectGrab != null && rightRayInteractor != null)
            rightGrabRay.SetActive(rightGrabRayEnabled && rightDirectGrab.interactablesSelected.Count == 0);
    }

    public void EnableGrabRay(bool left, bool right)
    {
        leftGrabRayEnabled = left;
        rightGrabRayEnabled = right;
        if(leftGrabRay != null) leftGrabRay.name = LEFT_GRAB_RAY_NAME + (left ? "" : GRAB_RAY_DISABLED);
        if(rightGrabRay != null) rightGrabRay.name = RIGHT_GRAB_RAY_NAME + (right ? "" : GRAB_RAY_DISABLED);
    }
}
