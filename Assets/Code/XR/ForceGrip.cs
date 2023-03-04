using UnityEngine;
using UnityEngine.InputSystem;

public class ForceGrip : MonoBehaviour
{
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    void Start() => AnimateHand("Grip", 1);
    private void AnimateHand(string name, float value)
    {
        if (handAnimator == null) return;
        handAnimator.SetFloat(name, value);
    }
}
