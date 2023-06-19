using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Events;
public class VideoManager : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public UnityEvent onVideoEnding;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoPlayer_LoopPointReached;
        videoPlayer.prepareCompleted += VideoPlayer_PrepareCompleted;
        GameMenuManager.OnMenuButtonPressed += GameMenuManager_OnMenuButtonPressed;
    }
    public void Play() => videoPlayer.Play(); 
    public void Stop() => videoPlayer.Stop();
    public void Pause() => videoPlayer.Pause();
    public void URLToVideo(string url)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = url;
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += VideoPlayer_PrepareCompleted;
    }

    // Insert normalized value between 0 - 1
    public void SetTime(Slider slider)
    {
        float value = Mathf.Clamp01(slider.value);
        videoPlayer.frame = (long)(value * videoPlayer.frameCount);
    }

    private void VideoPlayer_PrepareCompleted(VideoPlayer source)
    {
        Play();
    }
    private void VideoPlayer_LoopPointReached(VideoPlayer source)
    {
        Stop();
        onVideoEnding?.Invoke();
    }
    private void GameMenuManager_OnMenuButtonPressed(bool pause)
    {
        if(pause) Pause();
        else Play();
    }
    void OnDestroy()
    {
        GameMenuManager.OnMenuButtonPressed -= GameMenuManager_OnMenuButtonPressed;
    }
}
