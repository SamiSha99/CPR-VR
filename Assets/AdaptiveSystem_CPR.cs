using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdaptiveSystem_CPR : MonoBehaviour
{
    [Min(0)]
    public float fadeInDuration, fadeOutDuration;
    public SkinnedMeshRenderer leftHand, rightHand;
    private float currentAlpha;
    private bool isFadingIn;
    private Vector3 startingPosition;
    public float WaveStrength = 0.2f;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        leftHand.material = new Material(leftHand.material);
        rightHand.material = new Material(rightHand.material);
        startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFadingIn)
            currentAlpha = fadeInDuration <= 0 ? 1 : Mathf.Clamp01(currentAlpha + Time.deltaTime/fadeInDuration);   
        else
            currentAlpha = fadeOutDuration <= 0 ? 0 : Mathf.Clamp01(currentAlpha - Time.deltaTime/fadeOutDuration);   
    }
    
    public void FadeIn(bool immediate = false)
    {
        currentAlpha = immediate ? 1 : 0;
        SetAlpha(currentAlpha);
        isFadingIn = true;
    }

    public void FadeOut(bool immediate = false)
    {
        currentAlpha = immediate ? 0 : 1;
        SetAlpha(currentAlpha);
        isFadingIn = false;
    }
    public void SetAlpha(float amount = 0)
    {
        leftHand.material.SetFloat("_Opacity", amount);
        rightHand.material.SetFloat("_Opacity", amount);
    }

    public void SimulateMovement(float yWaveValue)
    {
        // returns it from 0 - 1 to -1 - 1;
        yWaveValue -= 0.5f;
        yWaveValue *= 2;
        // then we apply the wave correctly
        transform.position = new Vector3(startingPosition.x, startingPosition.y + yWaveValue * WaveStrength, startingPosition.z);
    }
}
