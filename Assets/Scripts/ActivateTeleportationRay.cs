using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject leftTeleportation, rightTeleportation;
    public InputActionProperty leftActivate, rightActivate;
    public bool useRightRay;
    private bool rayActive;
    public static event Action<bool> onTeleportRayActivated;

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
            OnRayActivation(teleportation.activeSelf || teleportation.activeInHierarchy);
        }
    }

    void OnRayActivation(bool b)
    {
        if (rayActive == b) return;
        rayActive = b;
        onTeleportRayActivated?.Invoke(b);
    }
}