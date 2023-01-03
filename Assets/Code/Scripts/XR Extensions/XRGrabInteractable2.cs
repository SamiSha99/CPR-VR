using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Additional variables meant to remember who interacted when it comes to the player
public class XRGrabInteractable2 : XRGrabInteractable
{
    public GameObject _lastInteractorSelect, _lastInteractorHover;
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
