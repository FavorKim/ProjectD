using Cinemachine;
using DG.Tweening.Core.Easing;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class Survivor : PlayableCharacter
{
    #region Class
    CharacterController m_CharacterController;

    // 해시로 애니메이터 다뤄보기 (최적화)
    [SerializeField] Animator Animator;
    NetworkAnimator netAnim;
    [SerializeField] Slider Slider_HealGauge;
    [SerializeField] Slider Slider_EscapeGauge;

    [SerializeField] GameObject VFX_FootPrintPref;
    [SerializeField] GameObject VFX_Bleeding;
    [SerializeField] GameObject Prefab_SurvivorTrack;

    SurvivorStateMachine m_StateMachine;
    public SurvivorStateMachine GetMoveState() { return m_StateMachine; }
    SurvivorHealthStateMachine m_healthStateMachine;
    public SurvivorHealthStateMachine GetHealthStateMachine() { return m_healthStateMachine; }

    Survivor m_healDest;
    KillerBase m_holdingKiller;

    [SerializeField] SkillCheckManager EscapeSkillCheckManager;

    #endregion
    #region Vector
    Vector3 MoveDir;
    Vector2 dir;
    public Vector3 HeldPosition { get; private set; }
    #endregion
    #region Float
    [SerializeField] float m_invincibleTime;

    private float walkSpeed = 226.0f;
    private float runSpeed = 400.0f;
    public float RunSpeed
    {
        get { return runSpeed; }
        set
        {
            if (RunSpeed != value)
            {
                runSpeed = value;
                OnChangedRunSpeed.Invoke();
            }
        }
    }
    private float downSpeed = 70.0f;

    private float crouchSpeed = 113.0f;
    public float CrouchSpeed { get { return crouchSpeed; } set { crouchSpeed = value; } }

    [SerializeField]
    float m_corruptTime = 120;
    public float CorruptTime
    {
        get
        {
            return m_corruptTime;
        }
        private set
        {
            if (m_corruptTime != value)
            {
                m_corruptTime = value;
                PlayerUIManager.Instance.SetPlayerUIGauge(m_playerID, m_corruptTime / 120);
                if (m_corruptTime <= 0)
                {
                    CmdOnSacrificed();
                }
            }
        }
    }

    [SerializeField]
    float DebugOnly_CorruptMulti;

    const float MaxHealGauge = 1;

    [SerializeField] float m_healGauge = 0;
    public float HealGauge
    {
        get { return m_healGauge; }
        set
        {
            if (m_healGauge != value)
            {
                m_healGauge = value;
                Slider_HealGauge.value = m_healGauge / MaxHealGauge;
                if (m_healGauge >= MaxHealGauge)
                {
                    CmdOnChangedState(m_healthStateMachine.GetCurState() - 1);
                }
            }
        }
    }

    [SerializeField] float m_healSpeed = 0.0f;
    public float HealSpeed
    {
        get
        {
            return m_healSpeed;
        }
        set
        {
            m_healSpeed = value;
            CmdOnChangedHealSpeed(value);

        }
    }

    [SerializeField] float m_entireHealTime = 16.0f;

    [SerializeField] float escapeGauge = 0;
    public float EscapeGauge
    {
        get { return escapeGauge; }
        private set
        {
            if (escapeGauge != value)
            {
                escapeGauge = value;
                Slider_EscapeGauge.value = escapeGauge;
                if (escapeGauge >= 1)
                    CmdOnEscaped();
            }
        }
    }
    [SerializeField] float timeToEscape = 20.0f;



    public float GetWalkSpeed() { return walkSpeed; }
    public float GetRunSpeed() { return runSpeed; }
    public float GetCrouchSpeed() { return crouchSpeed; }
    public float GetDownSpeed() { return downSpeed; }

    [SerializeField] float rotateSpeed;

    float resqueTime = 0;



    #endregion
    #region Boolean

    bool isFreeze = false;
    public bool IsFreeze { get { return isFreeze; } set { isFreeze = value; } }

    private bool isBleeding = false;
    public bool IsBleeding
    {
        get { return isBleeding; }
        set
        {
            if (isBleeding != value)
                isBleeding = value;
            if (isBleeding)
            {
                VFX_Bleeding.SetActive(true);
            }
            else
            {
                VFX_Bleeding.SetActive(false);
            }
        }
    }

    bool isInvincible = false;

    public bool isCorrupted = false;
    public bool IsCorrupted
    {
        get
        {
            return isCorrupted;
        }
        set
        {
            if (isCorrupted != value)
            {
                isCorrupted = value;
                if (value == true)
                {
                    CorruptTime = 60.0f;
                }
            }
        }
    }

    public bool IsSelfCare { get; set; }
    #endregion
    #region Inteager
    int hangedCount = 0;
    public int HangedCount
    {
        get { return hangedCount; }
        private set
        {
            if (hangedCount != value)
            {
                hangedCount = value;
                if (value == 2)
                {
                    IsCorrupted = true;
                }
                if (value == 3)
                {
                    CmdOnSacrificed();
                }
            }
        }
    }
    int m_playerID;
    int resistDir = 1;

    #endregion

    #region GetterSetter
    public int PlayerID() { return m_playerID; }


    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }


    public void SetPlayerID(int value) { m_playerID = value; }
    #endregion


    #region LifeCycle
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
        m_healthStateMachine = new SurvivorHealthStateMachine(this);
        netAnim = Animator.gameObject.GetComponent<NetworkAnimator>();

        StartCoroutine(CorPrintFoot());
    }
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        var cam = Instantiate(Prefab_SurvivorTrack).GetComponent<CinemachineFreeLook>();
        cam.LookAt = transform;
        cam.Follow = transform;
        GetComponent<PlayerInput>().enabled = true;
        PlayerUIManager.Instance.CreatePlayerUI(this);
        PlayerPerkManager.SetSurvivorPerk(SelectedPerkManager.EquippedPerkList, this);
        InGamePerkSlot.Instance.SetPerkIcons(SelectedPerkManager.EquippedPerkList);
        GameResultManager.Instance.CmdIncreaseRemainingPlayer();
    }

    private void OnEnable()
    {
        OnBeingHeld += OnBeingHeld_SetState;
        OnBeingHeld += OnBeingHeld_SetPosition;
        OnBeingHeld += OnBeingHeld_SetHoldingKiller;
        OnBeingHeld += OnBeingHeld_SetHeldSkillChekcer;

        OnBeingHanged += OnBeingHanged_SetState;
        OnBeingHanged += OnBeingHanged_SetPosition;
        OnBeingHanged += OnBeingHanged_SetCorrupt;
        OnBeingHanged += OnBeingHanged_ResetEscape;



        OnSacrificed += OnSacrificed_GoUp;

        OnEscapedFromKiller += OnEscapedFromKiller_SetState;
        OnEscapedFromKiller += OnEscapedFromKiller_StunKiller;
        OnEscapedFromKiller += OnEscapedFromKiller_ResetEscape;

        EscapeSkillCheckManager.GetSkillChecker().OnSkillCheckCritical += OnEscapeSkillCheckCritical;

        OnHitted += OnHitted_SetAnimation;

    }

    protected override void Update()
    {
        base.Update();
        if (isLocalPlayer)
        {
            PlayerMove();
            m_healthStateMachine.StateUpdate();

            if (m_healthStateMachine.GetCurState() != HealthStates.Down)
                m_StateMachine.StateUpdate();

            HealOtherSurvivor();
            SelfCare();
        }
    }

    private void OnDisable()
    {
        m_healthStateMachine.UnRegisterEvent();
        OnHitted = null;

        OnHitted -= OnHitted_SetAnimation;

        OnSacrificed -= OnSacrificed_GoUp;

        OnBeingHanged -= OnBeingHanged_ResetEscape;
        OnBeingHanged -= OnBeingHanged_SetCorrupt;
        OnBeingHanged -= OnBeingHanged_SetPosition;
        OnBeingHanged -= OnBeingHanged_SetState;

        OnBeingHeld -= OnBeingHeld_SetHeldSkillChekcer;
        OnBeingHeld -= OnBeingHeld_SetHoldingKiller;
        OnBeingHeld -= OnBeingHeld_SetPosition;
        OnBeingHeld -= OnBeingHeld_SetState;

        OnEscapedFromKiller -= OnEscapedFromKiller_ResetEscape;
        OnEscapedFromKiller -= OnEscapedFromKiller_StunKiller;
        OnEscapedFromKiller -= OnEscapedFromKiller_SetState;

        EscapeSkillCheckManager.GetSkillChecker().OnSkillCheckCritical -= OnEscapeSkillCheckCritical;

        OnBeingHanged = null;
        OnBeingHeld = null;
        OnSacrificed = null;
        OnEscapedFromKiller = null;

        OnHitted = null;
    }
    #endregion


    #region Command

    [Command(requiresAuthority = false)]
    void PrintFoot()
    {
        FootPrintPool.Instance.PrintFootPrint(new Vector3(transform.position.x, 0.001f, transform.position.z), Quaternion.Euler(90, 0, 0));
    }

    [Command(requiresAuthority = false)]
    public void BeingHeld(KillerBase killer)
    {
        RpcBeingHeld(killer);
    }

    [Command(requiresAuthority = false)]
    public void BeingHanged(Hanger hanger)
    {
        RpcBeingHanged(hanger);
    }

    [Command(requiresAuthority = false)]
    public void CmdGetHit()
    {
        RpcGetHit();
    }

    [Command(requiresAuthority = false)]
    void CmdOnSacrificed()
    {
        RpcOnSacrificed();
    }

    [Command(requiresAuthority = false)]
    public void CmdOnResqued()
    {
        RpcOnResqued();
    }

    [Command(requiresAuthority = false)]
    public void CmdOnHealed(Survivor healer)
    {
        HealGauge += Time.deltaTime * (1 + healer.HealSpeed * 0.01f) / m_entireHealTime;
        RpcOnHealed(HealGauge);
    }

    [Command(requiresAuthority = false)]
    public void CmdOnChangedHealSpeed(float value)
    {
        RpcOnChangedHealSpeed(value);
    }

    [Command(requiresAuthority = false)]
    void CmdOnStopHeal()
    {
        RpcOnStopHeal();
    }

    [Command(requiresAuthority = false)]
    void CmdOnEscaped()
    {
        RpcOnEscaped();
    }

    [Command(requiresAuthority = false)]
    void CmdOnChangedState(HealthStates newState)
    {
        OnChangedState(newState);
    }



    #endregion
    #region Rpc
    [ClientRpc]
    void RpcBeingHeld(KillerBase holdPos)
    {
        OnBeingHeld(holdPos);
    }

    [ClientRpc]
    void RpcBeingHanged(Hanger hangerPos)
    {
        OnBeingHanged(hangerPos);
    }

    [ClientRpc]
    public void RpcGetHit()
    {
        if (m_healthStateMachine.GetCurState() != HealthStates.Down && !isInvincible)
        {
            OnHitted();
            StartCoroutine(CorSprint());
            StartCoroutine(CorInvincibleTime());
        }
    }

    [ClientRpc]
    void RpcOnSacrificed()
    {
        OnSacrificed.Invoke();
    }

    [ClientRpc]
    void RpcOnResqued()
    {
        m_healthStateMachine.ChangeState(HealthStates.Injured);
        IsFreeze = false;
        m_CharacterController.Move(transform.up * -10f);
        Animator.SetTrigger(Animator.StringToHash("Resqued"));
        netAnim.SetTrigger("Resqued");
    }

    [ClientRpc]
    void RpcOnHealed(float gauge)
    {
        if (isLocalPlayer)
            Slider_HealGauge.gameObject.SetActive(true);


        HealGauge = gauge;
        IsFreeze = true;
    }

    [ClientRpc]
    void RpcOnChangedHealSpeed(float value)
    {
        m_healSpeed = value;
    }

    [ClientRpc]
    void RpcOnStopHeal()
    {
        StopHeal();
    }

    [ClientRpc]
    void RpcOnEscaped()
    {
        OnEscapedFromKiller.Invoke();
    }

    [ClientRpc]
    void OnChangedState(HealthStates newState)
    {
        m_healthStateMachine.ChangeState(newState);
    }
    #endregion


    #region Interact
    public override void Interact(Generator generator)
    {
        if (!isLocalPlayer) return;
        if (m_healthStateMachine.GetCurState() > HealthStates.Injured) return;

        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Generate");
            netAnim.SetTrigger("Generate");

            Animator.SetBool("isGenerating", true);

            generator.OnGeneratorFailed += OnGeneratorFailed;
        }
        if (Input.GetMouseButton(0))
        {
            if (generator.IsCompleted)
            {
                isFreeze = false;
                Animator.SetBool("isGenerating", false);
                return;
            }
            isFreeze = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Animator.SetBool("isGenerating", false);
            generator.OnGeneratorFailed -= OnGeneratorFailed;
            isFreeze = false;
        }
        generator.SurvivorInteract();
    }
    public override void Interact(Palete palete)
    {
        if (!isLocalPlayer || IsFreeze) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!palete.isUsed)
            {
                // 판자 내리기 애니메이션
                Animator.SetTrigger("UsePallete");
                netAnim.SetTrigger("UsePallete");
                StartCoroutine(CorFreeze(0.3f));
                palete.SurvivorInteract();
            }
            else
            {
                // 판자 넘고, 넘는 애니메이션
                var center = palete.transform.position + palete.transform.right * -1.0f;

                OnJumpFence(center);
                //palete.SurvivorInteract();
            }
        }
    }
    public override void Interact(JumpFence fence)
    {
        if (!isLocalPlayer || IsFreeze) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            var center = fence.transform.position + fence.transform.forward * -1.5f;
            OnJumpFence(center);
        }
        // 창틀 뛰어넘기
    }
    public override void Interact(Hanger hanger)
    {
        if (!isLocalPlayer) return;
        if (hanger.HangedSurvivor != null && hanger.HangedSurvivor != this)
        {
            if (Input.GetMouseButtonDown(0))
            {
                hanger.SurvivorInteract();
                StartCoroutine(CorFreeze(0.5f));
            }
        }

    }
    public override void Interact(Lever lever)
    {
        if (!isLocalPlayer) return;
        if (!lever.IsAvailable)
        {
            Animator.SetTrigger("PullOver");
            netAnim.SetTrigger("PullOver");
            IsFreeze = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Pull");
            netAnim.SetTrigger("Pull");
            IsFreeze = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Animator.SetTrigger("PullOver");
            netAnim.SetTrigger("PullOver");
            IsFreeze = false;
        }
        lever.SurvivorInteract();
    }
    #endregion

    #region Coroutine
    public IEnumerator CorPrintFoot()
    {
        while (true)
        {
            if (m_healthStateMachine.GetCurState() == HealthStates.Down || !m_StateMachine.CurStateIs(SurvivorStateMachine.StateName.Run))
            {
                yield return null;
                continue;
            }
            PrintFoot();
            yield return new WaitForSeconds(1.0f);
        }
    }
    IEnumerator CorSprint()
    {
        RunSpeed += 260.0f;
        yield return new WaitForSeconds(1.8f);
        RunSpeed -= 260.0f;
    }
    IEnumerator CorInvincibleTime()
    {
        isInvincible = true;
        yield return new WaitForSeconds(m_invincibleTime);
        isInvincible = false;
    }
    IEnumerator CorJumpFence()
    {
        float time = 0;
        IsFreeze = true;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * 5.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 4.0f * Time.deltaTime);
            yield return null;
        }
        while (time < 0.6f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * -9.5f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 4.0f * Time.deltaTime);
            yield return null;
        }
        //m_CharacterController.Move(-transform.up * 10f);
        IsFreeze = false;
    }
    IEnumerator CorCorrupt()
    {

        while (CorruptTime > 0 && m_healthStateMachine.GetCurState() == HealthStates.Hanged)
        {
            CorruptTime -= Time.deltaTime * DebugOnly_CorruptMulti;
            if (CorruptTime < 60.0f) IsCorrupted = true;
            yield return null;
        }
    }
    IEnumerator CorFreeze(float time)
    {
        IsFreeze = true;
        yield return new WaitForSeconds(time);
        IsFreeze = false;
    }
    IEnumerator CorSacrifice()
    {
        IsFreeze = true;
        Animator.SetTrigger("Sacrificed");
        netAnim.SetTrigger("Sacrificed");
        float time = 0;
        yield return new WaitForSeconds(2.0f);
        while (time < 5)
        {
            yield return null;
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * Time.deltaTime * 1.0f);
        }
        PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Killed);
        gameObject.SetActive(false);
        OnSacrificed_GameOver();
    }
    IEnumerator CorResistKiller()
    {
        float dur = 0;
        resistDir *= -1;
        while (dur < 0.5f)
        {
            dur += Time.deltaTime;
            MoveHoldingKiller();
            yield return null;
        }
    }
    IEnumerator CorIncreaseEscapeGauge()
    {
        while (EscapeGauge < 1 && m_healthStateMachine.GetCurState() == HealthStates.Held)
        {
            EscapeGauge += Time.deltaTime / timeToEscape;
            yield return null;
        }
    }
    #endregion

    #region Event
    public event Action OnHitted;
    public event Action<KillerBase> OnBeingHeld;
    public event Action<Hanger> OnBeingHanged;
    public event Action OnSacrificed;
    public event Action OnChangedRunSpeed;
    public event Action OnEscapedFromKiller;

    // 들렸을 때
    void OnBeingHeld_SetState(KillerBase killer)
    {
        if (m_healthStateMachine.GetCurState() == HealthStates.Down)
            m_healthStateMachine.ChangeState(HealthStates.Held);
    }
    void OnBeingHeld_SetPosition(KillerBase killer)
    {
        transform.parent = killer.GetHoldPosition();
        transform.localPosition = Vector3.zero;
    }
    void OnBeingHeld_SetHoldingKiller(KillerBase killer)
    {
        m_holdingKiller = killer;
    }
    void OnBeingHeld_SetHeldSkillChekcer(KillerBase killer)
    {
        if (isLocalPlayer)
        {
            Slider_EscapeGauge.gameObject.SetActive(true);
            EscapeSkillCheckManager.SkillCheckStart();
        }
        StartCoroutine(CorIncreaseEscapeGauge());
    }

    // 걸렸을 때
    void OnBeingHanged_SetState(Hanger hanger)
    {
        if (m_healthStateMachine.GetCurState() == HealthStates.Held)
        {
            m_healthStateMachine.ChangeState(HealthStates.Hanged);
        }
    }
    void OnBeingHanged_SetPosition(Hanger hanger)
    {
        transform.parent = null;
        transform.position = hanger.GetHangedPos().position;
        transform.localRotation = Quaternion.identity;
    }
    void OnBeingHanged_SetCorrupt(Hanger hanger)
    {
        HangedCount++;
        StartCoroutine(CorCorrupt());
    }
    void OnBeingHanged_ResetEscape(Hanger hanger)
    {
        OnEscapedFromKiller_ResetEscape();
    }


    void OnEscapedFromKiller_SetState()
    {
        m_healthStateMachine.ChangeState(HealthStates.Injured);
        IsFreeze = false;
        if (isLocalPlayer)
        {
            Animator.SetTrigger("Resqued");
            netAnim.SetTrigger("Resqued");
        }
    }
    void OnEscapedFromKiller_StunKiller()
    {
        if (isLocalPlayer)
            m_holdingKiller.OnStunCall();
    }
    void OnEscapedFromKiller_ResetEscape()
    {
        StopCoroutine(CorIncreaseEscapeGauge());
        EscapeSkillCheckManager.IsSkillChecking = false;
        EscapeGauge = 0;
        Slider_EscapeGauge.gameObject.SetActive(false);
        EscapeSkillCheckManager.GetSkillChecker().InvokeOnSkillCheckEnd();
    }

    void OnSacrificed_GoUp()
    {
        StartCoroutine(CorSacrifice());
    }
    void OnSacrificed_GameOver()
    {
        if (isLocalPlayer)
            GameResultManager.Instance.SetGameResult(GameResult.Sacrificed);
    }

    void OnJumpFence(Vector3 dest)
    {
        RotateTransformToDest(dest);

        m_StateMachine.ChangeState(SurvivorStateMachine.StateName.Walk);
        Animator.SetTrigger("JumpFence");
        netAnim.SetTrigger("JumpFence");
        StartCoroutine(CorJumpFence());
    }


    void OnEscapeSkillCheckCritical()
    {
        EscapeGauge += 0.05f;
        StartCoroutine(CorResistKiller());
    }

    void OnGeneratorFailed()
    {
        Animator.SetTrigger("GenFail");
        netAnim.SetTrigger("GenFail");
    }

    void OnMove(InputValue val)
    {
        // 로컬체크
        if (!isLocalPlayer) return;
        //if (isFreeze) return;


        dir = val.Get<Vector2>();
        MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
        MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
        MoveDir.Normalize();
        MoveDir *= MoveSpeed * Time.deltaTime;

        if (isFreeze) return;
        if (MoveDir != Vector3.zero) Animator.SetBool("isWalk", true);
        else Animator.SetBool("isWalk", false);
    }

    void OnHitted_SetAnimation()
    {
        if (m_healthStateMachine.GetCurState() == HealthStates.Healthy)
        {
            Animator.SetTrigger("GetHit");
            netAnim.SetTrigger("GetHit");
        }
    }

    #endregion

    #region Methods
    void StopHeal()
    {
        IsFreeze = false;
        Animator.SetBool("isHeal", false);


        if (m_healDest != null)
        {
            if (m_healDest.Slider_HealGauge.gameObject.activeSelf)
                m_healDest.Slider_HealGauge.gameObject.SetActive(false);
        }
        else
        {
            Slider_HealGauge.gameObject.SetActive(false);
        }
    }
    void HealOtherSurvivor()
    {
        if (!isLocalPlayer) return;
        if (m_healDest == null) return;

        HealSurvivor(m_healDest, this, 0);
    }
    void SelfCare()
    {
        if (IsSelfCare && isLocalPlayer && m_healthStateMachine.GetCurState() != HealthStates.Down)
        {
            HealSurvivor(this, this, 1);
        }
    }

    void HealSurvivor(Survivor dest, Survivor healer, int mouseIndex)
    {
        if (dest.m_healthStateMachine.GetCurState() == HealthStates.Healthy && Input.GetMouseButton(mouseIndex))
        {
            healer.StopHeal();
            return;
        }
        if (Input.GetMouseButtonDown(mouseIndex))
        {
            healer.IsFreeze = true;
            healer.Animator.SetBool("isHeal", true);
            if (dest == healer)
            {
                healer.Animator.SetTrigger("SelfCare");
                healer.netAnim.SetTrigger("SelfCare");
            }
            else
            {
                healer.Animator.SetTrigger("Heal");
                healer.netAnim.SetTrigger("Heal");
            }

            dest.Slider_HealGauge.gameObject.SetActive(true);

        }
        if (Input.GetMouseButton(mouseIndex))
        {
            healer.Animator.SetBool("isHeal", true);
            dest.CmdOnHealed(healer);
        }
        if (Input.GetMouseButtonUp(mouseIndex))
        {
            dest.CmdOnStopHeal();
            healer.CmdOnStopHeal();
        }
    }

    void RotateTransformToDest(Vector3 look)
    {
        //transform.rotation.SetLookRotation(look);
        m_CharacterController.Move(look - transform.position);
        transform.LookAt(look);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }

    void PlayerMove()
    {

        if (IsFreeze) return;


        if (MoveDir != Vector3.zero)
        {
            MoveDir = dir.y * Camera.main.transform.forward + dir.x * Camera.main.transform.right;
            MoveDir = new Vector3(MoveDir.x, 0, MoveDir.z);
            MoveDir.Normalize();
            MoveDir *= MoveSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(MoveDir), rotateSpeed * Time.deltaTime);
        }
        m_CharacterController.SimpleMove(MoveDir);
    }

    void MoveHoldingKiller()
    {
        m_holdingKiller.MoveToDirection(m_holdingKiller.transform.right * resistDir * Time.deltaTime * 1.5f);
    }
    #endregion


    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (other.TryGetComponent(out Survivor survivor))
        {
            if (survivor != null && survivor != this)
            {
                if (survivor.m_healthStateMachine.GetCurState() == HealthStates.Injured || survivor.m_healthStateMachine.GetCurState() == HealthStates.Down)
                {
                    m_healDest = survivor;
                }
            }
        }
        if (other.CompareTag("Escape"))
        {
            PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Escaped);
            if (isLocalPlayer)
                GameResultManager.Instance.SetGameResult(GameResult.Escape);
        }
    }
    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
        if (other.TryGetComponent(out Survivor survivor) && survivor.gameObject != this.gameObject)
        {
            StopHeal();
        }
    }


}
