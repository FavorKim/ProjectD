using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour, IInteractableObject
{
    private float curGauge;
    public float CurGauge
    {
        get
        {
            if (curGauge % 5 <= 0.2f)
                SetSteam();
            return curGauge;
        }
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
    [SerializeField] ParticleSystem VFX_Spark;
    [SerializeField] ParticleSystem VFX_Smoke;
    [SerializeField] ParticleSystem VFX_Steam;


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
                if (value == true)
                {
                    OnSabotage();
                    StartCoroutine(CorSabotage());
                }
                else
                {
                    OnEndSabotage();
                    StopCoroutine(CorSabotage());
                }

            }
        }
    }

    private void OnEnable()
    {
        OnCompleteHandler += SetComplete;
        OnCompleteHandler += TurnOnLight;

        OnSabotage += DecreaseGauge;
        OnSabotage += PlaySabotageVFX;

        OnEndSabotage += StopSabotageVFX;
    }

    private void OnDisable()
    {
        OnEndSabotage -= StopSabotageVFX;

        OnSabotage -= PlaySabotageVFX;
        OnSabotage -= DecreaseGauge;

        OnCompleteHandler -= TurnOnLight;
        OnCompleteHandler -= SetComplete;
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

    void SetSteam()
    {
        Debug.Log('s');
        var emission = VFX_Steam.emission;
        emission.rateOverTime = curGauge * 0.1f;
    }


    public void Sabotage()
    {
        IsSabotaging = true;//서버 작업 필
    }
    void DecreaseGauge()
    {
        curGauge *= 0.95f;
    }
    void PlaySabotageVFX()
    {
        VFX_Spark.Play();
        VFX_Smoke.Play();
    }

    void StopSabotageVFX()
    {
        VFX_Spark.Stop();
        VFX_Smoke.Stop();
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
    private event Action OnEndSabotage;
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