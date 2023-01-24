using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
// Constantly make a line using values passed
public class LiveLineDrawer : MonoBehaviour
{
    public float persistenceTime = 5;
    public int samplesPerSecond = 30;
    private float nextSample;
    public UILineRendererList _UILineRenderer;
    public float wobbleAmount = 1;
    private bool usingRelativeSize;
    float currentTime;
    void Start()
    {
        if(_UILineRenderer == null) _UILineRenderer = GetComponent<UILineRendererList>();
        if(_UILineRenderer != null)
        {
            usingRelativeSize = _UILineRenderer.RelativeSize;
            nextSample = 1.0f/samplesPerSecond;
        }
    }
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        float y = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * currentTime * wobbleAmount - 0.5f * Mathf.PI);
        if(nextSample > 0)
        {
            nextSample = Mathf.Max(0.0f, nextSample - Time.deltaTime);
            if(nextSample <= 0)
            {
                AddPoint(0, y);
                nextSample = 1.0f/samplesPerSecond;
            }
        }
        MovePoints();
    }

    void AddPoint(float x, float y) => _UILineRenderer.Points.Add(new Vector2(x, y));
    
    void MovePoints()
    {
        for(int i = 0; i < _UILineRenderer.Points.Count; i++)
        {
            _UILineRenderer.Points[i] = new Vector2(_UILineRenderer.Points[i].x + Time.deltaTime/persistenceTime, _UILineRenderer.Points[i].y);
            if(_UILineRenderer.Points[i].x >= 1.0f)
            {
                _UILineRenderer.Points.RemoveAt(0);
                i--;
            }
        }
        _UILineRenderer.SetVerticesDirty();
    }
}
