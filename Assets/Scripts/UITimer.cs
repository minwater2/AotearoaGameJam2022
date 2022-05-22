using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UITimer : MonoBehaviour
{
    [SerializeField] private GameObject _transitionGO;
    [SerializeField] private GameObject _wolfAttackGO;
    [SerializeField] private GameObject _shepardAttackGO;
    [SerializeField] private GameObject _wolfTimeoutGO;
    
    [SerializeField] private Image _transitionIcon;
    [SerializeField] private Image _wolfAttackIcon;
    [SerializeField] private Image _shepardAttackIcon;
    [SerializeField] private Image _wolfTimeoutIcon;

    private Coroutine _timeoutCoroutine;

    public static UITimer Instance;
    
    private void Awake()
    {
        if (Instance != null) Destroy(Instance);

        Instance = this;
    }

    public void Init(bool isWolf)
    {
        if (isWolf)
        {
            _transitionGO.SetActive(true);
            _wolfAttackGO.SetActive(true);
            _wolfTimeoutGO.SetActive(true);
        }
        else
        {
            _shepardAttackGO.SetActive(true);
        }
    }

    public void StartTransitionCooldown(float time)
    {
        StartCoroutine(StartTimer(time, _transitionIcon));
    }

    public void StartWolfTimeout(float time)
    {
        _timeoutCoroutine = StartCoroutine(StartTimer(time, _wolfTimeoutIcon));
    }

    public void StopWolfTimeout()
    {
        StopCoroutine(_timeoutCoroutine);
        _wolfTimeoutIcon.fillAmount = 1f;
    }

    public void StartWolfAttackTimeout(float time)
    {
        StartCoroutine(StartTimer(time, _wolfAttackIcon));
    }

    public void StartShepardAttackTimeout(float time)
    {
        StartCoroutine(StartTimer(time, _shepardAttackIcon));
    }

    private IEnumerator StartTimer(float totalTime, Image icon)
    {
        float timeLeft = totalTime;

        while (timeLeft > 0)
        {
            yield return new WaitForEndOfFrame();
            timeLeft -= Time.deltaTime;
            icon.fillAmount = (totalTime - timeLeft) / totalTime;
        }
    }
}
