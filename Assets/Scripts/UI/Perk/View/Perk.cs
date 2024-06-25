using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Perk : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public PerksScriptableObject perk;

    [SerializeField] Image Img_Icon;
    [SerializeField] Sprite Img_Blank;

    [SerializeField] GameObject Panel_Description;

    [SerializeField] Text Text_Description;
    [SerializeField] Text Text_Name;

    public void OnPointerEnter(PointerEventData data)
    {
        OnPointerEnter_ShowDescription();
    }

    public void OnPointerExit(PointerEventData data)
    {
        OnPointerExit_ShowDescription();
    }

    public void Init(PerksScriptableObject perk)
    {
        InitPerk(perk);
    }

    void InitPerk(PerksScriptableObject perk)
    {
        this.perk = perk;

        if (perk != null)
        {
            InitView();
        }
        else
        {
            Img_Icon.sprite = Img_Blank;
        }

    }

    void InitView()
    {
        Img_Icon.sprite = perk.IconImg;
        Text_Description.text = perk.Description;
        Text_Name.text = perk.PerkName;
    }

    public void OnPointerEnter_ShowDescription()
    {
        if (perk != null)
            Panel_Description.SetActive(true);
    }

    public void OnPointerExit_ShowDescription()
    {
        if (perk != null)
            Panel_Description.SetActive(false);
    }

    public void OnClick_EquipPerk()
    {
        PerkSettingModel.Instance.EquipPerk(perk);
    }
}
