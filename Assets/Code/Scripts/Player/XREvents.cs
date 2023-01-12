using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
// To-do: Make this an event class instead?
public class XREvents : MonoBehaviour
{
    public bool debugging;
    public static event Action<GameObject, GameObject, bool> onItemGrabbed, onItemInteracted, onItemTouched, onItemLookedAt;
    public static event Action<GameObject, GameObject, float> onItemShaked;
    // XR Events
    public void OnSelectEnteredEvent(SelectEnterEventArgs args) => ProcessSelectEvent(args, false);
    public void OnSelectExitedEvent(SelectExitEventArgs args) => ProcessSelectEvent(args, true);
    public void OnHoverEnteredEvent(HoverEnterEventArgs args) => ProcessHoverEvent(args, false);
    public void OnHoverExitedEvent(HoverExitEventArgs args) => ProcessHoverEvent(args, true);
    // Processors
    public void ProcessSelectEvent(BaseInteractionEventArgs args, bool exited)
    {
        GameObject o = args.interactableObject.transform.gameObject;
        GameObject interactor = args.interactorObject.transform.gameObject;
        if(o.HasComponent<XRGrabInteractable>())
        {
            if (exited) OnItemDropped(o, interactor);
            else OnItemGrabbed(o, interactor);
        }

        if (o.HasComponent<XRSimpleInteractable>())
        {
            if(exited) OnItemUninteracted(o, interactor);
            else OnItemInteracted(o, interactor);
        }

    }
    public void ProcessHoverEvent(BaseInteractionEventArgs args, bool exited)
    {
        GameObject o = args.interactableObject.transform.gameObject;
        // Interactor
        GameObject interactor = args.interactorObject.transform.gameObject;
        // Only direct interactables can trigger touch
        if(interactor.name == "Left Grab Ray" || interactor.name == "Right Grab Ray" ) return;

        if (exited) OnItemUntouched(o, interactor);
        else OnItemTouched(o, interactor);
    }
    // Events
    public void ProcessLookAtEvent(GameObject o, GameObject instigator, bool lookingAt)
    {
        //GlobalHelper.Print<XREvents>($"Target: {o.name}, Instigated by: {instigator.name}, lookingAt: {lookingAt}");
        if(lookingAt)
            Print("Gameobject looking at: " + o.name);
        else
            Print("Gameobject no longer looking at: " + o.name);
        onItemLookedAt?.Invoke(o, instigator, lookingAt);
    }
    private void OnItemGrabbed(GameObject o, GameObject instigator)
    {
        Print("GameObject grabbed: " + o.name);
        onItemGrabbed?.Invoke(o, instigator, false);
    }
    private void OnItemDropped(GameObject o, GameObject instigator)
    {
        Print("GameObject dropped: " + o.name);
        onItemGrabbed?.Invoke(o, instigator, true);
    }
    private void OnItemInteracted(GameObject o, GameObject instigator)
    {
        Print("GameObject interacted: " + o.name);
        onItemInteracted?.Invoke(o, instigator, false);
    }
    private void OnItemUninteracted(GameObject o, GameObject instigator)
    {
        Print("GameObject uninteracted: " + o.name);
        onItemInteracted?.Invoke(o, instigator, true);
    }
    private void OnItemTouched(GameObject o, GameObject instigator)
    {
        Print("GameObject touched: " + o.name);
        onItemTouched?.Invoke(o, instigator, false);
    }
    private void OnItemUntouched(GameObject o, GameObject instigator)
    {
        Print("GameObject untouched: " + o.name);
        onItemTouched?.Invoke(o, instigator, true);
    }

    public static void OnItemShake(GameObject o, GameObject instigator, float shakeAmount = 0)
    {
        GlobalHelper.Print<XREvents>("Shake Amount: " + shakeAmount);
        onItemShaked?.Invoke(o, instigator, shakeAmount);
    }

    private void Print(string s)
    {
        if (!debugging) return;
        GlobalHelper.Print(this, s);
        //GlobalHelper.Print<>(s);
    }
}
