using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
// To-do: Make this an event class instead?
public class XREvents : MonoBehaviour
{
    public bool debugging;
    private AudioClip microphoneClip;
    private float lastAudioDetectionTime;
    private bool listenToMic;

    public static event Action<GameObject, GameObject, bool> onItemGrabbed, onItemInteracted, onItemTouched, onItemLookedAt;
    public static event Action<GameObject, GameObject, float> onItemShaked; // Grab Innteractable 2
    public static event Action<GameObject, GameObject, float> onTalking; // Grab Innteractable 2
    public static event Action<GameObject, GameObject> onItemGrabbedNearby; // Grab Innteractable 2
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

    public void EnableMicRecording()
    {
        if(microphoneClip == null) microphoneClip = Util.MicrophoneToAudioClip();
        lastAudioDetectionTime = Time.timeSinceLevelLoad;
        listenToMic = true;
    }
    public void DisableMicRecording() => listenToMic = false;

    void Update()
    {
        if(microphoneClip == null) return;
        if(!listenToMic) return;

        float loudness = Util.GetLoudnessFromMicrophone(microphoneClip, 32) * Util.MICROPHONE_LOUDNESS_MULTIPLIER;
        // Reinstate it again in 5 seconds
        if(loudness < Util.MICROPHONE_LOUDNESS_THRESHOLD)
        {
            if(lastAudioDetectionTime + 20 < Time.timeSinceLevelLoad)
            {
                lastAudioDetectionTime = Time.timeSinceLevelLoad;
                microphoneClip = Util.MicrophoneToAudioClip();
                Util.Print<XREvents>("Microphone been silent for too long, reinstate the microphone again");
            }
            return;
        }
        lastAudioDetectionTime = Time.timeSinceLevelLoad;
        OnTalking(gameObject, gameObject, loudness);
    }

    // Events
    public void ProcessLookAtEvent(GameObject o, GameObject instigator, bool lookingAt) => onItemLookedAt?.Invoke(o, instigator, lookingAt);
    private void OnItemGrabbed(GameObject o, GameObject instigator) => onItemGrabbed?.Invoke(o, instigator, false);
    private void OnItemDropped(GameObject o, GameObject instigator) => onItemGrabbed?.Invoke(o, instigator, true);
    private void OnItemInteracted(GameObject o, GameObject instigator) => onItemInteracted?.Invoke(o, instigator, false);
    private void OnItemUninteracted(GameObject o, GameObject instigator) => onItemInteracted?.Invoke(o, instigator, true);
    private void OnItemTouched(GameObject o, GameObject instigator) => onItemTouched?.Invoke(o, instigator, false);
    private void OnItemUntouched(GameObject o, GameObject instigator) => onItemTouched?.Invoke(o, instigator, true);
    private void OnTalking(GameObject o, GameObject instigator, float talkAmount = 0) => onTalking?.Invoke(gameObject, gameObject, talkAmount);
    public static void OnItemShake(GameObject o, GameObject instigator, float shakeAmount = 0) => onItemShaked?.Invoke(o, instigator, shakeAmount);
    public static void OnItemGrabbedNearby(GameObject o, GameObject instigator) => onItemGrabbedNearby?.Invoke(o, instigator);

    private void Print(string s)
    {
        if (!debugging) return;
        Util.Print(this, s);
    }
}
