using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
// Draws 2 different lines, one is guidance, the other is the player, drawing is "sampling" points every second, this is done to avoid calling SetAllDirty in the graphics
// class being called so much to a point of Unity literally SHITTING itself, could it be better? Yes! Is it needed? NO!!! IT WORKS!!!
public class LiveLineDrawer : MonoBehaviour
{
    public int samplesPerSecond = 50;
    private float nextSample;
    public UILineRendererList _UILineRendererPlayer, _UILineRendererDemonstration;
    [HideInInspector] public float rate = 2;
    [Range(0.0f, 1.0f)]
    public float value = 0.5f;
    public AudioClip GuidanceSound;
    private float currentTime, nextClipPlayTime;
    private bool _enabled;
    void Start()
    {
        if(_UILineRendererPlayer == null || _UILineRendererDemonstration == null) gameObject.SetActive(false);
        StartGraphs();
    }
    void Update()
    {
        if(!_enabled) return;

        float y = value;
        float demoY = 0.5f + 0.5f * Mathf.Sin(2 * Mathf.PI * currentTime * rate - 0.5f * Mathf.PI);
        currentTime += Time.deltaTime;
        if(nextSample > 0)
        {
            nextSample = Mathf.Max(0.0f, nextSample - Time.deltaTime);
            if(nextSample <= 0)
            {
                _UILineRendererPlayer.AddPoint(new Vector2(0, y));
                _UILineRendererDemonstration.AddPoint(new Vector2(0, demoY));
                nextSample = 1.0f/samplesPerSecond;
                MovePoints();
            }
        }
        if(currentTime >= nextClipPlayTime)
        {
            nextClipPlayTime = currentTime + 1/rate;
            AudioSource.PlayClipAtPoint(GuidanceSound, GlobalHelper.GetPlayer().GetXRCameraObject().transform.position);
        }
    }
    
    void MovePoints()
    {
        for(int i = 0; i < _UILineRendererPlayer.Points.Count; i++)
        {
            _UILineRendererPlayer.Points[i] = new Vector2(_UILineRendererPlayer.Points[i].x + 1.0f / samplesPerSecond, _UILineRendererPlayer.Points[i].y);
            if(_UILineRendererPlayer.Points[i].x >= 1.0f)
            {
                _UILineRendererPlayer.Points.RemoveAt(0);
                i--;
            }
        }

        for(int i = 0; i < _UILineRendererDemonstration.Points.Count; i++)
        {
            _UILineRendererDemonstration.Points[i] = new Vector2(_UILineRendererDemonstration.Points[i].x + 1.0f / samplesPerSecond, _UILineRendererDemonstration.Points[i].y);
            if(_UILineRendererDemonstration.Points[i].x >= 1.0f)
            {
                _UILineRendererDemonstration.Points.RemoveAt(0);
                i--;
            }
        }
    }

    public void StartGraphs()
    {
        currentTime = 0;
        _enabled = true;
        nextSample = 1.0f/samplesPerSecond;
    }

    public void ShutdownGraphs()
    {
        _UILineRendererDemonstration.ClearPoints();
        _UILineRendererPlayer.ClearPoints();
        currentTime = 0;
        _enabled = false;
        nextSample = 1.0f/samplesPerSecond;
    }
}
