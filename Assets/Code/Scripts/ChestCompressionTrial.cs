using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
public class ChestCompressionTrial : MonoBehaviour
{
    public LiveLineDrawer _GraphScript;
    public GameObject _GraphCanvas;
    public float rate = 2;
    public void BeginTrial()
    {
        _GraphCanvas.gameObject.SetActive(true);
        _GraphScript.gameObject.SetActive(true);
        _GraphScript.rate = rate;
        _GraphScript.StartGraphs();
    }

    public void FinishTrial()
    {
        _GraphCanvas.gameObject.SetActive(false);
        _GraphScript.gameObject.SetActive(false);
        _GraphScript.ShutdownGraphs();
    }
}
