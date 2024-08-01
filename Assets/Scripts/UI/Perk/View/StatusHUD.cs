using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour
{
    [SerializeField] GameObject GameObject_StatusHUD;
    [SerializeField] Image Gauge_StatusCoolTime;
    StatusPerk m_perk;

    public void InitPerk(StatusPerk perk)
    {
        m_perk = perk;
        m_perk.OnChangedCoolTime += SetExhaustCoolTimeGauge;
        m_perk.OnActivate += OnActivated;
    }
    
    
    private void Start()
    {
        GameObject_StatusHUD.SetActive(false);
    }


    private void OnDisable()
    {
        if (m_perk != null)
        {
            m_perk.OnActivate -= OnActivated;
            m_perk.OnChangedCoolTime -= SetExhaustCoolTimeGauge;
        }
    }


    void SetExhaustCoolTimeGauge()
    {
        Gauge_StatusCoolTime.fillAmount = 1 - (m_perk.CurCoolTime / m_perk.MaxCoolTime);
        if(Gauge_StatusCoolTime.fillAmount <=0)
        {
            GameObject_StatusHUD.SetActive(false);
        }
    }
    

    void OnActivated()
    {
        GameObject_StatusHUD.SetActive(true);
    }
    
}
