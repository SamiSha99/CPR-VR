using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class DemonstrationLineDrawer : MonoBehaviour
{
    public int samplesPerSecond = 50;
    private float nextSample;
    public UILineRendererList _UILineRenderer;
    public float wobbleAmount = 4;
    float currentTime;
    void Start()
    {
        if (_UILineRenderer == null) _UILineRenderer = GetComponent<UILineRendererList>();
        if (_UILineRenderer != null) nextSample = 1.0f / samplesPerSecond;
        
    }
    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        float y = 0.5f + 0.5f * Mathf.Sin(Mathf.PI * currentTime * wobbleAmount - 0.5f * Mathf.PI);
        if (nextSample > 0)
        {
            nextSample = Mathf.Max(0.0f, nextSample - Time.deltaTime);
            if (nextSample <= 0)
            {
                AddPoint(0, y);
                nextSample = 1.0f / samplesPerSecond;
                _UILineRenderer.SetVerticesDirty();
                MovePoints();
            }
        }
    }

    void AddPoint(float x, float y) => _UILineRenderer.Points.Add(new Vector2(x, y));

    void MovePoints()
    {
        for (int i = 0; i < _UILineRenderer.Points.Count; i++)
        {
            _UILineRenderer.Points[i] = new Vector2(_UILineRenderer.Points[i].x + 1.0f / samplesPerSecond, _UILineRenderer.Points[i].y);
            if (_UILineRenderer.Points[i].x >= 1.0f)
            {
                _UILineRenderer.Points.RemoveAt(0);
                i--;
            }
        }
    }
}
