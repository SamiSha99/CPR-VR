using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatherTrial : MonoBehaviour
{
    public GameObject chest;
    public float chestRiseAmount = 0.15f;
    private bool facingMouth, lastTalk, complete;
    public bool incorrectSecondBreathInterval;
    private float talkDuration, notalkDuration, notalkforgiveness;
    private int breathesGive;

    public enum BreathResult
    {
        Normal,
        Slow,
        Fast
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
            {
                notalkforgiveness += Time.deltaTime;
                if(talkDuration >= 0.25f)
                {
                    talkDuration = 0;
                    notalkforgiveness = 0;
                }
            }
        }
        lastTalk = xrEvent.isTalking;

        if(!lastTalk)
            notalkDuration += Time.deltaTime;
        else if(breathesGive == 1 && (notalkDuration >= 0.15f && notalkDuration <= 0.7f || notalkDuration >= 1.3f))
        {
            incorrectSecondBreathInterval = true;
            breathResult = (BreathResult)(notalkDuration <= 0.7f ? 2 : (notalkDuration >= 1.3f ? 1 : 0));
        }
    }
    
    float CalculateMouthToMouthPosition()
    {
        GameObject cam = Util.GetPlayer().GetPlayerCameraObject();
        float range = Vector3.Distance(cam.transform.position, transform.position);
        float maxRange = Util.GetPlayer().transform.FindComponent<PlayerLookAtObject>("XREvents").lookRange;
        return Mathf.Lerp(1.0f, 4.0f, Mathf.Lerp(1, 0, range/maxRange));
    }

    void DoPenalty()
    {
        if(!incorrectSecondBreathInterval) return;
        GameManager gm = GameManager._Instance;
        switch(breathResult)
        {
            case BreathResult.Slow: gm.AddExamPenalty("ExamPenalty.BreatherSlow", 2); QuestManager._Instance.AddQuestToRetry(); break;
            case BreathResult.Fast: gm.AddExamPenalty("ExamPenalty.BreatherFast", 2); QuestManager._Instance.AddQuestToRetry(); break;
            default: break;
        };
    }

    void CleanUp()
    {
        talkDuration = 0;
        notalkDuration = 0;
        breathesGive = 0;
        notalkforgiveness = 0;
        Util.GetXREvents().micModifier = 1;
        lastTalk = false;
        incorrectSecondBreathInterval = false;
        breathResult = BreathResult.Normal;
        complete = false;
        enabled = false;
    }
    
}
