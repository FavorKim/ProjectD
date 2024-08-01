using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using MySql.Data.MySqlClient;


//using MySql.Data.MySqlClient;



public class PerkDataManager : SingletonMono<PerkDataManager>
{
    public Dictionary<string, PerkData> LoadedPerkList;

    //public readonly string rootPath = "C:/Users/KGA/Desktop/ProjectD_Data";

    public string connectionString = "server=3.36.69.87;database=projectd;user=root;password=1234";
    

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

            string query = "SELECT * FROM parseddata";
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
    
    
    public PerkData GetPerkByName(string perkName)
    {
        var perk = LoadedPerkList[perkName];
        return perk;
    }

}
