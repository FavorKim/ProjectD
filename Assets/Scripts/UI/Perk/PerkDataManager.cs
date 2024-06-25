using Palmmedia.ReportGenerator.Core.Reporting.Builders;
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
            tempPerk.IconName = data.Attribute(nameof(tempPerk.IconName)).Value;
            tempPerk.EffectTarget = data.Attribute(nameof(tempPerk.EffectTarget)).Value;
            tempPerk.ValuePercentage = float.Parse(data.Attribute(nameof(tempPerk.ValuePercentage)).Value);
            tempPerk.Duration = float.Parse(data.Attribute(nameof(tempPerk.Duration)).Value);
            tempPerk.Description = data.Attribute(nameof(tempPerk.Description)).Value;
            tempPerk.CoolTime = float.Parse(data.Attribute(nameof(tempPerk.CoolTime)).Value);

            LoadedPerkList.Add(tempPerk.PerkName, tempPerk);
        }
    }

    public PerksScriptableObject GetPerkByName(string perkName)
    {
        var perk = PerkParserToSCO.Parse(LoadedPerkList[perkName]);
        return perk;
    }

}
