using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AEDTrial : MonoBehaviour
{
    private int appliedPatches;
    //AudioClip ;
    public float analyzerTimer = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPatchApplied()
    {
        appliedPatches++;
        if(appliedPatches >= 2) OnAllPatchesApplied();
        
    }
    // Handled in inspector!!!
    public void OnAllPatchesApplied()
    {
        Util.Invoke(this, () => OnWaitingForAED(), 5.0f);
    }

    public void OnWaitingForAED()
    {
        QuestManager._Instance.CompleteCommandGoal("Wait_for_AED_Goal_Command");
    }
}
