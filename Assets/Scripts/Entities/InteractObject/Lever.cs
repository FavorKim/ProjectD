using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lever : NetworkBehaviour, IInteractableObject
{
    [SerializeField] float m_speed;
    [SerializeField] Animator Animator_Door;
    [SerializeField] NetworkAnimator netAnim_Door;
    Animator Animator;
    const float MAXGAUGE = 100;
    [SerializeField/*, SyncVar(hook = nameof(Hook_OnChangedLeverValue))*/]float curGauge;
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
                OnOpenDoor();
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

    public void SurvivorInteract()
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
            CurrentGauge += Time.deltaTime * m_speed;
            Animator.SetBool("isUsing", true);
        }
        if(Input.GetMouseButtonUp(0)) 
        {
            Slider_Gauge.gameObject.SetActive(false);
            Animator.SetBool("isUsing", false);
        }
    }
    public void KillerInteract()
    {

    }

    void OnOpenDoor()
    {
        Debug.Log("OnOpenDoor");
        IsAvailable = false;
        OpenDoor();
    }

    void OpenDoor()
    {
        Animator_Door.SetTrigger("Open");
        netAnim_Door.SetTrigger("Open");
    }


    void Hook_OnChangedLeverValue(float old, float recent)
    {
        curGauge = recent;
    }
}
