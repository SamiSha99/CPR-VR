using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction, gripAnimationAction;
    public Animator handAnimator;
    void Start()
    {
        if(handAnimator == null) Debug.LogWarning("handAnimator is not specified, returned " + handAnimator);
    }

    void OnEnable()
    {
        if(pinchAnimationAction != null) pinchAnimationAction.action.Reset();
        if(gripAnimationAction != null) gripAnimationAction.action.Reset();
    }

    void Update()
    {
        if(pinchAnimationAction != null)
        {
            float triggerValue = 0;
            if(pinchAnimationAction.action.IsPressed()) triggerValue = pinchAnimationAction.action.ReadValue<float>();
            AnimateHand("Trigger", triggerValue);
        }
        if(gripAnimationAction != null)
        {
            float gripValue = 0;
            if(gripAnimationAction.action.IsPressed()) gripValue = gripAnimationAction.action.ReadValue<float>();
            AnimateHand("Grip", gripValue);
        }
    }

    private void AnimateHand(string name, float value)
    {
        if(handAnimator == null) return;
        handAnimator.SetFloat(name, value);
    }
}
