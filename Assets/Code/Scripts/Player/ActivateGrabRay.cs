using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ActivateGrabRay : MonoBehaviour
{
    public GameObject leftGrabRay, rightGrabRay;
    public XRDirectInteractor leftDirectGrab, rightDirectGrab;
    void Update()
    {
        if(leftGrabRay != null && leftDirectGrab != null)
            leftGrabRay.SetActive(leftDirectGrab.interactablesSelected.Count == 0);        
        if(rightGrabRay != null && rightDirectGrab != null)
            rightGrabRay.SetActive(rightDirectGrab.interactablesSelected.Count == 0);
    }
}
