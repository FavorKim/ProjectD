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

    public void SurvivorInteract()
    {
        if (IsCompleted) return;

        if (Input.GetMouseButton(0))
        {
            IsSabotaging = false;
            Slider_Gauge.gameObject.SetActive(true);
            CurGauge += Time.deltaTime * Multi_Gauge;
            Slider_Gauge.value = CurGauge / maxGauge;
        }
        else
        {
            Slider_Gauge.gameObject.SetActive(false);
        }
    }
    
    public void KillerInteract()
    {
        IsSabotaging = true;//서버 작업 필
    }

    void SetSteam()
    {
        var emission = VFX_Steam.emission;
        emission.rateOverTime = curGauge * 0.05f;
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
        while (IsSabotaging && CurGauge >= 0.0f)
        {
            CurGauge -= Time.deltaTime * Multi_Gauge * 0.5f;
            yield return null;
        }
    }
}