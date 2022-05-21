using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _timer;

    [SerializeField] private Sprite _transitionIcon;
    [SerializeField] private Sprite _wolfTimeoutIcon;
    
    [SerializeField] private string _transitionTitle;
    [SerializeField] private string _wolfTimeoutTitle;

    public static UITimer Instance;
    
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);

        Instance = this;
    }

    public void StartTransitionCooldown(float time)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer(time, _transitionTitle, _transitionIcon));
    }

    public void StartWolfTimeout(float time)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer(time, _wolfTimeoutTitle, _wolfTimeoutIcon));
    }

    private IEnumerator StartTimer(float time, string title, Sprite icon)
    {
        _canvas.enabled = true;
        _icon.sprite = icon;
        _title.text = title;
        _timer.text = Mathf.RoundToInt(time).ToString();

        float timeLeft = time;

        while (timeLeft > 0)
        {
            yield return new WaitForEndOfFrame();
            timeLeft -= Time.deltaTime;
            _timer.text = Mathf.RoundToInt(timeLeft).ToString();
        }

        _canvas.enabled = false;
    }
}
