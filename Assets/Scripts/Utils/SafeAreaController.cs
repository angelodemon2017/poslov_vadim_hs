using System.Collections.Generic;
using UnityEngine;


public class SafeAreaController : MonoBehaviour
{
    private const float StartOffsetY = 40f;
    
    private static Rect _lastSafeArea;
    private RectTransform _panel;
    [SerializeField] private List<RectTransform> rectTransforms;
    
    private void Awake()
    {
        Application.targetFrameRate = 60;
        _panel = GetComponent<RectTransform>();
        if (_lastSafeArea == Rect.zero) Refresh();
        else ApplySafeArea(_lastSafeArea);
    }

    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        var safeArea = Screen.safeArea;

        if (safeArea != _lastSafeArea)
            ApplySafeArea(safeArea);
    }

    private void OnRectTransformDimensionsChange()
    {
        Refresh();
    }

    private void ApplySafeArea(Rect r)
    {
        _lastSafeArea = r;
        _lastSafeArea.position = r.y <= 0 ? new Vector2(r.x, r.y) : new Vector2(r.x, r.y + 50 + StartOffsetY);

        var anchorMin = _lastSafeArea.position;
        var anchorMax = _lastSafeArea.position + _lastSafeArea.size;
        
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        if (_panel != null)
        {
            _panel.anchorMin = Clamp(anchorMin);
            _panel.anchorMax = Clamp(anchorMax);
        }

        foreach (var rectTransform in rectTransforms)
        {
            rectTransform.anchorMin = Clamp(anchorMin);
            rectTransform.anchorMax = Clamp(anchorMax);
        }
    }

    private Vector2 Clamp(Vector2 origin) =>
        new (Mathf.Clamp(origin.x, 0F, 1F), Mathf.Clamp(origin.y, 0F, 1F));
}