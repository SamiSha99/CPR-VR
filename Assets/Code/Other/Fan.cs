using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : MonoBehaviour
{
    public float _RotationDegreeSpeed = 360; // In degrees
    public bool _ShouldSpin = true;
    void Update()
    {
        if (!_ShouldSpin) return;
        transform.Rotate(0, Time.deltaTime * _RotationDegreeSpeed, 0);
    }
}
