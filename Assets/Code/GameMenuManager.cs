using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class GameMenuManager : MonoBehaviour
{
    public static GameMenuManager _Instance;
    public static event Action<bool> OnMenuButtonPressed;
    public GameEventCommand OnPause;
    public GameObject pauseMenuPrefab, pauseMenuObject;
    public InputActionProperty pauseButton;
    public bool isPaused;
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
            TogglePause();
    }

    public void TogglePause()
    {
        if(isPaused) UnPause();
        else Pause();
        OnMenuButtonPressed?.Invoke(isPaused);
    }

    public void Pause()
    {
        previousTimeScale = Time.timeScale;
        Time.timeScale = 0;
        AudioListener.pause = true;
        isPaused = true;
        InstantiatePauseMenu();
        OnPause?.TriggerEvent();
    }

    public void UnPause()
    {
        if(pauseMenuObject != null) Destroy(pauseMenuObject);
        Time.timeScale = previousTimeScale;
        AudioListener.pause = false;
        isPaused = false;
        OnPause?.TriggerEvent("unpause");
    }

    void InstantiatePauseMenu()
    {
        if(pauseMenuPrefab != null) 
            pauseMenuObject = Instantiate(pauseMenuPrefab);
    }

    public bool IsPaused() { return isPaused; }   
}
