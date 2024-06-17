using UnityEngine;
using UnityEngine.UI;

public class Generator : MonoBehaviour, IInteractableObject
{
    [SerializeField] float curGauge = 0;
    [SerializeField] float maxGauge = 100;
    [SerializeField] float Multi_Gauge = 1;
    [SerializeField] Slider Slider_Gauge;   // 빌보드 붙여서 플레이어 UI처럼 만들기

    public void Interact()
    {
        if (Input.GetMouseButton(0))
        {
            Slider_Gauge.gameObject.SetActive(true);
            curGauge += Time.deltaTime * Multi_Gauge;
            Slider_Gauge.value = curGauge / maxGauge;
        }
        if (Input.GetMouseButtonUp(0))
            Slider_Gauge.gameObject.SetActive(false);

    }
}