using System;
using System.Collections.Generic;
using UnityEngine;
using MySql.Data.MySqlClient;



public class PerkDataManager : SingletonMono<PerkDataManager>
{
    public Dictionary<string, PerkData> LoadedPerkList;

    //public readonly string rootPath = "C:/Users/KGA/Desktop/ProjectD_Data";

    public string connectionString = "server=localhost;database=projectd;user=root;password=3010";
    

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
        LoadAllPerkData();
    }
    void LoadAllPerkData()
    {
        LoadedPerkList = new Dictionary<string, PerkData>();

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT * FROM perktable";
            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var tempPerk = new PerkData();
                        tempPerk.Owner = reader["Owner"].ToString();
                        tempPerk.PerkName = reader["PerkName"].ToString();
                        tempPerk.IconImg = Resources.Load<Sprite>($"Perks/{tempPerk.Owner}/{tempPerk.PerkName}");
                        tempPerk.EffectTarget = (HandleValue)Enum.Parse(typeof(HandleValue), reader["EffectTarget"].ToString());
                        tempPerk.ValuePercentage = float.Parse(reader["ValuePercentage"].ToString());
                        tempPerk.Duration = float.Parse(reader["Duration"].ToString());
                        tempPerk.CoolTime = float.Parse(reader["CoolTime"].ToString());
                        tempPerk.PerkType = (PerkType)Enum.Parse(typeof(PerkType), reader["PerkType"].ToString());
                        var desc = reader["Description"].ToString();
                        tempPerk.Description = string.Format(desc, tempPerk.ValuePercentage, tempPerk.CoolTime, tempPerk.Duration);

                        LoadedPerkList.Add(tempPerk.PerkName, tempPerk);
                    }
                }
            }
        }
    }
    /*
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
            tempPerk.CoolTime = float.Parse(data.Attribute(nameof(tempPerk.CoolTime)).Value);
            tempPerk.PerkType = (PerkType)Enum.Parse(typeof(PerkType), data.Attribute(nameof(tempPerk.PerkType)).Value);
            var desc = data.Attribute(nameof(tempPerk.Description)).Value;
            tempPerk.Description = string.Format(desc, tempPerk.ValuePercentage, tempPerk.CoolTime, tempPerk.Duration);

            LoadedPerkList.Add(tempPerk.PerkName, tempPerk);
        }
    }
    */
    public PerkData GetPerkByName(string perkName)
    {
        var perk = LoadedPerkList[perkName];
        return perk;
    }

}
