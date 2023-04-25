using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLineManager : MonoBehaviour
{
    public static QuestLineManager _Instance;
    
    [Serializable]
    public struct QuestLineModule
    {
        public Quest quest;
        public string localizationAudio;
    };

    public QuestLineModule[] _TutorialQuestLine;
    public List<UnityEngine.Object> default_ExamQuestsLine;
    void Awake() => _Instance = this;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
