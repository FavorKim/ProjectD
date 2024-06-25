using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SelectedPerk
{
    public SelectedPerk(PerksScriptableObject perk, int index)
    {
        this.perk = perk;
        this.index = index;
    }
    
    public PerksScriptableObject perk;
    public int index;
}

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
    

    public SelectedPerk selectedPerk;
    public List<SelectedPerk> selectedPerkList = new List<SelectedPerk>(4);

    public event Action<PerksScriptableObject,int> OnEquipPerk;
    public event Action<int> OnSelectPerk;

    public void RegisterEventOnEquip(Action<PerksScriptableObject,int> onEquip)
    {
        OnEquipPerk += onEquip;
    }
    public void UnRegisterEventOnEquip(Action<PerksScriptableObject,int> onEquip)
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
        if (selectedPerk==null)
        {
            foreach (var p in selectedPerkList)
            {
                if (p.perk == null)
                {
                    selectedPerk = p;
                    break;
                }
            }
        }
        else if(selectedPerk.perk == perkToChange)
        {
            selectedPerk.perk = null;
            OnEquipPerk(null, selectedPerk.index);
            return;
        }
        selectedPerk.perk = perkToChange;
        OnEquipPerk(selectedPerk.perk, selectedPerk.index);
    }

    public void SelectPerk(int index)
    {
        selectedPerk = selectedPerkList[index];
        OnSelectPerk(index);
    }

    
}