using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreatherTrial : MonoBehaviour
{
    public GameObject chest;
    public float chestRiseAmount = 0.15f;
    public bool facingMouth, lastTalk, incorrectSecondBreathInterval;
    public float talkDuration, notalkDuration;
    public int breathesGive;

    void OnDisable()
    {
        Util.GetXREvents().DisableMicRecording();
    }

    void OnEnable()
    {
        Util.GetXREvents().EnableMicRecording();
    }

    void Update()
    {
        QuestManager qm = QuestManager._Instance;
        facingMouth = qm.IsQuestGoalCompleted("Face_Mouth");
        if(!facingMouth)
        {
            //enabled = false;
            lastTalk = false;
            talkDuration = 0;
            return;
        }



        XREvents xrEvent = Util.GetXREvents();
        
        // cannot apply scale???
        //Vector3 scale = chest.transform.localScale;
        //scale.z = Mathf.Clamp(scale.z + Time.deltaTime, 1.0f, 1.0f + chestRiseAmount);
        //chest.transform.localScale.Set(scale.x, scale.y, 55);
        
        if(xrEvent.isTalking)
        {
            if(lastTalk)
            {
                if(talkDuration + Time.deltaTime >= 0.15f && talkDuration <= 0.15f) breathesGive++;
                if(breathesGive >= 2)
                {
                    qm.CompleteCommandGoal("Give_Two_Breaths_Goal_Command");
                    CleanUp();
                    enabled = false;
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
            incorrectSecondBreathInterval = true;
    }

    void CleanUp()
    {
        talkDuration = 0;
        lastTalk = false;
        notalkDuration = 0;
        incorrectSecondBreathInterval = false;
    }
    
}
