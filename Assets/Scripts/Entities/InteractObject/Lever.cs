using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class Lever : NetworkBehaviour, ISurvivorInteractable
{
    [SerializeField] float m_speed;
    [SerializeField] Animator Animator_Door;
    [SerializeField] NetworkAnimator netAnim_Door;
    Animator Animator;
    const float MAXGAUGE = 100;
    [SerializeField]float curGauge;
    public float CurrentGauge
    {
        get
        {
            return curGauge;
        }
        set
        {
            if(curGauge >= MAXGAUGE)
            {
                CmdOnOpenDoor();
                return;
            }
            if (CurrentGauge != value)
            {
                curGauge = value;
                Slider_Gauge.value = curGauge / MAXGAUGE;
            }
        }
    }
    [SerializeField] Slider Slider_Gauge;

    private void Start()
    {
        Animator = GetComponent<Animator>();
    }

    public bool IsAvailable { get; set; }

    public void SurvivorInteract(ISurvivorVisitor survivor)
    {
        if (!IsAvailable) 
        {
            Slider_Gauge.gameObject.SetActive(false);
            Animator.SetBool("isUsing", false);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Slider_Gauge.gameObject.SetActive(true);
            CmdSetCurGauge(curGauge + Time.deltaTime * m_speed);// CurrentGauge += Time.deltaTime * m_speed;
            Animator.SetBool("isUsing", true);
        }
        if(Input.GetMouseButtonUp(0)) 
        {
            Slider_Gauge.gameObject.SetActive(false);
            Animator.SetBool("isUsing", false);
        }
        survivor.OnSurvivorVisitWith(this);
    }

    [Command(requiresAuthority =false)]
    void CmdOnOpenDoor()
    {
        OnOpenDoor();
    }

    [ClientRpc]
    void OnOpenDoor()
    {
        IsAvailable = false;
        OpenDoor();
    }

    [Command(requiresAuthority =false)]
    void CmdSetCurGauge(float value)
    {
        curGauge = value;
        RpcSetCurGauge(curGauge);
    }

    [ClientRpc]
    void RpcSetCurGauge(float value)
    {
        CurrentGauge = value;
    }

    void OpenDoor()
    {
        Animator_Door.SetTrigger("Open");
        netAnim_Door.SetTrigger("Open");
        Slider_Gauge.gameObject.SetActive(false);
    }


}
