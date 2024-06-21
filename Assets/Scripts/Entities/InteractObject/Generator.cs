using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Generator : NetworkBehaviour, IInteractableObject
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
                IsCompleted = true;
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

    bool isCompleted = false;
    public bool IsCompleted 
    {
        get { return isCompleted; }
        private set 
        {
            if (isCompleted != value)
            {
                isCompleted = value;
                if(value == true)
                {
                    CmdOnCompleteHandler();
                }
            }
        } 
    }
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
                    CmdOnSabotage();
                }
                else
                {
                    CmdOnEndSabotage();
                }

            }
        }
    }

    private void OnEnable()
    {
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

        OnCompleteHandler = null;
    }

    public void SurvivorInteract()
    {
        if (IsCompleted)
        {
            Slider_Gauge.gameObject.SetActive(false);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Cmd_ProgressGenerator();
        }
        else
        {
            Slider_Gauge.gameObject.SetActive(false);
        }
    }
    [Command(requiresAuthority =false)]
    void Cmd_ProgressGenerator() { ProgressGenerator(); }
    [ClientRpc]
    void ProgressGenerator()
    {
        IsSabotaging = false;
        Slider_Gauge.gameObject.SetActive(true);
        CurGauge += Time.deltaTime * Multi_Gauge;
        Slider_Gauge.value = CurGauge / maxGauge;
    }

    [Command(requiresAuthority =false)]
    public void CmdKillerInteract()
    {
        KillerInteract();
    }

    [ClientRpc]
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

    
    void TurnOnLight()
    {
        Light.gameObject.SetActive(true);
    }

    private event Action OnSabotage;
    private event Action OnEndSabotage;
    public event Action OnCompleteHandler;

    [Command(requiresAuthority = false)]    
    private void CmdOnSabotage() { RpcOnSabotage(); }
    [Command(requiresAuthority = false)]
    private void CmdOnEndSabotage() { RpcOnEndSabotage(); }
    [Command(requiresAuthority = false)]
    private void CmdOnCompleteHandler() {  RpcOnCompleteHandler(); }

    [ClientRpc]
    private void RpcOnSabotage() { OnSabotage.Invoke(); StartCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnEndSabotage() { OnEndSabotage.Invoke(); StopCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnCompleteHandler() {  OnCompleteHandler.Invoke(); }

    IEnumerator CorSabotage()
    {
        while (IsSabotaging && CurGauge >= 0.0f)
        {
            CurGauge -= Time.deltaTime * Multi_Gauge * 0.5f;
            yield return null;
        }
    }
}