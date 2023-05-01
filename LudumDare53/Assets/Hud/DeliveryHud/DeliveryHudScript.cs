using System;
using TMPro;
using UnityEngine;

public class DeliveryHudScript : MonoBehaviour
{
    [SerializeField] RectTransform boxBackground;
    [SerializeField] RectTransform textCount;
    private TextMeshProUGUI tmp;

    private int _lastTotalBoxes;
    private float time;
    private bool runAnim;

    private Vector2 startSizeDelta;
    private IGameState gameState;

    private void Start()
    {
        startSizeDelta = boxBackground.sizeDelta;
        tmp = textCount.GetComponent<TextMeshProUGUI>();

        gameState = GameManager.GetGameState();
        gameState.BoxCountChanged += GameState_BoxCountChanged;
    }

    private void GameState_BoxCountChanged()
    {
        if (_lastTotalBoxes != gameState.TotalBoxes)
        {
            _lastTotalBoxes = gameState.TotalBoxes;
            StartAnimation();
        }
    }


    void Update()
    {
        if (runAnim)
        {
            time += Time.deltaTime;
            UpdateScale();
        }

    }
    private void StartAnimation()
    {
        time = 0;
        runAnim = true;
        tmp.text = _lastTotalBoxes.ToString();
    }

    private void UpdateScale()
    {
        var t = time * 3f;
        var sizeDelta = startSizeDelta;

        t = Math.Clamp(t, 0, 1);
        sizeDelta.x = startSizeDelta.x * EaseWidth(t);
        sizeDelta.y = startSizeDelta.y * EaseHeight(t);
        boxBackground.sizeDelta = sizeDelta;

        var anchoredPosition = textCount.anchoredPosition;
        anchoredPosition.y = Mathf.Lerp(-16, 16, t);
        textCount.anchoredPosition = anchoredPosition;

        if (t >= 1)
        {
            runAnim = false;
        }
    }

    private float EaseWidth(float t)
    {
        var sinVal = -Mathf.Sin((2 * Mathf.PI * t) - Mathf.PI);
        var result = 1 + (sinVal / 2);
        return result;
    }

    private float EaseHeight(float t)
    {
        var sinVal = -Mathf.Sin((2 * Mathf.PI * t));
        var result = 1 + (sinVal / 2);
        return result;
    }
}
