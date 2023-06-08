using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatherTrial : MonoBehaviour
{
    public GameObject chest;
    public float chestRiseAmount = 0.15f;
    private bool facingMouth, lastTalk, complete;
    public bool incorrectSecondBreathInterval, tooFast, tooSlow;
    private float talkDuration, notalkDuration;
    private int breathesGive;

    void OnDisable()
    {
        Util.GetXREvents().DisableMicRecording();
    }

    void OnEnable()
    {
        //Util.GetXREvents().EnableMicRecording();
    }

    void Start()
    {
        Vector3 lTemp = chest.transform.localScale;
        chest.transform.localScale = lTemp;
    }

    void Update()
    {
        QuestManager qm = QuestManager._Instance;
        facingMouth = qm.IsQuestGoalCompleted("Face_Mouth");
        XREvents xrEvent = Util.GetXREvents();
        
        if(facingMouth && !complete)
        {
            xrEvent.EnableMicRecording();
        }

        Vector3 scale = chest.transform.localScale;
        scale.z = Mathf.Clamp(scale.z + (!complete && xrEvent.isTalking ? Time.deltaTime : -Time.deltaTime), 1.0f, 1.0f + chestRiseAmount);
        chest.transform.localScale = scale;

        if(complete) return;

        if(!facingMouth)
        {
            //enabled = false;
            lastTalk = false;
            talkDuration = 0;
            return;
        }
        
        if(xrEvent.isTalking)
        {
            if(lastTalk)
            {
                if(talkDuration + Time.deltaTime >= 0.15f && talkDuration <= 0.15f) breathesGive++;
                if(breathesGive >= 2)
                {
                    complete = true;
                    Util.GetXREvents().DisableMicRecording();
                    qm.CompleteCommandGoal("Give_Two_Breaths_Goal_Command");
                    DoPenalty();
                    Util.Invoke(this, () => CleanUp(), 1.0f);
                }
                talkDuration += Time.deltaTime;
                notalkDuration = 0;
            }
            else
                talkDuration = 0;
        }
        lastTalk = xrEvent.isTalking;

        if(!lastTalk)
            notalkDuration += Time.deltaTime;
        else if(breathesGive == 1 && (notalkDuration <= 0.75f || notalkDuration >= 1.25f))
        {
            incorrectSecondBreathInterval = true;
            tooFast = notalkDuration <= 0.75f;
            tooSlow = notalkDuration >= 1.25f;
        }
    }

    void DoPenalty()
    {
        if(!incorrectSecondBreathInterval) return;
        GameManager gm = GameManager._Instance;
        if(tooFast) gm.AddExamPenalty("ExamPenalty.BreatherFast", 2);
        if(tooSlow) gm.AddExamPenalty("ExamPenalty.BreatherSlow", 2);
    }

    void CleanUp()
    {
        talkDuration = 0;
        notalkDuration = 0;
        breathesGive = 0;
        lastTalk = false;
        incorrectSecondBreathInterval = false;
        tooSlow = false;
        tooFast = false;
        complete = false;
        enabled = false;
    }
    
}
