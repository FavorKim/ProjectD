
using UnityEngine;

public class JumpFence : MonoBehaviour, IKillerInteractable, ISurvivorInteractable
{
    public void SurvivorInteract(ISurvivorVisitor survivor) 
    {
        survivor.OnSurvivorVisitWithJumpFence(this);
    }
    public void KillerInteract(IKillerVisitor killer) 
    {
        killer.OnKillerVisitWithJumpFence(this);
    }
}


