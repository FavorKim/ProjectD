using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Palete : NetworkBehaviour, IInteractableObject
{
    [SerializeField] GameObject VFX_GibletPref;
    Animator m_anim;
    public bool isUsed = false;
    [SerializeField]bool isAttack = false;
    public bool IsAttack { get { return isAttack; } private set { isAttack = value; } }
    private void Awake()
    {
        m_anim = GetComponent<Animator>();
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
            isUsed = true;
        }
    }

    public void KillerInteract()
    {
        if(isUsed)
        m_anim.SetTrigger("Break");
    }

    public void OnBreak()
    {
        Debug.Log("OnBreak");
        Instantiate(VFX_GibletPref,transform.position,Quaternion.identity);
        Destroy(gameObject); //서버 작업 필
    }
    [Command(requiresAuthority =false)]
    public void SetAttackTrue()
    {
        isAttack = true;
    }
    void RpcSetAttackTrue()
    {
        isAttack = true;
    }

    public void SetAttackFalse()
    {
        isAttack = false;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsAttack) return;
        other.GetComponent<KillerBase>()?.OnStunCall();
    }
}
