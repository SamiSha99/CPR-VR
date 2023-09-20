using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FailedConditions
{
    [HideInInspector] public string name;
    [NonReorderable] public List<string> conditions;
    public float amount;

    public void Validate()
    {
        name = $"{conditions.Count} condition" + (conditions.Count > 1 ? "s" : "") + " | >= " + amount + "";
    }
}
