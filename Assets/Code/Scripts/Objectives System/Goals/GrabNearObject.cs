using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assume its currently grabbed and close to the player, in whatever way (using XR Camera)
public class GrabNearObject : Quest.QuestGoal
{
    public enum NearbyType 
    {
        NT_Origin,
        NT_Camera
    };
    [Tooltip("Type of comparison, should it be near the player or near their head?\nReference:\nNT_Origin = XR Origin (Player)\nNT_Camera = XR Camera")]
    public NearbyType compareRangeTo = 0;
    [Tooltip("How close should the object be? (Minimum is 0.01f)")]
    [Min(0.01f)]
    public float _NearbyRange = 0.25f;
    public override void Initialize()
    {
        base.Initialize();
        XREvents.onItemGrabbedNearby += OnObjectNearby;
    }

    private void OnObjectNearby(GameObject o, GameObject instigator)
    {
        if(!objectiveNameList.Contains(o.name)) return;
        if(!InRange(o, instigator)) return;
        currentAmount = requiredAmount;
        Evaluate(true);
    }

    public override void CleanUp()
    {
        base.CleanUp();
        XREvents.onItemGrabbedNearby -= OnObjectNearby;
    }

    private bool InRange(GameObject o, GameObject instigator)
    {
        GameObject target = compareRangeTo switch
        {
            NearbyType.NT_Origin => instigator.GetRoot(),
            NearbyType.NT_Camera => instigator.GetXRCameraObject(),
            _ => null
        };
        if(target == null) return false;

        return Vector3.Distance(o.transform.position, target.transform.position) <= _NearbyRange;
    }
}
