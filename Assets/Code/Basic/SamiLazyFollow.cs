using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;

// Overrides rotation with a custom look at target.
// Rotation/Quaternion variables are useless.
public class SamiLazyFollow : LazyFollow
{
    protected override bool TryGetThresholdTargetRotation(out Quaternion newTarget)
    {
        transform.LookAt(2 * transform.position - target.position);
        newTarget = Quaternion.identity;
        return false;
    }
}
