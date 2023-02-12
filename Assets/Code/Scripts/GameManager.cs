using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager _Instance;
    public List<Quest> TutorialQuestsLine;
    public List<Quest> ExamQuestsLine; // We do this step by step to examinate how fast/slow and effecient the player is and we add score
    public bool isExam;
    public float score, maxScore;
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
