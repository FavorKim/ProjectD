using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanger : MonoBehaviour, IInteractableObject
{
    [SerializeField] Transform Pos_HangedPos;
    [SerializeField] GameObject Shackle;
    Survivor hangedSurvivor;
    public bool IsAvailable() { return Shackle.activeSelf; }
    public Survivor HangedSurvivor
    {
        get { return hangedSurvivor; }
        set 
        {
            if (hangedSurvivor != value)
            {
                if (hangedSurvivor != null)
                {
                    hangedSurvivor.OnSacrificed += RemoveHangedSurvivor;
                    hangedSurvivor.OnSacrificed -= DropShackle;
                }

                hangedSurvivor = value;

                if (hangedSurvivor != null)
                {
                    hangedSurvivor.OnSacrificed += DropShackle;
                    hangedSurvivor.OnSacrificed += RemoveHangedSurvivor;
                }
            }
        }
    }
    public Transform GetHangedPos() { return Pos_HangedPos; }

    public void SurvivorInteract() 
    {
        hangedSurvivor.OnResqued();
    }

    public void KillerInteract()
    {

    }

    public void DropShackle()
    {
        Shackle.SetActive(false);
    }

    void RemoveHangedSurvivor()
    {
        HangedSurvivor = null;
    }
}
