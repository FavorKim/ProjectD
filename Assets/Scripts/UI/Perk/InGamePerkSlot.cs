using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePerkSlot : SingletonMono<InGamePerkSlot>
{
    [SerializeField]Image[] images;


    

    public void SetPerkIcons(List<PerkData> perks)
    {
        /*
        if (perks == null)
        {
            Debug.Log("���� �� ����Ʈ Null");
            return;
        }
        */

        images = GetComponentsInChildren<Image>();


        for (int i = 0; i < images.Length; i++)
        {
            if (perks[i] != null)
                images[i].sprite = perks[i].IconImg;
            else
                images[i].gameObject.SetActive(false);

        }
    }
}
