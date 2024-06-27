using Cinemachine;
using Mirror;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

/*
 ������ ������ ��ҵ� (���� ���� ��� �÷��̾ �˾ƾ��� ���� �� ������)
������o
 ������ ��ô��
 ������ ��ô���� ���� ����Ʈ
 ������ �纸Ÿ�� ����
 ������ �Ϸ�(�� �ɷ��ִ� �̺�Ʈ��)

����o
 ���� ����o
 ���� �ı���o

����
 ������ �Ŵ޸�o
 ������ �����o
 
 
������
 ������ ���ڱ� (Spawn, Ǯ�� �ؾ�)o
 ������ �ǰ�o
 ������ ġ���o
 ������ Ż����o
 ������ Ż����

���θ�
 ���θ��� �����ڸ� ���ø�o
 (���θ� Ÿ�� -> X. ���θ��� ������ �ִϸ��̼� ����� ��, �����ڰ� �ǰݵǾ������� �Ǵ��ϸ� ��.)
 

 
 
 */
public class Survivor : PlayableCharacter
{
    CharacterController m_CharacterController;

    // �ؽ÷� �ִϸ����� �ٷﺸ�� (����ȭ)
    [SerializeField] Animator Animator;
    NetworkAnimator netAnim;
    [SerializeField] Slider Slider_HealGauge;

    [SerializeField] GameObject VFX_FootPrintPref;
    [SerializeField] GameObject VFX_Bleeding;
    [SerializeField] GameObject Prefab_SurvivorTrack;

    SurvivorStateMachine m_StateMachine;
    public SurvivorStateMachine GetMoveState() { return m_StateMachine; }
    SurvivorHealthStateMachine m_healthStateMachine;

    Survivor m_healDest;

    


    Vector3 MoveDir;
    Vector2 dir;
    public Vector3 HeldPosition { get; private set; }

    [SerializeField] float m_invincibleTime;

    private float walkSpeed = 200.0f;
    private float runSpeed = 500.0f;
    private float crouchSpeed = 100.0f;
    public float CrouchSpeed { get { return crouchSpeed; } set {  crouchSpeed = value; } }

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

    const float MaxHealGauge = 100;

    [SerializeField] float m_healGauge = 1;
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
                    m_healthStateMachine.Healed();
                }
            }
        }
    }

    [SerializeField] float m_healSpeed = 1;
    public float HealSpeed
    {
        get 
        {
            return 1 + (m_healSpeed * 0.01f);
        }
        set
        {
            if(m_healSpeed != value)
            {
                m_healSpeed = value;
            }
        }
    }

    public float GetWalkSpeed() { return walkSpeed; }
    public float GetRunSpeed() { return runSpeed; }
    public float GetCrouchSpeed() { return crouchSpeed; }

    [SerializeField] float rotateSpeed;

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
    public int PlayerID() { return m_playerID; }
    public void SetPlayerID(int value) { m_playerID = value; }


    public Vector3 GetMoveDir() { return MoveDir; }

    public Animator GetAnimator() { return Animator; }


    // Start is called before the first frame update
    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_StateMachine = new SurvivorStateMachine(this);
        m_healthStateMachine = new SurvivorHealthStateMachine(this);
        netAnim = Animator.gameObject.GetComponent<NetworkAnimator>();
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

        // �� �����Ͽ� ������Ʈ ���� ��ų�� ���� ���, ����� Ŭ������ �����ϰ�(��Ÿ��, ���ӽð����� �ڷ�ƾ���� Ȱ��),
        // �÷��̾�� �޾��ִ� ������ ������ ��� �����ϱ�
    }


    private void OnEnable()
    {
        OnBeingHeld += OnBeingHeld_SetState;
        OnBeingHeld += OnBeingHeld_SetPosition;

        OnBeingHanged += OnBeingHanged_SetState;
        OnBeingHanged += OnBeingHanged_SetPosition;
        OnBeingHanged += OnBeingHanged_SetCorrupt;

        OnSacrificed += OnSacrificed_GoUp;

    }
    private void OnDisable()
    {
        m_healthStateMachine.UnRegisterEvent();
        OnHitted = null;


        OnSacrificed -= OnSacrificed_GoUp;

        OnBeingHanged -= OnBeingHanged_SetCorrupt;
        OnBeingHanged -= OnBeingHanged_SetPosition;
        OnBeingHanged -= OnBeingHanged_SetState;

        OnBeingHeld -= OnBeingHeld_SetState;
        OnBeingHeld -= OnBeingHeld_SetState;

        OnBeingHanged = null;
        OnBeingHeld = null;
        OnSacrificed = null;
    }

    // Update is called once per frame
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
        }
    }


    void OnMove(InputValue val)
    {
        // ����üũ
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
    void PlayerMove()
    {

        if (isFreeze) return;


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



    void OnJumpFence(Vector3 dest)
    {
        RotateTransformToDest(dest);

        m_StateMachine.ChangeState(SurvivorStateMachine.StateName.Walk);
        Animator.SetTrigger("JumpFence");
        netAnim.SetTrigger("JumpFence");
        StartCoroutine(CorJumpFence());
    }

    [Command(requiresAuthority = false)]
    void PrintFoot()
    {
        PootPrintPool.Instance.PrintPootPrint(new Vector3(transform.position.x, 0.001f, transform.position.z), Quaternion.Euler(-90, 0, 0));
    }




    // command�� rpc�� �������� �ƴ� NetworkBehaviour�� ��ӹ��� ��ü�� ����ü���� �Ű������� ����� �� ����

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
    public void CmdOnHealed()
    {
        RpcOnHealed();
    }


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
        Animator.SetTrigger(Animator.StringToHash("JumpFence"));
        netAnim.SetTrigger("JumpFence");
    }

    [ClientRpc]
    void RpcOnHealed()
    {
        if (isLocalPlayer)
            Slider_HealGauge.gameObject.SetActive(true);
        HealGauge += Time.deltaTime * HealSpeed;
        IsFreeze = true;
    }

    void RotateTransformToDest(Vector3 look)
    {
        //transform.rotation.SetLookRotation(look);
        m_CharacterController.Move(look - transform.position);
        transform.LookAt(look);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
    }



    public override void Interact(Generator generator)
    {
        if (!isLocalPlayer) return;


        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Generate");
            netAnim.SetTrigger("Generate");

            Animator.SetBool("isGenerating", true);
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
            isFreeze = false;
        }
        generator.SurvivorInteract();
    }
    public override void Interact(Palete palete)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!palete.isUsed)
            {
                // ���� ������ �ִϸ��̼�
                palete.SurvivorInteract();
            }
            else
            {
                // ���� �Ѱ�, �Ѵ� �ִϸ��̼�
                var center = palete.transform.position + palete.transform.right * -1.0f;

                OnJumpFence(center);
                //palete.SurvivorInteract();
            }
        }
    }
    public override void Interact(JumpFence fence)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            var center = fence.transform.position + fence.transform.forward * -1.5f;
            OnJumpFence(center);
        }
        // âƲ �پ�ѱ�
    }
    public override void Interact(Hanger hanger)
    {
        if (!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (hanger.HangedSurvivor != null && hanger.HangedSurvivor != this)
            {
                hanger.SurvivorInteract();
                CorFreeze(1.2f);
            }
        }
    }
    public override void Interact(Lever lever)
    {
        if (!isLocalPlayer) return;
        if (!lever.IsAvailable) return;

        if (Input.GetMouseButtonDown(0))
        {
            Animator.SetTrigger("Pull");
            netAnim.SetTrigger("Pull");
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Animator.SetTrigger("PullOver");
            netAnim.SetTrigger("PullOver");
        }
        lever.SurvivorInteract();
    }



    //rpc
    public IEnumerator CorPrintFoot()
    {
        while (m_StateMachine.CurStateIs(SurvivorStateMachine.StateName.Run)
            && m_healthStateMachine.GetCurState() > HealthStates.Down)
        {
            PrintFoot();
            yield return new WaitForSeconds(0.5f);
        }
    }
    IEnumerator CorSprint()
    {
        runSpeed += 200.0f;
        yield return new WaitForSeconds(1.5f);
        runSpeed -= 200.0f;
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
        isFreeze = true;
        while (time < 0.3f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * 3.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 6.0f * Time.deltaTime);
            yield return null;
        }
        while (time < 0.7f)
        {
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * -5.0f * Time.deltaTime);
            m_CharacterController.Move(transform.forward * 6.0f * Time.deltaTime);
            yield return null;
        }
        //m_CharacterController.Move(-transform.up * 10f);
        isFreeze = false;
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
        while(time < 5)
        {
            yield return null;
            time += Time.deltaTime;
            m_CharacterController.Move(transform.up * Time.deltaTime * 1.0f);
        }
        PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Killed);
        gameObject.SetActive(false);
        // �� �κ��� ���� ��� ������ �ű�� ������ ��ü�ؾ���
    }


    public event Action OnHitted;
    public event Action<KillerBase> OnBeingHeld;
    public event Action<Hanger> OnBeingHanged;
    public event Action OnSacrificed;

    // ����� ��
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

    // �ɷ��� ��
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

    //void OnSacrificed_SetState()
    //{
    //    PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Killed);
    //    IsFreeze = true; // ������ ���� ��� â���� �̵��ؾ� ��
    //}
    void OnSacrificed_GoUp()
    {
        StartCoroutine(CorSacrifice());
    }


    void StopHeal()
    {
        IsFreeze = false;
        Animator.SetBool("isHeal", false);


        if (m_healDest != null)
        {
            if (m_healDest.Slider_HealGauge.gameObject.activeSelf)
                m_healDest.Slider_HealGauge.gameObject.SetActive(false);
            m_healDest = null;
        }
    }

    void HealOtherSurvivor()
    {
        if (!isLocalPlayer) return;
        if (m_healDest == null) return;
        if (Input.GetMouseButtonDown(0))
        {
            IsFreeze = true;
            Animator.SetTrigger("Heal");
            netAnim.SetTrigger("Heal");

            m_healDest.Slider_HealGauge.gameObject.SetActive(true);

        }
        if (Input.GetMouseButton(0))
        {
            Animator.SetBool("isHeal", true);
            m_healDest?.CmdOnHealed();
            if (m_healDest.m_healthStateMachine.GetCurState() == HealthStates.Healthy)
            {
                StopHeal();
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Animator.SetBool("isHeal", false);

            m_healDest.Slider_HealGauge.gameObject.SetActive(false);
            IsFreeze = false;
        }
    }

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
                    //m_healDest.Slider_HealGauge.gameObject.SetActive(true);
                }
            }
        }
        if (other.CompareTag("Escape"))
        {
            PlayerUIManager.Instance.SetPlayerUIState(m_playerID, PlayerUI.Icons.Escaped);
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
