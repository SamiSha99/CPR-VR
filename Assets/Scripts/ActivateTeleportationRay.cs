using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class ActivateTeleportationRay : MonoBehaviour
{
    public GameObject leftTeleportation, rightTeleportation;
    public InputActionProperty leftActivate, rightActivate;

    // Update is called once per frame
    void Update()
    {
        if(leftTeleportation != null && leftActivate != null)
        {
            leftTeleportation.SetActive(leftActivate.action.ReadValue<float>() > 0.1f);
        }

        if(rightTeleportation != null && rightActivate != null)
        {
            rightTeleportation.SetActive(rightActivate.action.ReadValue<float>() > 0.1f);
        }
    }
    
    void Start()
    {
        
    }

    public void TestEvent()
    {
        Debug.Log("EVENT HAS BEEN TRIGGERED FOR COLLIDING WITH TELEPORTER!!! IN ActivateTeleportationRay");
    }

}