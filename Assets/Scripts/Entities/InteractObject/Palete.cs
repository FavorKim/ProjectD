using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Palete : NetworkBehaviour, IInteractableObject
{
    [SerializeField] GameObject VFX_GibletPref;
    Animator m_anim;
    NetworkAnimator netAnim;
    public bool isUsed = false;
    [SerializeField]bool isAttack = false;
    public bool IsAttack { get { return isAttack; } private set { isAttack = value; } }
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
        netAnim = GetComponent<NetworkAnimator>();
    }

    [Command(requiresAuthority =false)]
    public void SurvivorInteract()
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

    public void KillerInteract()
    {
        if(isUsed)
        m_anim.SetTrigger("Break");
        netAnim.SetTrigger("Break");
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
