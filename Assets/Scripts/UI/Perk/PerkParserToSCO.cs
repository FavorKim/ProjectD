using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkParserToSCO : SingletonMono<PerkParserToSCO>
{
    private void Start()
    {
        instance = this; 
    }
    public static PerksScriptableObject Parse(PerkData data)
    {
        
        var tempPerk = ScriptableObject.CreateInstance<PerksScriptableObject>();

        tempPerk.Owner = data.Owner;
        tempPerk.PerkName = data.PerkName;
        tempPerk.IconImg = Resources.Load<Sprite>($"Perks/{data.Owner}/{data.IconName}");
        tempPerk.Target = (HandleValue)Enum.Parse(typeof(HandleValue), data.EffectTarget);
        tempPerk.Duration = data.Duration;
        tempPerk.ValuePercentage = data.ValuePercentage;
        tempPerk.CoolTime = data.CoolTime;
        tempPerk.Description = string.Format(data.Description, data.ValuePercentage,data.CoolTime,data.Duration);


        return tempPerk;
    }
}
