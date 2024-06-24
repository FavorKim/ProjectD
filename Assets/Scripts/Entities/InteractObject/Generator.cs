using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.UI;

public class Generator : NetworkBehaviour, IInteractableObject
{
    [SyncVar(hook = nameof(Hook_OnChangedProgress))]
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
    [SerializeField] float XrayShowDuration;

    [SerializeField] Slider Slider_Gauge;
    [SerializeField] GameObject Light;
    [SerializeField] GameObject Xray_Silhouette;
    [SerializeField] GameObject XrayLight;
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

    public override void OnStartServer()
    {
        base.OnStartServer();
        Xray_Silhouette.SetActive(true);
    }


    private void OnEnable()
    {
        OnCompleteHandler += TurnOnLight;
        OnCompleteHandler += CmdXrayOn;

        OnSabotage += DecreaseGauge;
        OnSabotage += PlaySabotageVFX;

        OnEndSabotage += StopSabotageVFX;
    }

    private void OnDisable()
    {
        OnEndSabotage -= StopSabotageVFX;

        OnSabotage -= PlaySabotageVFX;
        OnSabotage -= DecreaseGauge;

        OnCompleteHandler -= CmdXrayOn;
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

        if(Input.GetMouseButtonDown(0))
        {
            Slider_Gauge.gameObject.SetActive(true);
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
    void Cmd_ProgressGenerator() 
    {
        CurGauge += Time.deltaTime * Multi_Gauge;
        ProgressGenerator(); 
    }
    
    [ClientRpc]
    void ProgressGenerator()
    {
        IsSabotaging = false;
        //Slider_Gauge.gameObject.SetActive(true);
        //CurGauge += Time.deltaTime * Multi_Gauge;
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
    [Command(requiresAuthority =false)]
    void CmdXrayOn()
    {
        RpcXrayOn();
    }


    [ClientRpc]
    private void RpcOnSabotage() { OnSabotage.Invoke(); StartCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnEndSabotage() { OnEndSabotage.Invoke(); StopCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnCompleteHandler() {  OnCompleteHandler.Invoke(); }
    [ClientRpc]
    void RpcXrayOn()
    {
        StartCoroutine(CorShowXray());
    }
    void Hook_OnChangedProgress(float old, float recent)
    {
        CurGauge = recent;
    }

    IEnumerator CorSabotage()
    {
        while (IsSabotaging && CurGauge >= 0.0f)
        {
            CurGauge -= Time.deltaTime * Multi_Gauge * 0.5f;
            yield return null;
        }
    }
    IEnumerator CorShowXray()
    {
        Xray_Silhouette.SetActive(true);
        XrayLight.SetActive(true);
        yield return new WaitForSeconds(XrayShowDuration);
        Xray_Silhouette.SetActive(false);
        XrayLight.SetActive(false);
    }
}