using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// This is terrible, never used and not necessary but also buggy as hell.
// Avoid.
public class DoubleXRGrabInteractable : XRGrabInteractable
{
    [SerializeField] private Transform _secondAttachTransform;
    protected override void Awake()
    {
        base.Awake();
        selectMode = InteractableSelectMode.Multiple;
    }
    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        if(interactorsSelecting.Count == 1)
            base.ProcessInteractable(updatePhase);
        else if(interactorsSelecting.Count == 2 && updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            ProcessDoubleGrip();
        }
    }

    private void ProcessDoubleGrip()
    {
        Transform firstAttach = GetAttachTransform(null);
        Transform firstHand = interactorsSelecting[0].transform;
        Transform secondAttach = _secondAttachTransform;
        Transform secondHand = interactorsSelecting[1].transform;

        //Vector3 directionBetweenHands = secondHand.position - firstHand.position;
//
        //Quaternion targetRotation = Quaternion.LookRotation(directionBetweenHands, transform.up);
//
        //Vector3 worldDirectionFromHandleToBase = transform.position - firstAttach.position;
        //Vector3 localDirectionFromHandleToBase = transform.InverseTransformDirection(worldDirectionFromHandleToBase);
        //Vector3 directionBetweenAttaches = secondAttach.position - firstAttach.position;
        //Quaternion rotationFromAttachToForwrad = Quaternion.FromToRotation(directionBetweenAttaches, transform.forward);
        //transform.SetPositionAndRotation(transform.position, targetRotation * rotationFromAttachToForwrad);
        Vector3 directionBetweenHands = secondHand.position - firstHand.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionBetweenHands, transform.up);
        
        Vector3 worldDirectionFromHandleToBase = transform.position - firstAttach.position;
        Vector3 localDirectionFromHandleToBase = transform.InverseTransformDirection(worldDirectionFromHandleToBase);
        
        Vector3 directionBetweenAttaches = secondAttach.position - firstAttach.position;
        Quaternion rotationFromAttachToForwrad = Quaternion.FromToRotation(directionBetweenAttaches, transform.forward);
        
        Vector3 targetPosition = firstHand.position + localDirectionFromHandleToBase;

        transform.SetPositionAndRotation(transform.position, targetRotation * rotationFromAttachToForwrad);
    }
}
