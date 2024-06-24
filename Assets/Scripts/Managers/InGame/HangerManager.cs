using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangerManager : SingletonMono<HangerManager>
{
    List<Hanger> hangerList = new();
    private void Start()
    {
        foreach(Hanger hanger in GetComponentsInChildren<Hanger>())
        {
            hangerList.Add(hanger);
        }
    }

    public void TurnHangersXRay(bool onOff)
    {
        foreach (Hanger hanger in hangerList)
        {
            hanger.TurnSilhouette(onOff);
        }
    }
}
