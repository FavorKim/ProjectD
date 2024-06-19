using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hanger : MonoBehaviour, IInteractableObject
{
    [SerializeField] Transform Pos_HangedPos;
    Survivor hangedSurvivor;
    public Survivor HangedSurvivor
    {
        get { return hangedSurvivor; }
        set 
        {
            if (hangedSurvivor != value)
            hangedSurvivor = value;
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
}
