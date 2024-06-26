using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;



public class PerkDataManager : SingletonMono<PerkDataManager>
{
    public Dictionary<string, PerkData> LoadedPerkList;

    public readonly string rootPath = "C:/Users/KGA/Desktop/ProjectD_Data";

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
            tempPerk.Description = data.Attribute(nameof(tempPerk.Description)).Value;
            tempPerk.CoolTime = float.Parse(data.Attribute(nameof(tempPerk.CoolTime)).Value);

            LoadedPerkList.Add(tempPerk.PerkName, tempPerk);
        }
    }

    public PerkData GetPerkByName(string perkName)
    {
        var perk = LoadedPerkList[perkName];
        return perk;
    }

}
