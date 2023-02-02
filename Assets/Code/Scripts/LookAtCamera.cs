using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private Transform _XRCameraTransform;
    void Update()
    {
        if(_XRCameraTransform == null) _XRCameraTransform = GlobalHelper.GetPlayer()?.GetXRCameraObject().transform;
        if(_XRCameraTransform == null) return; // Failed to get the camera abort.
        transform.LookAt(2 * transform.position - _XRCameraTransform.position);
    }
}
