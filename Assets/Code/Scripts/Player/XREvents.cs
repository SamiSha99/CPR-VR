using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
// To-do: Make this an event class instead?
public class XREvents : MonoBehaviour
{
    public bool debugging;
    public static event Action<GameObject, bool> onItemGrabbed;
    public static event Action<GameObject, bool> onItemInteracted;
    public static event Action<GameObject, bool> onItemTouched;

    public void OnSelectEnteredEvent(SelectEnterEventArgs args) => ProcessSelectEvent(args, false);
    public void OnSelectExitedEvent(SelectExitEventArgs args) => ProcessSelectEvent(args, true);
    public void OnHoverEnteredEvent(HoverEnterEventArgs args) => ProcessHoverEvent(args, false);
    public void OnHoverExitedEvent(HoverExitEventArgs args) => ProcessHoverEvent(args, true);

    public void ProcessSelectEvent(BaseInteractionEventArgs args, bool exited)
    {
        GameObject o = args.interactableObject.transform.gameObject;
        
        if(o.HasComponent<XRGrabInteractable>())
        {
            if (exited) OnItemDropped(o);
            else OnItemGrabbed(o);
        }

        if (o.HasComponent<XRSimpleInteractable>())
        {
            if(exited) OnItemUninteracted(o);
            else OnItemInteracted(o);
        }

    }

    public void ProcessHoverEvent(BaseInteractionEventArgs args, bool exited)
    {
        GameObject o = args.interactableObject.transform.gameObject;
        // Interactor
        GameObject o2 = args.interactorObject.transform.gameObject;
        if(o2.name == "Left Grab Ray" || o2.name == "Right Grab Ray" ) return;

        if (exited) OnItemUntouched(o);
        else OnItemTouched(o);
    }

    private void OnItemGrabbed(GameObject o)
    {
        Print("GameObject grabbed: " + o.name);
        onItemGrabbed?.Invoke(o, false);
    }

    private void OnItemDropped(GameObject o)
    {
        Print("GameObject dropped: " + o.name);
        onItemGrabbed?.Invoke(o, true);
    }

    private void OnItemInteracted(GameObject o)
    {
        Print("GameObject interacted: " + o.name);
        onItemInteracted?.Invoke(o, false);
    }

    private void OnItemUninteracted(GameObject o)
    {
        Print("GameObject uninteracted: " + o.name);
        onItemInteracted?.Invoke(o, true);
    }

    private void OnItemTouched(GameObject o)
    {
        Print("GameObject touched: " + o.name);
        onItemTouched?.Invoke(o, false);
    }

    private void OnItemUntouched(GameObject o)
    {
        Print("GameObject untouched: " + o.name);
        onItemTouched?.Invoke(o, true);
    }

    private void Print(string s)
    {
        if (!debugging) return;
        GlobalHelper.Print(this, s);
        //GlobalHelper.Print<>(s);
    }
}
