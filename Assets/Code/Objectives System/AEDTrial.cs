using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AEDTrial : MonoBehaviour
{
    private int appliedPatches;
    public AudioClip analyzingNow, shockRequired, noShockRequired, pressButton, beepBeep, aedShockEffect;
    public UnityEvent _ButtonEnabled;
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
    void OnAllPatchesApplied()
    {
        appliedPatches = 0;
        AudioSource.PlayClipAtPoint(analyzingNow, transform.position);
        Util.Invoke(this, () => BeepBeep(), 1.5f);
        Util.Invoke(this, () => BeepBeep(), 4.0f);
        Util.Invoke(this, () => OnWaitingForAED(), 5.0f);
    }

    void OnWaitingForAED()
    {
        if(!GameManager._Instance.isExam || Random.Range(0.0f, 1.0f) > 0.25f)
        {
            QuestManager._Instance.CompleteCommandGoal("Wait_for_AED_Goal_Command");
            Util.Invoke(this, () => OnShockIsRequired(), 0.5f);
        }
        else
        {
            AudioSource.PlayClipAtPoint(noShockRequired, transform.position);
            Util.Invoke(this, () => QuestManager._Instance.ForceCompleteQuest(), 2.5f);
        }
    }

    void BeepBeep() => AudioSource.PlayClipAtPoint(beepBeep, transform.position);
    void OnShockIsRequired()
    {
        AudioSource.PlayClipAtPoint(shockRequired, transform.position);
        Util.Invoke(this, () => OnButtonEnabled(), 3.25f);
    }

    void OnButtonEnabled()
    {
        AudioSource.PlayClipAtPoint(pressButton, transform.position);
        
        _ButtonEnabled?.Invoke();
    }
    // Interacted with the button -> apply shock
    public void OnButtonPressed()
    {
        AudioSource.PlayClipAtPoint(aedShockEffect, transform.position);
        Util.Invoke(this, () => QuestManager._Instance.CompleteCommandGoal("Press_Shock_Button_Goal_Command"), 3.0f);
    }
}
