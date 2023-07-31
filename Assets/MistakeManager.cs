using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MistakeManager : MonoBehaviour
{
    public static MistakeManager _Instance;
    public GameObject mistakePrefab;
    public float duration = 6;
    [HideInInspector] public GameObject activeMistake;
    void Awake() => _Instance = this;
    public void OnMistakeRecieved(string localization)
    {
        if(mistakePrefab == null) return;

        if(activeMistake != null) Destroy(activeMistake);
        activeMistake = Instantiate(mistakePrefab);
        if(activeMistake != null)
        {
            if(activeMistake.HasComponent<MistakePrompt>(out MistakePrompt mp)) mp.DoLocalization(localization);
            Destroy(activeMistake, duration);
        }
    }
}
