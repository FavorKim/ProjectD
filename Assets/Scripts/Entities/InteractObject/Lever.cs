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
    const float m_maxGauge = 100;
    float curGauge;
    public float CurrentGauge
    {
        get
        {
            return curGauge;
        }
        set
        {
            if (curGauge >= m_maxGauge)
            {
                OnOpenDoor();
                return;
            }
            if (CurrentGauge != value && CurrentGauge < m_maxGauge)
            {
                curGauge = value;
                Slider_Gauge.value = CurrentGauge / m_maxGauge;
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
        if (!IsAvailable && CurrentGauge >= m_maxGauge) 
        {
            Slider_Gauge.gameObject.SetActive(false);
            Animator.SetBool("isUsing", false);
            return; 
        }

        if (Input.GetMouseButton(0))
        {
            if (!Slider_Gauge.gameObject.activeSelf)
                Slider_Gauge.gameObject.SetActive(true);
            CurrentGauge += Time.deltaTime * m_speed;
            Animator.SetBool("isUsing", true);
        }
        else
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
        OpenDoor();
    }

    void OpenDoor()
    {
        Animator_Door.SetTrigger("Open");
        netAnim_Door.SetTrigger("Open");
    }
}
