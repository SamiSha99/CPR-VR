using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject leftTeleportation, rightTeleportation;
    public InputActionProperty leftActivate, rightActivate;
    public bool useRightRay;
    public GameEvent onTeleportRayActivate, onTeleportRayDeactivate;

    private bool rayActive;

    private void Start()
    {
        if (leftTeleportation != null) leftTeleportation.SetActive(false);
        if (rightTeleportation != null) rightTeleportation.SetActive(false);
    }

    void Update()
    {
        GameObject teleportation;
        InputActionProperty iap_Activate;

        if (useRightRay)
        {
            if (rightTeleportation == null || rightActivate == null) return;
            teleportation = rightTeleportation;
            iap_Activate = rightActivate;
        }
        else
        {
            if (leftTeleportation == null || leftActivate == null) return;
            teleportation = leftTeleportation;
            iap_Activate = leftActivate;
        }

        if (teleportation != null && iap_Activate != null)
        {
            teleportation.SetActive(iap_Activate.action.ReadValue<float>() > 0.1f);
            if (teleportation.activeSelf || teleportation.activeInHierarchy)
                OnRayActivation();
            else
                OnRayDeactivation();
        }
    }

    void OnRayActivation()
    {
        if (rayActive) return;
        rayActive = true;
        onTeleportRayActivate.TriggerEvent();
    }

    void OnRayDeactivation()
    {
        if (!rayActive) return;
        rayActive = false;
        onTeleportRayDeactivate.TriggerEvent();
    }
}