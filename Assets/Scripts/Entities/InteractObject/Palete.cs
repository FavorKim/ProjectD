using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Palete : NetworkBehaviour, IKillerInteractable, ISurvivorInteractable
{
    [SerializeField] GameObject VFX_GibletPref;
    Animator m_anim;
    NetworkAnimator netAnim;
    [SerializeField] BoxCollider triggerZone;
    public bool isUsed = false;
    [SerializeField]bool isAttack = false;
    public bool IsAttack { get { return isAttack; } private set { isAttack = value; } }
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
    }

    public void SurvivorInteract(ISurvivorVisitor survivor)
    {
        CmdOnUsed();
        survivor.OnSurvivorVisitWith(this);
        
    }
    [Command(requiresAuthority =false)]
    void CmdOnUsed()
    {
        RpcOnUsed();
    }
    [ClientRpc]
    void RpcOnUsed()
    {
        if (!isUsed)
        {
            m_anim.SetTrigger("Fall");
            netAnim.SetTrigger("Fall");
            isUsed = true;
        }
    }

    public void KillerInteract(IKillerVisitor killer)
    {
        if (isUsed)
        {
            m_anim.SetTrigger("Break");
            netAnim.SetTrigger("Break");
            killer.OnKillerVisitWith(this);
        }
    }

    [Command(requiresAuthority =false)]
    public void OnBreak()
    {
        var giblet = Instantiate(VFX_GibletPref,transform.position,Quaternion.identity);
        NetworkServer.Spawn(giblet);
        RpcOnBreak();
    }

    [ClientRpc]
    void RpcOnBreak()
    {
        gameObject.SetActive(false);
    }


    [Command(requiresAuthority =false)]
    public void SetAttackTrue()
    {
        RpcSetAttackTrue();
    }
    [ClientRpc]
    void RpcSetAttackTrue()
    {
        IsAttack = true;
    }


    [Command(requiresAuthority =false)]
    public void SetAttackFalse()
    {
        RpcSetAttackFalse();
    }
    [ClientRpc]
    void RpcSetAttackFalse()
    {
        IsAttack = false;
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if (!IsAttack) return;
    //    if(TryGetComponent(out KillerBase killer))
    //    {
    //        killer.OnStunCall();
    //    }
    //}
}
