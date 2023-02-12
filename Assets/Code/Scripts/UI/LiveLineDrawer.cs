using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
public class LiveLineDrawer : MonoBehaviour
{
    public int samplesPerSecond = 50;
    public UILineRendererList _UILineRendererPlayer, _UILineRendererDemonstration;
    public ProgressBar _CompressionTimerBar;
    public TextMeshProUGUI _CompressionAverageText;
    [HideInInspector] public float rate = 2;
    [Range(0.0f, 1.0f)] public float value = 0.5f;
    public AudioClip GuidanceSound;
    private float currentTime, nextClipPlayTime, lastCompressionTime, playerDrawerValue, nextSample, lerpRate = 20;
    private int compressionAmount;
    private bool _enabled;
    private ChestCompressionTrial _ChestCompressionTrial;
    const int PERFECT_CHEST_COMPRESSION_PER_MINUTE = 110;
    void Start()
    {
        if(_UILineRendererPlayer == null || _UILineRendererDemonstration == null) gameObject.SetActive(false);
        //StartGraphs(null);
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
            AudioSource.PlayClipAtPoint(GuidanceSound, Util.GetPlayer().GetXRCameraObject().transform.position, 0.35f);
        }
        
        if(PlayerStartedCompressing())
        {
            _CompressionTimerBar.AddProgressBar(-Time.deltaTime);
            if(_CompressionTimerBar.IsEmpty()) ShutdownGraphs();
        }
    }
    
    void MovePoints()
    {
        Vector2 cords;
        for(int i = 0; i < _UILineRendererPlayer.Points.Count; i++)
        {
            cords = new Vector2(_UILineRendererPlayer.Points[i].x + 1.0f / samplesPerSecond, _UILineRendererPlayer.Points[i].y);
            _UILineRendererPlayer.Points[i] = cords;
            if(_UILineRendererPlayer.Points[i].x >= 1.0f)
            {
                _UILineRendererPlayer.Points.RemoveAt(0);
                i--;
            }
        }

        for(int i = 0; i < _UILineRendererDemonstration.Points.Count; i++)
        {
            cords = new Vector2(_UILineRendererDemonstration.Points[i].x + 1.0f / samplesPerSecond, _UILineRendererDemonstration.Points[i].y);
            _UILineRendererDemonstration.Points[i] = cords;
            if(_UILineRendererDemonstration.Points[i].x >= 1.0f)
            {
                _UILineRendererDemonstration.Points.RemoveAt(0);
                i--;
            }
        }
    }

    public void StartGraphs(ChestCompressionTrial cct)
    {
        currentTime = 0;
        _enabled = true;
        nextSample = 1.0f/samplesPerSecond;
        compressionAmount = -1;
        lastCompressionTime = 0;
        _CompressionAverageText.gameObject.SetActive(true);
        _CompressionTimerBar.gameObject.SetActive(true);
        _CompressionTimerBar.SetProgressBarClampValues(0, cct != null ? cct._TrialDuration : ChestCompressionTrial.DEFAULT_TIME_TRIAL);
        _CompressionTimerBar.FillProgressBar();
        _ChestCompressionTrial = cct;
        SetCompressionText(PERFECT_CHEST_COMPRESSION_PER_MINUTE);
    }
    public void OnCompressionRecieved()
    {
        int avgChestCompression;
        // Player started
        if(!PlayerStartedCompressing())
        {
            lastCompressionTime = currentTime;
            compressionAmount = 0;
        }
        avgChestCompression = (lastCompressionTime != 0 ? Mathf.RoundToInt(1/(currentTime - lastCompressionTime) * 60) : PERFECT_CHEST_COMPRESSION_PER_MINUTE);
        lastCompressionTime = currentTime;
        SetCompressionText(avgChestCompression);
        
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
        lastCompressionTime = 0;
        nextClipPlayTime = 0;
        _CompressionAverageText.gameObject.SetActive(false);
        _CompressionTimerBar.gameObject.SetActive(false);
        _ChestCompressionTrial.OnTrialFinish();
        gameObject.SetActive(false);
    }

    void SetCompressionText(int amount)
    {
        if(_CompressionAverageText == null) return;
        _CompressionAverageText.text = $"{amount} cc/m";
    }

    // -1 implies that we haven't done our first compression, this happens when _UILineRendererPlayer graph hits bottom as 0 value
    public bool PlayerStartedCompressing() { return compressionAmount != -1; }
    
}
