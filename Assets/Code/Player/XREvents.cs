using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using System;
using Bhaptics.SDK2;
// To-do: Make this an event class instead?
public class XREvents : MonoBehaviour
{
    private AudioClip microphoneClip;
    private float lastAudioDetectionTime;
    private bool listenToMic;
    public bool isTalking;
    public float micModifier = 1;
    public GameObject micSensorPrefab;
    private GameObject activeMicSensor;

    public static event Action<GameObject, GameObject, bool> onItemGrabbed, onItemInteracted, onItemTouched;
    public static event Action<GameObject, GameObject, bool, float> onItemLookedAt;
    public static event Action<GameObject, GameObject, float> onItemShaked;
    public static event Action<float, float> onTalking;
    public static event Action<GameObject, GameObject> onItemGrabbedNearby;
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

        string handName = args.interactorObject.transform.parent.name;
        if(handName == "Left Hand") BhapticsLibrary.Play(BhapticsEvent.LEFT_GLOVE_PINCH);
        if(handName == "Right Hand") BhapticsLibrary.Play(BhapticsEvent.RIGHT_GLOVE_PINCH);
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

        //string handName = args.interactorObject.transform.parent.name;
        //if(handName == "Left Hand") BhapticsLibrary.PlayParam(BhapticsEvent.RIGHT_CPR_PRESS, 0.4f, 0.5f, 20.0f, 3.0f);
        //if(handName == "Right Hand") BhapticsLibrary.PlayParam(BhapticsEvent.LEFT_CPR_PRESS, 0.4f, 0.5f, 20.0f, 3.0f);
    }

    public void EnableMicRecording(string localizeSensorMicPrompt = "")
    {
        if(microphoneClip == null) microphoneClip = Util.MicrophoneToAudioClip();
        lastAudioDetectionTime = Time.timeSinceLevelLoad;
        listenToMic = true;
        if(GameManager._Instance != null && GameManager._Instance.isExam) return;
        if(micSensorPrefab != null)
        {
            if(activeMicSensor == null)
                activeMicSensor = Instantiate(micSensorPrefab);
            activeMicSensor.GetComponent<MicSensor>()?.SetText(localizeSensorMicPrompt);
        }
    }
    public void DisableMicRecording()
    {
        listenToMic = false;
        if(activeMicSensor != null)
        {
            Destroy(activeMicSensor);
            activeMicSensor = null;
        }
    }
    public void OnPauseMenuPinch() => BhapticsLibrary.Play(BhapticsEvent.LEFT_GLOVE_PINCH);
    

    void Update()
    {
        if(microphoneClip == null) return;
        if(!listenToMic) return;

        float loudness = Util.GetLoudnessFromMicrophone(microphoneClip, 32) * Util.MICROPHONE_LOUDNESS_MULTIPLIER * micModifier;
        isTalking = loudness >= Util.MICROPHONE_LOUDNESS_THRESHOLD;
        OnTalking(loudness, Mathf.Clamp01(loudness/Util.MICROPHONE_LOUDNESS_THRESHOLD));
        // Reinstate it again in 5 seconds
        if(loudness < Util.MICROPHONE_LOUDNESS_THRESHOLD)
        {
#if UNITY_EDITOR
            if(lastAudioDetectionTime + 20 < Time.timeSinceLevelLoad)
            {
                lastAudioDetectionTime = Time.timeSinceLevelLoad;
                microphoneClip = Util.MicrophoneToAudioClip();
                Util.Print("Microphone been silent for too long, reinstate the microphone again..");
            }
#endif
            return;
        }
        lastAudioDetectionTime = Time.timeSinceLevelLoad;
    }

    // Events
    public void ProcessLookAtEventContinous(GameObject o, GameObject instigator, bool lookingAt, float focusTime) => onItemLookedAt?.Invoke(o, instigator, lookingAt, focusTime);
    public void ProcessLookAtEvent(GameObject o, GameObject instigator, bool lookingAt) => onItemLookedAt?.Invoke(o, instigator, lookingAt, -1);
    private void OnItemGrabbed(GameObject o, GameObject instigator) => onItemGrabbed?.Invoke(o, instigator, false);
    private void OnItemDropped(GameObject o, GameObject instigator) => onItemGrabbed?.Invoke(o, instigator, true);
    private void OnItemInteracted(GameObject o, GameObject instigator) => onItemInteracted?.Invoke(o, instigator, false);
    private void OnItemUninteracted(GameObject o, GameObject instigator) => onItemInteracted?.Invoke(o, instigator, true);
    private void OnItemTouched(GameObject o, GameObject instigator) => onItemTouched?.Invoke(o, instigator, false);
    private void OnItemUntouched(GameObject o, GameObject instigator) => onItemTouched?.Invoke(o, instigator, true);
    // talkAmount = raw value with no cap
    //talkAmountNoramlized = 0 - 1 and clamped
    // 1 = talking, <1 = not talking
    private void OnTalking(float talkAmount = 0, float talkAmountNormalized = 0) => onTalking?.Invoke(talkAmount, talkAmountNormalized);
    public static void OnItemShake(GameObject o, GameObject instigator, float shakeAmount = 0) => onItemShaked?.Invoke(o, instigator, shakeAmount);
    public static void OnItemGrabbedNearby(GameObject o, GameObject instigator) => onItemGrabbedNearby?.Invoke(o, instigator);
}
