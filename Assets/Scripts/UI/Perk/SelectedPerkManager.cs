using System.Collections.Generic;

public class SelectedPerkManager : SingletonNetwork<SelectedPerkManager>
{

    private static List<PerkData> perkList;
    public static List<PerkData> EquippedPerkList
    {
        get
        {
            if (perkList == null)
            {
                var selList = PerkSettingModel.Instance.selectedPerkList;
                List<PerkData> newperkList = new();
                foreach (var p in selList)
                {
                    newperkList.Add(p.perk);
                }
                perkList = newperkList;
            }
            return perkList;
        }
    }


    private void OnApplicationQuit()
    {
        perkList = null;
    }
}
