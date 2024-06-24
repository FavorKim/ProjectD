using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerManager : SingletonMono<HangerManager>
{
    [SerializeField] List<Hanger> hangerList = new();

    

    public void TurnHangersXRay(bool onOff)
    {
        if (hangerList.Count <= 0)
        {
            foreach (Hanger hanger in GetComponentsInChildren<Hanger>())
            {
                hangerList.Add(hanger);
            }
        }
        foreach (Hanger hanger in hangerList)
        {
            hanger.TurnSilhouette(onOff);
        }
    }
}
