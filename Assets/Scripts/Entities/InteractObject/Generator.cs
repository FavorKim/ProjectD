using System;
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
    public bool IsCompleted {  get; private set; }

    private void OnEnable()
    {
        OnCompleteHandler += SetComplete;
        OnCompleteHandler += TurnOnLight;
    }

    private void OnDisable()
    {
        OnCompleteHandler -= SetComplete;
        OnCompleteHandler -= TurnOnLight;
        OnCompleteHandler = null;
    }

    public void Interact()
    {
        if (IsCompleted) return;

        if (Input.GetMouseButton(0))
        {
            Slider_Gauge.gameObject.SetActive(true);
            CurGauge += Time.deltaTime * Multi_Gauge;
            Slider_Gauge.value = CurGauge / maxGauge;
        }
        if (Input.GetMouseButtonUp(0))
            Slider_Gauge.gameObject.SetActive(false);
    }

    
    void SetComplete()
    {
        IsCompleted = true;
    }

    void TurnOnLight()
    {
        Light.gameObject.SetActive(true);
    }

    public event Action OnCompleteHandler;
}