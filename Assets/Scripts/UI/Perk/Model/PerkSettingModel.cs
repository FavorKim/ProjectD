using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PerkSettingModel
{
    private static PerkSettingModel instance;
    public static PerkSettingModel Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PerkSettingModel();
            }
            return instance;
        }
    }
    

    public PerksScriptableObject selectedPerk;
    public List<PerksScriptableObject> selectedPerkList = new List<PerksScriptableObject>(4);

    public event Action<PerksScriptableObject> OnEquipPerk;
    public event Action<int> OnSelectPerk;

    public void RegisterEventOnEquip(Action<PerksScriptableObject> onEquip)
    {
        OnEquipPerk += onEquip;
    }
    public void UnRegisterEventOnEquip(Action<PerksScriptableObject> onEquip)
    {
        OnEquipPerk -= onEquip;
    }

    public void RegisterEventOnSelect(Action<int> onSelect)
    {
        OnSelectPerk += onSelect;
    }
    public void UnRegisterEventOnSelect(Action<int> onSelect)
    {
        OnSelectPerk -= onSelect;
    }

    public void EquipPerk(PerksScriptableObject perkToChange)
    {
        if (selectedPerk == null)
        {
            foreach (var p in selectedPerkList)
            {
                if (p == null)
                {
                    selectedPerk = p;
                    break;
                }
            }
            if (selectedPerk == null) return;
        }
        selectedPerk = perkToChange;
        OnEquipPerk(perkToChange);
    }

    public void SelectPerk(int index)
    {
        selectedPerk = selectedPerkList[index];
        OnSelectPerk(index);
    }

    
}