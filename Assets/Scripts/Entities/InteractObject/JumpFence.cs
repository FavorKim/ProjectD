
using UnityEngine;

public class JumpFence : MonoBehaviour, IKillerInteractable, ISurvivorInteractable
{
    public void SurvivorInteract(ISurvivorVisitor survivor) 
    {
        survivor.OnSurvivorVisitWith(this);
    }
    public void KillerInteract(IKillerVisitor killer) 
    {
        killer.OnKillerVisitWith(this);
    }
}


