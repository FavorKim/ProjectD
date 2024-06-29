using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;



public class PerkDataManager : SingletonMono<PerkDataManager>
{
    public Dictionary<string, PerkData> LoadedPerkList;

    public readonly string rootPath = "C:/Users/KGA/Desktop/ProjectD_Data";

    
    //[TODO 상대경로화] public readonly string perkTablePath = Directory.GetParent(Application.dataPath).FullName;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllPerkData();
    }

    void LoadAllPerkData()
    {
        LoadedPerkList = new Dictionary<string, PerkData>();
        XDocument doc = XDocument.Load($"{rootPath}/PerkTable.xml");
        //XDocument doc = XDocument.Load($"{perkTablePath}/PerkTable.xml");
        var dataElements = doc.Descendants("data");

        foreach ( var data in dataElements )
        {
            var tempPerk = new PerkData();
            tempPerk.Owner = data.Attribute(nameof(tempPerk.Owner)).Value;
            tempPerk.PerkName = data.Attribute(nameof(tempPerk.PerkName)).Value;
            tempPerk.IconImg = Resources.Load<Sprite>($"Perks/{tempPerk.Owner}/{tempPerk.PerkName}");
            tempPerk.EffectTarget = (HandleValue)Enum.Parse(typeof(HandleValue), data.Attribute(nameof(tempPerk.EffectTarget)).Value);
            tempPerk.ValuePercentage = float.Parse(data.Attribute(nameof(tempPerk.ValuePercentage)).Value);
            tempPerk.Duration = float.Parse(data.Attribute(nameof(tempPerk.Duration)).Value);
            tempPerk.CoolTime = float.Parse(data.Attribute(nameof(tempPerk.CoolTime)).Value);
            tempPerk.PerkType = (PerkType)Enum.Parse(typeof(PerkType), data.Attribute(nameof(tempPerk.PerkType)).Value);
            var desc = data.Attribute(nameof(tempPerk.Description)).Value;
            tempPerk.Description = string.Format(desc, tempPerk.ValuePercentage, tempPerk.CoolTime, tempPerk.Duration);

            LoadedPerkList.Add(tempPerk.PerkName, tempPerk);
        }
    }

    public PerkData GetPerkByName(string perkName)
    {
        var perk = LoadedPerkList[perkName];
        return perk;
    }

}
