using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
public class XRPlayer : MonoBehaviour
{
    public static event Action<GameObject, bool> onItemGrabbed;
    public void OnItemGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("GameObject grabbed: " + args.interactableObject.transform.gameObject.name);
        onItemGrabbed?.Invoke(args.interactableObject.transform.gameObject, false);
    }

    public void OnItemDropped(SelectExitEventArgs args)
    {
        Debug.Log("GameObject dropped: " + args.interactableObject.transform.gameObject.name);
        onItemGrabbed?.Invoke(args.interactableObject.transform.gameObject, true);
    }
}
