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
            anim.SetFloat("Progress", value);
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
    [SerializeField] GameObject XrayWhiteLight;
    [SerializeField] GameObject XrayRedLight;
    [SerializeField] ParticleSystem VFX_Spark;
    [SerializeField] ParticleSystem VFX_Smoke;
    [SerializeField] ParticleSystem VFX_Steam;

    Animator anim;

    [SerializeField] SkillCheckManager SkillCheckManager;

    bool isCompleted = false;
    public bool IsCompleted
    {
        get { return isCompleted; }
        private set
        {
            if (isCompleted != value)
            {
                isCompleted = value;
                if (value == true)
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

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        OnCompleteHandler += TurnOnLight;
        OnCompleteHandler += CmdXrayOn;
        OnCompleteHandler += OnComplete_StopSkillCheck;

        OnSabotage += DecreaseGauge;
        OnSabotage += PlaySabotageVFX;

        OnEndSabotage += StopSabotageVFX;

        SkillCheckManager.GetSkillChecker().OnSkillCheckSuccess += OnSkillCheckSuccess;
        SkillCheckManager.GetSkillChecker().OnSkillCheckCritical += OnSkillCheckCritical;
        SkillCheckManager.GetSkillChecker().OnSkillCheckFailed += OnSkillCheckFailed;
    }

    private void OnDisable()
    {
        SkillCheckManager.GetSkillChecker().OnSkillCheckFailed -= OnSkillCheckFailed;
        SkillCheckManager.GetSkillChecker().OnSkillCheckCritical -= OnSkillCheckCritical;
        SkillCheckManager.GetSkillChecker().OnSkillCheckSuccess -= OnSkillCheckSuccess;
        
        OnEndSabotage -= StopSabotageVFX;

        OnSabotage -= PlaySabotageVFX;
        OnSabotage -= DecreaseGauge;

        OnCompleteHandler -= CmdXrayOn;
        OnCompleteHandler -= TurnOnLight;
        OnCompleteHandler -= OnComplete_StopSkillCheck;

        OnCompleteHandler = null;
    }

    public void SurvivorInteract()
    {
        if (IsCompleted)
        {
            Slider_Gauge.gameObject.SetActive(false);
            SkillCheckManager.IsSkillChecking = false;

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Slider_Gauge.gameObject.SetActive(true);
            SkillCheckManager.IsSkillChecking = true;
        }

        if (Input.GetMouseButton(0))
        {
            Cmd_ProgressGenerator(1);
        }
        else
        {
            Slider_Gauge.gameObject.SetActive(false);
            SkillCheckManager.IsSkillChecking = false;
            SkillCheckManager.GetSkillChecker().InvokeOnSkillCheckEnd();
        }
    }


    [Command(requiresAuthority = false)]
    void Cmd_ProgressGenerator(float multi)
    {
        CurGauge += Time.deltaTime * Multi_Gauge * multi;
        ProgressGenerator(multi);
    }

    [ClientRpc]
    void ProgressGenerator(float multi)
    {
        IsSabotaging = false;
        //Slider_Gauge.gameObject.SetActive(true);
        //CurGauge += Time.deltaTime * Multi_Gauge;
        Slider_Gauge.value = CurGauge / maxGauge;
    }

    [Command(requiresAuthority = false)]
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
    void TurnOnLight()
    {
        Light.gameObject.SetActive(true);
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




    private event Action OnSabotage;
    private event Action OnEndSabotage;
    public event Action OnCompleteHandler;
    public event Action OnGeneratorFailed;

    [Command(requiresAuthority = false)]
    private void CmdOnSabotage() { RpcOnSabotage(); }
    [Command(requiresAuthority = false)]
    private void CmdOnEndSabotage() { RpcOnEndSabotage(); }
    [Command(requiresAuthority = false)]
    private void CmdOnCompleteHandler() { RpcOnCompleteHandler(); }
    [Command(requiresAuthority = false)]
    void CmdXrayOn()
    {
        RpcXrayOn();
    }
    [Command(requiresAuthority = false)]
    void CmdRedLightOn()
    {
        RpcRedLightOn();
    }




    [ClientRpc]
    private void RpcOnSabotage() { OnSabotage.Invoke(); StartCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnEndSabotage() { OnEndSabotage.Invoke(); StopCoroutine(CorSabotage()); }
    [ClientRpc]
    private void RpcOnCompleteHandler() { OnCompleteHandler.Invoke(); }
    [ClientRpc]
    void RpcXrayOn()
    {
        StartCoroutine(CorShowWhiteXray());
    }
    [ClientRpc]
    void RpcRedLightOn()
    {
        StartCoroutine(CorShowRedXray());
    }


    void Hook_OnChangedProgress(float old, float recent)
    {
        CurGauge = recent;
    }


    void OnSkillCheckSuccess()
    {
        Cmd_ProgressGenerator(20);
    }
    void OnSkillCheckCritical()
    {
        Cmd_ProgressGenerator(100);
    }
    void OnSkillCheckFailed()
    {
        CmdRedLightOn();
        Cmd_ProgressGenerator(-150.0f);
    }

    void OnComplete_StopSkillCheck()
    {
        if(isLocalPlayer)
        SkillCheckManager.gameObject.SetActive(false);
    }

    IEnumerator CorSabotage()
    {
        while (IsSabotaging && CurGauge >= 0.0f)
        {
            CurGauge -= Time.deltaTime * Multi_Gauge * 0.5f;
            yield return null;
        }
    }
    IEnumerator CorShowWhiteXray()
    {
        Xray_Silhouette.SetActive(true);
        XrayWhiteLight.SetActive(true);
        yield return new WaitForSeconds(XrayShowDuration);
        Xray_Silhouette.SetActive(false);
        XrayWhiteLight.SetActive(false);
    }
    IEnumerator CorShowRedXray()
    {
        Xray_Silhouette.SetActive(true);
        XrayRedLight.SetActive(true);
        yield return new WaitForSeconds(XrayShowDuration);
        Xray_Silhouette.SetActive(false);
        XrayRedLight.SetActive(false);
    }
}