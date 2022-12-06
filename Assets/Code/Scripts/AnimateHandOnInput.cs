using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimateHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction, gripAnimationAction;
    public Animator handAnimator;
    // Start is called before the first frame update
    void Start()
    {
        if(handAnimator == null) Debug.LogWarning("handAnimator is not specified, returned " + handAnimator);
    }

    // Update is called once per frame
    void Update()
    {
        if(pinchAnimationAction != null)
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            AnimateHand("Trigger", triggerValue);
        }
        if(gripAnimationAction != null)
        {
            float gripValue = gripAnimationAction.action.ReadValue<float>();
            AnimateHand("Grip", gripValue);
        }
    }

    private void AnimateHand(string name, float value)
    {
        if(handAnimator == null) return;
        handAnimator.SetFloat(name, value);
    }
}
