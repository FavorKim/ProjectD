using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactor : MonoBehaviour
{
    

    abstract public void Interact(JumpFence window);
    abstract public void Interact(Palete palete);
    abstract public void Interact(Generator generator);
    abstract public void Interact(Hanger hanger);

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
            case Hanger:
                Interact((Hanger)obj);
                break;
            default:
                break;
        }
    }
}
