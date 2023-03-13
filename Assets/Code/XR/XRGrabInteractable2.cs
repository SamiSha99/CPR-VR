using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Additional variables meant to remember who interacted when it comes to the player
public class XRGrabInteractable2 : XRGrabInteractable
{
    //public static event Action<GameObject, GameObject, bool> onItemShake;

    [Header("XR Grab Interactable 2")]
    public GameObject _lastInteractorSelect;
    public GameObject _lastInteractorHover;

    [Header("Restore to Origin")]
    public bool restoreOriginalPosition;
    public bool restoreOriginalRotation;
    Vector3 originalPosition;
    Quaternion originalRotation;
    public bool snapToOrigin;
    public float restorationSpeed = 10, restorationDelay;
    private float resotrationTimer;
    [Header("XR Shake Event")]
    [Tooltip("Set makes shaking less tedious by giving more points for less shake.")]
    public float shakeMultiplier = 1;
    float accumulatedShakeDistance = 0, allshake = 0;
    Vector3 lastPosition;
    Quaternion lastRotation;
    float shakeEventDelaySeconds = 0.03334f, shakeEventDelay;

    void Start()
    {
        if(restoreOriginalPosition) originalPosition = transform.position;
        if(restoreOriginalRotation) originalRotation = transform.rotation;
        //if(restoreOriginalPosition || restoreOriginalRotation) GetComponent<Rigidbody>().isKinematic = true;
        lastPosition = transform.position;
        lastRotation = transform.rotation;
        shakeEventDelay = shakeEventDelaySeconds;
        resotrationTimer = restorationDelay;
        restorationDelay = 0; // Begins restoring by default
    }

    void Update()
    {
        if(interactorsSelecting.Count <= 0)
        {   
            if(resotrationTimer > 0) restorationDelay = Mathf.Max(0.0f, restorationDelay - Time.deltaTime);
            
            if(resotrationTimer > 0 && restorationDelay <= 0 || resotrationTimer <= 0)
            {
                if(restoreOriginalPosition) 
                    transform.position = snapToOrigin ? originalPosition : Vector3.Lerp(transform.position, originalPosition, restorationSpeed * Time.deltaTime);
               if(restoreOriginalRotation) 
                    transform.rotation = snapToOrigin ? originalRotation : Quaternion.Lerp(transform.rotation, originalRotation, Mathf.Clamp(restorationSpeed * Time.deltaTime, 0f, 0.99f));
            } 
        }
        else if(shakeEventDelay > 0)
        { 
            shakeEventDelay = Mathf.Max(shakeEventDelay - Time.deltaTime, 0.0f);
            // distance from position movement
            accumulatedShakeDistance += Vector3.Distance(transform.position, lastPosition) * shakeMultiplier;
            // distance from directional rotation
            accumulatedShakeDistance += Vector3.Distance(transform.rotation * Vector3.forward, lastRotation * Vector3.forward) * shakeMultiplier;
            if(shakeEventDelay <= 0)
            {
                shakeEventDelay = shakeEventDelaySeconds;
                allshake += accumulatedShakeDistance;
                XREvents.OnItemShake(gameObject, _lastInteractorSelect, accumulatedShakeDistance);
                accumulatedShakeDistance = 0;
            }
        }

        if(interactorsSelecting.Count > 0)
        {
            if(resotrationTimer > 0) restorationDelay = resotrationTimer;
            XREvents.OnItemGrabbedNearby(gameObject, _lastInteractorSelect);
        }
        lastPosition = transform.position;
        lastRotation = transform.rotation;
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase) => base.ProcessInteractable(updatePhase);
    // Remember who selected/hovered
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        base.OnSelectEntering(args);
        _lastInteractorSelect = args.interactorObject.transform.gameObject;
    }
    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        _lastInteractorSelect = args.interactorObject.transform.gameObject;
    }
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        base.OnSelectExiting(args);
        _lastInteractorSelect = args.interactorObject.transform.gameObject;
    }
    protected override void OnHoverEntering(HoverEnterEventArgs args)
    {
        base.OnHoverEntering(args);
        _lastInteractorHover = args.interactorObject.transform.gameObject;
    }
    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        _lastInteractorHover = args.interactorObject.transform.gameObject;
    }
    protected override void OnHoverExiting(HoverExitEventArgs args)
    {
        base.OnHoverExiting(args);
        _lastInteractorHover = args.interactorObject.transform.gameObject;
    }    
}
