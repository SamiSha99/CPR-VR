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
        onItemGrabbed?.Invoke(args.interactableObject.transform.gameObject, false);
        Debug.Log("GameObject grabbed: " + args.interactableObject.transform.gameObject.name);
    }

    public void OnItemDropped(SelectExitEventArgs args)
    {
        onItemGrabbed?.Invoke(args.interactableObject.transform.gameObject, true);
        Debug.Log("GameObject dropped: " + args.interactableObject.transform.gameObject.name);
    }
}
