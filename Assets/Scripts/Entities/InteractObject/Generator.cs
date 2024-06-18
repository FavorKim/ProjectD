using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour, IInteractableObject
{
    private float curGauge;
    public float CurGauge
    {
        get { return curGauge; }
        private set
        {
            curGauge = value;
            if (curGauge >= maxGauge)
            {
                OnCompleteHandler();
            }
        }
    }
    [SerializeField] float maxGauge = 100;
    [SerializeField] float Multi_Gauge = 1;
    [SerializeField] Slider Slider_Gauge;
    [SerializeField] GameObject Light;
    public bool IsCompleted { get; private set; }
    bool isSabotaging;
    public bool IsSabotaging
    {
        get { return isSabotaging; }
        private set
        {
            if (isSabotaging != value)
            {
                isSabotaging = value;
                if (value == false)
                {
                    OnSabotage();
                    StartCoroutine(CorSabotage());
                }

            }
        }
    }

    private void OnEnable()
    {
        OnCompleteHandler += SetComplete;
        OnCompleteHandler += TurnOnLight;
        OnSabotage += DecreaseGauge;
    }

    private void OnDisable()
    {
        OnSabotage -= DecreaseGauge;
        OnCompleteHandler -= TurnOnLight;
        OnCompleteHandler -= SetComplete;
        OnCompleteHandler = null;
    }

    public void Interact()
    {
        if (IsCompleted) return;

        if (Input.GetMouseButton(0))
        {
            IsSabotaging = false;
            Slider_Gauge.gameObject.SetActive(true);
            CurGauge += Time.deltaTime * Multi_Gauge;
            Slider_Gauge.value = CurGauge / maxGauge;
        }
        if (Input.GetMouseButtonUp(0))
            Slider_Gauge.gameObject.SetActive(false);
    }

    public void Sabotage()
    {
        IsSabotaging = true;//서버 작업 필
    }
    void DecreaseGauge()
    {
        curGauge *= 0.95f;
    }

    void SetComplete()
    {
        IsCompleted = true;//서버 작업 필
    }
    void TurnOnLight()
    {
        Light.gameObject.SetActive(true);
    }

    private event Action OnSabotage;
    public event Action OnCompleteHandler;

    IEnumerator CorSabotage()
    {
        while (IsSabotaging || curGauge <= 0.0f)
        {
            yield return null;
            curGauge -= Time.deltaTime * Multi_Gauge * 0.5f;
        }
    }
}