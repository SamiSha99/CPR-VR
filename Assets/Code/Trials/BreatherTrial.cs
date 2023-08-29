using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatherTrial : MonoBehaviour
{
    public GameObject chest;
    public float chestRiseAmount = 0.15f;
    private bool facingMouth, complete;
    public bool incorrectSecondBreathInterval, chestRaised;
    private float talkDuration, notalkDuration, notalkforgiveness;
    private int breathesGive;
    private float breathNormal;

    public enum BreathResult
    {
        Normal,
        Slow,
        Fast,
        TooLong
    }
    public BreathResult breathResult;

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
        xrEvent.micModifier = CalculateMouthToMouthPosition();
        if(facingMouth && !complete)
        {
            xrEvent.EnableMicRecording();
        }

        Vector3 scale = chest.transform.localScale;
        bool isBreathing = !complete && (xrEvent.isTalking || notalkforgiveness > 0);
        float riseRate = 4.0f*Time.deltaTime;
        breathNormal = Mathf.Clamp01(breathNormal + (isBreathing ? riseRate : -riseRate));
        if(chestRaised && breathNormal < 0.8f) chestRaised = false;
        scale.z = Mathf.Lerp(1.0f, 1.0f + chestRiseAmount, breathNormal);//Mathf.Clamp(scale.z + (!complete && xrEvent.isTalking ? Time.deltaTime : -Time.deltaTime), 1.0f, 1.0f + chestRiseAmount);
        chest.transform.localScale = scale;

        if(complete) return;

        switch(breathesGive)
        {
            case 0:
                if(breathNormal < 0.8f) break;
                breathesGive++;
                chestRaised = true;
                break;
            case 1:
                if(chestRaised) break;
                if(notalkDuration > 0 && xrEvent.isTalking && !incorrectSecondBreathInterval && (notalkDuration <= 0.7f || notalkDuration >= 1.3f))
                {
                    incorrectSecondBreathInterval = true;
                    breathResult = (BreathResult)(notalkDuration <= 0.7f ? 2 : (notalkDuration >= 1.3f ? 1 : 0));
                }
                if(breathNormal < 0.8f) break;
                breathesGive++;
                //OnComplete();
                break;
            default:
                complete = true;
                Util.GetXREvents().DisableMicRecording();
                qm.CompleteCommandGoal("Give_Two_Breaths_Goal_Command");
                DoPenalty();
                Util.Invoke(this, () => CleanUp(), 1.0f);
                break;
        }

        if(xrEvent.isTalking)
        {
            if(talkDuration >= 2.5f)
            {
                incorrectSecondBreathInterval = true;
                breathResult = BreathResult.TooLong;
                OnComplete();
            }
            talkDuration += Time.deltaTime;
            notalkforgiveness = 0.1f;
            notalkDuration = 0;
        }
        else if(notalkforgiveness <= 0)
        {
            talkDuration = 0;
            notalkDuration += Time.deltaTime;
        }
        else
            notalkforgiveness -= Time.deltaTime;
    }
    
    float CalculateMouthToMouthPosition()
    {
        GameObject cam = Util.GetPlayer().GetPlayerCameraObject();
        float range = Vector3.Distance(cam.transform.position, transform.position);
        float maxRange = Util.GetPlayer().transform.FindComponent<PlayerLookAtObject>("XREvents").lookRange;
        return Mathf.Lerp(1.25f, 4.25f, Mathf.Lerp(1, 0, range/maxRange));
    }

    void DoPenalty()
    {
        if(!incorrectSecondBreathInterval) return;
        GameManager gm = GameManager._Instance;
        
        if(breathResult != BreathResult.Normal) QuestManager._Instance.AddQuestToRetry();
        gm.AddExamPenalty("ExamPenalty.Breather" + Enum.GetName(typeof(BreathResult), breathResult), 2);
    }

    void OnComplete()
    {
        QuestManager qm = QuestManager._Instance;
        complete = true;
        Util.GetXREvents().DisableMicRecording();
        qm.CompleteCommandGoal("Give_Two_Breaths_Goal_Command");
        DoPenalty();
        Util.Invoke(this, () => CleanUp(), 1.0f);
    }

    void CleanUp()
    {
        talkDuration = 0;
        notalkDuration = 0;
        breathesGive = 0;
        notalkforgiveness = 0;
        breathNormal = 0;
        Util.GetXREvents().micModifier = 1;
        incorrectSecondBreathInterval = false;
        breathResult = BreathResult.Normal;
        complete = false;
        enabled = false;
        chestRaised = false;
    }
    
}
