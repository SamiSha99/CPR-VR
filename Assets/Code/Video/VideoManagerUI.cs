using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Video;

public class VideoManagerUI : MonoBehaviour
{
    public VideoManager videoManager;
    public Slider timePosition;
    public TextMeshProUGUI currentTimeText;

    void Update() => UpdateTime();
    void UpdateTime()
    {
        if(videoManager == null) return;
        VideoPlayer vp = videoManager.videoPlayer;
        float currentTime = (float)videoManager.videoPlayer.frame/videoManager.videoPlayer.frameRate;
        float maxDuration = (float)videoManager.videoPlayer.frameCount/videoManager.videoPlayer.frameRate;
        if(timePosition != null)
            timePosition.SetValueWithoutNotify((float)videoManager.videoPlayer.frame/videoManager.videoPlayer.frameCount);
        currentTimeText.text = $"{GetTimeInMinSecs(currentTime)} / {GetTimeInMinSecs(maxDuration)}";
    }

    private string GetTimeInMinSecs(float seconds)
    {
        TimeSpan span = TimeSpan.FromSeconds(Mathf.RoundToInt(seconds));
        String timeStr = string.Format("{0}:{1:00}", (int)span.TotalMinutes, span.Seconds);
        return timeStr;
    }
}
