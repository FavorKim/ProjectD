using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SelectedPerk
{
    public SelectedPerk(PerkData perk, int index)
    {
        this.perk = perk;
        this.index = index;
    }
    
    public PerkData perk;
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

    public event Action<PerkData,int> OnEquipPerk;
    public event Action<int> OnSelectPerk;

    public void RegisterEventOnEquip(Action<PerkData, int> onEquip)
    {
        OnEquipPerk += onEquip;
    }
    public void UnRegisterEventOnEquip(Action<PerkData, int> onEquip)
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

    public void EquipPerk(PerkData perkToChange)
    {
        bool isNoSelect = false;
        if (selectedPerk == null)
        {
            for(int i = 0; i < selectedPerkList.Count; i++)
            {
                if (selectedPerkList[i].perk == null)
                {
                    selectedPerk = selectedPerkList[i];
                    isNoSelect = true;
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

        foreach(var p in selectedPerkList)
        {
            if (p.perk == null) break;
            if (p.perk == perkToChange)
            {
                return;
            }
        }
        selectedPerk.perk = perkToChange;
        OnEquipPerk(selectedPerk.perk, selectedPerk.index);
        if (isNoSelect) selectedPerk = null;
    }

    public void SelectPerk(int index)
    {
        selectedPerk = selectedPerkList[index];
        OnSelectPerk(index);
    }

    
}