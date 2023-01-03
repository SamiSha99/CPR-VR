using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideObject : MonoBehaviour
{
    [Tooltip("Hide the gameobject on run time, assuming we are handling it somewhere else.")]
    public bool _hidden;
    void Awake() { if(_hidden) Hide(); }
    public void Hide() => SetHidden(true);
    public void Unhide() => SetHidden(false);
    public void SetHidden(bool _hidden = false)
    {
        transform.ToggleHidden(_hidden);
        this._hidden = _hidden;
    }
    public bool IsHidden() { return _hidden; }

    void OnValidate() => SetHidden(_hidden);
}
