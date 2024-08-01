using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
public class StatusManager : MonoBehaviour
{
    [SerializeField] Survivor owner;

    [SerializeField] GameObject ExhaustHud;
    [SerializeField] Image Gauge_Exhaust;
    ExhaustPerk m_ownerExhaustPerk;

    private void Start()
    {
        m_ownerExhaustPerk = owner.GetExhaustPerk();
        ExhaustHud.SetActive(false);
    }

    private void OnEnable()
    {

        m_ownerExhaustPerk.OnChangedCoolDown += SetExhaustCoolTimeGauge;
        m_ownerExhaustPerk.OnActivate += OnActivated;
    }

    private void OnDisable()
    {
        m_ownerExhaustPerk.OnActivate -= OnActivated;
        m_ownerExhaustPerk.OnChangedCoolDown -= SetExhaustCoolTimeGauge;
    }


    void SetExhaustCoolTimeGauge()
    {
        Gauge_Exhaust.fillAmount = 1 - (m_ownerExhaustPerk.CurCoolDown / m_ownerExhaustPerk.GetMaxCoolDown());
        if(Gauge_Exhaust.fillAmount <=0)
        {
            ExhaustHud.SetActive(false);
        }
    }

    void OnActivated()
    {
        ExhaustHud.SetActive(false);
    }

}
*/
