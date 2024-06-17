using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayableCharactor : Interactor
{
    
    public void Interact(IInteractableObject obj)
    {
        InteractObject(obj);
    }

    public override abstract void Interact(Generator generator);
    public override abstract void Interact(Palete palete);
    public override abstract void Interact(JumpFence jumpFence);
}
