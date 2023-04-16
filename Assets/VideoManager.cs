using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoManager : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += VideoPlayer_LoopPointReached;
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
    private void VideoPlayer_PrepareCompleted(VideoPlayer source)
    {
        Play();
    }
    private void VideoPlayer_LoopPointReached(VideoPlayer source)
    {

    }
    
}
