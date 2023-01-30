using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
public class ChestCompressionTrial : MonoBehaviour
{
    public LiveLineDrawer _GraphScript;
    public GameObject CompressionCube; // The cube we are touching.
    public float rate = 2;
    void Start()
    {
        _GraphScript.rate = rate;
        BeginTrial();
    }
    // to-do this needs more work!
    void BeginTrial()
    {
        _GraphScript.gameObject.SetActive(true);
        _GraphScript.rate = rate;
        _GraphScript.StartGraphs();
    }

    void FinishTrial()
    {
        _GraphScript.gameObject.SetActive(false);
        _GraphScript.ShutdownGraphs();
    }
}
