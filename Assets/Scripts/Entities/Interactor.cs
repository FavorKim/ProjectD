using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    

    abstract public void Interact(JumpFence window);
    abstract public void Interact(Palete palete);
    abstract public void Interact(Generator generator);

    protected void InteractObject(IInteractableObject obj)
    {
        switch (obj)
        {
            case JumpFence:
                Interact((JumpFence)obj);
                break;
            case Generator:
                Interact((Generator)obj);
                break;
            case Palete:
                Interact((Palete)obj);
                break;
            default:
                break;
        }
    }
}
