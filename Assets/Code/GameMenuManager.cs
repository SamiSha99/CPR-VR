using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager _Instance;
    public GameObject pauseMenuPrefab, pauseMenuObject;
    public InputActionProperty pauseButton;
    public bool isPaused { get; set; }
    public float previousTimeScale = -1;

    void Awake() => _Instance = this;    
    void Start()
    {
        previousTimeScale = Time.timeScale;
    }

    void Update()
    {
        if(pauseButton == null) return;

        if(pauseButton.action.WasPerformedThisFrame())
        {
            TogglePause(!isPaused);
        }
    }

    void TogglePause(bool pause)
    {
        Util.Print<GameMenuManager>("Paused: " + pause);
        if(pause) Pause();
        else UnPause();
    }

    void Pause()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
        InstantiatePauseMenu();
    }

    void UnPause()
    {
        if(pauseMenuObject != null) Destroy(pauseMenuObject);
        Time.timeScale = previousTimeScale;
        AudioListener.pause = false;
        isPaused = false;
    }

    void InstantiatePauseMenu()
    {
        if(pauseMenuPrefab != null) 
            pauseMenuObject = Instantiate(pauseMenuPrefab);
    }

    public bool IsPaused() { return isPaused; }   
}
