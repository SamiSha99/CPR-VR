using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class LiveLineDrawer : MonoBehaviour
{
    public int samplesPerSecond = 50;
    public UILineRendererList _UILineRendererPlayer, _UILineRendererDemonstration;
    public ProgressBar _CompressionTimerBar;
    public TextMeshProUGUI _CompressionAverageText, _TimerNumberText, _CompressionDepthText;
    [HideInInspector] public float rate = 2;
    [Range(0.0f, 1.0f)] public float value = 0.5f;
    public AudioClip GuidanceSound;
    private float currentTime, nextClipPlayTime, lastCompressionTime, playerDrawerValue, nextSample, lerpRate = 20;
    private int compressionAmount;
    private bool _enabled;
    private ChestCompressionTrial _ChestCompressionTrial;

    const int PERFECT_CHEST_COMPRESSION_PER_MINUTE = 110;
    const int PERFECT_CHEST_COMPRESSION_DEPTH_INCHES = 2;
    const string TIMER_FORMAT = "f1";
    
    void Awake()
    {
        gameObject.SetActive(false);
        enabled = false;
        //StartGraphs(null);
    }
    void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLanguageChanged;
    }
    void OnDestroy()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLanguageChanged;
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
            AudioSource.PlayClipAtPoint(GuidanceSound, Util.GetPlayer().GetPlayerCameraObject().transform.position, 0.35f);
        }
        
        if(PlayerStartedCompressing())
        {
            _CompressionTimerBar.AddProgressBar(-Time.deltaTime);
            _TimerNumberText.text = _CompressionTimerBar.GetProgressBarValue().ToString(TIMER_FORMAT);
            if (_CompressionTimerBar.IsEmpty()) ShutdownGraphs();
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
        compressionAmount = 0;
        lastCompressionTime = 0;
        _CompressionAverageText.gameObject.SetActive(true);
        _CompressionDepthText.gameObject.SetActive(true);
        _CompressionTimerBar.gameObject.SetActive(true);
        _CompressionTimerBar.SetProgressBarClampValues(0, cct != null ? cct._TrialDuration : ChestCompressionTrial.DEFAULT_TIME_TRIAL);
        _CompressionTimerBar.FillProgressBar();
        _TimerNumberText.text = (cct != null ? cct._TrialDuration : ChestCompressionTrial.DEFAULT_TIME_TRIAL).ToString(TIMER_FORMAT);
        _ChestCompressionTrial = cct;
        SetCompressionText(PERFECT_CHEST_COMPRESSION_PER_MINUTE);
    }
    public void OnCompressionRecieved()
    {
        int avgChestCompression;
        // Player started
        //if(!PlayerStartedCompressing())
        //{
        //    lastCompressionTime = currentTime;
        //}
        avgChestCompression = (PlayerStartedCompressing() ? Mathf.RoundToInt(1/(currentTime - lastCompressionTime) * 60) : PERFECT_CHEST_COMPRESSION_PER_MINUTE);
        lastCompressionTime = currentTime;
        SetCompressionText(avgChestCompression);
        
        compressionAmount++;
    }
    public void OnCompressionDepthRecived(float amount)
    {
        SetCompressionDepthText(amount);
    }
    public void OnCompressionGraphInfo(float value) => this.value = value;

    public void ShutdownGraphs()
    {
        _UILineRendererDemonstration.ClearPoints();
        _UILineRendererPlayer.ClearPoints();
        currentTime = 0;
        _enabled = false;
        nextSample = 1.0f/samplesPerSecond;
        lastCompressionTime = 0;
        nextClipPlayTime = 0;
        _CompressionAverageText.gameObject.SetActive(false);
        _CompressionDepthText.gameObject.SetActive(false);
        _CompressionTimerBar.gameObject.SetActive(false);
        _ChestCompressionTrial.OnTrialFinish(compressionAmount);
        compressionAmount = 0;
        gameObject.SetActive(false);
    }

    void SetCompressionText(int amount)
    {
        if(_CompressionAverageText == null) return;
        GameManager gm = GameManager._Instance;
        QuestManager q = QuestManager._Instance;
        if(amount < 100)
        {
            gm.AddExamPenalty("ExamPenalty.CPRSlow", 0.1f);
        }
        else if(amount >= 130)
        {
            gm.AddExamPenalty("ExamPenalty.CPRFast", 0.1f);
        }

        LocalizationHelper.LocalizeTMP($"{amount} cc/m", _CompressionAverageText);
    }
    void SetCompressionDepthText(float amount)
    {
        if(_CompressionDepthText == null) return;
        bool useCentimeter = SettingsUtility.ShouldUseCentimeter();
        if(amount >= 2.375f || amount <= 1.5f)
        {
            GameManager gm = GameManager._Instance;
            gm.AddExamPenalty(amount >= 2.375f ? "ExamPenalty.CPRPressHigh" : "ExamPenalty.CPRPressLow", 0.1f);   
        }
        if (useCentimeter) amount *= Util.INCH_TO_CENTIMETER;
        LocalizationHelper.LocalizeTMP($"{Mathf.Round(amount)}" + (useCentimeter ? "cm" : " inch"), _CompressionDepthText);
    }
    // 0 implies that we haven't done our first compression, this happens when _UILineRendererPlayer graph hits bottom as 0 value
    public bool PlayerStartedCompressing() { return compressionAmount != 0; }

    void OnLanguageChanged(Locale selectedLanguage)
    {
        LocalizationHelper.LocalizeTMP(_CompressionDepthText.text, _CompressionDepthText);
        LocalizationHelper.LocalizeTMP(_CompressionAverageText.text, _CompressionAverageText);
        
        if(!LocalizationHelper.UsingRightToLeftLanguage()) return;
        _UILineRendererPlayer.gameObject.GetComponent<RectTransform>();
    }
}
