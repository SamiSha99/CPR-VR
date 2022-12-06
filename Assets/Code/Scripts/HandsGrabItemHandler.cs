using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
public class HandsGrabItemHandler : MonoBehaviour
{
    public XRDirectInteractor XRDirectInteractor;

    public void OnItemGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("GameObject grabbed: " + args.interactableObject.transform.gameObject.name);
    }

    public void OnItemDropped(SelectExitEventArgs args)
    {
        Debug.Log("GameObject dropped: " + args.interactableObject.transform.gameObject.name);
    }
}
