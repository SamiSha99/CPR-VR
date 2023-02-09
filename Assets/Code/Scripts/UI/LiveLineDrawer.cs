using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
public class LiveLineDrawer : MonoBehaviour
{
    public int samplesPerSecond = 50;
    private float nextSample;
    public UILineRendererList _UILineRendererPlayer, _UILineRendererDemonstration;
    public ProgressBar _CompressionTimerBar;
    public TextMeshProUGUI _CompressionAverageText;
    [HideInInspector] public float rate = 2;
    [Range(0.0f, 1.0f)] public float value = 0.5f;
    public float lerpRate = 5;
    private float playerDrawerValue;
    public AudioClip GuidanceSound;
    private float currentTime, nextClipPlayTime;
    // Relates to when we started compressing
    private float nextAverageCompresisonUpdate, postCompressionStartTime;
    private float elapsedCompressionTime; // adds 1/rate for performance and accuracy
    private int compressionAmount;

    private bool _enabled;

    void Start()
    {
        if(_UILineRendererPlayer == null || _UILineRendererDemonstration == null) gameObject.SetActive(false);
        StartGraphs();
    }
    void Update()
    {
        if(!_enabled) return;

        playerDrawerValue = Mathf.Lerp(playerDrawerValue, value, Time.deltaTime * lerpRate);
        float y = playerDrawerValue;
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
        // Plays the beep audio to guide the player
        if(currentTime >= nextClipPlayTime)
        {
            nextClipPlayTime = currentTime + 1/rate;
            if(PlayerStartedCompressing())
            {
                elapsedCompressionTime += 1/rate;
                UpdateAverageCompression();
            }
            AudioSource.PlayClipAtPoint(GuidanceSound, GlobalHelper.GetPlayer().GetXRCameraObject().transform.position, 0.5f);
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
        compressionAmount = -1;
        elapsedCompressionTime = 0;
    }
    public void OnCompressionRecieved()
    {
        // Player started
        if(!PlayerStartedCompressing())
        {
            elapsedCompressionTime = 0;
            compressionAmount = 0;
        }
        compressionAmount++;
    }

    public void OnCompressionGraphInfo(float value) => this.value = value;

    public void ShutdownGraphs()
    {
        _UILineRendererDemonstration.ClearPoints();
        _UILineRendererPlayer.ClearPoints();
        currentTime = 0;
        _enabled = false;
        nextSample = 1.0f/samplesPerSecond;
        compressionAmount = -1;
        elapsedCompressionTime = 0;
    }

    public void UpdateAverageCompression()
    {
        _CompressionAverageText.text = $"{compressionAmount/elapsedCompressionTime} cc/s";
    }

    // -1 implies that we haven't done our first compression, this happens when _UILineRendererPlayer graph hits bottom as 0 value
    public bool PlayerStartedCompressing() { return compressionAmount != -1; }
    
}
