using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Video;
using UnityEngine.EventSystems;

public class VideoManagerUI : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public VideoManager videoManager;
    public Slider timePosition;
    public TextMeshProUGUI currentTimeText;
    public bool dragging;
    void Update() => UpdateTime();
    void UpdateTime()
    {
        if(videoManager == null) return;
        if(dragging) return;
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

    //to-do: need to figure out graphic raycasing, wtf is this crap??
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
    }
}
